using System;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace DefaultNamespace.UI
{
	public class UIPauseMenu : MonoBehaviour
	{
		[SerializeField] private GameManager gameManager;
		[SerializeField] private PauseMenuDocumentAutoReferences elementsReferences;
		
		private void OnEnable()
		{
			//Register to GameManager events
		}

		private void OnDisable()
		{
			//Unregister from GameManager events
		}

		private void Show()
		{
			UIAnimationsUtils.FadeIn(elementsReferences.PauseMenu, 300, Easing.InOutQuad);
		}

		private void Hide()
		{
			UIAnimationsUtils.FadeIn(elementsReferences.PauseMenu, 100, Easing.InOutQuad);
		}
	}
}