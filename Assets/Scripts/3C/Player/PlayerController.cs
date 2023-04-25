using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement m_PlayerMovement;
        [SerializeField] private PlayerStateHandler m_PlayerStateHandler;

        private void Awake()
        {
            Assert.IsNotNull(m_PlayerMovement, "Player Character should not be null");
            Assert.IsNotNull(m_PlayerStateHandler, "Player State Handler should not be null");
        }

        public void OnMovementAsked(InputAction.CallbackContext _context)
        {
            m_PlayerMovement.Movement = _context.ReadValue<Vector2>();
        }

        public void OnDash(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Performed)
            {
                m_PlayerStateHandler.CurrentState = PlayerState.Dashing;
            }
        }

        public void OnMeleeAttack(InputAction.CallbackContext _context)
        {
            m_PlayerStateHandler.CurrentState = PlayerState.Attacking;
        }

        public void OnRangeAttack(InputAction.CallbackContext _context)
        {
            
        }
    }
}