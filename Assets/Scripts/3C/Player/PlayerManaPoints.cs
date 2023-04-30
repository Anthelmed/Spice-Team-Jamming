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
            TriggerManaChange();
        }
        
        public void Update()
        {
            m_CurrentManaPoint += m_ReloadAmountPerSecond * Time.deltaTime;
            TriggerManaChange();
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
                TriggerManaChange();
                return true;
            }

            return false;
        }

        public bool CheckIfPossible(float _manaAmount)
        {
            return m_CurrentManaPoint >= _manaAmount;
        }

        private void TriggerManaChange()
        {
            PlayerStaticEvents.s_PlayerManaChanged?.Invoke(m_CurrentManaPoint/m_MaxManaPoint);
        }

        public void TriggerOnCantConsumeMana()
        {
            PlayerStaticEvents.s_OnActionImpossibleBecauseOfMana?.Invoke();
        }
    }
}