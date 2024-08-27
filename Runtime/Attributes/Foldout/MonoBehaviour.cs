public class MonoBehaviour : UnityEngine.MonoBehaviour
{
	#if UNITY_EDITOR

	#pragma warning disable CS0414

	#region Variables & Properties

	[UnityEngine.HideInInspector]
    [UnityEngine.SerializeField] string properties = "[]";

	#endregion

	#pragma warning restore CS0414

	#endif
}
