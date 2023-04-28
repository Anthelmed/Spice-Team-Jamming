using System;
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
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            var ray = m_MainCamera.ScreenPointToRay(Mouse.current.position.value);
            var direction = Vector3.zero;
            if (plane.Raycast(ray, out float value))
            {
                direction = ray.GetPoint(value) - GameplayData.s_PlayerStateHandler.transform.position;
                GameplayData.s_PlayerInputs.AimDirection = direction.normalized;
                Debug.Log("Raycast hit: " + Mouse.current.position.value + " " + direction.normalized);
            }
            
            // if (GameplayData.s_PlayerInputs.AimDirection == Vector2.zero)
            // {
            //     return;
            // }
            
            transform.LookAt(transform.position + direction.XZ().X0Y());
        }
    }
}