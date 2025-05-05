using Attributes;
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
            SingletonsManager._instance.RegisterMonoBehaviourAs(GetType(), this, isPermanent);
        }

        public virtual void OnRegister() { }
    }
}