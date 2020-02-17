using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SerializedFunc
{
    public class SerializedFunc<T1, T2, T3, T4, T5, T6, T7, T8, TReturnValue> : SerializedFunc
    {
        private Delegate runtimeFunc;

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

        public TReturnValue Invoke(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7, T8 val8)
        {
            return InvokeInternal(val1, val2, val3, val4, val5, val6, val7, val8);
        }

        protected TReturnValue InvokeInternal(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7, T8 val8)
        {
            if (RuntimeFunc != null)
            {
                var count = runtimeFunc.Method.GetParameters().Length;
                if(!runtimeFunc.Method.IsHideBySig)
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

        protected void SetInternal(Delegate func)
        {
            runtimeFunc = func;
        }

        protected void SetInternal(UnityEngine.Object target, MethodInfo methodInfo)
        {
            ValidateMethodInfo(methodInfo);

            Call = new EventCall(target, methodInfo);
            CreateRuntimeFunc();
        }

        private static void ValidateMethodInfo(MethodInfo methodInfo)
        {
            if (!methodInfo.ReturnType.IsAssignableFrom(typeof(TReturnValue)))
                throw new ArgumentException("Incorrect return type", nameof(methodInfo));

            var parameters = methodInfo.GetParameters();

            if (parameters.Length > 0 && !parameters[0].ParameterType.IsAssignableFrom(typeof(T1)))
                throw new ArgumentException("Incorrect first parameters", nameof(methodInfo));

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

        private void CreateRuntimeFunc()
        {
            if (Call?.MethodInfo != null)
            {
                if (Call.MethodInfo.IsStatic)
                {
                    runtimeFunc = CreateDelegate(Call.TargetObject, Call.MethodInfo);
                    // runtimeFunc = (Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturnValue>) Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturnValue>),
                    //     Call.MethodInfo);
                }
                else
                {
                    runtimeFunc = CreateDelegate(Call.TargetObject, Call.MethodInfo);
                    // runtimeFunc = (Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturnValue>) Delegate.CreateDelegate(Call.MethodInfo.,
                    //     Call.TargetObject,
                    //     Call.MethodInfo.Name);
                }
            }
        }


        public static Delegate CreateDelegate(object instance, MethodInfo method)
        {
            var parameters = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var call = Expression.Call(Expression.Constant(instance), method, parameters);
            return Expression.Lambda(call, parameters).Compile();
        }
    }
}