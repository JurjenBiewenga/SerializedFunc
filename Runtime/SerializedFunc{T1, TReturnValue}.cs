using System;
using System.Reflection;

namespace SerializedFuncImpl
{
    /// <summary>
    /// Generic implementation with 1 generic parameters
    /// </summary>
    /// <typeparam name="T1">Type 1</typeparam>
    /// <typeparam name="TReturnValue">The return type</typeparam>
    public class SerializedFunc<T1, TReturnValue> : SerializedFunc<T1, object, object, object, object, object, object, object, TReturnValue>
    {
        /// <inheritdoc />
        protected override int ExpectedParameters
        {
            get => 1;
        }
        
        /// <summary>
        /// Invokes the saved function
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <returns>The return value of the function</returns>
        public TReturnValue Invoke(T1 val1)
        {
            return InvokeInternal(val1, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// Overwrites the current function
        /// </summary>
        /// <param name="func">The method to replace it with</param>
        public void Set(Func<T1, TReturnValue> func)
        {
            SetInternal(func);
        }
    }
}