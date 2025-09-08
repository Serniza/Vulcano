using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class SingletonData
    {
		#region Variables & Properties

		[SerializeField] string type;
        public string Type
        {
            get => type;
        }
		public static string defaultType = "None";

		[SerializeField] string interfaceType;
		public string InterfaceType
		{
			get => interfaceType;
		}

		[SerializeField] GameObject gameObject;
        public GameObject GameObject
        {
            get => gameObject;
            set => gameObject = value;
        }

        #endregion

        SingletonData() { }

        public SingletonData(string type, string interfaceType, GameObject gameObject) 
        {
            this.type = type;

            this.interfaceType = interfaceType;

            this.gameObject = gameObject;
        }
    }
}
