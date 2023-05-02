using DefaultNamespace.Audio;
using UnityEngine;

namespace Units
{
    public abstract class MobVisuals : UnitVisuals
    {
        public abstract void SetAnimation(AnimationID id);
        public abstract float GetDuration(AnimationID id);
        public abstract Vector2 MeleeRange { get; }
        public abstract float RangedDelay { get; }

        public enum AnimationID
        {
            Idle = 0,
            Walk,
            Attack,
            RangedAttack,
            Hit,
            Death
        }

        [HideInInspector][SerializeField] protected MobSounds m_sounds;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!m_sounds && transform.parent) m_sounds = transform.parent.gameObject.GetComponentInChildren<MobSounds>();
        }
    }
}