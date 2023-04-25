using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mobs
{
    [AddComponentMenu("Mobs/Navigation")]
    public class MobNavigation : MonoBehaviour
    {
        [Header("Basic parameters")]
        public float speed = 1;
        public float acceleration = 1f;

        [Header("Boids")]
        public float avoidanceRadius = 1f;
        public float avoidanceStrength = 1f;
        public float viewRadius = 5f;
        public float cohesionStrength = 1f;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
        }
    }
}