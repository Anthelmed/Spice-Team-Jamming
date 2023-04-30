using System;
using System.Collections;
using _3C.Player.Weapons;
using DefaultNamespace.HealthSystem.Damager;
using Units;
using UnityEngine;
using UnityEngine.Pool;

namespace _3C.Player
{
    enum RangeAttackPhase
    {
        SimpleAttack,
        ChargedBuildUp,
        OptimalChargedAttack,
        NotOptimalChargedAttack,
    }
    
    [Serializable]
    public class PlayerRangeAttack : PlayerStateBehavior
    {
        [Header("Settings")]
        [SerializeField] private float m_AttackDuration;
        [SerializeField] private AnimationCurve m_AttackAnimationCurve;
        [SerializeField] private int m_BaseDamage;
        [SerializeField] private int m_ChargedOptimalDamage;
        [SerializeField] private int m_ChargedDamage;
        
        [SerializeField] private float m_ManaCost;
        [SerializeField] private float m_ChargedManaCost;

        [SerializeField] private float m_DelayBeforeBuild;
        [SerializeField] private float m_BuildUpDelay;
        [SerializeField] private float m_PostOptimalTriggerPhaseDelay;
        [SerializeField] private bool m_NormalAttackIfMissedChargedOne;
        
        
        public override float BaseManaPoints => m_ManaCost;

        [Header("Scene components")]
        [SerializeField] private RangeAttackHolder m_RangeAttackPrefab;
        [SerializeField] private RangeAttackHolder m_ChargedRangeAttackPrefab;
        [Tooltip("Just for debug purpose before getting the VFX")]
        [SerializeField] private bool m_DoubleChargedWeaponScale;
        
        
        [SerializeField] private ParticleSystem m_VFXShootVFX;
        [SerializeField] private ParticleSystem m_ChargedBuildUpVFX;
        [SerializeField] private Animator m_Animator;
        
        [Tooltip("Planed for world UI, use to enable the thing you need during aiming, will be disabled at the end of aiming")]
        [SerializeField] private GameObject m_AimingWorldUI;

        private Transform m_Transform;
        private Coroutine m_RangeAttackBuildUpCoroutine;
        private IObjectPool<RangeAttackHolder> m_RangeAttackPool;
        private IObjectPool<RangeAttackHolder> m_ChargedRangeAttackPool;
        private RangeAttackPhase m_CurrentAttackPhase;

        protected override void Init(IStateHandler _stateHandler)
        {
            m_Transform = _stateHandler.gameObject.transform;
            if (m_AimingWorldUI != null)
            {
                m_AimingWorldUI.SetActive(false);
            }
            m_RangeAttackPool = new ObjectPool<RangeAttackHolder>(CreateRangeItem, maxSize: 10);
            m_ChargedRangeAttackPool = new ObjectPool<RangeAttackHolder>(CreateChargedRangeItem, maxSize: 4);
        }

        private RangeAttackHolder CreateChargedRangeItem()
        {
            return m_StateHandler.Instantiate(m_ChargedRangeAttackPrefab);
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
            CleanRangeAttackCoroutine();
        }

        public override void OnInput(InputType inputType)
        {
            if (inputType == InputType.RangeAttackPerformed)
            {
                if (!m_StateHandler.PlayerManaPoints.CheckIfPossible(m_ChargedManaCost))
                {
                    m_StateHandler.PlayerManaPoints.TriggerOnCantConsumeMana();
                    return;
                }
                
                CleanRangeAttackCoroutine();
                m_RangeAttackBuildUpCoroutine = m_StateHandler.StartCoroutine(c_RangeBuildUp());
            } else if (inputType == InputType.RangeAttackCanceled)
            {
                if (m_RangeAttackBuildUpCoroutine != null)
                {
                    CleanRangeAttackCoroutine();
                    TriggerAttack();
                }
            }
        }

        private void CleanRangeAttackCoroutine()
        {
            if (m_RangeAttackBuildUpCoroutine != null)
            {
                m_StateHandler.StopCoroutine(m_RangeAttackBuildUpCoroutine);
                m_RangeAttackBuildUpCoroutine = null;
            }
            m_ChargedBuildUpVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private IEnumerator c_RangeBuildUp()
        {
            m_CurrentAttackPhase = RangeAttackPhase.SimpleAttack;
            yield return new WaitForSeconds(m_DelayBeforeBuild);

            if (m_ChargedBuildUpVFX != null)
            {
                m_ChargedBuildUpVFX.Play();
            }
            m_CurrentAttackPhase = RangeAttackPhase.ChargedBuildUp;
            yield return new WaitForSeconds(m_BuildUpDelay);

            m_CurrentAttackPhase = RangeAttackPhase.OptimalChargedAttack;
            yield return new WaitForSeconds(m_PostOptimalTriggerPhaseDelay);

            m_CurrentAttackPhase = RangeAttackPhase.NotOptimalChargedAttack;
        }

        private void TriggerAttack()
        {
            CleanRangeAttackCoroutine();
            switch (m_CurrentAttackPhase)
            {
                case RangeAttackPhase.SimpleAttack:
                    SpawnSimpleAttack();
                    break;
                case RangeAttackPhase.ChargedBuildUp:
                    if (m_NormalAttackIfMissedChargedOne)
                    {
                        SpawnSimpleAttack();
                    }
                    break;
                case RangeAttackPhase.OptimalChargedAttack:
                    SpawnChargedAttack(m_ChargedOptimalDamage);
                    break;
                case RangeAttackPhase.NotOptimalChargedAttack:
                    SpawnChargedAttack(m_ChargedDamage);
                    break;
            }
        }

        private void SpawnChargedAttack(int _damage)
        {
            var spawned = SpawnAttack(m_ChargedRangeAttackPool, _damage, m_ChargedManaCost);
            if (m_DoubleChargedWeaponScale)
            {
                spawned.transform.localScale = new Vector3(2, 2, 2);
            }
        }

        private void SpawnSimpleAttack()
        {
            SpawnAttack(m_RangeAttackPool, m_BaseDamage, m_ManaCost);
        }

        private RangeAttackHolder SpawnAttack(IObjectPool<RangeAttackHolder> _pool, int _damage, float _manaCost)
        {
            if (m_StateHandler.PlayerManaPoints.CheckIfPossiblePlusConsume(_manaCost))
            {
                Debug.LogWarning("Should have been checked before !");
            }
            
            var spawned = _pool.Get();
            spawned.HitBox.damage = _damage;
            spawned.transform.position = m_Transform.position;
            spawned.transform.rotation = m_Transform.rotation;
            spawned.WeaponMovement.TriggerWeaponMovement(m_AttackDuration, m_AttackAnimationCurve);
            spawned.WeaponMovement.CurrentTweener.onComplete += () =>
            {
                _pool.Release(spawned);
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
            return spawned;
        }
    }
}