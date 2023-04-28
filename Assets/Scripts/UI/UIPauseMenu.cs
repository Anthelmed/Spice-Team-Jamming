using System;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace SpiceTeamJamming.UI
{
	public class UIPauseMenu : MonoBehaviour
	{
		[SerializeField] private GameManager gameManager;
		[SerializeField] private PauseMenuDocumentAutoReferences elementsReferences;

		[Header("Colors")] 
		[SerializeField] private Color baseColorButton;
		[SerializeField] private Color hoverColorButton;

		private Button[] _buttons;
		private UIButtonManipulator[] _buttonManipulators;
		
		private void Awake()
		{
			UIAnimationsUtils.FadeOut(elementsReferences.PauseMenu, 0, Easing.Linear);

			_buttons = new[]
			{
				elementsReferences.PauseMenuButtonResume,
				elementsReferences.PauseMenuButtonSettings,
				elementsReferences.PauseMenuButtonMainMenu,
				elementsReferences.PauseMenuButtonQuit
			};
			_buttonManipulators = new UIButtonManipulator[_buttons.Length];
		}

		private void OnEnable()
		{
			//gameManager.pauseScreenVisibilityEvent += OnPauseScreenVisibilityChanged;
			
			elementsReferences.PauseMenuButtonResume.clicked += OnResumeButtonPressed;
			elementsReferences.PauseMenuButtonSettings.clicked += OnSettingsButtonPressed;
			elementsReferences.PauseMenuButtonMainMenu.clicked += OnMainMenuButtonPressed;
			elementsReferences.PauseMenuButtonQuit.clicked += OnQuitButtonPressed;

			for (var index = 0; index < _buttons.Length; index++)
			{
				var button = _buttons[index];
				var buttonManipulator = new UIButtonManipulator(button, baseColorButton, hoverColorButton);
				
				button.AddManipulator(buttonManipulator);
				_buttonManipulators[index] = buttonManipulator;
			}
		}
		
		private void OnDisable()
		{
			//gameManager.pauseScreenVisibilityEvent -= OnPauseScreenVisibilityChanged;
			
			elementsReferences.PauseMenuButtonResume.clicked -= OnResumeButtonPressed;
			elementsReferences.PauseMenuButtonSettings.clicked -= OnSettingsButtonPressed;
			elementsReferences.PauseMenuButtonMainMenu.clicked -= OnMainMenuButtonPressed;
			elementsReferences.PauseMenuButtonQuit.clicked -= OnQuitButtonPressed;
			
			for (var index = 0; index < _buttons.Length; index++)
			{
				var button = _buttons[index];
				
				button.RemoveManipulator(_buttonManipulators[index]);
			}
		}

		private void OnPauseScreenVisibilityChanged(bool value)
		{
			if (value)
				Show();
			else
				Hide();
		}
		
		private void OnResumeButtonPressed()
		{
			//gameManager.TogglePause();
		}
		
		private void OnSettingsButtonPressed()
		{
			
		}
		
		private void OnMainMenuButtonPressed()
		{
			
		}
		
		private void OnQuitButtonPressed()
		{
			
		}

		[ContextMenu("Show")]
		private void Show()
		{
			UIAnimationsUtils.FadeIn(elementsReferences.PauseMenu, 300, Easing.InOutQuad);
		}

		[ContextMenu("Hide")]
		private void Hide()
		{
			UIAnimationsUtils.FadeOut(elementsReferences.PauseMenu, 150, Easing.InOutQuad);
		}
	}
}