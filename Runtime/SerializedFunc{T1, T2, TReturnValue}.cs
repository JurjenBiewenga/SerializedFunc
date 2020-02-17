using System;
using System.Reflection;

namespace SerializedFunc
{
    public class SerializedFunc<T1, T2, TReturnValue> : SerializedFunc<T1, T2, object, object, object, object, object, object, TReturnValue>
    {
        public TReturnValue Invoke(T1 val1, T2 val2)
        {
            return InvokeInternal(val1, val2, null, null, null, null, null, null);
        }

        public void Set(Func<T1, T2, TReturnValue> func)
        {
            SetInternal(func);
        }

        public void Set(UnityEngine.Object target, MethodInfo methodInfo)
        {
            SetInternal(target, methodInfo);
        }
    }
}