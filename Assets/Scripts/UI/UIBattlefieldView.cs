using DefaultNamespace;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
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
			//mapInputAction.action.Disable();
		//	mapInputAction.action.performed -= OnMapActionPerformed;
			elementsReferences.ActionHelperMap.clicked -= OnMapButtonPressed;
			
			//pauseInputAction.action.Disable();
			//pauseInputAction.action.performed -= OnPauseActionPerformed;
			elementsReferences.ActionHelperPause.clicked -= OnPauseButtonPressed;

			PlayerStaticEvents.s_PlayerHealthChanged -= OnPlayerHealthChanged;
			PlayerStaticEvents.s_PlayerManaChanged -= OnPlayerManaChanged;

			ClearButtonsManipulators();
		}

		//private void OnMapActionPerformed(InputAction.CallbackContext _)
		//{
		//	if (UIRouter.CurrentRoute != Route) return;

		//	GoToMap();
		//}
		
		private void OnMapButtonPressed()
		{
			GoToMap();
		}
		
		private void OnPauseButtonPressed()
		{
			//UIRouter.GoToRoute(UIRouter.RouteType.Pause);
			GameManager.instance.Pause();
		}

		private void OnPlayerHealthChanged(float value)
		{
			playerStatHealthCustomRenderTexture.material.SetFloat(_percent, value);
		}

		private void OnPlayerManaChanged(float value)
		{
			playerStatManaCustomRenderTexture.material.SetFloat(_percent, value);
		}

		private void GoToMap()
		{

			GameManager.instance.TryTransitionToMap();
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
}