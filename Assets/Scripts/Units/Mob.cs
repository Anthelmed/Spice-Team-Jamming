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
            public State NextState { get; set; }
        }

        [SerializeField] private Data m_data = new Data();

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

        private void OnDeath(float damage, Unit other, Vector3 hitPos)
        {
            m_data.NextState = State.Death;
        }

        private void OnHit(float damage, Unit other, Vector3 hitPos)
        {
            m_data.NextState = State.Hit;
        }

        private void OnValidate()
        {
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

        private void Update()
        {
            if (m_data.NextState == State.Destroy)
            {
                Destroy(gameObject);
                return;
            }

            UpdateTransition();
            m_states[(int)m_state]?.Tick(m_data);
            UpdateTransition();
        }
    }
}