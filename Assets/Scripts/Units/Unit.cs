using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Units
{
    public enum Faction
    {
        Nature,
        Fire,
        Ice
    }

    [SelectionBase]
    public class Unit : MonoBehaviour
    {
        public enum Type
        {
            Vegetation,
            Pawn,
            Knight,
            Player
        }

        public Faction Team => m_team;
        [SerializeField] private Faction m_team;
        [SerializeField] private Faction m_immuneTo;
        public Type UnitType => m_type;
        [SerializeField] private Type m_type;
        [SerializeField] [Min(1)] private int m_maxHealth = 10;
        [SerializeField] [Min(0f)] private float m_invencivilityAfterHit = 0.5f;
        [SerializeField] [Min(0f)] private float m_radius = 0.5f;

        [Header("Events")]
        public UnityEvent onImmuneHit;
        public UnityEvent<float, Unit, Vector3> onHit;
        public UnityEvent<float> onHeal;
        public UnityEvent onDie;
        public UnityEvent<bool> onVisibilityChanged;

        public bool Visible => DummyWorld.Instance.visible;

        private float m_currentHealth;
        private float m_lastHit;

        public void TakeHit(float damage, Unit other, Vector3 hitPosition)
        {
            if (m_currentHealth < 0f) return;

            if (Time.timeSinceLevelLoad - m_lastHit < m_invencivilityAfterHit) return;

            if (other.m_team == m_immuneTo)
            {
                onImmuneHit?.Invoke();
                return;
            }

            if (other.m_team == m_team) return;

            m_lastHit = Time.timeSinceLevelLoad;
            m_currentHealth = Mathf.Max(0, m_currentHealth - damage);

            if (m_currentHealth == 0)
            {
                onDie?.Invoke();
                DummyWorld.Instance.Unregister(this);
            }
            else
                onHit?.Invoke(damage, other, hitPosition);
        }

        public void Heal(float amount)
        {
            var wasDead = m_currentHealth == 0;

            m_currentHealth = Mathf.Min(m_maxHealth, m_currentHealth + amount);
            onHeal?.Invoke(amount);

            if (wasDead)
                DummyWorld.Instance.Register(this);
        }

        public void ChangeVisibility(bool visible)
        {
            onVisibilityChanged?.Invoke(visible);
        }

        private void Awake()
        {
            m_currentHealth = m_maxHealth;
        }

        private void OnEnable()
        {
            DummyWorld.Instance.Register(this);
        }

        private void OnDisable()
        {
            DummyWorld.Instance.Unregister(this);
        }

#if UNITY_EDITOR
        public float CurrentHealth => m_currentHealth;
        public float Radius => m_radius;

        [Header("Debug")]
        [SerializeField] private Color m_debugColor = Color.green;

        private void OnDrawGizmos()
        {
            if (Application.isPlaying && !Visible)
            {
                UnityEditor.Handles.color = m_debugColor;
                UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, m_radius);
            }
        }
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = m_debugColor;
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, m_radius);
        }
#endif
    }
}