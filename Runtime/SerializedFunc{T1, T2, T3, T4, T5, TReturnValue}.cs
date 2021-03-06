using System;
using System.Reflection;

namespace SerializedFuncImpl
{
    /// <summary>
    /// Generic implementation with 8 generic parameters
    /// </summary>
    /// <typeparam name="T1">Type 1</typeparam>
    /// <typeparam name="T2">Type 2</typeparam>
    /// <typeparam name="T3">Type 3</typeparam>
    /// <typeparam name="T4">Type 4</typeparam>
    /// <typeparam name="T5">Type 5</typeparam>
    /// <typeparam name="TReturnValue">The return type</typeparam>
    public class SerializedFunc<T1, T2, T3, T4, T5, TReturnValue> : SerializedFunc<T1, T2, T3, T4, T5, object, object, object, TReturnValue>
    {
        /// <inheritdoc />
        protected override int ExpectedParameters
        {
            get => 5;
        }
        
        /// <summary>
        /// Invokes the saved function
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <param name="val3">Value 3</param>
        /// <param name="val4">Value 4</param>
        /// <param name="val5">Value 5</param>
        /// <returns>The return value of the function</returns>
        public TReturnValue Invoke(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5)
        {
            return InvokeInternal(val1, val2, val3, val4, val5, null, null, null);
        }

        /// <summary>
        /// Overwrites the current function
        /// </summary>
        /// <param name="func">The method to replace it with</param>
        public void Set(Func<T1, T2, T3, T4, T5, TReturnValue> func)
        {
            SetInternal(func);
        }
    }
}