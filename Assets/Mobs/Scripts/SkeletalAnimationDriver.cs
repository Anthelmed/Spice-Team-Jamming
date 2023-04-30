using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SkeletalAnimationDriver : AnimationDriver
{
    [SerializeField] private Animator m_animator;

    private bool m_animationFinished;
    private bool m_hitFrame;

    private static readonly int SPEED = Animator.StringToHash("Speed");
    private static readonly int ATTACK = Animator.StringToHash("Attack");
    private static readonly int RANGED_ATTACK = Animator.StringToHash("RangedAttack");
    private static readonly int HIT = Animator.StringToHash("Hit");
    private static readonly int DEATH = Animator.StringToHash("Death");

    private void OnValidate()
    {
        if (!m_animator) m_animator = GetComponent<Animator>();
    }

    private void Reset()
    {
        OnValidate();
    }

    private void AnimationFinishedEvent()
    {
        m_animationFinished = true;
    }

    private void HitRangeStarts()
    {
        m_hitFrame = true;
    }

    private void HitRangeEnds()
    {
        m_hitFrame = false;
    }

    public override bool HasAnimationFinished()
    {
        return m_animationFinished;
    }

    public override void SetSpeed(float speed)
    {
        m_animator.SetFloat(SPEED, speed);
    }

    public override void TriggerAttack()
    {
        m_animator.SetTrigger(ATTACK);
        m_animationFinished = false;
        m_hitFrame = false;
    }

    public override void TriggerRangedAttack()
    {
        m_animator.SetTrigger(RANGED_ATTACK);
        m_animationFinished = false;
    }

    public override void TriggerHit()
    {
        m_animator.SetTrigger(HIT);
        m_animationFinished = false;
    }

    public override void TriggerDeath()
    {
        m_animator.SetTrigger(DEATH);
        m_animationFinished = false;
    }

    public override bool IsDamagingFrame()
    {
        return m_hitFrame;
    }
}
