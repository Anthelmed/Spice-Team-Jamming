using UnityEngine;

namespace Units
{
    public class AutoUpdateUnit : MonoBehaviour
    {
        private Unit m_unit;

        private void Awake()
        {
            m_unit = GetComponent<Unit>();
        }

        private void Update()
        {
            if (!m_unit.HasTile)
                m_unit.Tick();
            else 
                enabled = false;
        }
    }
}