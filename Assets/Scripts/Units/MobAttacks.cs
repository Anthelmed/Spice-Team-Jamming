using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Unit
{
    public class MobAttacks : MonoBehaviour
    {
        [SerializeField] private HitBox m_meleeHitBox;

        public float MeleeRange => m_meleeHitBox ? m_meleeHitBox.Radius : 0f;

        public void StartMelee()
        {
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