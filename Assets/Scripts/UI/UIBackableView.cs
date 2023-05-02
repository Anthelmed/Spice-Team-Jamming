using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public abstract class UIBackableView : UIView
	{
	//	[SerializeField] private InputActionReference backInputAction;

		protected abstract Button BackButton { get; }
		
		protected virtual void OnEnable()
		{
		//	backInputAction.action.Enable();
		//	backInputAction.action.performed += OnBackActionPerformed;
			BackButton.clicked += OnBackButtonPressed;
		}
		
		protected virtual void OnDisable()
		{
		//	backInputAction.action.Disable();
		//	backInputAction.action.performed -= OnBackActionPerformed;
		   BackButton.clicked -= OnBackButtonPressed;
		}

		//private void OnBackActionPerformed(InputAction.CallbackContext _)
		//{
		//	GoBack();
		//}

		private void OnBackButtonPressed()
		{
			if (UIRouter.CurrentRoute != Route) return;
			if (UIRouter.CurrentRoute == UIRouter.RouteType.Pause) GameManager.instance.UnPause();
			if (UIRouter.CurrentRoute == UIRouter.RouteType.Settings) UIRouter.GoToPreviousRoute();
			if (UIRouter.CurrentRoute == UIRouter.RouteType.SettingsAccessibility) UIRouter.GoToPreviousRoute();
			if (UIRouter.CurrentRoute == UIRouter.RouteType.SettingsAudio) UIRouter.GoToPreviousRoute();
			//	if (UIRouter.CurrentRoute == UIRouter.RouteType.Battlefield) return;

		}

		private void GoBack()
		{ 
	
			//only handling button presses from here. all other input gets handled in player controller


		}
	}
}