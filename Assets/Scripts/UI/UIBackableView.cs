using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public abstract class UIBackableView : UIView
	{
		[SerializeField] private InputActionReference backInputAction;

		protected abstract Button BackButton { get; }
		
		protected virtual void OnEnable()
		{
			backInputAction.action.Enable();
			backInputAction.action.performed += OnBackActionPerformed;
			BackButton.clicked += OnBackButtonPressed;
		}
		
		protected virtual void OnDisable()
		{
			backInputAction.action.Disable();
			backInputAction.action.performed -= OnBackActionPerformed;
			BackButton.clicked -= OnBackButtonPressed;
		}

		private void OnBackActionPerformed(InputAction.CallbackContext _)
		{
			GoBack();
		}

		private void OnBackButtonPressed()
		{
			GoBack();
		}

		private void GoBack()
		{
			if (UIRouter.CurrentRoute != Route) return;
			
			UIRouter.GoToPreviousRoute();
		}
	}
}