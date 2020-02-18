using System;
using System.Reflection;

namespace SerializedFuncImpl
{
    /// <summary>
    /// Generic implementation with no generic parameters
    /// </summary>
    /// <typeparam name="TReturnValue">The return type</typeparam>
    public class SerializedFunc<TReturnValue> : SerializedFunc<object, object, object, object, object, object, object, object, TReturnValue>
    {
        /// <summary>
        /// Invokes the function
        /// </summary>
        /// <returns>The return value of the function or null</returns>
        public TReturnValue Invoke()
        {
            return InvokeInternal(null, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// Overwrites the current function
        /// </summary>
        /// <param name="func">The function to replace it with</param>
        public void Set(Func<TReturnValue> func)
        {
            SetInternal(func);
        }
    }
}