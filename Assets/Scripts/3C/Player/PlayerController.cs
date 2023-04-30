using System.Linq.Expressions;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Camera m_MainCamera;
        private PlayerInput m_PlayerInput;
        private PlayerCursor m_PlayerCursor;


        private void Awake()
        {
            m_MainCamera = Camera.main;
            m_PlayerInput = GetComponent<PlayerInput>();
            m_PlayerInput.onControlsChanged += OnControlsChanged;
            m_PlayerCursor = GetComponent<PlayerCursor>();
            OnControlsChanged(m_PlayerInput);
        }

        private void OnControlsChanged(PlayerInput obj)
        {
            switch (obj.currentControlScheme) 
            {
                case "Gamepad":
                    UseGamepadAsCursor();
                    break;
                default:
                    ResetCursorToMouse();
                    break;
            }
        }

        private void UseGamepadAsCursor()
        {
            m_PlayerCursor.UseMouse = false;
        }

        private void ResetCursorToMouse()
        {
            m_PlayerCursor.UseMouse = true;
        }

        public void OnGamepadCursorMovement(InputAction.CallbackContext _context)
        {
            m_PlayerCursor.Movement = _context.ReadValue<Vector2>();
        }

        public void TriggerUIClick(InputAction.CallbackContext _context)
        {
            if (_context.performed)
            {
                GameplayData.UIPressThisFrame = true;
            }
        }

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
        
        public void OnLook(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Performed)
            {
                StackInputIfNotTop(InputType.AimPerformed);
                GameplayData.s_PlayerInputs.IsUsingCursorPositionForAim = false;
            } else if (_context.phase == InputActionPhase.Canceled)
            {
                StackInputIfNotTop(InputType.AimCanceled);
                GameplayData.s_PlayerInputs.IsUsingCursorPositionForAim = true;
            }

            GameplayData.s_PlayerInputs.AimDirection = _context.ReadValue<Vector2>();
        }

        // TO-DO: Held Melee Attack using InputAction HoldInteraction,
        // will normal melee attack need Tap interaction?
        public void OnMeleeAttack(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Performed)
            {
                StackInput(InputType.MeleeAttackPerformed);
            }
        }

        public void OnRangeAttack(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Performed)
            {
                StackInputIfNotTop(InputType.RangeAttackPerformed);
            } else if (_context.phase == InputActionPhase.Canceled)
            {
                StackInput(InputType.RangeAttackCanceled);
            }
        }

        public void OnKeyboardAim(InputAction.CallbackContext _context)
        {
            switch (_context.phase)
            {
                case InputActionPhase.Performed:
                    StackInputIfNotTop(InputType.AimPerformed);
                    break;
                case InputActionPhase.Canceled:
                    StackInputIfNotTop(InputType.AimCanceled);
                    break;
            }
        }
        
        private void ChangeAiming(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Canceled)
            {
                GameplayData.s_PlayerInputs.AimDirection = Vector2.zero;
            }

            var gamepadAimInput = _context.ReadValue<Vector2>();
            if (gamepadAimInput != Vector2.zero)
            {
                GameplayData.s_PlayerInputs.AimDirection = gamepadAimInput;
            }

            Plane plane = new Plane(Vector3.up, Vector3.zero);
            var ray = m_MainCamera.ScreenPointToRay(Mouse.current.position.value);
            if (plane.Raycast(ray, out float value))
            {
                var direction = ray.GetPoint(value) - GameplayData.s_PlayerStateHandler.transform.position;
                GameplayData.s_PlayerInputs.AimDirection = direction.normalized;
            }
        }

        private void StackInputIfNotTop(InputType _input)
        {
            if (!GameplayData.s_PlayerInputs.InputStack.IsEmpty && GameplayData.s_PlayerInputs.InputStack.Top == _input)
            {
                return;
            }

            StackInput(_input);
        }

        private void StackInput(InputType _input)
        {
            if (GameplayData.s_PlayerInputs == null)
            {
                return;
            }
            
            GameplayData.s_PlayerInputs.InputStack.Add(_input);
            GameplayData.s_PlayerStateHandler.OnInputAdded(_input);
        }

        public void OnPause(InputAction.CallbackContext _contex)
        {
            if (_contex.phase == InputActionPhase.Performed)
            {
                
            }
        }
    }
}