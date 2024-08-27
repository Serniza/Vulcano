using UnityEngine;

namespace Attributes
{
    public class Foldout: PropertyAttribute
    {
        #region Variables & Properties

        public string name;

        #endregion

        public Foldout(string name)
        {
            this.name = name;
        }
    }
}
