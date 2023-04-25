using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace _3C.Player
{
    public class PlayerMeleeAttack : PlayerStateBehavior
    {
        [Header("Animation")]
        [AnimatorParam("m_Animator")]
        [SerializeField] private int m_MainAttackTriggerParams;
        [SerializeField] private Animator m_Animator;

        public Action OnAttackSerieEnded;
        
        public override void StartState()
        {
            base.StartState();
            PlayNextAttack();
        }

        private void PlayNextAttack()
        {
            m_Animator.SetTrigger(m_MainAttackTriggerParams);
            StartCoroutine(c_WaitFor(0.5f));
        }

        private IEnumerator c_WaitFor(float _waitTime)
        {
            yield return new WaitForSeconds(_waitTime);
            OnAttackSerieEnded?.Invoke();
        }
    }
}