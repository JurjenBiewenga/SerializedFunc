using System;
using System.Reflection;

namespace SerializedFuncImpl
{
    /// <summary>
    /// Generic implementation with 3 generic parameters
    /// </summary>
    /// <typeparam name="T1">Type 1</typeparam>
    /// <typeparam name="T2">Type 2</typeparam>
    /// <typeparam name="T3">Type 3</typeparam>
    /// <typeparam name="TReturnValue">The return type</typeparam>
    public class SerializedFunc<T1, T2, T3, TReturnValue> : SerializedFunc<T1, T2, T3, object, object, object, object, object, TReturnValue>
    {
        /// <inheritdoc />
        protected override int ExpectedParameters
        {
            get => 3;
        }
        
        /// <summary>
        /// Invokes the saved function
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <param name="val3">Value 3</param>
        /// <returns>The return value of the function</returns>
        public TReturnValue Invoke(T1 val1, T2 val2, T3 val3)
        {
            return InvokeInternal(val1, val2, val3, null, null, null, null, null);
        }

        /// <summary>
        /// Overwrites the current function
        /// </summary>
        /// <param name="func">The method to replace it with</param>
        public void Set(Func<T1, T2, T3, TReturnValue> func)
        {
            SetInternal(func);
        }
    }
}