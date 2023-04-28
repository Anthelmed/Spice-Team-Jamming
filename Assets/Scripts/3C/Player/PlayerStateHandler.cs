using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Audio;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace _3C.Player
{
    [Serializable]
    public enum PlayerState
    {
        IdleMovement,
        Dashing,
        Attacking,
    }
    
    public class PlayerStateHandler : MonoBehaviour, IStateHandler
    {
        [Header("Handler Settings")]
        [SerializeField] private int m_InputStackSize;
        
        [Header("State Settings")]
        [SerializeField] private PlayerMovement m_PlayerMovement;
        [SerializeField] private DashBehavior m_DashBehavior;
        [SerializeField] private PlayerMeleeAttack m_PlayerMeleeAttack;

        [Header("Componens")]
        [SerializeField] private Animator m_Animator;
        [SerializeField] private PlayerSounds m_Sounds;

        public PlayerSounds PlayerSoundsInstance => m_Sounds;
        public void OnMovementStateChanged(bool _state)
        {
            if (_state)
            {
                m_PlayerMovement.StartState();
            }
            else
            {
                m_PlayerMovement.StopState();
            }
        }

        private PlayerInputs m_PlayerInputs = new();
        
        private IEnumerable<PlayerStateBehavior> StatesBehaviors
        {
            get
            {
                yield return m_PlayerMovement;
                yield return m_DashBehavior;
                yield return m_PlayerMeleeAttack;
            }
        }

        public PlayerMeleeAttack MeleeAttack => m_PlayerMeleeAttack; 
        
        [SerializeField][ReadOnly]
        private PlayerState m_CurrentState;
        private PlayerStateBehavior m_CurrentStateBehavior;

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
            if (m_Animator)
            {
                Debug.LogWarning("The state machine is thought for animated based character so watch out");
            }
            
            GameplayData.s_PlayerStateHandler = this;
            GameplayData.s_PlayerInputs = m_PlayerInputs = new PlayerInputs();
            m_PlayerInputs.InputStack = new(m_InputStackSize);
            foreach (var playerStateBehavior in StatesBehaviors)
            {
                playerStateBehavior.Awake(this);
            }

            CurrentState = PlayerState.IdleMovement;
        }

        private void OnStateChange(PlayerState _currentState, PlayerState _nextState)
        {
            m_CurrentStateBehavior?.StopState();
            m_CurrentStateBehavior = GetBehaviorFromState(_nextState); 
            m_CurrentStateBehavior.StartState();
        }

        private PlayerStateBehavior GetBehaviorFromState(PlayerState _state) => _state switch
        {
            PlayerState.IdleMovement => m_PlayerMovement,
            PlayerState.Dashing => m_DashBehavior,
            PlayerState.Attacking => m_PlayerMeleeAttack,
            _ => null,
        };

        public void OnStateEnded()
        {
            CurrentState = PlayerState.IdleMovement;
        }

        public void StartCoroutine(IEnumerator _coroutine)
        {
            base.StartCoroutine(_coroutine);
        }

        private void Update()
        {
            if (m_CurrentStateBehavior != m_PlayerMovement)
            {
                m_PlayerMovement?.Update();
            }
            m_CurrentStateBehavior?.Update();
        }

        private void OnValidate()
        {
            foreach (var playerStateBehavior in StatesBehaviors)
            {
                playerStateBehavior.OnValidate();
            }
        }

        private void OnDrawGizmos()
        {
            m_PlayerMovement?.OnDrawGizmos();
            m_CurrentStateBehavior?.OnDrawGizmos();
        }

        public void OnInputAdded(InputType _input)
        {
            if (!ShouldCurrentStateBeAborted(_input))
            {
                m_CurrentStateBehavior.OnInput(_input);
                return;
            }
            
            CurrentState = (m_CurrentState, _input) switch
            {
                (PlayerState.IdleMovement, InputType.AttackPerformed) => PlayerState.Attacking,
                (PlayerState.IdleMovement, InputType.DashPerformed) => PlayerState.Dashing,
                (PlayerState.Attacking, InputType.DashPerformed) => PlayerState.Dashing,
                _ => throw new Exception($" {m_CurrentState} - {_input} is not handled"),
            };
        }

        private bool ShouldCurrentStateBeAborted(InputType _input)
        {
            return (m_CurrentState, _input) switch
            {
                (PlayerState.Attacking, InputType.MovementCanceled) or (PlayerState.Attacking, InputType.MovementPerformed) => false,
                (PlayerState.Dashing, _) => false,
                (PlayerState.IdleMovement, InputType.MovementPerformed) or (PlayerState.IdleMovement, InputType.MovementCanceled) => false,
                (PlayerState.Attacking, InputType.AttackPerformed) => false,
                _ => true,
            };
        }
    }
}