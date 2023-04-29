using System;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public static class UIAnimationsUtils
{
	public static ValueAnimation<float> FadeIn(VisualElement visualElement, int durationMS, Func<float, float> easing, Action callback = null, bool disabledWhenInvisible = true)
	{
		var fromOpacity = visualElement.style.opacity.value;
		
		return visualElement.experimental.animation.Start(fromOpacity, 1, durationMS, (_, opacity) =>
		{
			visualElement.style.opacity = opacity;
			
			if (disabledWhenInvisible)
				visualElement.style.display = opacity > 0 ? DisplayStyle.Flex : DisplayStyle.None;
		})
		.OnCompleted(callback)
		.Ease(easing);
	}
	
	public static ValueAnimation<float> FadeOut(VisualElement visualElement, int durationMS, Func<float, float> easing, Action callback = null, bool disabledWhenInvisible = true)
	{
		var fromOpacity = visualElement.style.opacity.value;
		
		return visualElement.experimental.animation.Start(fromOpacity, 0, durationMS, (_, opacity) =>
		{
			visualElement.style.opacity = opacity;
			
			if (disabledWhenInvisible)
				visualElement.style.display = opacity > 0 ? DisplayStyle.Flex : DisplayStyle.None;
		})
		.OnCompleted(callback)
		.Ease(easing);
	}
}