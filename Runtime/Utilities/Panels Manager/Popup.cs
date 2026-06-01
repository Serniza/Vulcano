using CustomAttributes;
using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class Popup : Panel 
    {
		#region Variables & Properties

		[Foldout("Popup/Variables")]
		[SerializeField] Vector2 initialSizeDelta;
		[Foldout("Popup/Variables")]
		[SerializeField] protected Vector2 finalSizeDelta;

		[Space]

		[Foldout("Popup/Variables")]
		[SerializeField] protected float popupTransitionAnimationDuration;
		public float PopupTransitionAnimationDuration
		{
			get => popupTransitionAnimationDuration;
		}
		[Foldout("Popup/Variables")]
		[SerializeField] AnimationCurve popupTransitionAnimationCurve;

		#endregion

		#region Internal Components

		[Foldout("Popup/Internal Components")]
		[SerializeField] RectTransform rectTransform;

		#endregion

		#if UNITY_EDITOR

		protected virtual void OnValidate()
		{
			if (!Application.isPlaying)
			{
				if (rectTransform == null)
					rectTransform = GetComponent<RectTransform>();
			}
		}

		#endif

		public override void OnDelayedOpenStart()
		{
			StartCoroutine(OnDelayedOpenStartCoroutine());
		}

		protected virtual IEnumerator OnDelayedOpenStartCoroutine()
		{
			float time = Time.time;

			while (Time.time <= time + popupTransitionAnimationDuration)
			{
				rectTransform.sizeDelta = initialSizeDelta + ((finalSizeDelta - initialSizeDelta) * popupTransitionAnimationCurve.Evaluate((Time.time - time) / popupTransitionAnimationDuration));

				yield return null;
			}

			rectTransform.sizeDelta = finalSizeDelta;
		}

		public override void OnDelayedCloseStart(float delay)
		{
			StartCoroutine(OnDelayedCloseStartCoroutine(delay));
		}

		protected virtual IEnumerator OnDelayedCloseStartCoroutine(float delay)
		{
			float time = Time.time;

			while (Time.time <= time + delay)
			{
				rectTransform.sizeDelta = initialSizeDelta + ((finalSizeDelta - initialSizeDelta) * popupTransitionAnimationCurve.Evaluate(1f - ((Time.time - time) / popupTransitionAnimationDuration)));

				yield return null;
			}

			rectTransform.sizeDelta = initialSizeDelta;
		}
	}
}
