using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Mobs.Jobs
{
    public struct Boid
    {
        public float2 position;
        public float2 velocity;
    }

    [BurstCompile]
    public struct CalculateBoidsRules : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Boid> boids;
        public NativeArray<float2> newVelocity;

        public float2 targetPosition;

        //public float avoidanceRadius;
        //public float avoidanceStrength;

        //public float viewRadius;
        //public float alignementStrength;
        //public float cohesionStrength;

        [BurstCompile]
        public void Execute(int index)
        {
            var boid = boids[index];
            var targetVelocity = boid.velocity;

            var toTarget = targetPosition - boid.position;
            toTarget /= math.max(math.length(toTarget), 1f);

            targetVelocity += toTarget;

            newVelocity[index] = targetVelocity;
        }
    }
}