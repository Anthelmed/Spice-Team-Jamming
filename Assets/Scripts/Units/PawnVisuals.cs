using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PawnVisuals : MobVisuals
    {
        public VatData animationData;
        public float transitionDuration = 0.25f;

        [HideInInspector] [SerializeField] private MeshRenderer m_renderer;
        [SerializeField] private Vector2 m_meleeRange;

        private MaterialPropertyBlock m_mpb;
        private int m_currentAnimation = -1;
        private Vector4 m_previousData;

        private readonly int ANIM_DATA = Shader.PropertyToID("_AnimData");
        private readonly int PREV_ANIM_DATA = Shader.PropertyToID("_PrevAnimData");
        private readonly int TRANSITION = Shader.PropertyToID("_Transition");

        public override Vector2 MeleeRange => m_meleeRange;

        public override float RangedDelay => 0;

        public override void SetAnimation(AnimationID id, bool force = false)
        {
            if (!SwitchAnimation(id, force)) return;

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

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!m_renderer) m_renderer = GetComponent<MeshRenderer>();
        }

        protected override void Awake()
        {
            base.Awake();
            m_mpb = new MaterialPropertyBlock();
            SwitchAnimation(AnimationID.Idle);
            m_renderer.localBounds = new Bounds(Vector3.zero, Vector3.one * 2);
        }

        private bool SwitchAnimation(AnimationID desiredAnimationID, bool force = false)
        {
            var desiredAnimation = (int)desiredAnimationID;

            if (!animationData)
                return false;

            if (m_currentAnimation == desiredAnimation && !force)
            {
                if (desiredAnimation > 1)
                {
                    if (Time.timeSinceLevelLoad - m_previousData.w >= m_previousData.x)
                    {
                        m_previousData.w = Time.timeSinceLevelLoad - m_previousData.x;
                        m_mpb.SetVector(ANIM_DATA, m_previousData);
                        m_renderer.SetPropertyBlock(m_mpb);
                    }

                }
                return false;
            }

            m_currentAnimation = desiredAnimation;
            var data = animationData.animations[desiredAnimation];
            data.w = Time.timeSinceLevelLoad;
            m_mpb.SetVector(ANIM_DATA, data);
            m_mpb.SetVector(PREV_ANIM_DATA, m_previousData);
            m_mpb.SetFloat(TRANSITION, transitionDuration);
            m_previousData = data;
            m_renderer.SetPropertyBlock(m_mpb);

            return true;
        }

        public override float GetDuration(AnimationID id)
        {
            return animationData.animations[(int)id].x;
        }
    }
}