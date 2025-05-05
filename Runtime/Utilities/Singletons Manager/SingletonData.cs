using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class SingletonData
    {
        #region Constants

        public static string defaultSingletonType = "None";

        #endregion

        #region Variables & Properties

        [SerializeField] string type;
        public string _type
        {
            get => type;
        }
		[SerializeField] string interfaceType;
		public string _interfaceType
		{
			get => interfaceType;
		}
		[SerializeField] GameObject gameObject;
        public GameObject _gameObject
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
