using UnityEngine;

namespace Attributes
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
