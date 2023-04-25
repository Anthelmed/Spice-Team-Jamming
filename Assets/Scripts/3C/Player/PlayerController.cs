using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter m_PlayerCharacter;

        private void Awake()
        {
            Assert.IsNotNull(m_PlayerCharacter, "Player Character should not be null");
        }

        public void OnMovementAsked(InputAction.CallbackContext _context)
        {
            m_PlayerCharacter.Movement = _context.ReadValue<Vector2>();
        }

        public void OnMeleeAttack(InputAction.CallbackContext _context)
        {
            
        }

        public void OnRangeAttack(InputAction.CallbackContext _context)
        {
            
        }
    }
}