using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Target))]
public class KnightBehaviour : MonoBehaviour
{
    [Header("Public stuff")]
    public Transform target;

    [Header("Referencies")]
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private AnimationDriver m_animator;
    [SerializeField] private Target m_targetting;
    [SerializeField] private Collider m_attackCollider;

    [Header("Parameters")]
    [SerializeField] private Vector2 m_attackRange = Vector2.up;
    [SerializeField] private float m_attackCooldown = 2f;
    [SerializeField] private int m_targetQueryRate = 60;

    private Vector3 m_lastPosition;

    private enum State
    {
        Idle,
        GoToTarget,
        Queueing,
        CombatIdle,
        Retreat,
        Attack,
        Hit,
        Death
    }

    [SerializeField] private State m_state = State.Idle;
    private State m_nextState = State.Idle;
    private bool m_lookAtTarget = false;
    private bool m_alignWithMovement = false;
    private float m_currentAttackCooldown = 0f;
    private int m_queryTurn;
    private bool m_shouldQueryAdvanceBlocked = false;
    private bool m_advanceBlocked = false;
    private bool m_queryNow = false;

    private static readonly float COS_ATTACK = Mathf.Cos(15 * Mathf.Deg2Rad);
    private static readonly float COS_BLOCKED = Mathf.Cos(45 * Mathf.Deg2Rad);

    public void OnDeath()
    {
        m_nextState = State.Death;
    }

    public void OnHit()
    {
        m_nextState = State.Hit;
    }

