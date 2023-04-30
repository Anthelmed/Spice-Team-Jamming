using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpiceTeamJamming.UI
{
	public class UIButtonManipulator : Manipulator
	{
		private Color _baseColor;
		private Color _actionColor;

		private Translate _baseTranslate;
		private Rotate _baseRotate;
		private Scale _baseScale;
		
		private int _fadeInDurationMS;
		private int _fadeOutDurationMS;
		private Func<float, float> _fadeEasing;

		public UIButtonManipulator(Button button, Color baseColor, Color actionColor, 
			int fadeInDurationMS, int fadeOutDurationMS, Func<float, float> fadeEasing)
		{
			target = button;
			_baseColor = baseColor;
			_actionColor = actionColor;
			
			_fadeInDurationMS = fadeInDurationMS; 
			_fadeOutDurationMS = fadeOutDurationMS;
			_fadeEasing = fadeEasing;
			
			FadeColor(_baseColor, 0);
		}

		public void UpdateBaseTRS(Translate translate, Rotate rotate, Scale scale)
		{
			_baseTranslate = translate;
			_baseRotate = rotate;
			_baseScale = scale;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<PointerUpEvent>(OnPointerReleased);
			target.RegisterCallback<PointerDownEvent>(OnPointerPressed);
			
			target.RegisterCallback<PointerEnterEvent>(OnPointerEntered);
			target.RegisterCallback<PointerLeaveEvent>(OnPointerLeaved);
		}
		
		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<PointerUpEvent>(OnPointerReleased);
			target.UnregisterCallback<PointerDownEvent>(OnPointerPressed);
			
			target.UnregisterCallback<PointerEnterEvent>(OnPointerEntered);
			target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaved);
		}

		private void OnPointerReleased(PointerUpEvent _)
		{
			FadeColor(_baseColor, _fadeOutDurationMS);
		}
		
		private void OnPointerPressed(PointerDownEvent _)
		{
			FadeColor(_actionColor, _fadeInDurationMS);
		}
		
		private void OnPointerEntered(PointerEnterEvent _)
		{
			FadeTranslate(Translate.None(), _fadeInDurationMS);
			FadeRotate(Rotate.None(), _fadeInDurationMS);
			FadeScale(Scale.None(), _fadeInDurationMS);
		}
		
		private void OnPointerLeaved(PointerLeaveEvent _)
		{
			FadeTranslate(_baseTranslate, _fadeOutDurationMS);
			FadeRotate(_baseRotate, _fadeOutDurationMS);
			FadeScale(_baseScale, _fadeOutDurationMS);
		}

		private void FadeColor(Color targetColor, int durationMS)
		{
			var currentColor = target.style.unityBackgroundImageTintColor.value;
			
			target.experimental.animation.Start(currentColor, targetColor, durationMS, (element, color) =>
			{
				var tintColor = element.style.unityBackgroundImageTintColor;
				tintColor.value = color;

				element.style.unityBackgroundImageTintColor = tintColor;
			}).Ease(_fadeEasing);
		}
		
		private void FadeTranslate(Translate targetTranslate, int durationMS)
		{
			var currentTranslate = target.style.translate.value;
			var currentValue = new Vector2(currentTranslate.x.value, currentTranslate.y.value);
			var targetValue = new Vector2(targetTranslate.x.value, targetTranslate.y.value);
			
			target.experimental.animation.Start(currentValue, targetValue, durationMS, (element, value) =>
			{
				var translate = new Translate(value.x, value.y);

				element.style.translate = translate;
			}).Ease(_fadeEasing);
		}
		
		private void FadeRotate(Rotate targetRotate, int durationMS)
		{
			var currentRotate = target.style.rotate.value;
			var currentValue = currentRotate.angle.value;
			var targetValue = targetRotate.angle.value;
			
			target.experimental.animation.Start(currentValue, targetValue, durationMS, (element, value) =>
			{
				var rotate = new Rotate(value);

				element.style.rotate = rotate;
			}).Ease(_fadeEasing);
		}
		
		private void FadeScale(Scale targetScale, int durationMS)
		{
			var currentScale = target.style.scale.value;
			var currentValue = currentScale.value;
			var targetValue = targetScale.value;
			
			target.experimental.animation.Start(currentValue, targetValue, durationMS, (element, value) =>
			{
				var scale = new Scale(value);

				element.style.scale = scale;
			}).Ease(_fadeEasing);
		}
	}
}