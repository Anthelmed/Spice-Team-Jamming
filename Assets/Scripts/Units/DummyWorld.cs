using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public sealed class DummyWorld : MonoBehaviour
    {
        private static DummyWorld m_instance;
        public static DummyWorld Instance
        {
            get
            {
                //if (m_instance)
                    return m_instance;

                //m_instance = FindObjectOfType<DummyWorld>();
                //return m_instance;
            }
        }

        private List<Unit> m_vegetation = new List<Unit>();
        private List<Unit> m_pawns = new List<Unit>();
        private List<Unit> m_knights = new List<Unit>();
        private List<Unit> m_players = new List<Unit>();

        private List<Unit> GetListForType(Unit.Type type)
        {
            switch (type)
            {
                case Unit.Type.Vegetation:
                    return m_vegetation;
                case Unit.Type.Pawn:
                    return m_pawns;
                case Unit.Type.Knight:
                    return m_knights;
                case Unit.Type.Player:
                    return m_players;
            }
            return null;
        }

        public void Register(Unit unit)
        {
            GetListForType(unit.UnitType).Add(unit);
        }

        public void Unregister(Unit unit)
        {
            GetListForType(unit.UnitType).Remove(unit);
        }

        private void Awake()
        {
            m_instance = this;
        }

#if UNITY_EDITOR
        public List<Unit> Vegetation => m_vegetation;
        public List<Unit> Pawns => m_pawns;
        public List<Unit> Knights => m_knights;
        public List<Unit> Players => m_players;
#endif
    }
}