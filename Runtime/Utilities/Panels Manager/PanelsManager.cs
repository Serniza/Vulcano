using Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
	public abstract class PanelsManager : SingletonMonoBehaviour
	{
		#region GameObjects & External Components

		[Foldout("PanelsManager/GameObjects & External Components")]
		[ReadOnly][SerializeField] protected List<Panel> activePanels;

		#endregion

		public void OpenPanel(Panel panel, List<object> parameters = null, bool hideCurrentPanel = false)
		{
			if (activePanels.Count > 0)
			{
				Panel currentPanel = activePanels[activePanels.Count - 1];

				if (currentPanel == panel)
				{
					panel.OnOpen(parameters);

					return;
				}

				if (hideCurrentPanel)
				{
					if (currentPanel != panel)
						currentPanel.gameObject.SetActive(false);
				}
			}

			activePanels.Add(panel);

			panel.gameObject.SetActive(true);

			panel.OnOpen(parameters);
		}

		public void SwapCurrentPanel(Panel panel, List<object> parameters = null)
		{
			if (activePanels.Count > 0)
			{
				Panel currentPanel = activePanels[activePanels.Count - 1];

				if (currentPanel != panel)
				{
					activePanels.Remove(currentPanel);

					currentPanel.gameObject.SetActive(false);
				}
			}

			activePanels.Add(panel);

			panel.gameObject.SetActive(true);

			panel.OnOpen(parameters);
		}

		public void CloseCurrentPanel(bool openLastPanel = false)
		{
			Panel currentPanel = activePanels[activePanels.Count - 1];

			activePanels.Remove(currentPanel);

			currentPanel.gameObject.SetActive(false);

			currentPanel.OnClose();

			if (openLastPanel && activePanels.Count > 0)
			{
				currentPanel = activePanels[activePanels.Count - 1];

				if (!currentPanel.gameObject.activeSelf)
				{
					currentPanel.gameObject.SetActive(true);

					currentPanel.OnOpen();
				}
			}
		}

		public void CloseAllPanelsUntil(Panel panel, bool openPanel = false)
		{
			for (int i = activePanels.Count - 1; i >= 0; i--)
			{
				Panel currentPanel = activePanels[i];

				if (currentPanel != panel)
				{
					activePanels.Remove(currentPanel);

					currentPanel.gameObject.SetActive(false);

					currentPanel.OnClose();
				}
				else
				{
					if (openPanel && !panel.gameObject.activeSelf)
					{
						panel.gameObject.SetActive(true);

						panel.OnOpen();
					}

					break;
				}
			}
		}
	}
}