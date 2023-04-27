using System;
using DefaultNamespace.HealthSystem.Damageable;
using UnityEngine;

namespace DefaultNamespace.HealthSystem.Damager
{
    public class ColliderDamager : MonoBehaviour
    {
        [SerializeField]
        private int m_Damage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(m_Damage);
            }
        }
    }
}