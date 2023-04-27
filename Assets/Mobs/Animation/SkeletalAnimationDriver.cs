using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SkeletalAnimationDriver : AnimationDriver
{
    [SerializeField] private Animator m_animator;

    private bool m_animationFinished;

    private static readonly int SPEED = Animator.StringToHash("Speed");
    private static readonly int ATTACK = Animator.StringToHash("Attack");

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
    }
}
