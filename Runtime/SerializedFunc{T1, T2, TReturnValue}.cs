using System;
using System.Reflection;

namespace SerializedFuncImpl
{
    /// <summary>
    /// Generic implementation with 2 generic parameters
    /// </summary>
    /// <typeparam name="T1">Type 1</typeparam>
    /// <typeparam name="T2">Type 2</typeparam>
    /// <typeparam name="TReturnValue">The return type</typeparam>
    public class SerializedFunc<T1, T2, TReturnValue> : SerializedFunc<T1, T2, object, object, object, object, object, object, TReturnValue>
    {
        /// <summary>
        /// Invokes the saved function
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <returns>The return value of the function</returns>
        public TReturnValue Invoke(T1 val1, T2 val2)
        {
            return InvokeInternal(val1, val2, null, null, null, null, null, null);
        }

        /// <summary>
        /// Overwrites the current function
        /// </summary>
        /// <param name="func">The method to replace it with</param>
        public void Set(Func<T1, T2, TReturnValue> func)
        {
            SetInternal(func);
        }
    }
}