using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Units
{
    public class MobAttacks : MonoBehaviour
    {
        [SerializeField] private HitBox m_meleeHitBox;
        [SerializeField] [Min(0f)] private float m_meleeCooldown = 2f;
        [SerializeField] private GameObject m_rangedObject;
        [SerializeField] private Vector2 m_rangedRange = Vector2.up;
        [SerializeField] [Min(0f)] private float m_rangedCooldown = 2f;

        private float m_cooldownEnds;

        public float MeleeColdown => m_meleeCooldown;
        public float MeleeRange => m_meleeHitBox ? m_meleeHitBox.Radius : 0f;
        public float MaxRange => m_rangedRange.y;
        public float MinRange => m_rangedRange.x;
        public bool IsAttackReady => Time.timeSinceLevelLoad > m_cooldownEnds;
        public bool HasRangedAttack => m_rangedObject;

        private void Awake()
        {
            if (m_rangedObject)
                m_rangedObject.SetActive(false);
        }

        public void SetAttackDelay(float delay)
        {
            m_cooldownEnds = Time.timeSinceLevelLoad + delay;
        }

        public void StartMelee()
        {
            if (m_meleeHitBox.isActiveAndEnabled) return;
            if (!IsAttackReady) return;

            SetAttackDelay(m_meleeCooldown);
            if (m_meleeHitBox)
                m_meleeHitBox.gameObject.SetActive(true);
        }

        public void EndMelee()
        {
            if (m_meleeHitBox)
                m_meleeHitBox.gameObject.SetActive(false);
        }

        public void DoRanged()
        {
            if (!m_rangedObject || m_rangedObject.activeSelf) return;
            if (!IsAttackReady) return;

            SetAttackDelay(m_rangedCooldown);
            var proj = m_rangedObject.GetComponent<Projectile>();
            if (proj)
                proj.target = transform.position + transform.forward * Mathf.Lerp(m_rangedRange.x, m_rangedRange.y, 0.5f);

            m_rangedObject.transform.position = transform.position;
            m_rangedObject.SetActive(true);
        }

        public bool IsInMeleeRange(Unit unit)
        {
            var toTarget = (unit.transform.position - transform.position).sqrMagnitude;
            var range = MeleeRange + unit.Radius;
            return (toTarget < range * range);
        }

        public bool IsInRangedRange(Unit unit)
        {
            var toTarget = (unit.transform.position - transform.position).sqrMagnitude;
            var minRange = m_rangedRange.x - unit.Radius;
            var maxRange = m_rangedRange.y + unit.Radius;
            return (toTarget > minRange * minRange && toTarget < maxRange * maxRange);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!HasRangedAttack) return;

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, m_rangedRange.x);
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, m_rangedRange.y);
        }
#endif
    }
}