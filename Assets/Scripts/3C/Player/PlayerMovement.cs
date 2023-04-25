﻿using System;
using NaughtyAttributes;
using Runtime.Utilities;
using UnityEngine;

namespace _3C.Player
{
    
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : PlayerStateBehavior
    {
        [Header("Movement Settings")]
        [SerializeField] private float m_Speed;
        [Min(0)]
        [Tooltip("Defines how fast the character achieve player wanted speed, higher is the most reactive")]
        [SerializeField] private float m_MovementDamping;

        [Header("Animation")]
        [AnimatorParam("m_Animator")]
        [SerializeField] private int m_SpeedAnimatorParam;
        [SerializeField] private Animator m_Animator;

        [HideInInspector]
        public Vector2 Movement;
        private Rigidbody m_Rigidbody;

        private Vector2 m_CurrentMovement;

        private Vector3 CurrentWorldSpeed => (m_CurrentMovement * m_Speed).X0Y();  

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            m_CurrentMovement = Vector2.LerpUnclamped(m_CurrentMovement, Movement, Time.deltaTime * m_MovementDamping);
            m_Rigidbody.velocity = CurrentWorldSpeed;
            transform.LookAt(transform.position + m_CurrentMovement.X0Y());
            m_Animator.SetFloat(m_SpeedAnimatorParam, m_CurrentMovement.magnitude);
        }

        public override void StopState()
        {
            base.StopState();
            m_Rigidbody.velocity = Vector3.zero;
        }
    }
}