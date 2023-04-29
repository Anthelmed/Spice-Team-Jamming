using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public abstract class UnitVisuals : MonoBehaviour
    {
        [HideInInspector][SerializeField] protected Unit m_unit;

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            m_unit = GetComponentInParent<Unit>();
        }

        protected virtual void Awake()
        {
            if (m_unit)
            {
                m_unit.onVisibilityChanged.AddListener(OnVisibilityChanged);
            }
        }

        protected virtual void OnVisibilityChanged(bool visible) 
        {
            gameObject.SetActive(visible);
        }
    }
}