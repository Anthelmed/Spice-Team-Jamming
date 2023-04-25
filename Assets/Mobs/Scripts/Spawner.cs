using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace Mobs
{
    [AddComponentMenu("Mobs/Spawner")]
    public class Spawner : MonoBehaviour
    {
        public MobNavigation prefab;
        public int amount = 1000;

        public Transform target;

        public TransformAccessArray transforms;

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

            var transArray = new Transform[amount];

            for (int i = 0; i < amount; ++i)
            {
                var offset = Random.insideUnitCircle * radius;
                var position = center + new Vector3(offset.x, 0, offset.y);

                var mob = Instantiate(prefab, position, Quaternion.identity);
                transArray[i] = mob.transform;
            }

            transforms = new TransformAccessArray(transArray);
        }

        private void OnDestroy()
        {
            transforms.Dispose();
        }

        private void Update()
        {
            if (!target)
                return;

            new Jobs.MoveMobsJob
            {
                angularSpeed = prefab.angularSpeed,
                speed = prefab.speed,
                target = target.position,
                dt = Time.deltaTime
            }.Schedule(transforms);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}