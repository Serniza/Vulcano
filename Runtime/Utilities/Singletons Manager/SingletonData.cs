using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class SingletonData
    {
		#region Variables & Properties

		[SerializeField] string type = "None";
        public string Type
        {
            get => type;
        }

		[SerializeField] GameObject gameObject;
        public GameObject GameObject
        {
            get => gameObject;
            set => gameObject = value;
        }

		[SerializeField]
		private UnityEngine.MonoBehaviour monoBehaviour;
		public UnityEngine.MonoBehaviour MonoBehaviour
		{
			get => monoBehaviour;
			set => monoBehaviour = value;
		}

		#endregion

		SingletonData() { }

        public SingletonData(string type, GameObject gameObject, UnityEngine.MonoBehaviour monoBehaviour) 
        {
            this.type = type;

            this.gameObject = gameObject;

            this.monoBehaviour = monoBehaviour;
        }
    }
}
