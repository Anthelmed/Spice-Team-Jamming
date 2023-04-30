
using UnityEngine;
using UnityEngine.UIElements;

/*
* This file was auto-generated by the UIToolkitAutoReferencesPostprocessor class
*/

namespace UIToolkitAutoReferences
{
    public class BattlefieldDocumentAutoReferences : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        
        private VisualElement _rootVisualElement;

		private VisualElement RootVisualElement => _rootVisualElement ??=
			uiDocument.rootVisualElement;

		private VisualElement _battlefield;
		private VisualElement _battlefieldBalanceIndicator;
		private VisualElement _battlefieldMinimap;
		private Button _actionHelperMap;
		private Button _actionHelperPause;

		public VisualElement Battlefield => _battlefield ??=
			RootVisualElement.Q<VisualElement>("Battlefield");
		public VisualElement BattlefieldBalanceIndicator => _battlefieldBalanceIndicator ??=
			RootVisualElement.Q<VisualElement>("BattlefieldBalanceIndicator");
		public VisualElement BattlefieldMinimap => _battlefieldMinimap ??=
			RootVisualElement.Q<VisualElement>("BattlefieldMinimap");
		public Button ActionHelperMap => _actionHelperMap ??=
			RootVisualElement.Q<Button>("ActionHelperMap");
		public Button ActionHelperPause => _actionHelperPause ??=
			RootVisualElement.Q<Button>("ActionHelperPause");

		private void Start()
		{
			if (uiDocument == null)
                Debug.LogError($"uiDocument field empty in {name} component", this);
		}
    }
}