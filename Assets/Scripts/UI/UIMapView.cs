using SpiceTeamJamming.UI;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

public class UIMapView : UIView
{
	[Space]
	[SerializeField] private MapDocumentAutoReferences elementsReferences;

	protected override VisualElement MainElement => elementsReferences.Map;
	protected override UIRouter.RouteType Route => UIRouter.RouteType.Map;
	
	protected void OnEnable()
	{
		elementsReferences.ActionHelperPause.clicked += OnPauseButtonPressed;
			
		InitializeButtonsManipulators(new[]
		{
			elementsReferences.ActionHelperPause
		});
	}
		
	protected void OnDisable()
	{
		elementsReferences.ActionHelperPause.clicked -= OnPauseButtonPressed;

		ClearButtonsManipulators();
	}

	private void OnPauseButtonPressed()
	{
		UIRouter.GoToRoute(UIRouter.RouteType.Pause);
	}
	
	protected override void DisplaceElementsRandomly()
	{
		DisplaceElementRandomly(elementsReferences.MapTileInfo);
		DisplaceElementRandomly(elementsReferences.MapBalanceIndicator);
		DisplaceElementRandomly(elementsReferences.ActionHelperSelect);
		DisplaceElementRandomly(elementsReferences.ActionHelperPause);
	}
}
