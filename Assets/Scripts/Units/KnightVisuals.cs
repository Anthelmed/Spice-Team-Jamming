using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(Animator))]
    public class KnightVisuals : MobVisuals
    {
        [SerializeField] private Animator m_animator;

        private bool m_animationFinished;
        private bool m_hitFrame;

        private static readonly int SPEED = Animator.StringToHash("Speed");
        private static readonly int ATTACK = Animator.StringToHash("Attack");
        private static readonly int RANGED_ATTACK = Animator.StringToHash("RangedAttack");
        private static readonly int HIT = Animator.StringToHash("Hit");
        private static readonly int DEATH = Animator.StringToHash("Death");

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!m_animator) m_animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            m_animationFinished = true;
            m_hitFrame = false;
        }

        public void AnimationFinishedEvent()
        {
            m_animationFinished = true;
        }

        public void HitRangeStarts()
        {
            m_hitFrame = true;
        }

        public void HitRangeEnds()
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
            if (m_sounds && m_unit.Visible) m_sounds.PlayAttackSound();

            m_animator.SetTrigger(ATTACK);
            m_animationFinished = false;
            m_hitFrame = false;
        }

        public override void TriggerRangedAttack()
        {
            if (m_sounds && m_unit.Visible) m_sounds.PlayAttackSound();

            m_animator.SetTrigger(RANGED_ATTACK);
            m_animationFinished = false;
        }

        public override void TriggerHit()
        {
            if (m_sounds && m_unit.Visible) m_sounds.PlayDamageSound();

            m_animator.SetTrigger(HIT);
            m_animationFinished = false;
        }

        public override void TriggerDeath()
        {
            if (m_sounds && m_unit.Visible) m_sounds.PlayDamageSound();

            m_animator.SetTrigger(DEATH);
            m_animationFinished = false;
        }

        public override bool IsDamagingFrame()
        {
            return m_hitFrame;
        }
    }
}