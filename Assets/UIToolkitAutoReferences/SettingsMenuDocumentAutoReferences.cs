
using UnityEngine;
using UnityEngine.UIElements;

/*
* This file was auto-generated by the UIToolkitAutoReferencesPostprocessor class
*/

namespace UIToolkitAutoReferences
{
    public class SettingsMenuDocumentAutoReferences : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        
        private VisualElement _rootVisualElement;

		private VisualElement RootVisualElement => _rootVisualElement ??=
			uiDocument.rootVisualElement;

		private VisualElement _settingsMenu;
		private VisualElement _settingsMenuTitle;
		private Button _settingsMenuButtonControls;
		private Button _settingsMenuButtonAudio;
		private Button _settingsMenuButtonGraphics;
		private Button _settingsMenuButtonAccessibility;
		private VisualElement _actionHelperSelect;
		private VisualElement _actionHelperBack;

		public VisualElement SettingsMenu => _settingsMenu ??=
			RootVisualElement.Q<VisualElement>("SettingsMenu");
		public VisualElement SettingsMenuTitle => _settingsMenuTitle ??=
			RootVisualElement.Q<VisualElement>("SettingsMenuTitle");
		public Button SettingsMenuButtonControls => _settingsMenuButtonControls ??=
			RootVisualElement.Q<Button>("SettingsMenuButtonControls");
		public Button SettingsMenuButtonAudio => _settingsMenuButtonAudio ??=
			RootVisualElement.Q<Button>("SettingsMenuButtonAudio");
		public Button SettingsMenuButtonGraphics => _settingsMenuButtonGraphics ??=
			RootVisualElement.Q<Button>("SettingsMenuButtonGraphics");
		public Button SettingsMenuButtonAccessibility => _settingsMenuButtonAccessibility ??=
			RootVisualElement.Q<Button>("SettingsMenuButtonAccessibility");
		public VisualElement ActionHelperSelect => _actionHelperSelect ??=
			RootVisualElement.Q<VisualElement>("ActionHelperSelect");
		public VisualElement ActionHelperBack => _actionHelperBack ??=
			RootVisualElement.Q<VisualElement>("ActionHelperBack");

		private void Start()
		{
			if (uiDocument == null)
                Debug.LogError($"uiDocument field empty in {name} component", this);
		}
    }
}