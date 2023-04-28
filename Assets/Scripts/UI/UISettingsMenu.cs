using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public class UISettingsMenu : UIGeneric
	{
		[Space]
		[SerializeField] private GameManager gameManager;
		[SerializeField] private SettingsMenuDocumentAutoReferences elementsReferences;

		protected override VisualElement MainElement => elementsReferences.SettingsMenu;
		
		private void OnEnable()
		{
			//Bind OnVisibilityChanged;
			
			elementsReferences.SettingsMenuButtonControls.clicked += OnControlsButtonPressed;
			elementsReferences.SettingsMenuButtonAudio.clicked += OnAudioButtonPressed;
			elementsReferences.SettingsMenuButtonGraphics.clicked += OnGraphicsButtonPressed;
			elementsReferences.SettingsMenuButtonAccessibility.clicked += OnAccessibilityButtonPressed;
			
			InitializeButtonsManipulators(new[]
			{
				elementsReferences.SettingsMenuButtonControls,
				elementsReferences.SettingsMenuButtonAudio,
				elementsReferences.SettingsMenuButtonGraphics,
				elementsReferences.SettingsMenuButtonAccessibility
			});
		}
		
		private void OnDisable()
		{
			//Unbind OnVisibilityChanged;
			
			elementsReferences.SettingsMenuButtonControls.clicked -= OnControlsButtonPressed;
			elementsReferences.SettingsMenuButtonAudio.clicked -= OnAudioButtonPressed;
			elementsReferences.SettingsMenuButtonGraphics.clicked -= OnGraphicsButtonPressed;
			elementsReferences.SettingsMenuButtonAccessibility.clicked -= OnAccessibilityButtonPressed;

			ClearButtonsManipulators();
		}
		
		private void OnControlsButtonPressed()
		{
			
		}
		
		private void OnAudioButtonPressed()
		{
			
		}
		
		private void OnGraphicsButtonPressed()
		{
			
		}
		
		private void OnAccessibilityButtonPressed()
		{
			
		}
		
		protected override void DisplaceElementsRandomly()
		{
			DisplaceElementRandomly(elementsReferences.SettingsMenuTitle);
			DisplaceElementRandomly(elementsReferences.SettingsMenuButtonControls);
			DisplaceElementRandomly(elementsReferences.SettingsMenuButtonAudio);
			DisplaceElementRandomly(elementsReferences.SettingsMenuButtonGraphics);
			DisplaceElementRandomly(elementsReferences.SettingsMenuButtonAccessibility);
		}
	}
}