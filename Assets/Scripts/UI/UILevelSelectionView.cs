using System.Collections;
using System.Collections.Generic;
using SpiceTeamJamming.UI;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public class UILevelSelectionView : UIBackableView
	{
		[Space]
		[SerializeField] private LevelSelectionDocumentAutoReferences elementsReferences;
		
		protected override VisualElement MainElement => elementsReferences.LevelSelection;
		protected override UIRouter.RouteType Route => UIRouter.RouteType.LevelSelection;
		protected override VisualElement BackButton => elementsReferences.ActionHelperBack;
		
		protected override void OnEnable()
		{
			base.OnEnable();

			InitializeButtonsManipulators(new[]
			{
				elementsReferences.ActionHelperBack
			});
		}
		
		protected override void OnDisable()
		{
			base.OnDisable();
			
			ClearButtonsManipulators();
		}
		
		protected override void DisplaceElementsRandomly()
		{
			DisplaceElementRandomly(elementsReferences.LevelSelectionTitle);
			DisplaceElementRandomly(elementsReferences.LevelSelectionInfo);
			DisplaceElementRandomly(elementsReferences.ActionHelperSelect);
			DisplaceElementRandomly(elementsReferences.ActionHelperBack);
		}
	}
}


