using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class KnightBehaviour : MonoBehaviour
{
    public Transform target;

    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Animator m_animator;

    private readonly int SPEED = Animator.StringToHash("Speed");

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
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }
    private void Reset()
    {
        OnValidate();
    }

    private void Update()
    {
        m_animator.SetFloat(SPEED, m_rigidbody.velocity.magnitude);
    }

    #region States
    #endregion
}
