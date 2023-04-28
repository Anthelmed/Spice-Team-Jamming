using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace _3C.Player.Weapons
{
    public abstract class AWeaponMovement : MonoBehaviour
    {
        private Collider m_Collider;
        private Tween m_CurrentTweener;

        private void Awake()
        {
            m_Collider = GetComponentInChildren<Collider>();
            m_Collider.enabled = false;
            m_Collider.isTrigger = true;
        }

        public void TriggerWeaponMovement(float _duration, AnimationCurve _MovementCurve)
        {
            m_Collider.enabled = true;
            m_CurrentTweener = TriggerTween(_duration);
            m_CurrentTweener.onComplete += OnMovementEnded;
        }

        private void OnMovementEnded()
        {
            ResetState();
            OnStateReset();
        }

        protected virtual void OnStateReset() {}

        protected abstract Tween TriggerTween(float _duration);

        public void StopWeaponMovement()
        {
            m_CurrentTweener.Kill();
            ResetState();
        }

        private void ResetState()
        {
            m_Collider.enabled = false;
        }
    }
}