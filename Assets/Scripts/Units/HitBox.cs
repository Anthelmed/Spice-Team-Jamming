using System.Collections.Generic;
using UnityEditor;
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

        [SerializeField][HideInInspector] public Unit m_owner;

        public float Radius => m_radius;
        public float damage = 1f;
        [SerializeField] private Shape m_shape = Shape.Circle;
        [SerializeField][Min(0f)] private float m_radius = 1f;
        [SerializeField][Min(0f)] private float m_fanAngle = 30f;

        private List<Unit> m_targetsHit = new List<Unit>(5);

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
            enabled = false;
        }

        private void OnEnable()
        {
            if (!m_owner)
                m_owner = GetComponentInParent<Unit>();
            m_targetsHit.Clear();
        }

        private void Update()
        {
            var world = LevelTilesManager.instance;

            if (!world || !m_owner) return;

            List<Unit> newHits = null;

            if (m_shape == Shape.Circle)
                newHits = world.QueryCircleEnemies(transform.position, m_radius, m_owner.Team);
            else if (m_shape == Shape.Fan)
                newHits = world.QueryFanEnemies(transform.position, m_radius, transform.forward, m_fanAngle, m_owner.Team);

            if (newHits == null) return;

            for (int i = 0; i < newHits.Count; ++i)
            {
                if (!m_targetsHit.Contains(newHits[i]))
                {
                    m_targetsHit.Add(newHits[i]);
                    newHits[i].TakeHit(damage, m_owner);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.red;
            if (m_shape == Shape.Circle)
            {
                Handles.DrawWireDisc(transform.position, Vector3.up, m_radius);
            }
            else
            {
                var center = transform.position;
                var start = transform.rotation * Quaternion.AngleAxis(-0.5f * m_fanAngle, Vector3.up) * Vector3.forward;
                var startp = center + start * m_radius;
                var end = transform.rotation * Quaternion.AngleAxis(0.5f * m_fanAngle, Vector3.up) * Vector3.forward;
                var endp = center + end * m_radius;
                Handles.DrawWireArc(transform.position, Vector3.up, start, m_fanAngle, m_radius);
                Handles.DrawLine(center, startp);
                Handles.DrawLine(center, endp);
            }
        }
#endif
    }
}