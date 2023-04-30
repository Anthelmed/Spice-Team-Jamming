using DefaultNamespace;
using SpiceTeamJamming.UI;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

public class UIBattlefieldView : UIView
{
	[Space]
	[SerializeField] private BattlefieldDocumentAutoReferences elementsReferences;

	[SerializeField] private CustomRenderTexture playerStatHealthCustomRenderTexture;
	[SerializeField] private CustomRenderTexture playerStatManaCustomRenderTexture;
	
	private static readonly int _percent = Shader.PropertyToID("_Percent");

	protected override VisualElement MainElement => elementsReferences.Battlefield;
	protected override UIRouter.RouteType Route => UIRouter.RouteType.Battlefield;
	
	protected void OnEnable()
	{
		elementsReferences.ActionHelperMap.clicked += OnMapButtonPressed;
		elementsReferences.ActionHelperPause.clicked += OnPauseButtonPressed;
			
		PlayerStaticEvents.s_PlayerHealthChanged += OnPlayerHealthChanged;
		PlayerStaticEvents.s_PlayerManaChanged += OnPlayerManaChanged;
		
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
		
		PlayerStaticEvents.s_PlayerHealthChanged -= OnPlayerHealthChanged;
		PlayerStaticEvents.s_PlayerManaChanged -= OnPlayerManaChanged;

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
	
	private void OnPlayerHealthChanged(float value)
	{
		playerStatHealthCustomRenderTexture.material.SetFloat(_percent, value);
	}
	
	private void OnPlayerManaChanged(float value)
	{
		playerStatManaCustomRenderTexture.material.SetFloat(_percent, value);
	}
	
	protected override void DisplaceElementsRandomly()
	{
		DisplaceElementRandomly(elementsReferences.BattlefieldPlayerStatHealth);
		DisplaceElementRandomly(elementsReferences.BattlefieldPlayerStatMana);
		DisplaceElementRandomly(elementsReferences.BattlefieldBalanceIndicator);
		DisplaceElementRandomly(elementsReferences.BattlefieldMinimap);
		DisplaceElementRandomly(elementsReferences.ActionHelperMap);
		DisplaceElementRandomly(elementsReferences.ActionHelperPause);
	}
}
