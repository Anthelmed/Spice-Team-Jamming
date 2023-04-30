using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static MobAI;

namespace Units
{
    public class Mob : Unit
    {
        [System.Serializable]
        public class Data
        {
            public Transform transform;
            public Mob mob;
            public Perception perception;
            public Locomotion locomotion;
            public MobVisuals visuals;
            public MobAttacks attacks;
            public Squad squad;

            public int framesLeft;

            public State NextState { get; set; }
        }

        [HideInInspector] [SerializeField] private Data m_data = new Data();

        public interface IState
        {
            void Enter(Data data);
            void Tick(Data data);
            void Exit(Data data);
        }

        public enum State
        {
            Uninitialized = 0,
            Idle,
            GoToTarget,
            Regroup,
            Queueing,
            CombatIdle,
            Attack,
            RangedAttack,
            Hit,
            Death,
            Destroy
        }

        private static IState[] m_states = new IState[]
        {
            null,
            new MobIdleState(),
            new MobGoToTargetState(),
            new MobRegroupState(),
            new MobQueueingState(),
            new MobCombatIdleState(),
            new MobAttackState(),
            new MobRangedAttackState(),
            new MobHitState(),
            new MobDeathState(),
            null
        };

        private State m_state = State.Uninitialized;

        public static readonly float COS_ATTACK = Mathf.Cos(15 * Mathf.Deg2Rad);

        private void OnDeath(float damage, Unit other)
        {
            m_data.NextState = State.Death;
        }

        private void OnHit(float damage, Unit other)
        {
            m_data.NextState = State.Hit;
        }

        private void OnValidate()
        {
            m_data.transform = transform;
            m_data.mob = this;
            m_data.perception = GetComponentInChildren<Perception>();
            m_data.locomotion = GetComponentInChildren<Locomotion>();
            m_data.visuals = GetComponentInChildren<MobVisuals>();
            m_data.attacks = GetComponentInChildren<MobAttacks>();
            m_data.squad = GetComponentInChildren<Squad>();
        }

        private void Reset()
        {
            OnValidate();
        }

        private void UpdateTransition()
        {
            if (m_state == m_data.NextState)
                return;

            m_states[(int)m_state]?.Exit(m_data);
            m_state = m_data.NextState;
            m_states[(int)m_state]?.Enter(m_data);
        }

        protected override void Awake()
        {
            base.Awake();
            m_state = State.Uninitialized;
            m_data.NextState = State.Idle;

            onDie.AddListener(OnDeath);
            onHit.AddListener(OnHit);
        }

        protected override void Update()
        {
            base.Update();

            if (m_data.NextState == State.Destroy)
            {
                Destroy(gameObject);
                return;
            }

            if (m_data.perception) m_data.perception.Tick();

            UpdateTransition();
            m_states[(int)m_state]?.Tick(m_data);
            UpdateTransition();
        }

#if UNITY_EDITOR
        public State CurrentState => m_state;
#endif
    }
}