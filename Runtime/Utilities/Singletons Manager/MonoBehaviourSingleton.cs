using Attributes;
using System;
using UnityEngine;

namespace Utilities
{
    public class MonoBehaviourSingleton : MonoBehaviour
    {
        #region Variables & Properties

        [Foldout("Variables (MonoBehaviourSingleton)")]
        [SerializeField] bool isPermanent;

        #endregion

        protected virtual void Awake()
        {
            Type type = GetType();

            while (type.BaseType != typeof(MonoBehaviourSingleton))
                type = type.BaseType;

			SingletonsManager._instance.RegisterMonoBehaviourAs(type, this, isPermanent);
        }

        public virtual void OnRegister() { }
    }
}