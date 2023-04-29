
using UnityEngine;
using UnityEngine.UIElements;

/*
* This file was auto-generated by the UIToolkitAutoReferencesPostprocessor class
*/

namespace UIToolkitAutoReferences
{
    public class TutorialDocumentAutoReferences : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        
        private VisualElement _rootVisualElement;

		private VisualElement RootVisualElement => _rootVisualElement ??=
			uiDocument.rootVisualElement;

		private VisualElement _tutorial;
		private VisualElement _tutorialTitle;
		private Label _tutorialTitleLabel;
		private VisualElement _tutorialInfo;
		private Label _tutorialInfoSubTitle;
		private Label _tutorialInfoText;
		private Button _actionHelperBack;

		public VisualElement Tutorial => _tutorial ??=
			RootVisualElement.Q<VisualElement>("Tutorial");
		public VisualElement TutorialTitle => _tutorialTitle ??=
			RootVisualElement.Q<VisualElement>("TutorialTitle");
		public Label TutorialTitleLabel => _tutorialTitleLabel ??=
			RootVisualElement.Q<Label>("TutorialTitleLabel");
		public VisualElement TutorialInfo => _tutorialInfo ??=
			RootVisualElement.Q<VisualElement>("TutorialInfo");
		public Label TutorialInfoSubTitle => _tutorialInfoSubTitle ??=
			RootVisualElement.Q<Label>("TutorialInfoSubTitle");
		public Label TutorialInfoText => _tutorialInfoText ??=
			RootVisualElement.Q<Label>("TutorialInfoText");
		public Button ActionHelperBack => _actionHelperBack ??=
			RootVisualElement.Q<Button>("ActionHelperBack");

		private void Start()
		{
			if (uiDocument == null)
                Debug.LogError($"uiDocument field empty in {name} component", this);
		}
    }
}