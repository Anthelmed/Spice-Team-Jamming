using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using Random = Unity.Mathematics.Random;

namespace SpiceTeamJamming.UI
{
	public abstract class UIView : MonoBehaviour
	{
		[SerializeField] private bool loadViewAtStart = false;
		
		private readonly Color _baseColorButton = new (0.37f, 0.56f, 0.44f);
		private readonly Color _actionColorButton = new (0.57f, 0.86f, 0.67f);
		
		private readonly float2 _minMaxTranslation = new (-6f, 6f);
		private readonly float2 _minMaxAngle = new (-2f, 2f);
		private readonly float2 _minMaxScale = new (-0.05f, 0.05f);
		
		private const int FadeInDurationMS = 300;
		private const int FadeOutDurationMS = 150;
		private readonly Func<float, float> _fadeEasing = Easing.OutQuad;
		
		protected abstract VisualElement MainElement { get; }
		protected abstract UIRouter.RouteType Route { get; }

		private Random _random;
		
		private float RandomTranslation => _random.NextFloat(_minMaxTranslation.x, _minMaxTranslation.y);
		private float RandomAngle => _random.NextFloat(_minMaxAngle.x, _minMaxAngle.y); 
		private float RandomScale => _random.NextFloat(_minMaxScale.x, _minMaxScale.y);
		
		private Button[] _buttons = Array.Empty<Button>();
		private UIButtonManipulator[] _buttonManipulators = Array.Empty<UIButtonManipulator>();
		private readonly Dictionary<VisualElement, UIButtonManipulator> _elementToButtonManipulatorBridge = new ();

		protected virtual void Awake()
		{
			UIAnimationsUtils.FadeOut(MainElement, 0, Easing.Linear);

			_random = new Random((uint)GetInstanceID());
			
			UIRouter.ConfigureRoute(Route, this);
		}
		
		protected virtual void Start()
		{
			if (loadViewAtStart)
				UIRouter.GoToRoute(Route);
		}

		protected virtual void OnDestroy()
		{
			UIRouter.ClearRoute(Route);
		}

		protected void OnVisibilityChanged(bool value)
		{
			if (value)
				UIRouter.GoToRoute(Route);
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