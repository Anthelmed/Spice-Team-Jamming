using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Mobs.Jobs
{
    [BurstCompile]
    public struct MoveMobsJob : IJobParallelForTransform
    {
        public Vector3 target;
        public float speed;
        public float angularSpeed;
        public float dt;

        [BurstCompile]
        public void Execute(int index, TransformAccess transform)
        {
            var direction = target - transform.position;
            if (direction.sqrMagnitude > Mathf.Epsilon)
            {
                direction.Normalize();
                direction *= speed;

                var desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, angularSpeed * dt);
            }

            transform.position += direction * dt;
        }
    }
}