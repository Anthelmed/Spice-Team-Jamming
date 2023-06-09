
using UnityEngine;
using UnityEngine.UIElements;

/*
* This file was auto-generated by the UIToolkitAutoReferencesPostprocessor class
*/

namespace UIToolkitAutoReferences
{
    public class ActionHelperMapAutoReferences : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        
        private VisualElement _rootVisualElement;

		private VisualElement RootVisualElement => _rootVisualElement ??=
			uiDocument.rootVisualElement;

		private Button _actionHelperMap;

		public Button ActionHelperMap => _actionHelperMap ??=
			RootVisualElement.Q<Button>("ActionHelperMap");

		private void Start()
		{
			if (uiDocument == null)
                Debug.LogError($"uiDocument field empty in {name} component", this);
		}
    }
}