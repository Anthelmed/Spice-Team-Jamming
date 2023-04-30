using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class Squad : MonoBehaviour
    {
        [SerializeField] private Unit m_leader;
        [SerializeField] private float m_maxDistance;
        [SerializeField] private float m_offset;

        public Unit Leader => m_leader;

        public Vector3 LeaderPosition
        {
            get
            {
                if  (!m_leader)
                    return Vector3.zero;

                return m_leader.transform.position + m_leader.transform.forward * m_offset;
            }
        }

        public float MaxDistance => m_maxDistance;

        public bool IsTooFar()
        {
            var toLeader = (LeaderPosition - transform.position).sqrMagnitude;
            return (toLeader > m_maxDistance * m_maxDistance);
        }
    }
}