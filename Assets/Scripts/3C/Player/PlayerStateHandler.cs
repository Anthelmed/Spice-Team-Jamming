using System;
using UnityEngine;

namespace _3C.Player
{

    public enum PlayerState
    {
        Moving,
        Dashing,
        Attacking,
    }
    
    public class PlayerStateHandler : MonoBehaviour
    {
        [SerializeField] private PlayerMovement m_PlayerMovement;
        [SerializeField] private DashBehavior m_DashBehavior;
        [SerializeField] private PlayerMeleeAttack m_PlayerMeleeAttack;

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
            m_PlayerMeleeAttack.enabled = false;
        }

        private void OnEnable()
        {
            m_DashBehavior.OnDashEnded += ResetToMovement;
            m_PlayerMeleeAttack.OnAttackSerieEnded += ResetToMovement;
        }

        private void OnDisable()
        {
            m_DashBehavior.OnDashEnded -= ResetToMovement;
            m_PlayerMeleeAttack.OnAttackSerieEnded -= ResetToMovement;

        }

        private void ResetToMovement()
        {
            CurrentState = PlayerState.Moving;
        }

        private void OnStateChange(PlayerState _currentState, PlayerState _nextState)
        {
            GetBehaviorFromState(_currentState).StopState();
            GetBehaviorFromState(_nextState).StartState();
        }

        private PlayerStateBehavior GetBehaviorFromState(PlayerState _state) => _state switch
        {
            PlayerState.Moving => m_PlayerMovement,
            PlayerState.Dashing => m_DashBehavior,
            PlayerState.Attacking => m_PlayerMeleeAttack,
        };
    }
}