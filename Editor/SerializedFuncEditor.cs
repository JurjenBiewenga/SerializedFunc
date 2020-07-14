using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializedFuncImpl.Editor
{
    /// <summary>
    /// Drawer for <see cref="SerializedFunc"/> and subclasses
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializedFunc), true)]
    public class SerializedFuncEditor : PropertyDrawer
    {
        /// <summary>
        /// The cached matching methods
        /// </summary>
        private List<MethodInfo> matchingMethods = null;

        /// <summary>
        /// Unity OnGUI
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="property">The property being drawn</param>
        /// <param name="label">The label of the property</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var funcCall = property.FindPropertyRelative("methodReference");
            var target = funcCall.FindPropertyRelative("targetObject");
            var functionName = funcCall.FindPropertyRelative("methodName");
            var invokeStaticCall = funcCall.FindPropertyRelative("invokeStatic");
            var serializedTypeCall = funcCall.FindPropertyRelative("targetType");
            var typeAssemblyName = serializedTypeCall.FindPropertyRelative("assemblyQualifiedName");
            var paramTypes = funcCall.FindPropertyRelative("paramTypes");

            CalculateRects(position, out Rect switchRect, out Rect labelRect, out Rect objectRect, out Rect funcRect);

            if (GUI.Button(switchRect, new GUIContent("*", "Switch modes")))
            {
                invokeStaticCall.boolValue = !invokeStaticCall.boolValue;
                ResetSelectedFunction(functionName);
            }

            GUI.Label(labelRect, label);

            EditorGUI.BeginChangeCheck();
            if (invokeStaticCall.boolValue)
            {
                EditorGUI.PropertyField(objectRect, serializedTypeCall, new GUIContent());
            }
            else
            {
                EditorGUI.PropertyField(objectRect, target, new GUIContent());
            }

            if (EditorGUI.EndChangeCheck())
            {
                ResetSelectedFunction(functionName);
            }

            if ((target.objectReferenceValue == null && !invokeStaticCall.boolValue) || (string.IsNullOrWhiteSpace(typeAssemblyName.stringValue) && invokeStaticCall.boolValue))
                GUI.enabled = false;
            DrawMethodSelection(property,
                funcRect,
                functionName,
                paramTypes,
                target.objectReferenceValue,
                SerializableSystemType.GetSystemType(typeAssemblyName.stringValue),
                invokeStaticCall.boolValue);
            GUI.enabled = true;
        }

        private void ResetSelectedFunction(SerializedProperty functionName)
        {
            functionName.stringValue = string.Empty;
            matchingMethods = null;
        }

        /// <summary>
        /// Draws the method/property selection for the <paramref name="property"/>
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="rect">The rect to draw in</param>
        /// <param name="functionName">The function name serialized property</param>
        /// <param name="target">The target object</param>
        /// <param name="invokeStatic">Whether it is set to invoke static mode</param>
        private void DrawMethodSelection(SerializedProperty property, Rect rect, SerializedProperty functionName, SerializedProperty paramTypes, Object target, Type staticType, bool invokeStatic)
        {
            GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            if (GUI.Button(rect, functionName.stringValue, skin.GetStyle("DropDownButton")))
            {
                GenericMenu menu = new GenericMenu();
                List<MethodInfo> list = GetMatchingMethods(target, staticType, invokeStatic);
                foreach (MethodInfo method in list)
                {
                    menu.AddItem(new GUIContent(method.Name), false, () =>
                    {
                        functionName.stringValue = method.Name;
                        paramTypes.ClearArray();
                        var parameters = method.GetParameters();
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            paramTypes.InsertArrayElementAtIndex(i);
                            var elem = paramTypes.GetArrayElementAtIndex(i);
                            var assemblyQualifiedName = elem.FindPropertyRelative("assemblyQualifiedName");
                            var nameProperty = elem.FindPropertyRelative("name");
                            var assemblyName = elem.FindPropertyRelative("assemblyName");
                            assemblyQualifiedName.stringValue = parameters[0].ParameterType.AssemblyQualifiedName;
                            nameProperty.stringValue = parameters[0].ParameterType.Name;
                            assemblyName.stringValue = parameters[0].ParameterType.FullName;
                        }

                        property.serializedObject.ApplyModifiedProperties();
                    });
                }

                if (menu.GetItemCount() == 0)
                {
                    menu.AddDisabledItem(new GUIContent("No matching methods found"));
                }

                menu.DropDown(rect);
            }
        }

        /// <summary>
        /// Calculates the rects used to draw the label, object field, and the method selection
        /// </summary>
        /// <param name="position">The space available to draw in</param>
        /// <param name="switchRect">The calculated rect for the switch button</param>
        /// <param name="labelRect">The calculated rect for the label</param>
        /// <param name="objectRect">The calculated rect for the object field</param>
        /// <param name="selectionRect">The calculated rect for the method selection</param>
        private static void CalculateRects(Rect position, out Rect switchRect, out Rect labelRect, out Rect objectRect, out Rect selectionRect)
        {
            if (EditorGUIUtility.wideMode)
            {
                float dividedWidth = (position.width - 15) / 3;
                switchRect = new Rect(position.x, position.y, 15, position.height);
                labelRect = new Rect(position.x + 15, position.y, dividedWidth, position.height);
                objectRect = new Rect(labelRect.x + dividedWidth, position.y, dividedWidth, position.height);
                selectionRect = new Rect(objectRect.x + dividedWidth, position.y, dividedWidth, position.height);
            }
            else
            {
                float dividedWidth = (position.width - 15) / 2;
                labelRect = new Rect(position.x, position.y, dividedWidth, EditorGUIUtility.singleLineHeight);
                switchRect = new Rect(position.x, position.y, 15, position.height);
                objectRect = new Rect(position.x + 15, position.y + EditorGUIUtility.singleLineHeight, dividedWidth, EditorGUIUtility.singleLineHeight);
                selectionRect = new Rect(objectRect.x + dividedWidth, position.y, dividedWidth, EditorGUIUtility.singleLineHeight);
            }
        }

        /// <summary>
        /// Gets the matching methods that are available in the <paramref name="target"/>
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="invokeStatic">Whether it is set to invoke static mode</param>
        /// <returns>The matching methods</returns>
        private List<MethodInfo> GetMatchingMethods(Object target, Type staticType, bool invokeStatic)
        {
            if (matchingMethods != null)
            {
                return matchingMethods;
            }

            matchingMethods = new List<MethodInfo>();

            Type t = fieldInfo.FieldType.BaseType;
            if(!invokeStatic)
                matchingMethods = FindMatchingMethodsForType(target.GetType(), t, invokeStatic) ?? new List<MethodInfo>();
            else
                matchingMethods = FindMatchingMethodsForType(staticType, t, invokeStatic) ?? new List<MethodInfo>();

            return matchingMethods;
        }

        /// <summary>
        /// Tries to find the methods for the given <paramref name="type"/> in <paramref name="targetType"/>
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="type">The type</param>
        /// <param name="invokeStatic">Whether it is set to invoke static mode</param>
        /// <returns>Returns the found methods or null if none are found</returns>
        private List<MethodInfo> FindMatchingMethodsForType(Type targetType, Type type, bool invokeStatic)
        {
            if (type != null)
            {
                while (!type.IsGenericType)
                {
                    type = type.BaseType;
                }

                var genericArguments = type.GetGenericArguments();

                MethodInfo[] methods = null;
                if (invokeStatic)
                {
                    methods = targetType.GetMethods(BindingFlags.Static | BindingFlags.Public);
                }
                else
                {
                    methods = targetType.GetMethods();
                }

                return FilterMethods(methods, genericArguments);
            }

            return null;
        }

        /// <summary>
        /// Filters the given <paramref name="methods"/> based on whether the parameters and return value match the <paramref name="genericArguments"/>
        /// </summary>
        /// <param name="methods">The methods</param>
        /// <param name="genericArguments">The generic arguments</param>
        /// <returns>The matched methods or null if none are found</returns>
        private List<MethodInfo> FilterMethods(IEnumerable<MethodInfo> methods, IReadOnlyList<Type> genericArguments)
        {
            List<MethodInfo> foundMethods = new List<MethodInfo>();

            foreach (MethodInfo method in methods)
            {
                bool success = IsValidMethod(method, genericArguments);

                if (success)
                {
                    foundMethods.Add(method);
                }
            }

            if (foundMethods.Count == 0)
                return null;

            return foundMethods;
        }

        /// <summary>
        /// Checks whether the given <paramref name="method"/> is valid
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="genericArguments">The arguments to check</param>
        /// <returns>Whether it is valid</returns>
        private static bool IsValidMethod(MethodInfo method, IReadOnlyList<Type> genericArguments)
        {
            return IsReturnValueValid(method, genericArguments) && DoParametersMatchArguments(method, genericArguments);
        }

        /// <summary>
        /// Checks whether the parameters match the generic arguments
        /// </summary>
        /// <param name="method">The method to check</param>
        /// <param name="genericArguments">The generic arguments</param>
        /// <returns>Whether it was a valid match</returns>
        private static bool DoParametersMatchArguments(MethodInfo method, IReadOnlyList<Type> genericArguments)
        {
            ParameterInfo[] parameters = method.GetParameters();

            if (genericArguments.Count - 1 != parameters.Length)
                return false;

            for (var i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameters[i];
                if (!genericArguments[i].IsAssignableFrom(parameterInfo.ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether the return value is the same as the <paramref name="method"/> return type
        /// </summary>
        /// <remarks>Assumes that the last generic argument is the return type</remarks>
        /// <param name="method">The method to check</param>
        /// <param name="genericArguments">The arguments to use</param>
        /// <returns>Whether it was a valid match</returns>
        private static bool IsReturnValueValid(MethodInfo method, IReadOnlyList<Type> genericArguments)
        {
            return genericArguments[genericArguments.Count - 1].IsAssignableFrom(method.ReturnType);
        }

        /// <summary>
        /// Calculates the property height
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="label">The label</param>
        /// <returns>The calculated height</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (EditorGUIUtility.wideMode)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}