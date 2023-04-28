using UnityEngine;
using UnityEngine.InputSystem;

namespace SpiceTeamJamming.UI
{
	public abstract class UIBackableView : UIView
	{
		[SerializeField] private InputActionReference backInputAction;

		protected virtual void OnEnable()
		{
			backInputAction.action.performed += OnBackActionPerformed;
		}
		
		protected virtual void OnDisable()
		{
			backInputAction.action.performed -= OnBackActionPerformed;
		}

		private void OnBackActionPerformed(InputAction.CallbackContext _)
		{
			UIRouter.GoToPreviousRoute();
		}
	}
}