    private void OnValidate()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<AnimationDriver>();
        m_targetting = GetComponent<Target>();
    }
    private void Reset()
    {
        OnValidate();
    }

    private void Start()
    {
        m_lastPosition = transform.position;
        m_queryTurn = UnityEngine.Random.Range(0, m_targetQueryRate);
        m_attackCollider.enabled = false;
    }

    private void Update()
    {
        // Update cooldowns
        m_currentAttackCooldown -= Time.deltaTime;

        // Update before just in case
        UpdateTransition();

        // Update targetting info so it's ready for the update
        QueryTargets();

        switch (m_state)
        {
            case State.Idle:
                Idle_Update();
                break;
            case State.GoToTarget:
                GoToTarget_Update();
                break;
            case State.Queueing:
                Queueing_Update();
                break;
            case State.CombatIdle:
                CombatIdle_Update();
                break;
            case State.Retreat:
                Retreat_Update();
                break;
            case State.Attack:
                Attack_Update();
                break;
            case State.Hit:
                Hit_Update();
                break;
            case State.Death:
                Death_Update();
                break;
        }

        // Update after to pick changes during the update
        UpdateTransition();

        UpdateMovement();
    }

    private void UpdateMovement()
    {
        // Update the animations
        var movement = transform.position - m_lastPosition;
        m_lastPosition = transform.position;
        m_animator.SetSpeed(movement.magnitude / Time.deltaTime);

        // Update the rotation
        if (m_lookAtTarget)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(target.position - transform.position),
                m_agent.angularSpeed * Time.deltaTime);
        }
        if (m_alignWithMovement)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(movement),
                m_agent.angularSpeed * Time.deltaTime);
        }
    }

    private void QueryTargets()
    {
        // Only update when forced or when it's our turn to do so
        if (!m_queryNow)
            return;
        if ((Time.frameCount % m_targetQueryRate) != m_queryTurn)
            return;
        Profiler.BeginSample("Query");

        // Check if there are enemies in front
        if (m_shouldQueryAdvanceBlocked)
        {
            m_advanceBlocked = false;
            if (target)
            {
                var toTarget = (target.position - transform.position).normalized;
                var nearbyAllies = Target.QueryTargets(transform.position, 1.5f, m_targetting.isMain, m_targetting.team);
                for (int i = 0; i < nearbyAllies.Count; ++i)
                {
                    if (nearbyAllies[i] == m_targetting) continue;
                    if (Vector3.Dot((nearbyAllies[i].transform.position - transform.position).normalized, toTarget) > COS_BLOCKED)
                    {
                        m_advanceBlocked = true;
                        break;
                    }
                }
            }
        }

        Profiler.EndSample();
    }

    private void UpdateTransition()
    {
        if (m_state == m_nextState)
            return;

        switch (m_state)
        {
            case State.GoToTarget:
                GoToTarget_Exit();
                break;
            case State.Queueing:
                Queueing_Exit();
                break;
            case State.CombatIdle:
                CombatIdle_Exit();
                break;
            case State.Retreat:
                Retreat_Exit();
                break;
            case State.Attack:
                Attack_Exit();
                break;
        }

        switch (m_nextState)
        {
            case State.GoToTarget:
                GoToTarget_Enter();
                break;
            case State.Queueing:
                Queueing_Enter();
                break;
            case State.CombatIdle:
                CombatIdle_Enter();
                break;
            case State.Retreat:
                Retreat_Enter();
                break;
            case State.Attack:
                Attack_Enter();
                break;
            case State.Hit:
                Hit_Enter();
                break;
            case State.Death:
                Death_Enter();
                break;
        }

        m_state = m_nextState;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRange.x);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_attackRange.y);
    }

    #region States
    #region Idle
    private void Idle_Update()
    {
        if (target)
            m_nextState = State.GoToTarget;
    }
    #endregion

    #region GoToTarget
    private void GoToTarget_Enter()
    {
        // Set the destination to start moving
        m_agent.SetDestination(target.position);
        m_agent.updateRotation = true;
        m_shouldQueryAdvanceBlocked = true;
        // Query immediately so we don't even start moving
        m_queryNow = true;
    }

    private void GoToTarget_Update()
    {
        if (!target)
        {
            m_nextState = State.Idle;
            return;
        }

        if (m_advanceBlocked)
        {
            m_nextState = State.Queueing;
            return;
        }

        // Go to combat if we are close enough
        if ((target.position - transform.position).sqrMagnitude < (m_attackRange.y * m_attackRange.y))
        {
            m_nextState = State.CombatIdle;
            return;
        }

        // Update the navigation path if the current one is too outdated
        if ((m_agent.destination - target.position).sqrMagnitude > (m_agent.radius * m_agent.radius))
        {
            m_agent.SetDestination(target.position);
        }
    }

    private void GoToTarget_Exit()
    {
        m_agent.ResetPath();
        m_agent.updateRotation = false;
        m_shouldQueryAdvanceBlocked = true;
    }
    #endregion

    #region Queueing
    private void Queueing_Enter()
    {
        m_shouldQueryAdvanceBlocked = true;
        m_lookAtTarget = true;
    }

    private void Queueing_Update()
    {
        if (!m_advanceBlocked)
            m_nextState = State.GoToTarget;
    }

    private void Queueing_Exit()
    {
        m_shouldQueryAdvanceBlocked = false;
        m_lookAtTarget = false;
    }
    #endregion

    #region CombatIdle
    private void CombatIdle_Enter()
    {
        m_lookAtTarget = true;
    }

    private void CombatIdle_Update()
    {
        if (!target)
        {
            m_nextState = State.Idle;
            return;
        }

        var toTarget = target.position - transform.position;
        var targetDistSq = toTarget.sqrMagnitude;
        if (targetDistSq > (m_attackRange.y * m_attackRange.y))
        {
            m_nextState = State.GoToTarget;
            return;
        }

        if (targetDistSq < (m_attackRange.x * m_attackRange.x))
        {
            m_nextState = State.Retreat;
            return;
        }

        if (m_currentAttackCooldown < 0f)
        {
            var dot = Vector3.Dot(toTarget.normalized, transform.forward);
            if (dot > COS_ATTACK)
            {
                m_currentAttackCooldown = m_attackCooldown;
                m_nextState = State.Attack;
            }
        }
    }

    private void CombatIdle_Exit()
    {
        m_lookAtTarget = false;
    }
    #endregion

    #region Retreat
    private void Retreat_Enter()
    {
        m_alignWithMovement = true;
    }

    private void Retreat_Update()
    {
        if (!target)
        {
            m_nextState = State.Idle;
            return;
        }

        var fromTarget = (transform.position - target.position);
        var distance = fromTarget.magnitude;
        m_agent.velocity = (transform.position - target.position) / distance * m_agent.speed;

        if (distance > m_attackRange.x)
            m_nextState = State.CombatIdle;
    }

    private void Retreat_Exit()
    {
        m_alignWithMovement = false;
    }
    #endregion

    #region Attack
    private void Attack_Enter()
    {
        m_animator.TriggerAttack();
    }

    private void Attack_Update()
    {
        if (m_attackCollider)
            m_attackCollider.enabled = m_animator.IsDamagingFrame();

        if (m_animator.HasAnimationFinished())
            m_nextState = State.CombatIdle;
    }

    private void Attack_Exit()
    {
        if (m_attackCollider)
            m_attackCollider.enabled = false;
    }
    #endregion

    #region Hit
    private void Hit_Enter()
    {
        m_animator.TriggerHit();
    }

    private void Hit_Update()
    {
        if (m_animator.HasAnimationFinished())
            m_nextState = State.CombatIdle;
    }
    #endregion

    #region Death
    private void Death_Enter()
    {
        m_animator.TriggerDeath();
    }

    private void Death_Update()
    {
        if (m_animator.HasAnimationFinished())
            Destroy(gameObject);
    }
    #endregion
    #endregion
}
