﻿using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Camera m_MainCamera;

        private void Awake()
        {
            m_MainCamera = Camera.main;
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
            if (_context.phase == InputActionPhase.Canceled)
            {
                StackInput(InputType.RangeAttackCanceled);
            }
            //ChangeAiming(_context);

            // TODO: Handle aiming depending on mouse or gamepad right joystick
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
            GameplayData.s_PlayerInputs.InputStack.Add(_input);
            GameplayData.s_PlayerStateHandler.OnInputAdded(_input);
        }
    }
}