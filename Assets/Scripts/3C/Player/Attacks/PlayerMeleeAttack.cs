using System;
using System.Collections;
using DefaultNamespace;
using NaughtyAttributes;
using UnityEngine;

namespace _3C.Player
{
    [Serializable]
    public class PlayerMeleeAttack : PlayerStateBehavior
    {
        [Header("Settings")]
        [SerializeField] private float m_InputWaitDelay;
        
        [Header("Animation")]
        [AnimatorParam("m_Animator")]
        [SerializeField] private int m_StartAttackTriggerParam;
        [AnimatorParam("m_Animator")]
        [SerializeField] private int m_EndAttackTriggerParam;
        
        [AnimatorParam("m_Animator")]
        [SerializeField] private int m_FirstAttackTrigger;
        
        [AnimatorParam("m_Animator")]
        [SerializeField] private int m_SecondAttackTrigger;
        
        [AnimatorParam("m_Animator")]
        [SerializeField] private int m_ThirdAttackTrigger;
        
        [SerializeField] private Animator m_Animator;



        private bool m_IsAttackAsked;

        private int m_AttackIndex = 0;

        private Coroutine m_WaitForInputCoroutine;

        private bool IsWaitingForInput => m_WaitForInputCoroutine != null;

        public override void StartState()
        {
            m_Animator.SetTrigger(m_StartAttackTriggerParam);
            m_WaitForInputCoroutine = null;
            m_AttackIndex = 0;
            PlayNextAttack();
        }

        private void PlayNextAttack()
        {
            m_IsAttackAsked = false;
            m_Animator.SetTrigger(TriggerParameterFromAttackIndex);
        }

        public void OnAttackAnimationEnded()
        {
            if (m_IsAttackAsked)
            {
                IncrementAttackIndex();
                PlayNextAttack();
            }
            else
            {
                if (GameplayData.s_PlayerInputs.InputStack.Top == InputType.AttackPerformed)
                {
                    m_WaitForInputCoroutine = m_StateHandler.StartCoroutine(c_WaitForInput());
                }
                else
                {
                    ExitState();
                }
            }
        }

        private void IncrementAttackIndex()
        {
            ++m_AttackIndex;
            if (m_AttackIndex >= 3)
            {
                m_AttackIndex = 0;
            }
        }

        private IEnumerator c_WaitForInput()
        {
            yield return new WaitForSeconds(m_InputWaitDelay);
            ExitState();
        }

        private void ExitState()
        {
            m_Animator.SetTrigger(m_EndAttackTriggerParam);
            m_StateHandler.OnStateEnded();
            m_WaitForInputCoroutine = null;
        }

        public override void OnInput(InputType inputType)
        {
            if (IsWaitingForInput)
            {
                m_StateHandler.StopCoroutine(m_WaitForInputCoroutine);
                m_WaitForInputCoroutine = null;
                if (inputType != InputType.AttackPerformed)
                {
                    ExitState();
                }
                else
                {
                    IncrementAttackIndex();
                    PlayNextAttack();
                }
            }
            else
            {
                if (inputType == InputType.AttackPerformed)
                {
                    m_IsAttackAsked = true;
                }
            }
        }

        private int TriggerParameterFromAttackIndex => m_AttackIndex switch
        {
            0 => m_FirstAttackTrigger,
            1 => m_SecondAttackTrigger,
            2 => m_ThirdAttackTrigger,
            _ => throw new Exception($"Attack Index {m_AttackIndex} not handled")
        };
    }
}