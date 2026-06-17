using UnityEngine;

namespace SernizaGamesCore
{
    public class Foldout: PropertyAttribute
    {
        #region Variables & Properties

        public string path;

        #endregion

        public Foldout(string path)
        {
            this.path = path;
        }
    }
}
