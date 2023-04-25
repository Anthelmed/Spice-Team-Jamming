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
        public float acceleration;
        public float dt;

        [BurstCompile]
        public void Execute(int index, TransformAccess transform)
        {
            var boid = boids[index];

            // Set the velocity clamped
            var wantedVelocity = targetSpeed[index];
            if (math.lengthsq(wantedVelocity) > (maxSpeed * maxSpeed))
            {
                wantedVelocity = math.normalize(wantedVelocity) * maxSpeed;
            }

            boid.velocity = math.lerp(boid.velocity, wantedVelocity, acceleration * dt);
            boid.position += boid.velocity * dt;

            transform.position = new Vector3(boid.position.x, 0, boid.position.y);
            transform.rotation = Quaternion.LookRotation(new Vector3(boid.velocity.x, 0, boid.velocity.y), Vector3.up);

            boids[index] = boid;
        }
    }
}