using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
	public abstract class Panel : MonoBehaviour
	{
		#region Game Objects & External Components

		protected PanelsManager panelsManager;

		#endregion

		public virtual void Initialize()
		{
			panelsManager = this.GetSingleton<PanelsManager>();
		}

		public virtual void OnOpen(List<object> parameters = null) { }

		public virtual void Close() { }

		public virtual void OnClose() { }
	}
}