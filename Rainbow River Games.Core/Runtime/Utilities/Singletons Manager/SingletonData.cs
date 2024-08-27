using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class SingletonData
    {
        #region Constants

        public static string DEFAULT_SINGLETON_TYPE = "None";

        #endregion

        #region Variables & Properties

        [SerializeField] string type;
        public string _type
        {
            get => type;
        }
        [SerializeField] GameObject gameObject;
        public GameObject _gameObject
        {
            get => gameObject;
            set => gameObject = value;
        }

        #endregion

        SingletonData() { }

        public SingletonData(string type, GameObject gameObject) 
        {
            this.type = type;

            this.gameObject = gameObject;
        }
    }
}
