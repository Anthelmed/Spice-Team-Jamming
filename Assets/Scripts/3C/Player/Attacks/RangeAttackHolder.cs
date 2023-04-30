using DefaultNamespace.HealthSystem.Damager;
using Units;
using UnityEngine;
using UnityEngine.Pool;

namespace _3C.Player.Weapons
{
    public class RangeAttackHolder : MonoBehaviour
    {
        [SerializeField] private AWeaponMovement m_WeaponMovement;
        public AWeaponMovement WeaponMovement => m_WeaponMovement;
        
        [SerializeField] private ParticleSystem[] m_VFXToPlay;

        public ParticleSystem[] VFXToPlay => m_VFXToPlay;

        [SerializeField] private HitBox m_HitBox;
        
        public HitBox HitBox => m_HitBox;
    }
}