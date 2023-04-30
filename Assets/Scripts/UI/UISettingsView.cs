using System;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public class UISettingsView : UIBackableView
	{
		[Space]
		[SerializeField] private SettingsMenuDocumentAutoReferences elementsReferences;

		protected override VisualElement MainElement => elementsReferences.SettingsMenu;
		protected override UIRouter.RouteType Route => UIRouter.RouteType.Settings;
		protected override Button BackButton => elementsReferences.ActionHelperBack;

		protected override void Start()
		{
			base.Start();
			
			elementsReferences.SettingsMenuButtonControls.SetEnabled(false);
			elementsReferences.SettingsMenuButtonAudio.SetEnabled(false);
			elementsReferences.SettingsMenuButtonGraphics.SetEnabled(false);
			elementsReferences.SettingsMenuButtonAccessibility.SetEnabled(false);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			
			elementsReferences.SettingsMenuButtonControls.clicked += OnControlsButtonPressed;
			elementsReferences.SettingsMenuButtonAudio.clicked += OnAudioButtonPressed;
			elementsReferences.SettingsMenuButtonGraphics.clicked += OnGraphicsButtonPressed;
			elementsReferences.SettingsMenuButtonAccessibility.clicked += OnAccessibilityButtonPressed;
			
			InitializeButtonsManipulators(new[]
			{
				elementsReferences.SettingsMenuButtonControls,
				elementsReferences.SettingsMenuButtonAudio,
				elementsReferences.SettingsMenuButtonGraphics,
				elementsReferences.SettingsMenuButtonAccessibility,
				elementsReferences.ActionHelperBack
			});
		}
		
		protected override void OnDisable()
		{
			base.OnDisable();

			elementsReferences.SettingsMenuButtonControls.clicked -= OnControlsButtonPressed;
			elementsReferences.SettingsMenuButtonAudio.clicked -= OnAudioButtonPressed;
			elementsReferences.SettingsMenuButtonGraphics.clicked -= OnGraphicsButtonPressed;
			elementsReferences.SettingsMenuButtonAccessibility.clicked -= OnAccessibilityButtonPressed;

			ClearButtonsManipulators();
		}
		
		private void OnControlsButtonPressed()
		{
			UIRouter.GoToRoute(UIRouter.RouteType.SettingsControls);
		}
		
		private void OnAudioButtonPressed()
		{
			UIRouter.GoToRoute(UIRouter.RouteType.SettingsAudio);
		}
		
		private void OnGraphicsButtonPressed()
		{
			UIRouter.GoToRoute(UIRouter.RouteType.SettingsGraphics);
		}
		
		private void OnAccessibilityButtonPressed()
		{
			UIRouter.GoToRoute(UIRouter.RouteType.SettingsAccessibility);
		}
		
		protected override void DisplaceElementsRandomly()
		{
			DisplaceElementRandomly(elementsReferences.SettingsMenuTitle);
			DisplaceElementRandomly(elementsReferences.SettingsMenuButtonControls);
			DisplaceElementRandomly(elementsReferences.SettingsMenuButtonAudio);
			DisplaceElementRandomly(elementsReferences.SettingsMenuButtonGraphics);
			DisplaceElementRandomly(elementsReferences.SettingsMenuButtonAccessibility);
			DisplaceElementRandomly(elementsReferences.ActionHelperSelect);
			DisplaceElementRandomly(elementsReferences.ActionHelperBack);
		}
	}
}