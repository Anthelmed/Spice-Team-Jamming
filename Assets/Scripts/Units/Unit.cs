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
        [SerializeField] [Min(0f)] private float m_radius = 0.5f;

        [Header("Events")]
        public UnityEvent<float, Unit, Vector3> onHit;

        private float m_currentHealth;

        public void TakeHit(float damage, Unit other, Vector3 hitPosition)
        {
            if (other.m_team == m_team || other.m_team == m_immuneTo) return;

            m_currentHealth -= damage;
            onHit?.Invoke(damage, other, hitPosition);
        }

        private void Awake()
        {
            m_currentHealth = m_maxHealth;
        }

#if UNITY_EDITOR
        public float CurrentHealth => m_currentHealth;
        public float Radius => m_radius;
#endif
    }
}