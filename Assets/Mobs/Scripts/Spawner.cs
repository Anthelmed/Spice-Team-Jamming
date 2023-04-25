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
        public MobNavigation prefab;
        public int amount = 1000;

        public Transform target;

        private TransformAccessArray transforms;
        private NativeArray<Jobs.Boid> boids;
        private NativeArray<float2> targetSpeed;

        private JobHandle m_moveJobHandle;

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
            boids = new NativeArray<Jobs.Boid>(amount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            targetSpeed = new NativeArray<float2>(amount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < amount; ++i)
            {
                var offset = UnityEngine.Random.insideUnitCircle * radius;
                var position = center + new Vector3(offset.x, 0, offset.y);

                var mob = Instantiate(prefab, position, Quaternion.identity);
                var trans = mob.transform;
                transArray[i] = trans;
                boids[i] = new Jobs.Boid
                {
                    position = new float2(trans.position.x, trans.position.z),
                    velocity = float2.zero
                };
            }

            transforms = new TransformAccessArray(transArray);
        }

        private void OnDestroy()
        {
            transforms.Dispose();
            boids.Dispose();
            targetSpeed.Dispose();
        }

        private void Update()
        {
            if (!target)
                return;

            var dependency = new Jobs.CalculateBoidsRules
            {
                boids = boids,
                newVelocity = targetSpeed,
                targetPosition = new float2(target.position.x, target.position.z)
            }.Schedule(boids.Length, boids.Length / 8 + 8);

            m_moveJobHandle = new Jobs.MoveMobsJob
            {
                boids = boids,
                targetSpeed = targetSpeed,
                maxSpeed = prefab.speed,
                acceleration = prefab.acceleration,
                dt = Time.deltaTime
            }.Schedule(transforms, dependency);
        }

        private void LateUpdate()
        {
            m_moveJobHandle.Complete();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}