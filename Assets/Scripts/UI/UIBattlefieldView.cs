using SpiceTeamJamming.UI;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

public class UIBattlefieldView : UIView
{
	[Space]
	[SerializeField] private BattlefieldDocumentAutoReferences elementsReferences;

	protected override VisualElement MainElement => elementsReferences.Battlefield;
	protected override UIRouter.RouteType Route => UIRouter.RouteType.Battlefield;
	
	protected void OnEnable()
	{
		elementsReferences.ActionHelperMap.clicked += OnMapButtonPressed;
		elementsReferences.ActionHelperPause.clicked += OnPauseButtonPressed;
			
		InitializeButtonsManipulators(new[]
		{
			elementsReferences.ActionHelperMap,
			elementsReferences.ActionHelperPause
		});
	}
		
	protected void OnDisable()
	{
		elementsReferences.ActionHelperMap.clicked -= OnMapButtonPressed;
		elementsReferences.ActionHelperPause.clicked -= OnPauseButtonPressed;

		ClearButtonsManipulators();
	}

	private void OnMapButtonPressed()
	{
		UIRouter.GoToRoute(UIRouter.RouteType.Map);
	}
	
	private void OnPauseButtonPressed()
	{
		UIRouter.GoToRoute(UIRouter.RouteType.Pause);
	}
	
	protected override void DisplaceElementsRandomly()
	{
		DisplaceElementRandomly(elementsReferences.BattlefieldBalanceIndicator);
		DisplaceElementRandomly(elementsReferences.BattlefieldMinimap);
		DisplaceElementRandomly(elementsReferences.ActionHelperMap);
		DisplaceElementRandomly(elementsReferences.ActionHelperPause);
	}
}
