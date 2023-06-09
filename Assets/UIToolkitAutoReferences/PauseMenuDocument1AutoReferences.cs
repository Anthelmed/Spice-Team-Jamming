
using UnityEngine;
using UnityEngine.UIElements;

/*
* This file was auto-generated by the UIToolkitAutoReferencesPostprocessor class
*/

namespace UIToolkitAutoReferences
{
    public class PauseMenuDocument1AutoReferences : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        
        private VisualElement _rootVisualElement;

		private VisualElement RootVisualElement => _rootVisualElement ??=
			uiDocument.rootVisualElement;

		private VisualElement _pauseMenu;
		private VisualElement _pauseMenuTitle;
		private Button _pauseMenuButtonResume;
		private Button _pauseMenuButtonSettings;
		private Button _pauseMenuButtonMainMenu;
		private Button _pauseMenuButtonQuit;
		private VisualElement _actionHelperSelect;
		private VisualElement _actionHelperBack;

		public VisualElement PauseMenu => _pauseMenu ??=
			RootVisualElement.Q<VisualElement>("PauseMenu");
		public VisualElement PauseMenuTitle => _pauseMenuTitle ??=
			RootVisualElement.Q<VisualElement>("PauseMenuTitle");
		public Button PauseMenuButtonResume => _pauseMenuButtonResume ??=
			RootVisualElement.Q<Button>("PauseMenuButtonResume");
		public Button PauseMenuButtonSettings => _pauseMenuButtonSettings ??=
			RootVisualElement.Q<Button>("PauseMenuButtonSettings");
		public Button PauseMenuButtonMainMenu => _pauseMenuButtonMainMenu ??=
			RootVisualElement.Q<Button>("PauseMenuButtonMainMenu");
		public Button PauseMenuButtonQuit => _pauseMenuButtonQuit ??=
			RootVisualElement.Q<Button>("PauseMenuButtonQuit");
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