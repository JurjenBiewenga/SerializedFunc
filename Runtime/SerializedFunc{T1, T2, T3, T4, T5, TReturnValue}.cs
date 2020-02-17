using System;
using System.Reflection;

namespace SerializedFunc
{
    public class SerializedFunc<T1, T2, T3, T4, T5, TReturnValue> : SerializedFunc<T1, T2, T3, T4, T5, object, object, object, TReturnValue>
    {
        public TReturnValue Invoke(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5)
        {
            return InvokeInternal(val1, val2, val3, val4, val5, null, null, null);
        }

        public void Set(Func<T1, T2, T3, T4, T5, TReturnValue> func)
        {
            SetInternal(func);
        }

        public void Set(UnityEngine.Object target, MethodInfo methodInfo)
        {
            SetInternal(target, methodInfo);
        }
    }
}