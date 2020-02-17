using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializedFunc
{
    [Serializable]
    public class EventCall
    {
        [SerializeField]
        private Object targetObject;
        [SerializeField]
        private string functionName = default;
        
        private MethodInfo methodInfo;

        public MethodInfo MethodInfo
        {
            get
            {
                if (methodInfo == null)
                {
                    Type t = TargetObject.GetType();
                    methodInfo = t.GetMethod(FunctionName);
                }
                return methodInfo;
            }
        }

        public string FunctionName
        {
            get => functionName;
        }

        public Object TargetObject
        {
            get => targetObject;
        }

        public EventCall(Object target, MethodInfo methodInfo)
        {
            targetObject = target;
            this.methodInfo = methodInfo;
        }
    }
}