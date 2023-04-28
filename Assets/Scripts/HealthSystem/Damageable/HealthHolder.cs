using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.HealthSystem.Damageable
{
    public class HealthHolder : MonoBehaviour, IDamageable
    {
        public Targetable.Team team;
        [SerializeField] private int m_MaxHealth;

        [ReadOnly]
        [SerializeField] private int m_CurrentHealth;

        [SerializeField] private UnityEvent m_OnTakeDamage;
        [SerializeField] private UnityEvent m_OnHealed;
        [SerializeField] private UnityEvent<int> m_OnHealthChanged;
        [SerializeField] private UnityEvent m_OnDeath;
        
        private void Awake()
        {
            m_CurrentHealth = m_MaxHealth;
        }

        public void TakeDamage(int _damage, Targetable.Team _team)
        {
            // Don't receive damage from allies
            if (_team.HasFlag(team)) return;

            m_CurrentHealth -= _damage;
            if (m_CurrentHealth <= 0)
            {
                m_CurrentHealth = 0;
                m_OnDeath?.Invoke();
            }
            else
            {
                m_OnTakeDamage?.Invoke();
            }
        }

        public void Heal(int _healAmount)
        {
            m_CurrentHealth += _healAmount;
            if (m_CurrentHealth > m_MaxHealth)
            {
                m_CurrentHealth = m_MaxHealth;
            }
            
            m_OnHealed?.Invoke();
            TriggerHealthChange();
        }
        
        private void TriggerHealthChange()
        {
            m_OnHealthChanged?.Invoke(m_CurrentHealth);
        }
    }
}