using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mobs
{
    [AddComponentMenu("Mobs/Navigation")]
    public class MobNavigation : MonoBehaviour
    {
        public Transform target;

        public float speed = 1;
        public float angularSpeed = 360f*2f;

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
        }

        private void Update()
        {
            if (!target)
                return;

            var direction = target.position - transform.position;
            if (direction.sqrMagnitude > Mathf.Epsilon)
            {
                direction.Normalize();
                direction *= speed;

                var desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, angularSpeed * Time.deltaTime);
            }

            transform.position += direction * Time.deltaTime;
        }
    }
}