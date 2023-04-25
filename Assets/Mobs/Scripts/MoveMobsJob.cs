using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Mobs.Jobs
{
    [BurstCompile]
    public struct MoveMobsJob : IJobParallelForTransform
    {
        public NativeArray<Boid> boids;
        [ReadOnly] public NativeArray<float2> targetSpeed;

        public float maxSpeed;
        public float angularSpeed;
        public float dt;

        [BurstCompile]
        public void Execute(int index, TransformAccess transform)
        {
            var boid = boids[index];

            // Set the velocity clamped
            boid.velocity = targetSpeed[index];
            if (math.lengthsq(boid.velocity) > (maxSpeed * maxSpeed))
            {
                boid.velocity = math.normalize(boid.velocity) * maxSpeed;
            }

            boid.position += boid.velocity * dt;

            transform.position = new Vector3(boid.position.x, 0, boid.position.y);
            var lookTarget = Quaternion.LookRotation(new Vector3(boid.velocity.x, 0, boid.velocity.y), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookTarget, angularSpeed * dt);

            boids[index] = boid;
        }
    }
}