using System;
using System.Collections;
using _3C.Player.Weapons;
using DefaultNamespace.HealthSystem.Damager;
using UnityEngine;

namespace _3C.Player
{
    [Serializable]
    public class PlayerRangeAttack : PlayerStateBehavior
    {
        [Header("Settings")]
        [SerializeField] private float m_AttackDuration;
        [SerializeField] private AnimationCurve m_AttackAnimationCurve;
        [SerializeField] private int m_BaseDamage;

        [Header("Scene components")]
        [SerializeField] private ColliderDamager m_Damager;
        [Tooltip("This one is used to hold the weapon and the vfx to unparent them")]
        [SerializeField] private Transform m_AttackHolder;
        
        [SerializeField] private AWeaponMovement m_RangeWeaponMovement;
        [SerializeField] private ParticleSystem[] m_VFXToPlay;
        
        [Tooltip("Planed for world UI, use to enable the thing you need during aiming, will be disabled at the end of aiming")]
        [SerializeField] private GameObject m_AimingWorldUI;
        
        
        
        private Transform m_Transform;

        protected override void Init(IStateHandler _stateHandler)
        {
            m_Transform = _stateHandler.gameObject.transform;
            if (m_AimingWorldUI != null)
            {
                m_AimingWorldUI.SetActive(false);
            }
        }

        public override void StartState()
        {
            m_StateHandler.OnAimingStateChanged(true);
            if (m_AimingWorldUI != null)
            {
                m_AimingWorldUI.SetActive(true);
            }
        }

        public override void StopState()
        {
            m_StateHandler.OnAimingStateChanged(false);
            StateCleaning();
        }

        private void StateCleaning()
        {
            if (m_AimingWorldUI != null)
            {
                m_AimingWorldUI.SetActive(false);
            }
        }

        public override void OnInput(InputType inputType)
        {
            if (inputType == InputType.AimCanceled)
            {
                TriggerAttack();
                m_StateHandler.OnStateEnded();
                //TODO: TriggerAttackAfterDelay
            }
        }
        
        

        private void TriggerAttack()
        {
            StateCleaning();
            PostAttackCleaning();
            m_Damager.Damage = m_BaseDamage;
            m_AttackHolder.transform.localPosition = Vector3.zero;
            m_AttackHolder.transform.localRotation = Quaternion.identity;
            m_AttackHolder.SetParent(null, true);
            m_RangeWeaponMovement.TriggerWeaponMovement(m_AttackDuration, m_AttackAnimationCurve);
            foreach (var particleSystem in m_VFXToPlay)
            {
                particleSystem.Play();
            }

            m_StateHandler.StartCoroutine(c_OnAttackAnimationEnded());
        }

        private IEnumerator c_OnAttackAnimationEnded()
        {
            yield return new WaitForSeconds(m_AttackDuration);
            PostAttackCleaning();
        }

        private void PostAttackCleaning()
        {
            m_RangeWeaponMovement.StopWeaponMovement();
            m_AttackHolder.SetParent(m_Transform, false);
            m_AttackHolder.transform.localPosition = Vector3.zero;
            m_AttackHolder.transform.localRotation = Quaternion.identity;
            foreach (var particleSystem in m_VFXToPlay)
            {
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}