using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Unit
{
    public class MobAttacks : MonoBehaviour
    {
        [SerializeField] private HitBox m_meleeHitBox;
        [SerializeField] [Min(0f)] private float m_meleeCooldown = 2f;

        private float m_meleeColdownEnd;

        public float MeleeColdown => m_meleeCooldown;
        public float MeleeRange => m_meleeHitBox ? m_meleeHitBox.Radius : 0f;
        public bool IsMeleeReady => Time.timeSinceLevelLoad > m_meleeColdownEnd;

        public void SetMeleeDelay(float delay)
        {
            m_meleeColdownEnd = Time.timeSinceLevelLoad + delay;
        }

        public void StartMelee()
        {
            SetMeleeDelay(m_meleeCooldown);
            if (m_meleeHitBox)
                m_meleeHitBox.gameObject.SetActive(true);
        }

        public void EndMelee()
        {
            if (m_meleeHitBox)
                m_meleeHitBox.gameObject.SetActive(false);
        }
    }
}