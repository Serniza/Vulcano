using Attributes;
using UnityEngine;

namespace Utilities
{
    public class MonoBehaviourSingleton : MonoBehaviour
    {
        #region Variables & Properties

        [Foldout("Variables & Properties")]
        [SerializeField] bool permanent;

        #endregion

        protected virtual void Awake()
        {
            SingletonsManager._instance.RegisterAsSingleton(this, GetType(), permanent);
        }

        public virtual void OnRegister() { }
    }
}