using System;
using System.Collections;
using _3C.Player.Weapons;
using DefaultNamespace;
using DefaultNamespace.HealthSystem.Damager;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    [Serializable]
    struct AttackSetting
    {
        public float InputWaitDelay;

        public float DashSpeed;
        public float DashDuration;
        public AnimationCurve DashCurve;
        
        
        public float AttackDuration;
        
        public AnimationCurve WeaponMovementCurve;
        
        public int BaseDamage;
    }
    
    [Serializable]
    public class PlayerMeleeAttack : PlayerStateBehavior
    {
        // [SerializeField] private float m_InputWaitDelay;
        //
        // [SerializeField] private float m_DashSpeed;
        // [SerializeField] private float m_DashDuration;
        // [SerializeField] private AnimationCurve m_DashCurve;
        //
        //
        // [SerializeField] private float m_AttackDuration;
        //
        // [SerializeField] private AnimationCurve m_WeaponMovementCurve;
        //
        // [SerializeField] private int m_BaseDamage;
        // [SerializeField] private int m_SuccessfullComboDamage;

        
        [Header("Settings")]
        [SerializeField] private AttackSetting[] m_AttacksSettings;

        [Header("VFX")]
        [SerializeField] private float m_VFX_Start;
        [SerializeField] private ParticleSystem[] m_VFX;
        [SerializeField] private bool m_AreVFXWorldBased;
        
        private Transform m_VFXParent;



        [Header("Components")]
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
        private Tween m_DashTween;
        private Transform m_Transform;
        private bool IsWaitingForInput => m_WaitForInputCoroutine != null;

        protected override void Init(IStateHandler _stateHandler)
        {
            m_Transform = _stateHandler.gameObject.transform;
            m_VFXParent = m_VFX[0].transform.parent;
        }

        public override void StartState()
        {
            //m_Animator?.SetTrigger(m_StartAttackTriggerParam);
            m_WaitForInputCoroutine = null;
            m_AttackCoroutine = null;
            m_AttackIndex = 0;
            PlayNextAttack();
        }

        private AttackSetting CurrentAttackSettings => m_AttacksSettings[m_AttackIndex];
        
        private void PlayNextAttack()
        {
            m_IsAttackAsked = false;
            m_StateHandler.OnMovementStateChanged(false);
            m_Damager.Damage = CurrentAttackSettings.BaseDamage;
            m_WeaponMovement.TriggerWeaponMovement(CurrentAttackSettings.AttackDuration, CurrentAttackSettings.WeaponMovementCurve);
            m_StateHandler.PlayerSoundsInstance.PlayAttackSound();
            m_Animator?.SetTrigger(TriggerParameterFromAttackIndex);
            //if (m_Animator == null)
            {
                m_AttackCoroutine = m_StateHandler.StartCoroutine(c_AttackDuration());
                m_DashTween = m_Transform.DOMove(
                    m_Transform.position + m_Transform.forward * CurrentAttackSettings.DashSpeed * CurrentAttackSettings.DashDuration, CurrentAttackSettings.DashDuration
                    ).SetEase(CurrentAttackSettings.DashCurve);
                m_DashTween.onComplete += () =>
                {
                    m_StateHandler.OnMovementStateChanged(true);
                };
            }
            //else
            {
                // m_Animator?.SetTrigger(TriggerParameterFromAttackIndex);
            }
        }

        private IEnumerator c_AttackDuration()
        {
            yield return new WaitForSeconds(m_VFX_Start);
            
            PlayVFX(m_VFX[m_AttackIndex]);
            yield return new WaitForSeconds(CurrentAttackSettings.AttackDuration - m_VFX_Start);
            OnAttackAnimationEnded();
            m_AttackCoroutine = null;
        }

        private void PlayVFX(ParticleSystem particleSystem)
        {
            if (m_AreVFXWorldBased)
            {
                particleSystem.transform.parent.SetParent(null, true);
            }
            
            particleSystem.Play();
        }

        private void StopVFX(ParticleSystem particleSystem)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (m_AreVFXWorldBased)
            {
                particleSystem.transform.parent.parent = m_Transform;
                m_VFXParent.localPosition = Vector3.zero;
                m_VFXParent.localRotation = Quaternion.identity;
            }
        }

        public void OnAttackAnimationEnded()
        {
            StopVFX(m_VFX[m_AttackIndex]);
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
            yield return new WaitForSeconds(CurrentAttackSettings.InputWaitDelay);
            ExitState();
        }

        private void StateCleaning()
        {
            //m_Animator?.SetTrigger(m_EndAttackTriggerParam);
            m_StateHandler.OnMovementStateChanged(true);
            m_WeaponMovement.StopWeaponMovement();
            StopVFX(m_VFX[m_AttackIndex]);
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

            if (m_DashTween != null)
            {
                m_DashTween.Kill();
                m_DashTween = null;
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
                if (inputType != InputType.MeleeAttackPerformed)
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
                if (inputType == InputType.MeleeAttackPerformed)
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