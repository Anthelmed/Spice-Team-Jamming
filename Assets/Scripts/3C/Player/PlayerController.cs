using System;
using System.Linq.Expressions;
using DefaultNamespace;
using NaughtyAttributes.Test;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace _3C.Player
{
    public struct InputState
    {
        public bool pause;
        public bool openMap;
        public bool closeMap;
        public bool mapBack;
        public bool confirm;
        public bool select;
        public bool menuBack;
    }
    public class PlayerController : MonoBehaviour
    {
        private Camera m_MainCamera;
        private PlayerCursor m_PlayerCursor;


        MainInput mainInput;
        [SerializeField] GameManager gameManager;

        public InputState inputState = new InputState();
        private bool m_OnDeviceChangeHooked;

        private void Awake()
        {

            gameManager.OnGameStateChanged += SwitchInputMap;
            m_MainCamera = Camera.main;
            mainInput = new MainInput();
            m_PlayerCursor = GetComponent<PlayerCursor>();

            //init default map
            mainInput.Map.Enable();
            
            mainInput.Gameplay.Movement.performed += OnMovement;
            mainInput.Gameplay.Movement.canceled += OnMovement;
            mainInput.Gameplay.Look.performed += OnLook;
            mainInput.Gameplay.Look.canceled += OnLook;
            mainInput.Gameplay.Dash.performed += OnDash;
            mainInput.Gameplay.RangeAttack.performed += OnRangeAttack;
            mainInput.Gameplay.RangeAttack.canceled += OnRangeAttack;
            mainInput.Gameplay.KeyboardAim.performed += OnKeyboardAim;
            mainInput.Gameplay.KeyboardAim.canceled += OnKeyboardAim;
            mainInput.Gameplay.MeleeAttack.performed += OnMeleeAttack;
            mainInput.Gameplay.MeleeAttack.canceled += OnMeleeAttack;
            mainInput.Gameplay.Pause.started += OnPause;

            mainInput.Map.Back.started += MapBackPressed;
            mainInput.Map.Confirm.started += ConfirmPressed;
            mainInput.Map.Select.started += SelectPressed;

            mainInput.Menu.Back.started += MenuBackPressed;
            InputSystem.onEvent += CheckIfSchemeChanged;

        }

        private void CheckIfSchemeChanged(InputEventPtr _, InputDevice _device)
        {
            if (mainInput.GamepadScheme.SupportsDevice(_device))
            {
                m_PlayerCursor.UseMouse = false;
            } else if (mainInput.KeyboardMouseScheme.SupportsDevice(_device))
            {
                m_PlayerCursor.UseMouse = true;
            }
        }

        private void OnMovement(InputAction.CallbackContext _context)
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

        public void SwitchInputMap(GameState state)
        {
            switch (state)
            {
                case GameState.map:
                    {
                        mainInput.Gameplay.Disable();
                        mainInput.Map.Enable();
                        mainInput.Menu.Disable();
                        print("enabled the map map");
                    }
                    break;
                case GameState.level:
                    {
                        mainInput.Gameplay.Enable();
                        mainInput.Map.Disable();
                        mainInput.Menu.Disable();
                        print("enabled the level map");
                    }
                    break;
                case GameState.pause:
                    {
                        mainInput.Gameplay.Disable();
                        mainInput.Map.Disable();
                        mainInput.Menu.Enable();
                        print("enabled the pause  map");
                    }
                    break;
            }
        }

        public void OnGamepadCursorMovement(InputAction.CallbackContext _context)
        {
            m_PlayerCursor.Movement = _context.ReadValue<Vector2>();
        }

        public void TriggerUIClick(InputAction.CallbackContext _context) // this comes from the event system no? or you need it for gamepad?
        {
            if (_context.performed)
            {
               
            }
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
           inputState.pause = true;
        }

        private void MenuBackPressed(InputAction.CallbackContext obj)
        {
            inputState.mapBack = true;
        }

        private void SelectPressed(InputAction.CallbackContext obj)
        {
            inputState.select = true;
        }

        private void ConfirmPressed(InputAction.CallbackContext obj)
        {
           inputState.confirm = true;
            GameplayData.UIPressThisFrame = true;
        }

        private void MapBackPressed(InputAction.CallbackContext obj)
        {
           inputState.openMap = true;
        }

        private void LateUpdate()
        {
            inputState.select = false;
            inputState.pause = false;
            inputState.closeMap = false;
            inputState.mapBack = false;
            inputState.menuBack = false;
            inputState.openMap = false;
            inputState.confirm = false;
           // GameplayData.UIPressThisFrame = true;
        }
    }
}

