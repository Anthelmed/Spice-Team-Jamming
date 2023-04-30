using System;
using DefaultNamespace;
using NaughtyAttributes;
using Runtime.Utilities;
using UnityEngine;

namespace _3C.Player
{
 
    [Serializable]
    public class PlayerMovement : PlayerStateBehavior
    {
        [Header("Movement Settings")]
        [SerializeField] private float m_Speed;
        [Min(0)]
        [Tooltip("Defines how fast the character achieve player wanted speed, higher is the most reactive")]
        [SerializeField] private float m_MovementDamping;
        [SerializeField] private float m_RotationDamping;

        [Header("Animation")]
        [AnimatorParam("m_Animator")]
        [SerializeField] private int m_SpeedAnimatorParam;
        [SerializeField] private float m_AnimatorSpeedMultiplier;
        [SerializeField] private Animator m_Animator;

        [HideInInspector]
        public Vector2 Movement;

        [ReadOnly] public bool IsLookingAtMovement = true;

        
        private Rigidbody m_Rigidbody;
        private Vector2 m_CurrentMovement;
        private Vector2 m_LookAtDirection;
        private Transform m_Transform;
        [ReadOnly]
        public bool Enabled;

        private Vector3 CurrentWorldSpeed => (m_CurrentMovement * m_Speed).X0Y().SetY(m_Rigidbody.velocity.y);  

        protected override void Init(IStateHandler _stateHandler)
        {
            m_Rigidbody = _stateHandler.gameObject.GetComponent<Rigidbody>();
            m_Transform = _stateHandler.gameObject.transform;
            Enabled = true;
            ChangeAnimatorSpeedParameter(0);
        }

        public override void Update()
        {
            if (!Enabled)
            {
                return;
            }
            
            Movement = GameplayData.s_PlayerInputs.Movement;
            m_CurrentMovement = Vector2.LerpUnclamped(m_CurrentMovement, Movement, Time.deltaTime * m_MovementDamping);

            
            m_Rigidbody.velocity = CurrentWorldSpeed;
            ChangeAnimatorSpeedParameter(m_CurrentMovement.magnitude * m_AnimatorSpeedMultiplier);

            if (!IsLookingAtMovement) return;
            
            if (Movement == Vector2.zero)
            {
                m_LookAtDirection = Vector2.LerpUnclamped(m_LookAtDirection, m_CurrentMovement, Time.deltaTime * m_RotationDamping);
            }
            else
            {
                m_LookAtDirection = VectorExtension.Slerp(m_LookAtDirection, Movement, Time.deltaTime * m_RotationDamping);
            }
            m_Transform.LookAt(m_Transform.position + m_LookAtDirection.X0Y());
        }

        private void ChangeAnimatorSpeedParameter(float _value)
        {
            m_Animator?.SetFloat(m_SpeedAnimatorParam, _value);
        }

        public override void StopState()
        {
            //Enabled = false;
            m_Rigidbody.velocity = Vector3.zero;
            ChangeAnimatorSpeedParameter(0);
        }

        public override void StartState()
        {
            //Enabled = true;
            //m_Animator?.SetTrigger(m_MovementTriggerParam);
        }

        public override void OnDrawGizmos()
        {
            if (m_Transform == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_Transform.position, m_CurrentMovement.normalized);
            Gizmos.DrawWireSphere(m_Transform.position + m_CurrentMovement.X0Y(), 0.25f);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_Transform.position, m_LookAtDirection.normalized);

        }

        public void InstantOrientationTo(Vector2 _direction)
        {
            m_LookAtDirection = Movement;
        }
    }
}