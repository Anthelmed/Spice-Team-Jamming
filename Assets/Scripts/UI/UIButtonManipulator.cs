using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace SpiceTeamJamming.UI
{
	public class UIButtonManipulator : Manipulator
	{
		private Color _baseColor;
		private Color _hoverColor;

		private const int FadeInDurationMS = 300;
		private const int FadeOutDurationMS = 150;
		private readonly Func<float, float> FadeEasing = Easing.InOutQuad;

		public UIButtonManipulator(Button button, Color baseColor, Color hoverColor)
		{
			target = button;
			_baseColor = baseColor;
			_hoverColor = hoverColor;
			
			FadeColor(_baseColor, 0);
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<PointerEnterEvent>(OnPointerEntered);
			target.RegisterCallback<PointerLeaveEvent>(OnPointerLeaved);
		}
		
		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<PointerEnterEvent>(OnPointerEntered);
			target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaved);
		}
		
		private void OnPointerEntered(PointerEnterEvent _)
		{
			FadeColor(_hoverColor, FadeInDurationMS);
		}
		
		private void OnPointerLeaved(PointerLeaveEvent _)
		{
			FadeColor(_baseColor, FadeOutDurationMS);
		}

		private void FadeColor(Color targetColor, int durationMS)
		{
			var currentColor = target.style.unityBackgroundImageTintColor.value;
			
			target.experimental.animation.Start(currentColor, targetColor, durationMS, (element, color) =>
			{
				var tintColor = element.style.unityBackgroundImageTintColor;
				tintColor.value = color;

				element.style.unityBackgroundImageTintColor = tintColor;
			}).Ease(FadeEasing);
		}
	}
}