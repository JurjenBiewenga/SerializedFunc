using System;
using System.Reflection;

namespace SerializedFunc
{
    public class SerializedFunc<T1, TReturnValue> : SerializedFunc<T1, object, object, object, object, object, object, object, TReturnValue>
    {
        public TReturnValue Invoke(T1 val1)
        {
            return InvokeInternal(val1, null, null, null, null, null, null, null);
        }

        public void Set(Func<T1, TReturnValue> func)
        {
            SetInternal(func);
        }

        public void Set(UnityEngine.Object target, MethodInfo methodInfo)
        {
            SetInternal(target, methodInfo);
        }
    }
}