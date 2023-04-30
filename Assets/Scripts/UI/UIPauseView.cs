using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public class UIPauseView : UIBackableView
	{
		[Space]
		[SerializeField] private PauseMenuDocumentAutoReferences elementsReferences;

		protected override VisualElement MainElement => elementsReferences.PauseMenu;
		protected override UIRouter.RouteType Route => UIRouter.RouteType.Pause;
		protected override Button BackButton => elementsReferences.ActionHelperBack;
		
		protected override void OnEnable()
		{
			base.OnEnable();

			elementsReferences.PauseMenuButtonResume.clicked += OnResumeButtonPressed;
			elementsReferences.PauseMenuButtonSettings.clicked += OnSettingsButtonPressed;
			elementsReferences.PauseMenuButtonMainMenu.clicked += OnMainMenuButtonPressed;
			elementsReferences.PauseMenuButtonQuit.clicked += OnQuitButtonPressed;
			
			InitializeButtonsManipulators(new[]
			{
				elementsReferences.PauseMenuButtonResume,
				elementsReferences.PauseMenuButtonSettings,
				elementsReferences.PauseMenuButtonMainMenu,
				elementsReferences.PauseMenuButtonQuit,
				elementsReferences.ActionHelperBack
			});
		}
		
		protected override void OnDisable()
		{
			base.OnDisable();

			elementsReferences.PauseMenuButtonResume.clicked -= OnResumeButtonPressed;
			elementsReferences.PauseMenuButtonSettings.clicked -= OnSettingsButtonPressed;
			elementsReferences.PauseMenuButtonMainMenu.clicked -= OnMainMenuButtonPressed;
			elementsReferences.PauseMenuButtonQuit.clicked -= OnQuitButtonPressed;

			ClearButtonsManipulators();
		}
		
		private void OnResumeButtonPressed()
		{
			UIRouter.GoToPreviousRoute();
		}
		
		private void OnSettingsButtonPressed()
		{
			UIRouter.GoToRoute(UIRouter.RouteType.Settings);
		}
		
		private void OnMainMenuButtonPressed()
		{
			UIRouter.GoToRoute(UIRouter.RouteType.Main);
		}
		
		private void OnQuitButtonPressed()
		{
			Application.Quit();
		}
		
		protected override void DisplaceElementsRandomly()
		{
			DisplaceElementRandomly(elementsReferences.PauseMenuTitle);
			DisplaceElementRandomly(elementsReferences.PauseMenuButtonResume);
			DisplaceElementRandomly(elementsReferences.PauseMenuButtonSettings);
			DisplaceElementRandomly(elementsReferences.PauseMenuButtonMainMenu);
			DisplaceElementRandomly(elementsReferences.PauseMenuButtonQuit);
			DisplaceElementRandomly(elementsReferences.ActionHelperSelect);
			DisplaceElementRandomly(elementsReferences.ActionHelperBack);
		}
	}
}