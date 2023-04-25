using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mobs
{
    [AddComponentMenu("Mobs/Spawner")]
    public class Spawner : MonoBehaviour
    {
        public MobNavigation prefab;
        public int amount = 1000;

        public Transform target;

        private float Radius
        {
            get
            {
                var scale = transform.localScale;
                return Mathf.Max(scale.x, scale.y, scale.z);
            }
        }

        private void Start()
        {
            if (!prefab)
                return;

            var radius = Radius;
            var center = transform.position;

            for (int i = 0; i < amount; ++i)
            {
                var offset = Random.insideUnitCircle * radius;
                var position = center + new Vector3(offset.x, 0, offset.y);

                var mob = Instantiate(prefab, position, Quaternion.identity);
                mob.target = target;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}