using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class VatAnimationDriver : AnimationDriver
{
    public VatData animationData;
    public float transitionDuration = 0.25f;

    [SerializeField] private MeshRenderer m_renderer;

    private MaterialPropertyBlock m_mpb;
    [SerializeField] private int m_currentAnimation = -1;
    private Vector4 m_previousData;
    private float m_specialAnimationEnds;

    private readonly int ANIM_DATA = Shader.PropertyToID("_AnimData");
    private readonly int PREV_ANIM_DATA = Shader.PropertyToID("_PrevAnimData");
    private readonly int TRANSITION = Shader.PropertyToID("_Transition");

    private enum AnimationID
    {
        Idle = 0,
        Walk,
        Attack,
        Hit
    }

    private void OnValidate()
    {
        m_renderer = GetComponent<MeshRenderer>();
    }

    private void Reset()
    {
        OnValidate();
    }

    private void Start()
    {
        m_mpb = new MaterialPropertyBlock();
        SwitchAnimation(AnimationID.Idle);
        m_renderer.localBounds = new Bounds(Vector3.zero, Vector3.one * 2);
    }

    private void SwitchAnimation(AnimationID desiredAnimationID)
    {
        var desiredAnimation = (int)desiredAnimationID;

        if (!animationData || m_currentAnimation == desiredAnimation)
            return;

        m_currentAnimation = desiredAnimation;
        var data = animationData.animations[desiredAnimation];
        data.w = Time.timeSinceLevelLoad;
        m_mpb.SetVector(ANIM_DATA, data);
        m_mpb.SetVector(PREV_ANIM_DATA, m_previousData);
        m_mpb.SetFloat(TRANSITION, transitionDuration);
        m_previousData = data;
        m_renderer.SetPropertyBlock(m_mpb);
    }

    public override void SetSpeed(float speed)
    {
        if (HasAnimationFinished())
            SwitchAnimation(speed < 0.5f ? AnimationID.Idle : AnimationID.Walk);
    }

    public override void TriggerAttack()
    {
        m_specialAnimationEnds = Time.timeSinceLevelLoad + animationData.animations[(int)AnimationID.Attack].x - transitionDuration;
        SwitchAnimation(AnimationID.Attack);
    }

    public override bool HasAnimationFinished()
    {
        return m_specialAnimationEnds < Time.timeSinceLevelLoad;
    }

    public override void TriggerHit()
    {
        m_specialAnimationEnds = Time.timeSinceLevelLoad + animationData.animations[(int)AnimationID.Hit].x - transitionDuration;
        SwitchAnimation(AnimationID.Hit);
    }

    public override void TriggerDeath()
    {
        m_specialAnimationEnds = Time.timeSinceLevelLoad + animationData.animations[(int)AnimationID.Hit].x * 0.5f - transitionDuration;
        SwitchAnimation(AnimationID.Hit);
    }
}
