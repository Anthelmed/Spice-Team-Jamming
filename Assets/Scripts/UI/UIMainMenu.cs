﻿using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public class UIMainMenu : UIView
	{
		[Space]
		[SerializeField] private GameManager gameManager;
		[SerializeField] private MainMenuDocumentAutoReferences elementsReferences;

		protected override VisualElement MainElement => elementsReferences.MainMenu;
		protected override UIRouter.RouteType Route => UIRouter.RouteType.Main;
		
		protected void OnEnable()
		{
			//Bind OnVisibilityChanged;
			
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
			//Unbind OnVisibilityChanged;
			
			elementsReferences.MainMenuButtonNewGame.clicked -= OnNewGameButtonPressed;
			elementsReferences.MainMenuButtonSettings.clicked -= OnSettingsButtonPressed;
			elementsReferences.MainMenuButtonQuit.clicked -= OnQuitButtonPressed;

			ClearButtonsManipulators();
		}
		
		private void OnNewGameButtonPressed()
		{
			UIRouter.GoToRoute(UIRouter.RouteType.LevelSelection);
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