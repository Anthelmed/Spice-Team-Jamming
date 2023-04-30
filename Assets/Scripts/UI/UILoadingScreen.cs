using System;
using System.Collections;
using SpiceTeamJamming.UI;
using UIToolkitAutoReferences;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Random = Unity.Mathematics.Random;

public class UILoadingScreen : MonoBehaviour
{
    public enum ParticleAnimationState
    {
        Play,
        Pause,
        Rewind
    }
    
    [SerializeField] private LoadingScreenDocumentAutoReferences elementsReferences;
    [SerializeField] private CustomRenderTexture backgroundCustomRenderTexture;
    [SerializeField] private float transitionSpeed = 1;
    
    private ParticleAnimationState _currentAnimationState = ParticleAnimationState.Pause;
    private float _animationTime = 0;
    
    private Texture2D _cameraScreenshot;
    private Random _random;
    
    private static readonly int ScreenshotProperty = Shader.PropertyToID("_ScreenshotRenderTexture");
    private static readonly int AnimationTimeProperty = Shader.PropertyToID("_AnimationTime");

    private void Awake()
    {
        UIAnimationsUtils.FadeOut(elementsReferences.LoadingScreen, 0, Easing.Linear);

        UIRouter.LoadingScreen = this;
        _random = new Random((uint)GetInstanceID());
    }
    
    private void Update()
    {
        SimulateAnimationTime();
    }

    private void SimulateAnimationTime()
    {
        if (_currentAnimationState == ParticleAnimationState.Pause) return;
        
        switch (_currentAnimationState)
        {
            case ParticleAnimationState.Play:
                _animationTime += Time.deltaTime * transitionSpeed;
                break;
            case ParticleAnimationState.Rewind:
                _animationTime -= Time.deltaTime * transitionSpeed;
                break;
        }
        
        _animationTime = math.clamp(_animationTime, 0f, 1f);
        
        backgroundCustomRenderTexture.material.SetFloat(AnimationTimeProperty, _animationTime);
    }

    [ContextMenu("Show")]
    internal void Show()
    {
        StartCoroutine(ShowCoroutine());
    }
    
    private IEnumerator ShowCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _cameraScreenshot = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.LeftEye);
        
        backgroundCustomRenderTexture.material.SetTexture(ScreenshotProperty, _cameraScreenshot);
        
        _animationTime = 0;
        _currentAnimationState = ParticleAnimationState.Play;
        
        UIAnimationsUtils.FadeIn(elementsReferences.LoadingScreen, 0, Easing.Linear);
    }

    [ContextMenu("Hide")]
    internal void Hide()
    {
        StartCoroutine(HideCoroutine());
    }
    
    private IEnumerator HideCoroutine()
    {
        _currentAnimationState = ParticleAnimationState.Rewind;
        
        yield return new WaitForEndOfFrame();
        _cameraScreenshot = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.LeftEye);
        
        yield return new WaitUntil(() => _animationTime == 0);
        
        UIAnimationsUtils.FadeOut(elementsReferences.LoadingScreen, 0, Easing.Linear);
        
        _currentAnimationState = ParticleAnimationState.Pause;
    }
    
}
