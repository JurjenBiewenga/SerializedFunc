using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializedFuncImpl
{
    /// <summary>
    /// The serialized method reference
    /// </summary>
    [Serializable]
    public class SerializedMethodReference
    {
        /// <summary>
        /// The object to invoke the method on
        /// </summary>
        [SerializeField]
        private Object targetObject;

        /// <summary>
        /// The type to invoke a static call on
        /// </summary>
        [SerializeField]
        private SerializableSystemType targetType = null;

        [SerializeField]
        private bool invokeStatic = false;
        
        /// <summary>
        /// The function name
        /// </summary>
        [SerializeField]
        private string methodName = default;

        /// <summary>
        /// The types of the parameters
        /// </summary>
        [SerializeField]
        private SerializableSystemType[] paramTypes;
        
        /// <summary>
        /// The method info
        /// </summary>
        private MethodInfo methodInfo;

        /// <summary>
        /// Gets the method reference if not null; else tries to find it
        /// </summary>
        public MethodInfo MethodInfo
        {
            get
            {
                if (methodInfo == null)
                {
                    InitializeMethodInfo();
                }

                return methodInfo;
            }
        }

        private void InitializeMethodInfo()
        {
            Type t = TargetObject.GetType();
            MethodInfo[] methods;
            if (invokeStatic)
            {
                methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
            }
            else
            {
                methods = t.GetMethods();
            }
            foreach (MethodInfo method in methods)
            {
                if (method.Name == methodName)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == paramTypes.Length)
                    {
                        bool failed = false;
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].ParameterType != paramTypes[i].SystemType)
                            {
                                failed = true;
                                break;
                            }
                        }

                        if (!failed)
                        {
                            methodInfo = method;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the method name
        /// </summary>
        public string MethodName
        {
            get => methodName;
        }

        /// <summary>
        /// Gets the target object
        /// </summary>
        public Object TargetObject
        {
            get => targetObject;
        }

        /// <summary>
        /// The target type to invoke on
        /// </summary>
        public Type TargetType
        {
            get => this.targetType?.SystemType;
        }

        /// <summary>
        /// Whether to invoke the method statically 
        /// </summary>
        public bool InvokeStatic
        {
            get => invokeStatic;
        }

        /// <summary>
        /// Constructs a new instance of <see cref="SerializedMethodReference"/>
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="methodInfo">The method info</param>
        public SerializedMethodReference(Object target, MethodInfo methodInfo)
        {
            targetObject = target;
            this.methodInfo = methodInfo;
        }
    }
}