using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    /// <typeparam name="T6">Type 6</typeparam>
    /// <typeparam name="T7">Type 7</typeparam>
    /// <typeparam name="T8">Type 8</typeparam>
    /// <typeparam name="TReturnValue">The return type</typeparam>
    public class SerializedFunc<T1, T2, T3, T4, T5, T6, T7, T8, TReturnValue> : SerializedFunc
    {
        /// <summary>
        /// The runtime function or null if not set
        /// </summary>
        private Delegate runtimeFunc;

        /// <summary>
        /// Gets the runtime function or null if not set
        /// </summary>
        protected Delegate RuntimeFunc
        {
            get
            {
                if (runtimeFunc == null)
                {
                    CreateRuntimeFunc();
                }

                return runtimeFunc;
            }
            set => runtimeFunc = value;
        }

        /// <summary>
        /// Gets the amount of expected parameters
        /// </summary>
        protected virtual int ExpectedParameters { get; } = 8;

        /// <summary>
        /// Overwrites the current function
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="methodInfo">The method to replace it with</param>
        public void Set(UnityEngine.Object target, MethodInfo methodInfo)
        {
            ValidateMethodInfo(methodInfo);

            MethodReference = new SerializedMethodReference(target, methodInfo);
            CreateRuntimeFunc();
        }

        /// <summary>
        /// Invokes the saved function
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <param name="val3">Value 3</param>
        /// <param name="val4">Value 4</param>
        /// <param name="val5">Value 5</param>
        /// <param name="val6">Value 6</param>
        /// <param name="val7">Value 7</param>
        /// <param name="val8">Value 8</param>
        /// <returns>The return value of the function</returns>
        protected TReturnValue InvokeInternal(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7, T8 val8)
        {
            if (RuntimeFunc != null)
            {
                var count = runtimeFunc.Method.GetParameters().Length;
                if (!runtimeFunc.Method.IsHideBySig)
                    count -= 1;

                if (count == 0)
                    return (TReturnValue) RuntimeFunc.DynamicInvoke();

                if (count == 1)
                    return (TReturnValue) RuntimeFunc.DynamicInvoke(val1);

                if (count == 2)
                    return (TReturnValue) RuntimeFunc.DynamicInvoke(val1, val2);

                if (count == 3)
                    return (TReturnValue) RuntimeFunc.DynamicInvoke(val1, val2, val3);

                if (count == 4)
                    return (TReturnValue) RuntimeFunc.DynamicInvoke(val1, val2, val3, val4);

                if (count == 5)
                    return (TReturnValue) RuntimeFunc.DynamicInvoke(val1, val2, val3, val4, val5);

                if (count == 6)
                    return (TReturnValue) RuntimeFunc.DynamicInvoke(val1, val2, val3, val4, val5, val6);

                if (count == 7)
                    return (TReturnValue) RuntimeFunc.DynamicInvoke(val1, val2, val3, val4, val5, val6, val7);

                if (count == 8)
                    return (TReturnValue) RuntimeFunc.DynamicInvoke(val1, val2, val3, val4, val5, val6, val7, val8);
            }

            return default;
        }

        /// <summary>
        /// Overwrites the current function
        /// </summary>
        /// <param name="func">The new function</param>
        protected void SetInternal(Delegate func)
        {
            runtimeFunc = func;
        }

        /// <summary>
        /// Validates the parameters and return value of the given method info
        /// </summary>
        /// <param name="methodInfo">The method info</param>
        /// <exception cref="ArgumentException">Thrown when an invalid method info was passed</exception>
        private void ValidateMethodInfo(MethodInfo methodInfo)
        {
            if (!methodInfo.ReturnType.IsAssignableFrom(typeof(TReturnValue)))
                throw new ArgumentException("Incorrect return type", nameof(methodInfo));

            var parameters = methodInfo.GetParameters();
            
            if(parameters.Length != ExpectedParameters)
                throw new ArgumentException("Invalid parameter count");

            if (parameters.Length > 0 && !parameters[0].ParameterType.IsAssignableFrom(typeof(T1)))
                throw new ArgumentException("Incorrect first parameter", nameof(methodInfo));

            if (parameters.Length > 1 && !parameters[1].ParameterType.IsAssignableFrom(typeof(T2)))
                throw new ArgumentException("Incorrect second parameters", nameof(methodInfo));

            if (parameters.Length > 2 && !parameters[2].ParameterType.IsAssignableFrom(typeof(T3)))
                throw new ArgumentException("Incorrect third parameters", nameof(methodInfo));

            if (parameters.Length > 3 && !parameters[3].ParameterType.IsAssignableFrom(typeof(T4)))
                throw new ArgumentException("Incorrect fourth parameters", nameof(methodInfo));

            if (parameters.Length > 4 && !parameters[4].ParameterType.IsAssignableFrom(typeof(T5)))
                throw new ArgumentException("Incorrect fifth parameters", nameof(methodInfo));

            if (parameters.Length > 5 && !parameters[5].ParameterType.IsAssignableFrom(typeof(T6)))
                throw new ArgumentException("Incorrect sixth parameters", nameof(methodInfo));

            if (parameters.Length > 6 && !parameters[6].ParameterType.IsAssignableFrom(typeof(T7)))
                throw new ArgumentException("Incorrect seventh parameters", nameof(methodInfo));

            if (parameters.Length > 7 && !parameters[7].ParameterType.IsAssignableFrom(typeof(T8)))
                throw new ArgumentException("Incorrect eight parameters", nameof(methodInfo));
        }

        /// <summary>
        /// Creates a delegate from the <see cref="SerializedMethodReference"/> field
        /// </summary>
        private void CreateRuntimeFunc()
        {
            if (MethodReference?.MethodInfo != null)
            {
                runtimeFunc = CreateDelegate(MethodReference.TargetObject, MethodReference.MethodInfo);
            }
        }

        /// <summary>
        /// Creates a delegate from the <paramref name="target"/> and <paramref name="method"/>
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="method">The method</param>
        /// <returns>The created delegate</returns>
        private static Delegate CreateDelegate(object target, MethodInfo method)
        {
            var parameters = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var call = Expression.Call(Expression.Constant(target), method, parameters);
            return Expression.Lambda(call, parameters).Compile();
        }
    }
}