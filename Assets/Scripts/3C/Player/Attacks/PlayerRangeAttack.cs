using System;
using System.Collections;
using _3C.Player.Weapons;
using DefaultNamespace.HealthSystem.Damager;
using Units;
using UnityEngine;
using UnityEngine.Pool;

namespace _3C.Player
{
    [Serializable]
    public class PlayerRangeAttack : PlayerStateBehavior
    {
        [Header("Settings")]
        [SerializeField] private float m_AttackDuration;
        [SerializeField] private AnimationCurve m_AttackAnimationCurve;
        [SerializeField] private int m_BaseDamage;
        
        [SerializeField] private float m_ManaCost;

        public override float BaseManaPoints => m_ManaCost;

        [Header("Scene components")]
        [SerializeField] private RangeAttackHolder m_RangeAttackPrefab;
        
        [SerializeField] private ParticleSystem m_VFXShootVFX;
        [SerializeField] private Animator m_Animator;
        
        [Tooltip("Planed for world UI, use to enable the thing you need during aiming, will be disabled at the end of aiming")]
        [SerializeField] private GameObject m_AimingWorldUI;
        
        
        
        private Transform m_Transform;
        private IObjectPool<RangeAttackHolder> m_RangeAttackPool;

        protected override void Init(IStateHandler _stateHandler)
        {
            m_Transform = _stateHandler.gameObject.transform;
            if (m_AimingWorldUI != null)
            {
                m_AimingWorldUI.SetActive(false);
            }
            m_RangeAttackPool = new ObjectPool<RangeAttackHolder>(CreateRangeItem, maxSize: 10);
        }

        private RangeAttackHolder CreateRangeItem()
        {
            return m_StateHandler.Instantiate(m_RangeAttackPrefab);
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
            if (inputType == InputType.RangeAttackCanceled)
            {
                TriggerAttack();
            }
        }
        
        private void TriggerAttack()
        {
            StateCleaning();
            SpawnAttack();
        }

        private void SpawnAttack()
        {
            var spawned = m_RangeAttackPool.Get();
            spawned.HitBox.damage = m_BaseDamage;
            spawned.transform.position = m_Transform.position;
            spawned.transform.rotation = m_Transform.rotation;
            spawned.WeaponMovement.TriggerWeaponMovement(m_AttackDuration, m_AttackAnimationCurve);
            spawned.WeaponMovement.CurrentTweener.onComplete += () =>
            {
                m_RangeAttackPool.Release(spawned);
                // TODO: On end explosion VFX
                // TODO: Clean
            } ;
            m_Animator.SetTrigger("Ranged Attack");
            foreach (var particleSystem in spawned.VFXToPlay)
            {
                particleSystem.Play();
            }
            m_VFXShootVFX.Play();
            m_StateHandler.PlayerSoundsInstance.PlayRangeSound();
        }
    }
}