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

            CalculateRects(position, out Rect labelRect, out Rect objectRect, out Rect funcRect);

            GUI.Label(labelRect, label);
            
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(objectRect, target, new GUIContent());
            if (EditorGUI.EndChangeCheck())
            {
                functionName.stringValue = string.Empty;
                matchingMethods = null;
            }

            DrawMethodSelection(property, funcRect, functionName, target.objectReferenceValue);
        }

        /// <summary>
        /// Draws the method/property selection for the <paramref name="property"/>
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="rect">The rect to draw in</param>
        /// <param name="functionName">The function name serialized property</param>
        /// <param name="target">The target object</param>
        private void DrawMethodSelection(SerializedProperty property, Rect rect, SerializedProperty functionName, Object target)
        {
            GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            if (GUI.Button(rect, functionName.stringValue, skin.GetStyle("DropDownButton")))
            {
                GenericMenu menu = new GenericMenu();
                List<MethodInfo> list = GetMatchingMethods(target);
                foreach (MethodInfo method in list)
                {
                    menu.AddItem(new GUIContent(method.Name), false, () =>
                    {
                        functionName.stringValue = method.Name;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }

                if (menu.GetItemCount() > 0)
                    menu.DropDown(rect);
            }
        }

        /// <summary>
        /// Calculates the rects used to draw the label, object field, and the method selection
        /// </summary>
        /// <param name="position">The space available to draw in</param>
        /// <param name="labelRect">The calculated rect for the label</param>
        /// <param name="objectRect">The calculated rect for the object field</param>
        /// <param name="selectionRect">The calculated rect for the method selection</param>
        private static void CalculateRects(Rect position, out Rect labelRect, out Rect objectRect, out Rect selectionRect)
        {
            if (EditorGUIUtility.wideMode)
            {
                float dividedWidth = position.width / 3;
                labelRect = new Rect(position.x, position.y, dividedWidth, position.height);
                objectRect = new Rect(position.x + dividedWidth, position.y, dividedWidth, position.height);
                selectionRect = new Rect(position.x + dividedWidth * 2, position.y, dividedWidth, position.height);
            }
            else
            {
                float dividedWidth = position.width / 2;
                labelRect = new Rect(position.x, position.y, dividedWidth, EditorGUIUtility.singleLineHeight);
                objectRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, dividedWidth, EditorGUIUtility.singleLineHeight);
                selectionRect = new Rect(position.x + dividedWidth, position.y, dividedWidth, EditorGUIUtility.singleLineHeight);
            }
        }

        /// <summary>
        /// Gets the matching methods that are available in the <paramref name="target"/>
        /// </summary>
        /// <param name="target">The target</param>
        /// <returns>The matching methods</returns>
        private List<MethodInfo> GetMatchingMethods(Object target)
        {
            if (matchingMethods != null)
            {
                return matchingMethods;
            }

            matchingMethods = new List<MethodInfo>();

            Type t = fieldInfo.FieldType.BaseType;
            matchingMethods = FindMatchingMethodsForType(target, t) ?? new List<MethodInfo>();

            return matchingMethods;
        }

        /// <summary>
        /// Tries to find the methods for the given <paramref name="type"/> in <paramref name="targetObject"/>
        /// </summary>
        /// <param name="targetObject">The target object</param>
        /// <param name="type">The type</param>
        /// <returns>Returns the found methods or null if none are found</returns>
        private List<MethodInfo> FindMatchingMethodsForType(Object targetObject, Type type)
        {
            if (type != null)
            {
                while (!type.IsGenericType)
                {
                    type = type.BaseType;
                }

                var genericArguments = type.GetGenericArguments();

                Type targetType = targetObject.GetType();
                MethodInfo[] methods = targetType.GetMethods();
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
                ParameterInfo[] parameters = method.GetParameters();
             
                bool success = IsReturnValueValid(method, genericArguments) && DoParametersMatchArguments(parameters, genericArguments);
                
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
        /// Checks whether the parameters match the generic arguments
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <param name="genericArguments">The generic arguments</param>
        /// <returns>Whether it was a valid match</returns>
        private static bool DoParametersMatchArguments(ParameterInfo[] parameters, IReadOnlyList<Type> genericArguments)
        {
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