#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Postprocessors
{
	internal class UIToolkitAutoReferencesPostprocessor : AssetPostprocessor
	{
		private const string FolderName = "UIToolkitAutoReferences";
		private static readonly string FolderPath = $"Assets/{FolderName}";
		private const string FileNameSuffix = "AutoReferences";
		private const string AutoReferenceClassName = "auto-reference";
		private const string UXMLFileExtension = ".uxml";

		private static void OnPostprocessAllAssets(string[] importedAssetPaths, string[] deletedAssetPaths,
			string[] movedAssetPaths,
			string[] movedFromAssetPaths)
		{
			var refreshAssetDatabase = false;
			
			foreach (var assetPath in importedAssetPaths)
			{
				var fileInfo = new FileInfo(assetPath);

				if (!string.Equals(fileInfo.Extension, UXMLFileExtension)) continue;

				refreshAssetDatabase = CreateOrUpdateScriptFile(assetPath);
			}

			foreach (var assetPath in deletedAssetPaths)
			{
				var fileInfo = new FileInfo(assetPath);

				if (!string.Equals(fileInfo.Extension, UXMLFileExtension)) continue;

				refreshAssetDatabase = DeleteScriptFile(assetPath);
			}
			
			if (refreshAssetDatabase)
				AssetDatabase.Refresh();
		}
		
		private static bool CreateOrUpdateScriptFile(string assetPath)
		{
			var refreshAssetDatabase = false;
			
			if (!AssetDatabase.IsValidFolder(FolderPath))
				AssetDatabase.CreateFolder("Assets", FolderName);

			var fileInfo = new FileInfo(assetPath);

			var shortName = ShortName(fileInfo);
			var fileName = $"{ToPascalCase(shortName)}{FileNameSuffix}";
			var filePath = $"{FolderPath}/{fileName}.cs";

			if (File.Exists(filePath))
				refreshAssetDatabase = DeleteScriptFile(assetPath);

			var content = GenerateScriptContent(assetPath);

			if (string.IsNullOrEmpty(content)) return refreshAssetDatabase;

			File.WriteAllText(filePath, content);
			
			return true;
		}

		private static bool DeleteScriptFile(string assetPath)
		{
			var fileInfo = new FileInfo(assetPath);

			var shortName = ShortName(fileInfo);
			var fileName = $"{ToPascalCase(shortName)}{FileNameSuffix}";
			var filePath = $"{FolderPath}/{fileName}.cs";

			if (!File.Exists(filePath)) return false;

			File.Delete(filePath);
			
			return true;
		}

		private static string GenerateScriptContent(string assetPath)
		{
			var fileInfo = new FileInfo(assetPath);

			var shortName = ShortName(fileInfo);
			var fileName = $"{ToPascalCase(shortName)}{FileNameSuffix}";

			var visualTreeAsset = (VisualTreeAsset)AssetDatabase.LoadAssetAtPath(assetPath, typeof(VisualTreeAsset));

			var root = visualTreeAsset.CloneTree();

			var taggedVisualElements = root.Query(className: AutoReferenceClassName).ToList();

			if (taggedVisualElements.Count == 0) return string.Empty;

			var content = ScriptContentTemplate;
			content = content.Replace(FileNameVariable, fileName);

			var privateElementReferences = GeneratePrivateElementReferencesContent(taggedVisualElements);
			var publicElementReferences = GeneratePublicElementReferencesContent(taggedVisualElements);

			content = content.Replace(PrivateElementReferencesVariable, privateElementReferences);
			content = content.Replace(PublicElementReferencesVariable, publicElementReferences);

			return content;
		}

		private static string GeneratePrivateElementReferencesContent(List<VisualElement> visualElements)
		{
			var elementReferences = new List<string>();

			foreach (var visualElement in visualElements)
			{
				var elementType = visualElement.GetType().Name;
				var elementID = visualElement.name;
				var elementPrivateName = ToCamelCase(elementID);

				var elementReference = PrivateElementReferenceTemplate;
				elementReference = elementReference.Replace(ElementTypeVariable, elementType);
				elementReference = elementReference.Replace(PrivateElementNameVariable, elementPrivateName);

				elementReferences.Add(elementReference);
			}

			return string.Join("\n", elementReferences);
		}

		private static string GeneratePublicElementReferencesContent(List<VisualElement> visualElements)
		{
			var elementReferences = new List<string>();

			foreach (var visualElement in visualElements)
			{
				var elementType = visualElement.GetType().Name;
				var elementID = visualElement.name;
				var elementPrivateName = ToCamelCase(elementID);
				var elementPublicName = ToPascalCase(elementID);

				var elementReference = PublicElementReferenceTemplate;
				elementReference = elementReference.Replace(ElementTypeVariable, elementType);
				elementReference = elementReference.Replace(PrivateElementNameVariable, elementPrivateName);
				elementReference = elementReference.Replace(PublicElementNameVariable, elementPublicName);
				elementReference = elementReference.Replace(ElementIDVariable, elementID);

				elementReferences.Add(elementReference);
			}

			return string.Join("\n", elementReferences);
		}

		#region StringExtensionsRegion

		private static string ShortName(FileInfo fileInfo)
		{
			return fileInfo.Name.Replace(fileInfo.Extension, "");
		}
		
		private static string ToCamelCase(string value)
		{
			value = value.Replace(" ", "-");
			value = value.Replace("_", "-");
			value = value.Replace("--", "-");
			
			value = Regex.Replace(value, "([A-Z])([A-Z]+)($|[A-Z])",
				m => $"{m.Groups[1].Value}{m.Groups[2].Value.ToLower()}{m.Groups[3].Value}");
			value = Regex.Replace(value, "-.", m => m.Value.ToUpper()[1..]);
			value = $"{char.ToLower(value[0])}{value[1..]}";

			return value;
		}

		private static string ToPascalCase(string value)
		{
			value = ToCamelCase(value);
			value = $"{char.ToUpper(value[0])}{value[1..]}";

			return value;
		}

		#endregion

		#region TemplateRegion

		private const string FileNameVariable = "#FILE_NAME#";

		private const string PrivateElementReferencesVariable = "#PRIVATE_ELEMENT_REFERENCES#";
		private const string PublicElementReferencesVariable = "#PUBLIC_ELEMENT_REFERENCES#";

		private const string ElementTypeVariable = "#ELEMENT_TYPE#";
		private const string PrivateElementNameVariable = "#PRIVATE_ELEMENT_NAME#";
		private const string PublicElementNameVariable = "#PUBLIC_ELEMENT_NAME#";
		private const string ElementIDVariable = "#ELEMENT_ID#";

		private const string ScriptContentTemplate = @"
using UnityEngine;
using UnityEngine.UIElements;

/*
* This file was auto-generated by the UIToolkitAutoReferencesPostprocessor class
*/

namespace "+FolderName+@"
{
    public class "+FileNameVariable+@" : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        
        private VisualElement _rootVisualElement;

		private VisualElement RootVisualElement => _rootVisualElement ??=
			uiDocument.rootVisualElement;

"+PrivateElementReferencesVariable+@"

"+PublicElementReferencesVariable+@"

		private void Start()
		{
			if (uiDocument == null)
                Debug.LogError($""uiDocument field empty in {name} component"", this);
		}
    }
}";

		private const string PrivateElementReferenceTemplate = @"		private "+ElementTypeVariable+@" _"+PrivateElementNameVariable+@";";

		private const string PublicElementReferenceTemplate =
			@"		public " + ElementTypeVariable + @" "+PublicElementNameVariable+@" => _"+PrivateElementNameVariable+@" ??=
			RootVisualElement.Q<"+ElementTypeVariable+@">("""+ElementIDVariable+@""");";

		#endregion
	}
}

#endif