using DefaultNamespace.Audio;
using UnityEngine;

namespace Units
{
    public abstract class MobVisuals : UnitVisuals
    {
        public abstract void SetSpeed(float speed);
        public abstract void TriggerAttack();
        public abstract void TriggerRangedAttack();
        public abstract void TriggerHit();
        public abstract void TriggerDeath();
        public abstract bool HasAnimationFinished();
        public abstract bool IsDamagingFrame();

        [HideInInspector][SerializeField] protected MobSounds m_sounds;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!m_sounds && transform.parent) m_sounds = transform.parent.gameObject.GetComponentInChildren<MobSounds>();
        }
    }
}