using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class MobCombatIdleState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            if (data.locomotion)
            {
                data.locomotion.StopImmediate();
                data.locomotion.LookAtTarget = data.perception.Target.transform;
            }
        }

        public void Exit(Mob.Data data)
        {
            if (data.locomotion) data.locomotion.LookAtTarget = null;
        }

        public void Tick(Mob.Data data)
        {
            // TODO: Regroup if leader too far

            if (!data.perception.Target)
            {
                data.NextState = Mob.State.Idle;
                return;
            }

            // Chase the target if it got out of range
            var distance = data.attacks.MeleeRange + data.perception.Target.Radius;
            distance *= 0.9f;
            if ((data.perception.Target.transform.position - data.transform.position).sqrMagnitude > distance * distance)
                data.NextState = Mob.State.GoToTarget;

            // TODO ranged attacks
            if (data.attacks.IsMeleeReady)
                data.NextState = Mob.State.Attack;
        }
    }
}