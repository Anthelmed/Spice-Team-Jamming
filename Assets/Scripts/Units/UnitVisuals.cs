using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public abstract class UnitVisuals : MonoBehaviour
    {
        [HideInInspector][SerializeField] private Unit m_unit;

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            m_unit = GetComponentInParent<Unit>();
        }

        private void Awake()
        {
            if (m_unit)
            {
                m_unit.onVisibilityChanged.AddListener(OnVisibilityChanged);
            }
        }

        protected virtual void OnVisibilityChanged(bool visible) { }
    }
}