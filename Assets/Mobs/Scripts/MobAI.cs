using DefaultNamespace.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Targetable))]
[SelectionBase]
public class MobAI : MonoBehaviour
{
    [System.Serializable]
    public class Data
    {
        [Header("Referencies")]
        public NavMeshAgent agent;
        public AnimationDriver animator;
        public Targetable targetting;
        public Collider attackCollider;
        public PlayerSounds sounds;

        [Header("Parameters")]
        public Vector2 attackRange = Vector2.up;
        public float attackCooldown = 2f;
        public int targetQueryRate = 60;
        public float smallTargetDistance = 10f;

        private Targetable m_target;
        public Targetable Target { get => m_target; 
            set
            {
                m_target = value;
                if (m_target)
                    TargetTransform = m_target.transform;
                else
                    TargetTransform = null;
            } 
        }
        public Transform TargetTransform { get; private set; }
        public State NextState { get; set; } = State.Idle;
        public float TargetDistance { get; set; } = 0f;
        public float ToTargetCos { get; set; } = 0f;
        public bool LookAtTarget { get; set; } = false;
        public bool AlignWithMovement { get; set; } = false;
        public float CurrentAttackCooldown { get; set; } = 0f;
        
        public bool ShouldQueryAdvanceBlocked { get; set; } = false;
        public bool ShouldQueryTargets { get; set; } = false;
        public bool AdvanceBlocked { get; set; } = false;
        public bool QueryNow { get; set; } = false;
        public bool Destroy { get; set; } = false;

    }

    public static readonly float COS_ATTACK = Mathf.Cos(15 * Mathf.Deg2Rad);
    public static readonly float COS_BLOCKED = Mathf.Cos(45 * Mathf.Deg2Rad);

    [SerializeField] private Data m_data = new Data();

    public interface IMobState
    {
        void Enter(Data data);
        void Tick(Data data);
        void Exit(Data data);
    }

    public enum State
    {
        Uninitialized = 0,
        Idle,
        GoToTarget,
        Queueing,
        CombatIdle,
        Attack,
        Hit,
        Death,
    }

    private static IMobState[] m_states = new IMobState[]
    {
        null,
        new MobIdleState(),
        new MobGoToTargetState(),
        new MobQueueingState(),
        new MobCombatIdleState(),
        new MobAttackState(),
        new MobHitState(),
        new MobDeathState(),
    };

    private State m_state = State.Uninitialized;
    private int m_queryTurn;
    private Vector3 m_lastPosition;

    public void OnDeath()
    {
        m_data.NextState = State.Death;
    }

    public void OnHit()
    {
        m_data.NextState = State.Hit;
    }

    private void OnValidate()
    {
        m_data.agent = GetComponent<NavMeshAgent>();
        m_data.animator = GetComponentInChildren<AnimationDriver>();
        m_data.targetting = GetComponent<Targetable>();
    }

    private void Reset()
    {
        OnValidate();
    }

    private void Start()
    {
        m_lastPosition = transform.position;
        m_queryTurn = UnityEngine.Random.Range(0, m_data.targetQueryRate);
        m_data.attackCollider.enabled = false;
    }

    private void Update()
    {
        // Discard invalid targets
        if (m_data.Target && !m_data.Target.enabled)
            m_data.Target = null;

        // Update target info
        if (m_data.TargetTransform)
        {
            var toTarget = m_data.TargetTransform.position - transform.position;
            m_data.TargetDistance = toTarget.magnitude;
            m_data.ToTargetCos = Vector3.Dot(toTarget / Mathf.Max(0.001f, m_data.TargetDistance), transform.forward);
        }
        else
        {
            m_data.TargetDistance = 0f;
            m_data.ToTargetCos = 0f;
        }

        // Update cooldowns
        m_data.CurrentAttackCooldown -= Time.deltaTime;

        // Update before just in case
        UpdateTransition();

        // Update targetting info so it's ready for the update
        QueryTargets();

        // Run the update
        m_states[(int)m_state]?.Tick(m_data);

        // Update after to pick changes during the update
        UpdateTransition();

        UpdateMovement();

        if (m_data.Destroy)
            Destroy(gameObject);
    }

    private void UpdateMovement()
    {
        // Update the animations
        var movement = transform.position - m_lastPosition;
        m_lastPosition = transform.position;
        m_data.animator.SetSpeed(movement.magnitude / Time.deltaTime);

        // Update the rotation
        if (m_data.LookAtTarget && m_data.Target)
        {
            if (m_data.ToTargetCos < 0.99f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(m_data.TargetTransform.position - transform.position),
                    m_data.agent.angularSpeed * Time.deltaTime);
            }
        }
        if (m_data.AlignWithMovement)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(movement),
                m_data.agent.angularSpeed * Time.deltaTime);
        }
    }

    private void QueryTargets()
    {
        // Only update when forced or when it's our turn to do so
        if (!m_data.QueryNow && (Time.frameCount % m_data.targetQueryRate) != m_queryTurn)
            return;

        m_data.QueryNow = false;

        Profiler.BeginSample("Query");

        if (m_data.ShouldQueryTargets)
        {
            m_data.Target = Targetable.QueryClosestTarget(transform.position, m_data.smallTargetDistance, out _, ~m_data.targetting.team);
        }

        // Check if there are enemies in front
        if (m_data.ShouldQueryAdvanceBlocked)
        {
            m_data.AdvanceBlocked = false;
            if (m_data.Target)
            {
                var toTarget = (m_data.TargetTransform.position - transform.position).normalized;
                var nearbyAllies = Targetable.QueryTargets(transform.position, 1.5f, m_data.targetting.team, minPriority: m_data.targetting.priority);
                for (int i = 0; i < nearbyAllies.Count; ++i)
                {
                    if (nearbyAllies[i] == m_data.targetting) continue;
                    if (Vector3.Dot((nearbyAllies[i].transform.position - transform.position).normalized, toTarget) > COS_BLOCKED)
                    {
                        m_data.AdvanceBlocked = true;
                        break;
                    }
                }
            }
        }

        Profiler.EndSample();
    }

    private void UpdateTransition()
    {
        if (m_state == m_data.NextState)
            return;

        m_states[(int)m_state]?.Exit(m_data);
        m_state = m_data.NextState;
        m_states[(int)m_state]?.Enter(m_data);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_data.attackRange.x);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_data.attackRange.y);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, m_data.smallTargetDistance);
    }
}
