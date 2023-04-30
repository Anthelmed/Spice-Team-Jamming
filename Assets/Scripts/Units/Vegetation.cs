using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class Vegetation : Unit
    {
        private void Reset()
        {
            m_team = Faction.Nature;
            OnValidate();
        }

        private void OnValidate()
        {
            m_immuneTo = Faction.Nature;
            m_type = Type.Vegetation;
        }

        protected override void Awake()
        {
            base.Awake();

            onDie.AddListener(OnDie);
        }

        private void OnDie (float damage, Unit other)
        {
            // Turn to the type of the aggressor
            m_team = other.Team;
            // Go back to life
            Heal(1000f);

#if UNITY_EDITOR
            switch (m_team)
            {
                case Faction.Nature:
                    m_debugColor = Color.green * 0.25f;
                    break;
                case Faction.Fire:
                    m_debugColor = Color.red * 0.25f;
                    break;
                case Faction.Ice:
                    m_debugColor = Color.blue * 0.25f;
                    break;
            }
#endif
        }
    }
}