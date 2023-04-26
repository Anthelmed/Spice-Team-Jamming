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

    private static readonly int SPEED = Animator.StringToHash("Speed");

    private Vector3 m_lastPosition;

    private enum State
    {
        Idle,
        GoToTarget,
        CombatIdle
    }

    private State m_state = State.Idle;
    private State m_nextState = State.Idle;
    private bool m_lookAtTarget = false;

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
        }

        switch (m_nextState)
        {
            case State.GoToTarget:
                GoToTarget_Enter();
                break;
            case State.CombatIdle:
                CombatIdle_Enter();
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

        if ((target.position - transform.position).sqrMagnitude > (m_attackRange.y * m_attackRange.y))
        {
            m_nextState = State.GoToTarget;
            return;
        }
    }

    private void CombatIdle_Exit()
    {
        m_lookAtTarget = false;
    }
    #endregion
    #endregion
}
