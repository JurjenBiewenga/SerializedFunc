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
        /// The function name
        /// </summary>
        [SerializeField]
        private string methodName = default;
        
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
                    Type t = TargetObject.GetType();
                    methodInfo = t.GetMethod(MethodName);
                }
                return methodInfo;
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