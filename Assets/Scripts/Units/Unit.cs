using AmplifyShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Units
{
    public enum Faction
    {
        Nature,
        Fire,
        Ice
    }

    [SelectionBase]
    public sealed class Unit : MonoBehaviour
    {
        public enum Type
        {
            Vegetation,
            Pawn,
            Knight,
            Player
        }

        [SerializeField] private Faction m_team;
        [SerializeField] private Faction m_immuneTo;
        [SerializeField] private Type m_type;
        [SerializeField] [Min(1)] private int m_maxHealth = 10;
        [SerializeField] [Min(0f)] private float m_invencivilityAfterHit = 0.5f;
        [SerializeField] [Min(0f)] private float m_radius = 0.5f;

        [Header("Events")]
        public UnityEvent onImmuneHit;
        public UnityEvent<float, Unit, Vector3> onHit;
        public UnityEvent<float> onHeal;
        public UnityEvent onDie;

        private float m_currentHealth;

        public void TakeHit(float damage, Unit other, Vector3 hitPosition)
        {
            if (other.m_team == m_immuneTo)
            {
                onImmuneHit?.Invoke();
                return;
            }

            if (other.m_team == m_team) return;

            m_currentHealth = Mathf.Max(0, m_currentHealth - damage);

            if (m_currentHealth == 0)
                onDie?.Invoke();
            else
                onHit?.Invoke(damage, other, hitPosition);
        }

        public void Heal(float amount)
        {
            m_currentHealth = Mathf.Min(m_maxHealth, m_currentHealth + amount);
            onHeal?.Invoke(amount);
        }

        private void Awake()
        {
            m_currentHealth = m_maxHealth;
        }

#if UNITY_EDITOR
        public float CurrentHealth => m_currentHealth;
        public float Radius => m_radius;

        [Header("Debug")]
        [SerializeField] private Color m_debugColor = Color.green;

        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = m_debugColor;
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, m_radius);
        }
#endif
    }
}