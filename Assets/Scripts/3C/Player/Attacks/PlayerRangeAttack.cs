using System;
using UnityEngine;

namespace _3C.Player
{
    [Serializable]
    public class PlayerRangeAttack : PlayerStateBehavior
    {
        private Transform m_Transform;

        protected override void Init(IStateHandler _stateHandler)
        {
            m_Transform = _stateHandler.gameObject.transform;
        }

        public override void StartState()
        {
            m_StateHandler.OnAimingStateChanged(true);
        }

        public override void StopState()
        {
            m_StateHandler.OnAimingStateChanged(false);
        }

        public override void OnInput(InputType inputType)
        {
            if (inputType == InputType.AimCanceled)
            {
                m_StateHandler.OnStateEnded();
                //TODO: TriggerAttackAfterDelay
            }
        }
    }
}