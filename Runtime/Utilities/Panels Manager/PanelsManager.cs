using CustomAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
	public abstract class PanelsManager : SingletonMonoBehaviour<PanelsManager>
	{
		#region Game Objects & External Components

		[Foldout("PanelsManager/Game Objects & External Components")]
		[ReadOnly][SerializeField] protected List<Panel> activePanels;

		#endregion

		void OnDestroy()
		{
			for (int i = 0; i < activePanels.Count; i++)
			{
				Panel panel = activePanels[i];

				if (panel == null) 
					continue;

				if (panel is Popup popup && popup.gameObject.scene.isLoaded)
					SceneManager.UnloadSceneAsync(popup.gameObject.scene);
			}
		}

		public void OpenPanel(Panel panel, object[] openingParameters = null, bool hideCurrentPanel = false)
		{
			if (activePanels.Contains(panel))
				return;

			if (hideCurrentPanel && activePanels.Count > 0)
				activePanels[activePanels.Count - 1].gameObject.SetActive(false);

			activePanels.Add(panel);

			panel.gameObject.SetActive(true);

			panel.OnOpen(openingParameters);
		}

		public void OpenPopupWithDelay<T>(string sceneName, object[] initializationParameters = null, object[] openingParameters = null, bool hideCurrentPanel = false) where T : Popup
		{
			if (hideCurrentPanel && activePanels.Count > 0)
				activePanels[activePanels.Count - 1].gameObject.SetActive(false);

			StartCoroutine(OpenPopupWithDelayCoroutine<T>(sceneName, initializationParameters, openingParameters));
		}

		IEnumerator OpenPopupWithDelayCoroutine<T>(string sceneName, object[] initializationParameters = null, object[] openingParameters = null) where T : Popup
		{
			AsyncOperation asyncSceneLoading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			while (!asyncSceneLoading.isDone)
				yield return null;

			Scene loadedScene = SceneManager.GetSceneByName(sceneName);

			T popup = loadedScene.GetRootGameObjects()[0].GetComponentInChildren<T>(includeInactive: true);

			popup.Initialize(initializationParameters);

			activePanels.Add(popup);

			popup.gameObject.SetActive(true);

			popup.OnDelayedOpenStart();

			yield return new WaitForSeconds(popup.PopupTransitionAnimationDuration);

			popup.OnOpen(openingParameters);
		}

		public void SwapCurrentPanel(Panel panel, object[] parameters = null)
		{
			if (activePanels.Count > 0)
			{
				Panel currentPanel = activePanels[activePanels.Count - 1];

				if (currentPanel != panel)
				{
					activePanels.Remove(currentPanel);

					currentPanel.gameObject.SetActive(false);

					currentPanel.OnClose();
				}
			}

			activePanels.Add(panel);

			panel.gameObject.SetActive(true);

			panel.OnOpen(parameters);
		}

		public void CloseCurrentPopupWithDelay(float delay, bool openLastPanel = false)
		{
			Popup currentPopup = (Popup)activePanels[activePanels.Count - 1];

			StartCoroutine(ClosePopupWithDelayCoroutine(currentPopup, delay, openLastPanel));
		}

		public void ClosePanel(Panel panel)
		{
			for (int i = activePanels.Count - 1; i >= 0; i--)
			{
				Panel activePanel = activePanels[i];

				if (panel == activePanel)
				{
					activePanels.Remove(activePanel);

					activePanel.gameObject.SetActive(false);

					activePanel.OnClose();

					break;
				}
			}
		}

		public void ClosePanelWithDelay(Panel panel, float delay)
		{
			StartCoroutine(ClosePanelWithDelayCoroutine(panel, delay));
		}

		IEnumerator ClosePanelWithDelayCoroutine(Panel panel, float delay)
		{
			panel.OnDelayedCloseStart(delay);

			yield return new WaitForSeconds(delay);

			ClosePanel(panel);
		}

		public void ClosePopupWithDelay(Popup popup, float delay, bool openLastPanel = false)
		{
			StartCoroutine(ClosePopupWithDelayCoroutine(popup, delay, openLastPanel));
		}

		IEnumerator ClosePopupWithDelayCoroutine(Popup popup, float delay, bool openLastPanel = false)
		{
			popup.OnDelayedCloseStart(delay);

			yield return new WaitForSeconds(delay);

			activePanels.Remove(popup);

			popup.gameObject.SetActive(false);

			popup.OnClose();

			AsyncOperation asyncSceneUnloading = SceneManager.UnloadSceneAsync(popup.gameObject.scene.name);

			while (!asyncSceneUnloading.isDone)
				yield return null;

			if (openLastPanel && activePanels.Count > 0)
			{
				Panel currentPanel = activePanels[activePanels.Count - 1];

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