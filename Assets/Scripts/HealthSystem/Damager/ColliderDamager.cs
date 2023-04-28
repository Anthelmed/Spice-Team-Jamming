using System;
using DefaultNamespace.HealthSystem.Damageable;
using UnityEngine;

namespace DefaultNamespace.HealthSystem.Damager
{
    public class ColliderDamager : MonoBehaviour
    {
        public int Damage;

        public Targetable.Team team;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(Damage, team);
            }
        }
    }
}