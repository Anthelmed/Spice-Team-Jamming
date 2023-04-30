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
    [RequireComponent(typeof(AutoUpdateUnit))]
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
        [SerializeField] protected Faction m_team;
        [SerializeField] protected Faction m_immuneTo;
        public Type UnitType => m_type;
        [SerializeField] protected Type m_type;
        [SerializeField] [Min(1)] private int m_maxHealth = 10;
        [SerializeField] [Min(0f)] private float m_invencivilityAfterHit = 0.5f;
        [SerializeField] [Min(0f)] private float m_radius = 0.5f;
        [SerializeField] private bool doesntMove = false;

        public int MAXHealth => m_maxHealth;

        [Header("Events")]
        public UnityEvent onImmuneHit;
        public UnityEvent<float, Unit> onHit;
        public UnityEvent<float> onHeal;
        public UnityEvent<float> onHealthChanged;
        public UnityEvent<float, Unit> onDie;
        public UnityEvent<bool> onVisibilityChanged;

        public bool HasTile => m_currentTile;
        public bool Visible => !m_currentTile || m_currentTile.tileActivated;

        private float m_currentHealth;
        private float m_lastHit;

        [SerializeField] private LevelTile m_currentTile;

        public void TakeHit(float damage, Unit other)
        {
            if (m_currentHealth <= 0f) return;

            if (Time.timeSinceLevelLoad - m_lastHit < m_invencivilityAfterHit) return;

            if (other.m_team == m_immuneTo)
            {
                onImmuneHit?.Invoke();
                return;
            }

            if (other.m_team == m_team) return;

            m_lastHit = Time.timeSinceLevelLoad;
            m_currentHealth = Mathf.Max(0, m_currentHealth - damage);
            onHealthChanged?.Invoke(m_currentHealth);


            if (m_currentHealth == 0)
            {
                onDie?.Invoke(damage, other);
                m_currentTile.Unregister(this);
                m_currentTile = null;
                GetComponent<AutoUpdateUnit>().enabled = true;
            }
            else
                onHit?.Invoke(damage, other);
        }

        public void Heal(float amount)
        {
            var wasDead = m_currentHealth == 0;

            m_currentHealth = Mathf.Min(m_maxHealth, m_currentHealth + amount);
            onHealthChanged?.Invoke(m_currentHealth);
            onHeal?.Invoke(amount);

            if (wasDead)
                m_currentTile.Register(this);
        }

        public void ChangeVisibility(bool visible)
        {
            onVisibilityChanged?.Invoke(visible);
        }

        public virtual void FixedTick() { }

        public virtual void Tick()
        {
            if (!this) return;
            if (doesntMove && m_currentTile) return;

            var world = LevelTilesManager.instance;
            if (!world) return;
            var newTile = world.GetTileAtPosition(transform.position);
            if (newTile != m_currentTile)
            {
                var visible = Visible;
                if (m_currentTile)
                    m_currentTile.Unregister(this);

                newTile.Register(this);
                m_currentTile = newTile;

                if (visible != Visible)
                    ChangeVisibility(Visible);
            }
        }

        protected virtual void Awake()
        {
            m_currentHealth = m_maxHealth;
            onHealthChanged?.Invoke(m_currentHealth);
            m_currentTile = null;
        }
        public float CurrentHealth => m_currentHealth;
        public float Radius => m_radius;
        
#if UNITY_EDITOR


        [Header("Debug")]
        [SerializeField] protected Color m_debugColor = Color.green;

        // private void OnDrawGizmos()
        // {
        //     if (Application.isPlaying && !Visible)
        //     {
        //         UnityEditor.Handles.color = m_debugColor;
        //         UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, m_radius);
        //     }
        // }
        // private void OnDrawGizmosSelected()
        // {
        //     UnityEditor.Handles.color = m_debugColor;
        //     UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, m_radius);
        // }
#endif
    }
}