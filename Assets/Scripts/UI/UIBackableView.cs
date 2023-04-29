using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace SpiceTeamJamming.UI
{
	public abstract class UIBackableView : UIView
	{
		[SerializeField] private InputActionReference backInputAction;

		protected abstract VisualElement BackButton { get; }
		
		protected virtual void OnEnable()
		{
			backInputAction.action.Enable();
			backInputAction.action.performed += OnBackActionPerformed;
			BackButton.RegisterCallback<PointerUpEvent>(OnPointerReleased);
		}
		
		protected virtual void OnDisable()
		{
			backInputAction.action.Disable();
			backInputAction.action.performed -= OnBackActionPerformed;
			BackButton.UnregisterCallback<PointerUpEvent>(OnPointerReleased);
		}

		private void OnBackActionPerformed(InputAction.CallbackContext _)
		{
			GoBack();
		}

		private void OnPointerReleased(PointerUpEvent _)
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