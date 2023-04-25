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
            }

            transform.position += direction * Time.deltaTime;
        }
    }
}