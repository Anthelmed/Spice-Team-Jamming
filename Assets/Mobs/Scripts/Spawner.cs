using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Mobs
{
    [AddComponentMenu("Mobs/Spawner")]
    public class Spawner : MonoBehaviour
    {
        public MobAI prefab;
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
                var offset = UnityEngine.Random.insideUnitCircle * radius;
                var position = center + new Vector3(offset.x, 0, offset.y);

                var mob = Instantiate(prefab, position, Quaternion.identity);
                mob.target = target;
                var trans = mob.transform;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}