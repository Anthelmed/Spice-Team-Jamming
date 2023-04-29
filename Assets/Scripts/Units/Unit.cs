using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private int m_currentHealth;

        private void Awake()
        {
            m_currentHealth = m_maxHealth;
        }

#if UNITY_EDITOR
        public int CurrentHealth => m_currentHealth;
        public float Radius => m_radius;
#endif
    }
}