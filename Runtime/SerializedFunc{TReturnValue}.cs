using System;
using System.Reflection;

namespace SerializedFunc
{
    public class SerializedFunc<TReturnValue> : SerializedFunc<object, object, object, object, object, object, object, object, TReturnValue>
    {
        public TReturnValue Invoke()
        {
            return InvokeInternal(null, null, null, null, null, null, null, null);
        }

        public void Set(Func<TReturnValue> func)
        {
            SetInternal(func);
        }

        public void Set(UnityEngine.Object target, MethodInfo methodInfo)
        {
            SetInternal(target, methodInfo);
        }
    }
}