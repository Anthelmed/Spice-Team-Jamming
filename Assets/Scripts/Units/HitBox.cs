using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Units
{
    public class HitBox : MonoBehaviour
    {
        public enum Shape
        {
            Circle,
            Fan
        }

        [SerializeField][HideInInspector] private Unit m_owner;

        public float Radius => m_radius;
        public float damage = 1f;
        [SerializeField] private Shape m_shape = Shape.Circle;
        [SerializeField][Min(0f)] private float m_radius = 1f;
        [SerializeField][Min(0f)] private float m_fanAngle = 30f;

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            m_owner = GetComponentInParent<Unit>();
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Color.red;
            if (m_shape == Shape.Circle)
            {
                UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, m_radius);
            }
            else
            {
                var center = transform.position;
                var start = transform.rotation * Quaternion.AngleAxis(-0.5f * m_fanAngle, Vector3.up) * Vector3.forward;
                var startp = center + start * m_radius;
                var end = transform.rotation * Quaternion.AngleAxis(0.5f * m_fanAngle, Vector3.up) * Vector3.forward;
                var endp = center + end * m_radius;
                UnityEditor.Handles.DrawWireArc(transform.position, Vector3.up, start, m_fanAngle, m_radius);
                UnityEditor.Handles.DrawLine(center, startp);
                UnityEditor.Handles.DrawLine(center, endp);
            }
        }
#endif
    }
}