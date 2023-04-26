using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace _3C.Player
{
    [Serializable]
    public class PlayerMeleeAttack : PlayerStateBehavior
    {
        [Header("Animation")]
        [AnimatorParam("m_Animator")]
        [SerializeField] private int m_MainAttackTriggerParams;
        [SerializeField] private Animator m_Animator;

        public Action OnAttackSerieEnded;

        [Header("Debug")]
        [SerializeField, ReadOnly]
        private bool m_CurrentlyAttacking;
        
        public override void StartState()
        {
            if (!m_CurrentlyAttacking)
            {
                PlayNextAttack();
            }
        }

        private void PlayNextAttack()
        {
            m_CurrentlyAttacking = true;
            m_Animator.SetTrigger(m_MainAttackTriggerParams);
        }

        public void OnAttackAnimationEnded()
        {
            m_CurrentlyAttacking = false;
            m_StateHandler.OnStateEnded();
        }
    }
}