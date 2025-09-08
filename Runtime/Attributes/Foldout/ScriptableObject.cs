using UnityEngine;

public class ScriptableObject : UnityEngine.ScriptableObject
{
	#if UNITY_EDITOR

	#pragma warning disable CS0414

    #region Variables & Properties

    [HideInInspector]
	[SerializeField] string properties = "[]";

	#endregion

	#pragma warning restore CS0414

	#endif
}
