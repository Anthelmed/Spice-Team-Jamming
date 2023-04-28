using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using Random = Unity.Mathematics.Random;

namespace SpiceTeamJamming.UI
{
	public abstract class UIGeneric : MonoBehaviour
	{
		private readonly Color _baseColorButton = new (0.5254902f, 0.8588235f, 0.6470588f);
		private readonly Color _actionColorButton = new (0.5254902f, 0.7607843f, 0.572549f);
		
		private readonly float2 _minMaxTranslation = new (-12f, 12f);
		private readonly float2 _minMaxAngle = new (-4f, 4f);
		private readonly float2 _minMaxScale = new (-0.05f, 0.05f);
		
		private const int FadeInDurationMS = 300;
		private const int FadeOutDurationMS = 150;
		private readonly Func<float, float> _fadeEasing = Easing.OutQuad;
		
		protected abstract VisualElement MainElement { get; }

		private Random _random;
		
		private float RandomTranslation => _random.NextFloat(_minMaxTranslation.x, _minMaxTranslation.y);
		private float RandomAngle => _random.NextFloat(_minMaxAngle.x, _minMaxAngle.y); 
		private float RandomScale => _random.NextFloat(_minMaxScale.x, _minMaxScale.y);
		
		private Button[] _buttons;
		private UIButtonManipulator[] _buttonManipulators;
		private readonly Dictionary<VisualElement, UIButtonManipulator> _elementToButtonManipulatorBridge = new ();

		private void Awake()
		{
			UIAnimationsUtils.FadeOut(MainElement, 0, Easing.Linear);

			_random = new Random((uint)GetInstanceID());
		}
		
		protected void OnVisibilityChanged(bool value)
		{
			if (value)
				Show();
			else
				Hide();
		}
		
		[ContextMenu("Show")]
		internal void Show()
		{
			DisplaceElementsRandomly();
			
			UIAnimationsUtils.FadeIn(MainElement, FadeInDurationMS, _fadeEasing);
		}

		[ContextMenu("Hide")]
		internal void Hide()
		{
			UIAnimationsUtils.FadeOut(MainElement, FadeOutDurationMS, _fadeEasing);
		}

		protected abstract void DisplaceElementsRandomly();

		protected void InitializeButtonsManipulators(Button[] buttons)
		{
			_buttons = buttons;
			_buttonManipulators = new UIButtonManipulator[_buttons.Length];
			
			for (var index = 0; index < _buttons.Length; index++)
			{
				var button = _buttons[index];
				var buttonManipulator = new UIButtonManipulator(button, _baseColorButton, _actionColorButton,
					FadeInDurationMS, FadeOutDurationMS, _fadeEasing);
				
				button.AddManipulator(buttonManipulator);
				_buttonManipulators[index] = buttonManipulator;
				
				_elementToButtonManipulatorBridge.Add(button, buttonManipulator);
			}
		}

		protected void ClearButtonsManipulators()
		{
			for (var index = 0; index < _buttons.Length; index++)
			{
				var button = _buttons[index];
				
				button.RemoveManipulator(_buttonManipulators[index]);
			}
			
			_elementToButtonManipulatorBridge.Clear();
		}

		protected void DisplaceElementRandomly(VisualElement element)
		{
			var newTranslate = new Translate(RandomTranslation, 0);
			var newRotate = new Rotate(RandomAngle);
			var uniformScale = Vector2.one + Vector2.one * RandomScale;
			var newScale = new Scale(uniformScale);
			
			element.style.translate = newTranslate;
			element.style.rotate = newRotate;
			element.style.scale = newScale;
			
			if (_elementToButtonManipulatorBridge.TryGetValue(element, out var buttonManipulator))
				buttonManipulator.UpdateBaseTRS(newTranslate, newRotate, newScale);
		}
	}
}