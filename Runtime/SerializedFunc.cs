using System;
using UnityEngine;

namespace SerializedFunc
{
    [Serializable]
    public abstract class SerializedFunc
    {
        [SerializeField, HideInInspector]
        private EventCall eventCall;

        protected EventCall Call
        {
            get => eventCall;
            set => eventCall = value;
        }

        protected object Invoke(params object[] parameters)
        {
            if (Call != null && Call.MethodInfo != null)
            {
                return Call.MethodInfo.Invoke(Call.TargetObject, parameters);
            }

            return null;
        }
    }
}