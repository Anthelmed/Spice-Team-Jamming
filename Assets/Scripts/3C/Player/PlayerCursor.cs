using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _3C.Player
{
    public class PlayerCursor : MonoBehaviour
    {
        [SerializeField] private float m_GamepadCursorSpeed;

        public bool UseMouse = true;
        
        private Vector2 m_CursorPosition;

        public Vector2 Movement;
        
        private void Update()
        {
            if (UseMouse)
            {
                m_CursorPosition = Mouse.current.position.ReadValue();
            }
            else
            {
                m_CursorPosition += Movement * m_GamepadCursorSpeed * Time.deltaTime;
            }

            GameplayData.CursorPosition = m_CursorPosition;
        }
    }
}