using System;
using System.Reflection;

namespace SerializedFunc
{
    public class SerializedFunc<T1, T2, T3, T4, T5, T6, T7, TReturnValue> : SerializedFunc<T1, T2, T3, T4, T5, T6, T7, object, TReturnValue>
    {
        public TReturnValue Invoke(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7)
        {
            return InvokeInternal(val1, val2, val3, val4, val5, val6, val7, null);
        }

        public void Set(Func<T1, T2, T3, T4, T5, T6, T7, TReturnValue> func)
        {
            SetInternal(func);
        }

        public void Set(UnityEngine.Object target, MethodInfo methodInfo)
        {
            SetInternal(target, methodInfo);
        }
    }
}