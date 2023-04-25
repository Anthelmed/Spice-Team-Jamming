using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement m_PlayerMovement;

        private void Awake()
        {
            Assert.IsNotNull(m_PlayerMovement, "Player Character should not be null");
        }

        public void OnMovementAsked(InputAction.CallbackContext _context)
        {
            m_PlayerMovement.Movement = _context.ReadValue<Vector2>();
        }

        public void OnMeleeAttack(InputAction.CallbackContext _context)
        {
            
        }

        public void OnRangeAttack(InputAction.CallbackContext _context)
        {
            
        }
    }
}