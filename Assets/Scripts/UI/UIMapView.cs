using SpiceTeamJamming.UI;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIMapView : UIView
{
	[Space]
	[SerializeField] private MapDocumentAutoReferences elementsReferences;
	//[SerializeField] private InputActionReference pauseInputAction;

	protected override VisualElement MainElement => elementsReferences.Map;
	protected override UIRouter.RouteType Route => UIRouter.RouteType.Map;
	
	protected void OnEnable()
	{
	//	pauseInputAction.action.Enable();
	//	pauseInputAction.action.performed += OnPauseActionPerformed;
		elementsReferences.ActionHelperPause.clicked += OnPauseButtonPressed;

		GameManager.onHoverTileChanged += OnHoverTileChanged;		
		
		InitializeButtonsManipulators(new[]
		{
			elementsReferences.ActionHelperPause
		});
	}
	
	protected void OnDisable()
	{
	//	pauseInputAction.action.Disable();
	//	pauseInputAction.action.performed -= OnPauseActionPerformed;
		elementsReferences.ActionHelperPause.clicked -= OnPauseButtonPressed;
		
		GameManager.onHoverTileChanged -= OnHoverTileChanged;	

		ClearButtonsManipulators();
	}

	//private void OnPauseActionPerformed(InputAction.CallbackContext _)
	//{
	//	if (UIRouter.CurrentRoute != Route) return;
		
	//	UIRouter.GoToRoute(UIRouter.RouteType.Pause);
	//}
	
	private void OnPauseButtonPressed()
	{
		GameManager.instance.Pause();
	}
	
	private void OnHoverTileChanged(GameTile tile)
	{
		var tileData = tile.mapTileData;

		elementsReferences.MapTileInfoText.text = @"Biome: " + tileData.biome + "\n"
		                                          + "Status: " + tileData.tileStatus + "\n"
		                                           + "Coordinates: " + tileData.tileCoords;
	}
	
	protected override void DisplaceElementsRandomly()
	{
		DisplaceElementRandomly(elementsReferences.MapTileInfo);
		DisplaceElementRandomly(elementsReferences.MapBalanceIndicator);
		DisplaceElementRandomly(elementsReferences.ActionHelperSelect);
		DisplaceElementRandomly(elementsReferences.ActionHelperPause);
	}
}
