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

        public float avoidanceRadiusSq;
        public float avoidanceStrength;

        public float viewRadiusSq;
        //public float alignementStrength;
        public float cohesionStrength;

        [BurstCompile]
        public void Execute(int index)
        {
            var boid = boids[index];
            var targetVelocity = boid.velocity;

            // Follow target
            var toTarget = targetPosition - boid.position;
            toTarget /= math.max(math.length(toTarget), 1f);

            targetVelocity += toTarget;

            // Avoidance
            float2 avoidance = float2.zero;
            float2 cohesion = float2.zero;
            int numNeighbours = 0;
            float2 toBoid;
            float distSq;
            for (int i = 0 ; i < boids.Length; ++i)
            {
                toBoid = boids[i].position - boid.position;
                distSq = math.lengthsq(toBoid);
                if (distSq < avoidanceRadiusSq)
                {
                    avoidance -= toBoid;
                }
                else if (distSq < viewRadiusSq)
                {
                    cohesion += boids[i].position;
                    ++numNeighbours;
                }
            }

            if (numNeighbours > 0)
                cohesion = (cohesion / numNeighbours) - boid.position;
            else
                cohesion = 0f;

            newVelocity[index] = targetVelocity + avoidanceStrength * avoidance + cohesion * cohesionStrength;
        }
    }
}