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
            switch (_context.phase)
            {
                case InputActionPhase.Performed:
                    StackInputIfNotTop(InputType.MovementPerformed);
                    break;
                case InputActionPhase.Canceled:
                    StackInputIfNotTop(InputType.MovementCanceled);
                    break;
            }

            GameplayData.s_PlayerInputs.Movement = _context.ReadValue<Vector2>();
        }

        public void OnDash(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Performed)
            {
                StackInput(InputType.DashPerformed);
            }
        }

        public void OnMeleeAttack(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Performed)
            {
                StackInput(InputType.AttackPerformed);
            }
        }

        public void OnRangeAttack(InputAction.CallbackContext _context)
        {
            
        }

        private void StackInputIfNotTop(InputType _input)
        {
            if (!GameplayData.s_PlayerInputs.InputStack.IsEmpty &&GameplayData.s_PlayerInputs.InputStack.Top == _input)
            {
                return;
            }
            
            StackInput(_input);
        }

        private void StackInput(InputType _input)
        {
            GameplayData.s_PlayerInputs.InputStack.Add(_input);
            GameplayData.s_PlayerStateHandler.OnInputAdded(_input);
        }
    }
}