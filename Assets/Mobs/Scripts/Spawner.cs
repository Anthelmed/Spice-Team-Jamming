using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mobs
{
    [AddComponentMenu("Mobs/Spawner")]
    public class Spawner : MonoBehaviour
    {
        public MobNavigation prefab;
        public int amount = 1000;

        public Transform target;

        public Transform[] transforms;

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

            transforms = new Transform[amount];

            for (int i = 0; i < amount; ++i)
            {
                var offset = Random.insideUnitCircle * radius;
                var position = center + new Vector3(offset.x, 0, offset.y);

                var mob = Instantiate(prefab, position, Quaternion.identity);
                transforms[i] = mob.transform;
            }
        }

        private void Update()
        {
            if (!target)
                return;

            for (int i = 0; i < transforms.Length; ++i)
            {
                var trans = transforms[i];

                var direction = target.position - trans.position;
                if (direction.sqrMagnitude > Mathf.Epsilon)
                {
                    direction.Normalize();
                    direction *= prefab.speed;

                    var desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
                    trans.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, prefab.angularSpeed * Time.deltaTime);
                }

                trans.position += direction * Time.deltaTime;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}