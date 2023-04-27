using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class KnightBehaviour : MonoBehaviour
{
    [Header("Public stuff")]
    public Transform target;

    [Header("Referencies")]
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Animator m_animator;

    [Header("Parameters")]
    [SerializeField] private Vector2 m_attackRange = Vector2.up;
    [SerializeField] private float m_attackCooldown = 2f;

    private static readonly int SPEED = Animator.StringToHash("Speed");
    private static readonly int ATTACK = Animator.StringToHash("Attack");

    private Vector3 m_lastPosition;

    private enum State
    {
        Idle,
        GoToTarget,
        CombatIdle,
        Retreat,
        Attack,
    }

    private State m_state = State.Idle;
    private State m_nextState = State.Idle;
    private bool m_lookAtTarget = false;
    private bool m_alignWithMovement = false;
    private bool m_animationFinished = false;
    private float m_currentAttackCooldown = 0f;

    private void OnValidate()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
    }
    private void Reset()
    {
        OnValidate();
    }

    private void Start()
    {
        m_lastPosition = transform.position;
    }

    private void Update()
    {
        // Update cooldowns
        m_currentAttackCooldown -= Time.deltaTime;

        // Update before just in case
        UpdateTransition();

        switch (m_state)
        {
            case State.Idle:
                Idle_Update();
                break;
            case State.GoToTarget:
                GoToTarget_Update();
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
        }

        // Update after to pick changes during the update
        UpdateTransition();

        // Update the animations
        var movement = transform.position - m_lastPosition;
        m_lastPosition = transform.position;
        m_animator.SetFloat(SPEED, movement.magnitude / Time.deltaTime);

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

    private void UpdateTransition()
    {
        if (m_state == m_nextState)
            return;

        switch (m_state)
        {
            case State.GoToTarget:
                GoToTarget_Exit();
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
            case State.CombatIdle:
                CombatIdle_Enter();
                break;
            case State.Retreat:
                Retreat_Enter();
                break;
            case State.Attack:
                Attack_Enter();
                break;
        }

        m_state = m_nextState;
    }

    private void OnAnimationFinished()
    {
        m_animationFinished = true;
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
    }

    private void GoToTarget_Update()
    {
        if (!target)
        {
            m_nextState = State.Idle;
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

        var targetDistSq = (target.position - transform.position).sqrMagnitude;
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
            m_currentAttackCooldown = m_attackCooldown;
            m_nextState = State.Attack;
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
        m_animator.SetTrigger(ATTACK);
        m_animationFinished = false;
    }

    private void Attack_Update()
    {
        if (m_animationFinished)
            m_nextState = State.CombatIdle;
    }

    private void Attack_Exit()
    {
        m_animationFinished = false;
    }
    #endregion
    #endregion
}