using System;
using System.Collections;
using _3C.Player.Weapons;
using DefaultNamespace;
using DefaultNamespace.HealthSystem.Damager;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    [Serializable]
    public class PlayerMeleeAttack : PlayerStateBehavior
    {
        [Header("Settings")]
        [SerializeField] private float m_InputWaitDelay;
        
        [SerializeField] private float m_AttackDuration;
        [SerializeField] private float m_VFX_Start;
        [SerializeField] private AnimationCurve m_WeaponMovementCurve;
        
        [SerializeField] private int m_BaseDamage;
        [SerializeField] private int m_SuccessfullComboDamage;

        [Header("Components")]
        [SerializeField] private ParticleSystem[] m_VFX;
        [SerializeField] private AWeaponMovement m_WeaponMovement;
        [SerializeField] private ColliderDamager m_Damager;

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
        private Coroutine m_AttackCoroutine;

        private bool IsWaitingForInput => m_WaitForInputCoroutine != null;

        public override void StartState()
        {
            //m_Animator?.SetTrigger(m_StartAttackTriggerParam);
            m_WaitForInputCoroutine = null;
            m_AttackCoroutine = null;
            m_AttackIndex = 0;
            PlayNextAttack();
        }

        private void PlayNextAttack()
        {
            m_IsAttackAsked = false;
            m_StateHandler.OnMovementStateChanged(false);
            m_Damager.Damage = m_AttackIndex == 2 ? m_SuccessfullComboDamage : m_BaseDamage;
            m_WeaponMovement.TriggerWeaponMovement(m_AttackDuration, m_WeaponMovementCurve);
            m_StateHandler.PlayerSoundsInstance.PlayAttackSound();
            if (m_Animator == null)
            {
                m_AttackCoroutine = m_StateHandler.StartCoroutine(c_AttackDuration());
            }
            else
            {
                //m_Animator?.SetTrigger(TriggerParameterFromAttackIndex);
            }
        }

        private IEnumerator c_AttackDuration()
        {
            yield return new WaitForSeconds(m_VFX_Start);
            m_VFX[m_AttackIndex].Play();
            yield return new WaitForSeconds(m_AttackDuration - m_VFX_Start);
            OnAttackAnimationEnded();
            m_AttackCoroutine = null;
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
                m_StateHandler.OnMovementStateChanged(true);
                m_WeaponMovement.StopWeaponMovement();
                m_WaitForInputCoroutine = m_StateHandler.StartCoroutine(c_WaitForInput());
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

        private void StateCleaning()
        {
            //m_Animator?.SetTrigger(m_EndAttackTriggerParam);
            m_StateHandler.OnMovementStateChanged(true);
            m_WeaponMovement.StopWeaponMovement();
            m_VFX[m_AttackIndex].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (m_WaitForInputCoroutine != null)
            {
                m_StateHandler.StopCoroutine(m_WaitForInputCoroutine);
                m_WaitForInputCoroutine = null;
            }

            if (m_AttackCoroutine != null)
            {
                m_StateHandler.StopCoroutine(m_AttackCoroutine);
                m_AttackCoroutine = null;
            }
        }
        
        private void ExitState()
        {
            m_StateHandler.OnStateEnded();
            StateCleaning();
        }

        public override void StopState()
        {
            StateCleaning();
        }

        public override void OnInput(InputType inputType)
        {
            if (IsWaitingForInput)
            {
                if (inputType != InputType.AttackPerformed)
                {
                    return;
                }
                m_StateHandler.StopCoroutine(m_WaitForInputCoroutine);
                m_WaitForInputCoroutine = null;
                
                IncrementAttackIndex();
                PlayNextAttack();
            }
            else
            {
                if (inputType == InputType.MovementPerformed)
                {
                    m_IsAttackAsked = false;
                }
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