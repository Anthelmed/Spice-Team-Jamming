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

            Target = world.FindClosestEnemyOfType(position, m_importantTargetRange, Unit.Type.Player, m_unit.Team, out float distSq);
            var candidate = world.FindClosestEnemyOfType(position, m_importantTargetRange, Unit.Type.Knight, m_unit.Team, out float newDistSq);

            if (candidate && (!Target || newDistSq < distSq && candidate))
            {
                Target = candidate;
                distSq = newDistSq;
            }

            if (m_prioritizeImportant && distSq < (m_normalTargetRange * m_normalTargetRange) && Target)
            {
                return;
            }

            candidate = world.FindClosestEnemyOfType(position, m_normalTargetRange, Unit.Type.Pawn, m_unit.Team, out newDistSq);
            if (candidate && (!Target || newDistSq < distSq))
            {
                Target = candidate;
            }

            if (Target) return;

            Target = world.FindClosestEnemyOfType(position, m_targetFarVegetation ? 10000f : m_normalTargetRange, Unit.Type.Vegetation, m_unit.Team, out _);
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