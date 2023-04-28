using DG.Tweening;
using UnityEngine;

namespace _3C.Player.Weapons
{
    public class SlashWeaponMovement : AWeaponMovement
    {
        [Range(0, 360)]
        [SerializeField] private float m_Angle;

        protected override Tween TriggerTween(float _duration)
        {
            transform.localRotation = Quaternion.Euler(0, - m_Angle * 0.5f, 0);
            return transform.DOLocalRotate(new Vector3(0, m_Angle * 0.5f, 0), _duration);
        }
    }
}