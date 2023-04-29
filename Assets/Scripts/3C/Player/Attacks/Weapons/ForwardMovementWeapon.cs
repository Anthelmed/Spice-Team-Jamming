using _3C.Player.Weapons;
using DG.Tweening;
using UnityEngine;

public class ForwardWeaponMovement : AWeaponMovement
{
    [SerializeField] private float m_Speed;

    protected override Tween TriggerTween(float _duration)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        return transform.DOMove(transform.position + transform.forward * m_Speed * _duration, _duration);
    }
}