using Attributes;
using System;
using UnityEngine;

namespace Utilities
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour
    {
        #region Variables & Properties

        [Foldout("SingletonMonoBehaviour/Variables")]
        [SerializeField] bool isPermanent;

		#endregion

        #if UNITY_EDITOR

		protected virtual void OnValidate()
		{
			if (!isPermanent && ((gameObject.transform.parent == null) ? false : gameObject.GetComponentInParent<SingletonsManager>(true) != null))
				isPermanent = true;
		}

        #endif

        protected virtual void Awake()
        {
            Type type = typeof(T);

			SingletonsManager.Instance.RegisterMonoBehaviourAsASingleton(type, gameObject, this, isPermanent);
        }
    }
}