using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SerializedFuncImpl
{
    /// <summary>
    /// Base class for the serialized func implementation
    /// </summary>
    /// <remarks>Mostly needed for custom editor needs</remarks>
    [Serializable]
    public abstract class SerializedFunc
    {
        /// <summary>
        /// The method reference
        /// </summary>
        [SerializeField, HideInInspector]
        private SerializedMethodReference methodReference;

        /// <summary>
        /// The method reference
        /// </summary>
        protected SerializedMethodReference MethodReference
        {
            get => methodReference;
            set => methodReference = value;
        }

        /// <summary>
        /// Invokes the method from the <see cref="methodReference"/> if not null
        /// </summary>
        /// <param name="parameters">The parameters to pass</param>
        /// <returns>The value returned by <see cref="methodReference"/> or null</returns>
        protected object Invoke(params object[] parameters)
        {
            if (MethodReference != null && MethodReference.MethodInfo != null)
            {
                return MethodReference.MethodInfo.Invoke(MethodReference.TargetObject, parameters);
            }

            return null;
        }
    }
}