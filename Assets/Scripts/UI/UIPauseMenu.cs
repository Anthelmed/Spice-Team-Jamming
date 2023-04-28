using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public class UIPauseMenu : UIGeneric
	{
		[Space]
		[SerializeField] private GameManager gameManager;
		[SerializeField] private PauseMenuDocumentAutoReferences elementsReferences;
		[SerializeField] private UISettingsMenu settingsMenu;

		protected override VisualElement MainElement => elementsReferences.PauseMenu;
		
		private void OnEnable()
		{
			//Bind OnVisibilityChanged;
			
			elementsReferences.PauseMenuButtonResume.clicked += OnResumeButtonPressed;
			elementsReferences.PauseMenuButtonSettings.clicked += OnSettingsButtonPressed;
			elementsReferences.PauseMenuButtonMainMenu.clicked += OnMainMenuButtonPressed;
			elementsReferences.PauseMenuButtonQuit.clicked += OnQuitButtonPressed;
			
			InitializeButtonsManipulators(new[]
			{
				elementsReferences.PauseMenuButtonResume,
				elementsReferences.PauseMenuButtonSettings,
				elementsReferences.PauseMenuButtonMainMenu,
				elementsReferences.PauseMenuButtonQuit
			});
		}
		
		private void OnDisable()
		{
			//Unbind OnVisibilityChanged;
			
			elementsReferences.PauseMenuButtonResume.clicked -= OnResumeButtonPressed;
			elementsReferences.PauseMenuButtonSettings.clicked -= OnSettingsButtonPressed;
			elementsReferences.PauseMenuButtonMainMenu.clicked -= OnMainMenuButtonPressed;
			elementsReferences.PauseMenuButtonQuit.clicked -= OnQuitButtonPressed;

			ClearButtonsManipulators();
		}
		
		private void OnResumeButtonPressed()
		{
			
		}
		
		private void OnSettingsButtonPressed()
		{
			Hide();
			settingsMenu.Show();
		}
		
		private void OnMainMenuButtonPressed()
		{
			
		}
		
		private void OnQuitButtonPressed()
		{
			
		}
		
		protected override void DisplaceElementsRandomly()
		{
			DisplaceElementRandomly(elementsReferences.PauseMenuTitle);
			DisplaceElementRandomly(elementsReferences.PauseMenuButtonResume);
			DisplaceElementRandomly(elementsReferences.PauseMenuButtonSettings);
			DisplaceElementRandomly(elementsReferences.PauseMenuButtonMainMenu);
			DisplaceElementRandomly(elementsReferences.PauseMenuButtonQuit);
		}
	}
}