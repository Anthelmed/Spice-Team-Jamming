using System;
using UnityEngine;

namespace _3C.Player
{

    public enum PlayerState
    {
        Moving,
        Dashing,
    }
    
    public class PlayerStateHandler : MonoBehaviour
    {
        [SerializeField] private PlayerMovement m_PlayerMovement;
        [SerializeField] private DashBehavior m_DashBehavior;

        private PlayerState m_CurrentState;

        public PlayerState CurrentState
        {
            get => m_CurrentState;
            set
            {
                OnStateChange(m_CurrentState, value);
                m_CurrentState = value;
            }
        }

        private void Awake()
        {
            m_PlayerMovement.enabled = true;
            m_DashBehavior.enabled = false;
        }

        private void OnEnable()
        {
            m_DashBehavior.OnDashEnded += OnDashEnded;
        }

        private void OnDisable()
        {
            m_DashBehavior.OnDashEnded -= OnDashEnded;
        }

        private void OnDashEnded()
        {
            CurrentState = PlayerState.Moving;
        }

        private void OnStateChange(PlayerState _currentState, PlayerState _nextState)
        {
            switch (_currentState)
            {
                case PlayerState.Moving:
                    m_PlayerMovement.enabled = false;
                    break;
                case PlayerState.Dashing:
                    m_DashBehavior.enabled = false;
                    break;
            }

            switch (_nextState)
            {
                case PlayerState.Moving:
                    m_PlayerMovement.enabled = true;
                    break;
                case PlayerState.Dashing:
                    m_DashBehavior.enabled = true;
                    break;
            }
        }
    }
}