using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Units
{
    public class Locomotion : MonoBehaviour
    {
        [HideInInspector][SerializeField] private Unit m_unit;
        [HideInInspector][SerializeField] private MobVisuals m_visuals;

        [SerializeField] [Min(0f)] private float m_speed = 5f;
        [SerializeField] [Min(0f)] private float m_acceleration = 10f;
        [SerializeField] [Min(0f)] private float m_angularSpeed = 360f;

        public Transform Destination { get; set; }
        public Transform LookAtTarget { get; set; }

        private Vector3 m_lastPosition;

        private Transform Transform => m_unit ? m_unit.transform : transform;

        public void FixedUpdate()
        {
            var trans = Transform;
            var lastMovement = trans.position - m_lastPosition;
            m_lastPosition = trans.position;
            
            var targetVelocity = Vector3.zero;

            if (Destination)
            {
                var toTarget = (Destination.position - m_lastPosition).normalized;
                targetVelocity = toTarget * m_speed;
            }

            var lastVelocity = lastMovement / Time.deltaTime;
            var velocity = Vector3.MoveTowards(lastVelocity, targetVelocity, m_acceleration * Time.deltaTime);

            trans.position += velocity * Time.deltaTime;

            var lastSpeed = lastVelocity.magnitude;
            if (m_visuals) m_visuals.SetSpeed(lastSpeed);

            if (LookAtTarget)
            {
                trans.rotation = Quaternion.RotateTowards(
                    trans.rotation,
                    Quaternion.LookRotation(LookAtTarget.position - m_lastPosition),
                    m_angularSpeed * Time.deltaTime);
            }
            else if (lastSpeed > 0.1f)
            {
                trans.rotation = Quaternion.RotateTowards(
                    trans.rotation,
                    Quaternion.LookRotation(lastVelocity),
                    m_angularSpeed * Time.deltaTime);
            }
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            m_unit = GetComponentInParent<Unit>();
            if (m_unit) m_visuals = m_unit.GetComponentInChildren<MobVisuals>();
            else m_visuals = null;
        }

        private void Awake()
        {
            m_lastPosition = Transform.position;
        }
    }
}