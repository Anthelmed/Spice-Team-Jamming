using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(Animation))]
    public class KnightVisuals : MobVisuals
    {
        [SerializeField] private Animation m_animator;
        [SerializeField] private AnimationClip[] m_animations;
        [SerializeField] private Vector2 m_meleeRange;
        [SerializeField] private float m_rangedDelay;

        private int m_current;
        private float m_currentStart;

        public override Vector2 MeleeRange => m_meleeRange;

        public override float RangedDelay => m_rangedDelay;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!m_animator) m_animator = GetComponent<Animation>();
        }

        protected override void Awake()
        {
            base.Awake();
            m_animator.AddClip(m_animations[(int)AnimationID.Idle], AnimationID.Idle.ToString());
            m_animator.AddClip(m_animations[(int)AnimationID.Walk], AnimationID.Walk.ToString());
            m_animator.AddClip(m_animations[(int)AnimationID.Attack], AnimationID.Attack.ToString());
            m_animator.AddClip(m_animations[(int)AnimationID.RangedAttack], AnimationID.RangedAttack.ToString());
            m_animator.AddClip(m_animations[(int)AnimationID.Hit], AnimationID.Hit.ToString());
            m_animator.AddClip(m_animations[(int)AnimationID.Death], AnimationID.Death.ToString());
        }

        private void OnEnable()
        {
            m_animator.Play(((AnimationID)m_current).ToString());
            var state = m_animator[((AnimationID)m_current).ToString()];
            state.time = Time.timeSinceLevelLoad - m_currentStart;
        }

        public override void SetAnimation(AnimationID id)
        {
            if ((int)id == m_current) return;

            if (isActiveAndEnabled)
            {
                m_animator.CrossFade(id.ToString(), 0.2f);
            }

            m_current = (int)id;
            m_currentStart = Time.timeSinceLevelLoad;

            if (m_sounds)
            {
                switch (id)
                {
                    case AnimationID.Attack:
                        m_sounds.PlayAttackSound();
                        break;
                    case AnimationID.RangedAttack:
                        m_sounds.PlayRangeSound();
                        break;
                    case AnimationID.Hit:
                        m_sounds.PlayDamageSound();
                        break;
                    case AnimationID.Death:
                        m_sounds.PlayDeathSound();
                        break;
                }
            }
        }

        public override float GetDuration(AnimationID id)
        {
            return m_animations[(int)id].length;
        }
    }
}