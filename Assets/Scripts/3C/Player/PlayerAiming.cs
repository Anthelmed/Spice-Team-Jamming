using DefaultNamespace;
using Runtime.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    public class PlayerAiming : MonoBehaviour
    {
        private Camera m_MainCamera;

        private void Awake()
        {
            m_MainCamera = Camera.main;
        }

        private void Update()
        {
            var direction = Vector2.zero;
            if (GameplayData.s_PlayerInputs.IsUsingCursorPositionForAim)
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                var ray = m_MainCamera.ScreenPointToRay(Mouse.current.position.value);
                if (plane.Raycast(ray, out float value))
                {
                    direction = ray.GetPoint(value).XZ() - GameplayData.s_PlayerStateHandler.transform.position.XZ();
                    GameplayData.s_PlayerInputs.AimDirection = direction.normalized;
                }
            }
            else
            {
                direction = GameplayData.s_PlayerInputs.AimDirection;
            }

            transform.LookAt(transform.position + direction.X0Y());
        }
    }
}