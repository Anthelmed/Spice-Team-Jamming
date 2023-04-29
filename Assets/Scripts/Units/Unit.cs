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

    public class Unit : MonoBehaviour
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
        [SerializeField] private int m_maxHealth;
        [SerializeField] [Min(0f)] private float m_radius;

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