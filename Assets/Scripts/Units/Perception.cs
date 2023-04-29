using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class Perception : MonoBehaviour
    {
        [SerializeField][HideInInspector] private Unit m_unit;

        [SerializeField][Min(0f)] private float m_importantTargetRange = 20f;
        [SerializeField][Min(0f)] private float m_normalTargetRange = 10f;

        [SerializeField] private bool m_prioritizeImportant = false;
        [SerializeField] private bool m_targetFarVegetation = false;

        public bool QueryTargetEnabled { get; set; } = false;
        public Unit Target { get; private set; } = null;

        public void Tick()
        {
            if (!m_unit) return;

            if (QueryTargetEnabled) QueryTarget();
        }

        private void Start()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            m_unit = GetComponentInParent<Unit>();
        }

        private void QueryTarget()
        {
            Target = null;
            var position = m_unit ? m_unit.transform.position : transform.position;

            var world = DummyWorld.Instance;
            if (!world) return;

            Target = world.FindClosestEnemyOfType(position, m_importantTargetRange + m_unit.Radius, Unit.Type.Player, m_unit.Team, out float dist);
            var candidate = world.FindClosestEnemyOfType(position, m_importantTargetRange + m_unit.Radius, Unit.Type.Knight, m_unit.Team, out float newDist);

            if (candidate && (!Target || newDist < dist && candidate))
            {
                Target = candidate;
                dist = newDist;
            }

            if (m_prioritizeImportant && dist < (m_normalTargetRange + m_unit.Radius) && Target)
            {
                return;
            }

            candidate = world.FindClosestEnemyOfType(position, m_normalTargetRange + m_unit.Radius, Unit.Type.Pawn, m_unit.Team, out newDist);
            if (candidate && (!Target || newDist < dist))
            {
                Target = candidate;
            }

            if (Target) return;

            Target = world.FindClosestEnemyOfType(position, m_targetFarVegetation ? 10000f : m_normalTargetRange + m_unit.Radius, Unit.Type.Vegetation, m_unit.Team, out _);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var position = m_unit ? m_unit.transform.position : transform.position;
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.DrawWireDisc(position, Vector3.up, m_importantTargetRange);
            UnityEditor.Handles.DrawWireDisc(position, Vector3.up, m_normalTargetRange);
        }
#endif
    }
}