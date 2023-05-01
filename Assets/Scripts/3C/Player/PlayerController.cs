using System;
using System.Linq.Expressions;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
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

            //init default map
            mainInput.Map.Enable();

            mainInput.Gameplay.Look.performed += OnLook;
            mainInput.Gameplay.Dash.performed += OnDash;
            mainInput.Gameplay.RangeAttack.performed += OnRangeAttack;
            mainInput.Gameplay.KeyboardAim.performed += OnKeyboardAim;
            mainInput.Gameplay.MeleeAttack.performed += OnMeleeAttack;
            mainInput.Gameplay.Pause.started += OnPause;

            mainInput.Map.Back.started += MapBackPressed;
            mainInput.Map.Confirm.started += ConfirmPressed;
            mainInput.Map.Select.started += SelectPressed;

            mainInput.Menu.Back.started += MenuBackPressed;

        }

        public void SwitchInputMap(GameState state)
        {
            // GameState state = GameManager.instance.GetCurrentGameState();
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
                default:
                    break;
            }




        }

        //i dunno how to call this with the current implementation. i've had succes with # of gamepads connected >0 but it's a hack
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

        public void TriggerUIClick(InputAction.CallbackContext _context) // this comes from the event system no? or you need it for gamepad?
        {
            if (_context.performed)
            {
               
            }
        }

        //public void OnMovementAsked(InputAction.CallbackContext _context)
        //{

        //    switch (_context.phase)
        //    {
        //        case InputActionPhase.Performed:
        //            StackInputIfNotTop(InputType.MovementPerformed);
        //            break;
        //        case InputActionPhase.Canceled:
        //            StackInputIfNotTop(InputType.MovementCanceled);
        //            break;
        //    }

        //    GameplayData.s_PlayerInputs.Movement = _context.ReadValue<Vector2>();
        //}

  
        float x;
        float y;
        Vector2 lastMoveInput;
        private void Update()   // this is as close as i could get to what you had before
        {
              x = mainInput.Gameplay.Movement.ReadValue<Vector2>().x;
              y = mainInput.Gameplay.Movement.ReadValue<Vector2>().y;

            var move = new Vector2(x,y);

            if (move == Vector2.zero && lastMoveInput != Vector2.zero )
            {
                StackInputIfNotTop(InputType.MovementCanceled);
            }
            else StackInputIfNotTop(InputType.MovementPerformed);

            lastMoveInput = move;
            GameplayData.s_PlayerInputs.Movement = move;

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

