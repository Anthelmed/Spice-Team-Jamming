using DefaultNamespace;
using DefaultNamespace.Tutorial;
using UIToolkitAutoReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public class UITutorialView : UIBackableView
	{
		[Space] 
		[SerializeField] private TutorialDocumentAutoReferences elementsReferences;
		
		protected override VisualElement MainElement => elementsReferences.Tutorial;
		protected override UIRouter.RouteType Route => UIRouter.RouteType.Tutorial;
		protected override Button BackButton => elementsReferences.ActionHelperBack;

		protected override void OnEnable()
		{
			base.OnEnable();
			
			GameStaticEvents.OnTutorialAsked += OnTutorialViewAsked;
			GameStaticEvents.OnDisplayPopUp += OnTutorialViewContentUpdated;
			
			InitializeButtonsManipulators(new[]
			{
				elementsReferences.ActionHelperBack
			});
		}
		
		protected override void OnDisable()
		{
			base.OnDisable();
			
			GameStaticEvents.OnTutorialAsked -= OnTutorialViewAsked;
			GameStaticEvents.OnDisplayPopUp -= OnTutorialViewContentUpdated;
			
			ClearButtonsManipulators();
		}
		
		private void OnTutorialViewAsked(TutorialEvent _)
		{
			UIRouter.GoToRoute(UIRouter.RouteType.Tutorial);
		}
		
		private void OnTutorialViewContentUpdated(string subtitle, string text, Sprite sprite)
		{
			elementsReferences.TutorialInfoSubTitle.text = subtitle;
			elementsReferences.TutorialInfoText.text = text;
			elementsReferences.TutorialInfoImage.style.backgroundImage = 
				(sprite!= null) ? Background.FromSprite(sprite) : null;
		}
		
		protected override void DisplaceElementsRandomly()
		{
			DisplaceElementRandomly(elementsReferences.TutorialTitle);
			DisplaceElementRandomly(elementsReferences.TutorialInfo);
			DisplaceElementRandomly(elementsReferences.ActionHelperBack);
		}
	}
}


