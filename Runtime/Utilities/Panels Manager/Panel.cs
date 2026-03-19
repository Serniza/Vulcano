using UnityEngine;

namespace Utilities
{
	public abstract class Panel : MonoBehaviour, IInitializable
	{
		#region Variables & Properties

		public bool IsInitialized { get; private set; }

		#endregion

		#region Game Objects & External Components

		protected PanelsManager panelsManager;

		#endregion

		public virtual void Initialize()
		{
			IsInitialized = true;

			panelsManager = this.GetSingleton<PanelsManager>();
		}

		public virtual void OnOpen(object[] parameters = null) { }

		public virtual void OnClose(object[] parameters = null) { }
	}
}