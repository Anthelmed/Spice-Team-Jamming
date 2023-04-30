using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public class UIMainView : UIView
	{
		[Space]
		[SerializeField] private MainMenuDocumentAutoReferences elementsReferences;

		[SerializeField] private bool loadViewAtStart = false;
		
		protected override VisualElement MainElement => elementsReferences.MainMenu;
		protected override UIRouter.RouteType Route => UIRouter.RouteType.Main;

		private void Start()
		{
			if (loadViewAtStart)
				UIRouter.GoToRoute(UIRouter.RouteType.Main);
		}

		protected void OnEnable()
		{
			elementsReferences.MainMenuButtonNewGame.clicked += OnNewGameButtonPressed;
			elementsReferences.MainMenuButtonSettings.clicked += OnSettingsButtonPressed;
			elementsReferences.MainMenuButtonQuit.clicked += OnQuitButtonPressed;
			
			InitializeButtonsManipulators(new[]
			{
				elementsReferences.MainMenuButtonNewGame,
				elementsReferences.MainMenuButtonSettings,
				elementsReferences.MainMenuButtonQuit
			});
		}
		
		protected void OnDisable()
		{
			elementsReferences.MainMenuButtonNewGame.clicked -= OnNewGameButtonPressed;
			elementsReferences.MainMenuButtonSettings.clicked -= OnSettingsButtonPressed;
			elementsReferences.MainMenuButtonQuit.clicked -= OnQuitButtonPressed;

			ClearButtonsManipulators();
		}
		
		private void OnNewGameButtonPressed()
		{
			UIRouter.GoToRoute(UIRouter.RouteType.Map);
		}
		
		private void OnSettingsButtonPressed()
		{
			UIRouter.GoToRoute(UIRouter.RouteType.Settings);
		}
		
		private void OnQuitButtonPressed()
		{
			Application.Quit();
		}
		
		protected override void DisplaceElementsRandomly()
		{
			DisplaceElementRandomly(elementsReferences.MainMenuLogo);
			DisplaceElementRandomly(elementsReferences.MainMenuButtonNewGame);
			DisplaceElementRandomly(elementsReferences.MainMenuButtonSettings);
			DisplaceElementRandomly(elementsReferences.MainMenuButtonQuit);
			DisplaceElementRandomly(elementsReferences.ActionHelperSelect);
		}
	}
}