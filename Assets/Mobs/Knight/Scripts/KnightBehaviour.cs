using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class KnightBehaviour : MonoBehaviour
{
    public Transform target;

    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Animator m_animator;

    private static readonly int SPEED = Animator.StringToHash("Speed");

    private Vector3 m_lastPosition;

    private enum State
    {
        Idle,
        GoToTarget
    }

    private State m_state = State.Idle;
    private State m_nextState = State.Idle;

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
        }

        // Update after to pick changes during the update
        UpdateTransition();

        var movement = transform.position - m_lastPosition;
        m_lastPosition = transform.position;
        m_animator.SetFloat(SPEED, movement.magnitude / Time.deltaTime);
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
        }

        switch (m_nextState)
        {
            case State.GoToTarget:
                GoToTarget_Enter();
                break;
        }

        m_state = m_nextState;
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
    }

    private void GoToTarget_Update()
    {
        if (!target)
        {
            m_nextState = State.Idle;
            return;
        }

        if ((m_agent.destination - target.position).sqrMagnitude > (m_agent.radius * m_agent.radius))
        {
            m_agent.SetDestination(target.position);
        }
    }

    private void GoToTarget_Exit()
    {
        m_agent.ResetPath();
    }
    #endregion
    #endregion
}
