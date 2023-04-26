using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    public class PlayerController : MonoBehaviour
    {
        public void OnMovementAsked(InputAction.CallbackContext _context)
        {
            GameplayData.s_PlayerInputs.Movement = _context.ReadValue<Vector2>();
            Debug.Log("On Movement Asked");
        }

        public void OnDash(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Performed)
            {
                GameplayData.s_PlayerStateHandler.CurrentState = PlayerState.Dashing;
            }
        }

        public void OnMeleeAttack(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Performed)
            {
                GameplayData.s_PlayerStateHandler.CurrentState = PlayerState.Attacking;
            }
        }

        public void OnRangeAttack(InputAction.CallbackContext _context)
        {
            
        }
    }
}