using System;
using DefaultNamespace;
using NaughtyAttributes;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace _3C.Player
{
    [Serializable]
    public class PlayerManaPoints
    {
        [SerializeField] private float m_MaxManaPoint;
        [SerializeField] private float m_ReloadAmountPerSecond;

        [ReadOnly]
        [SerializeField]
        private float m_CurrentManaPoint;
        private IStateHandler m_StateHandler;

        public void Init(IStateHandler _stateHandler)
        {
            m_StateHandler = _stateHandler;
            m_CurrentManaPoint = m_MaxManaPoint;
            PlayerStaticEvents.s_PlayerManaChanged?.Invoke(m_CurrentManaPoint);
        }
        
        public void Update()
        {
            m_CurrentManaPoint += m_ReloadAmountPerSecond * Time.deltaTime;
            PlayerStaticEvents.s_PlayerManaChanged?.Invoke(m_CurrentManaPoint);
            if (m_CurrentManaPoint > m_MaxManaPoint)
            {
                m_CurrentManaPoint = m_MaxManaPoint;
            }
        }

        public bool CheckIfPossiblePlusConsume(float _manaAmount)
        {
            if (CheckIfPossible(_manaAmount))
            {
                m_CurrentManaPoint -= _manaAmount;
                PlayerStaticEvents.s_PlayerManaChanged?.Invoke(m_CurrentManaPoint);
                return true;
            }

            return false;
        }

        public bool CheckIfPossible(float _manaAmount)
        {
            return m_CurrentManaPoint >= _manaAmount;
        }

        public void TriggerOnCantConsumeMana()
        {
            PlayerStaticEvents.s_OnActionImpossibleBecauseOfMana?.Invoke();
        }
    }
}