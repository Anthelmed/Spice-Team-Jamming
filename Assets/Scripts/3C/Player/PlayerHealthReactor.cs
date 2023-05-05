using DefaultNamespace;
using Units;
using UnityEngine;

namespace _3C.Player
{
    public class PlayerHealthReactor : MonoBehaviour
    {
        private Unit m_Unit;

        private void Awake()
        {
            m_Unit = GetComponent<Unit>();
        }

        public void OnHealthChanged(float _health)
        {
            PlayerStaticEvents.s_PlayerHealthChanged?.Invoke(_health / m_Unit.MAXHealth);
          
        }

        public void OnDied()
        {
            PlayerStaticEvents.s_PlayerDied?.Invoke();
        }
    }
}