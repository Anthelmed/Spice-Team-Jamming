using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private Vector3 m_lastVelocity;
        private Vector3 m_targetPosition;
        private bool m_goToPosition;

        public void GoToPosition(Vector3 position)
        {
            m_goToPosition = true;
            m_targetPosition = position;
        }

        public void ClearPosition()
        {
            m_goToPosition = false;
        }

        public void StopImmediate()
        {
            m_lastVelocity = Vector3.zero;
            m_lastPosition = transform.position;
        }

        private Transform Transform => m_unit ? m_unit.transform : transform;

        public void FixedUpdate()
        {
            var trans = Transform;

            var targetVelocity = Vector3.zero;

            if (Destination)
            {
                m_targetPosition = Destination.transform.position;
            }

            if (Destination || m_goToPosition)
            {
                var toTarget = (m_targetPosition - trans.position).normalized;
                targetVelocity = toTarget * m_speed;
                HandleIntersections(trans, toTarget);
            }

            var lastMovement = trans.position - m_lastPosition;
            m_lastPosition = trans.position;

            var lastVelocity = lastMovement / Time.deltaTime;
            lastVelocity = Vector3.ClampMagnitude(lastVelocity, m_speed);
            lastVelocity = Vector3.MoveTowards(m_lastVelocity, lastVelocity, m_acceleration * Time.deltaTime);
            m_lastVelocity = lastVelocity;
            var velocity = Vector3.MoveTowards(lastVelocity, targetVelocity, m_acceleration * Time.deltaTime);

            var newPosition = trans.position + velocity * Time.deltaTime;
            newPosition.y = 0f;
            trans.position = newPosition;

            var lastSpeed = lastVelocity.magnitude;
            if (m_visuals) m_visuals.SetSpeed(lastSpeed);

            if (LookAtTarget)
            {
                trans.rotation = Quaternion.RotateTowards(
                    trans.rotation,
                    Quaternion.LookRotation(LookAtTarget.position - trans.position),
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

        private void HandleIntersections(Transform trans, Vector3 direction)
        {
            var world = DummyWorld.Instance;
            if (!world) return;

            var intersections = world.QueryCircleAll(trans.position, m_unit.Radius);

            for (int i = 0; i < intersections.Count; ++i)
            {
                if (intersections[i] == m_unit) continue;
                var vector = intersections[i].transform.position - trans.position;
                var amount = vector.magnitude;

                if (amount <= 0f) continue;
                vector /= amount;
                if (Vector3.Dot(vector, direction) < 0f) continue;
                trans.position += vector * (amount - m_unit.Radius - intersections[i].Radius);
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