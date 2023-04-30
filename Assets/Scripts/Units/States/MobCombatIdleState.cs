using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class MobCombatIdleState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            if (data.locomotion && data.perception.Target)
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

            float distance;
            var toTarget = data.perception.Target.transform.position - data.transform.position;
            var cos = Vector3.Dot(toTarget.normalized, data.transform.forward);
            bool canAttack = cos > Mob.COS_ATTACK;

            float toTargetSq = (data.perception.Target.transform.position - data.transform.position).sqrMagnitude;
            if (data.attacks.HasRangedAttack)
            {
                // Chase the target if it got out of range
                distance = data.attacks.MaxRange + data.perception.Target.Radius;
                if (toTargetSq > distance * distance)
                {
                    data.NextState = Mob.State.GoToTarget;
                    return;
                }
                
                // Attack if in range
                distance = data.attacks.MinRange - data.perception.Target.Radius;
                if (toTargetSq > distance * distance)
                {
                    if (/*canAttack &&*/ data.attacks.IsAttackReady)
                        data.NextState = Mob.State.RangedAttack;
                    return;
                }

            }

            // Go to melee range
            distance = data.attacks.MeleeRange + data.perception.Target.Radius;
            if (toTargetSq > distance * distance)
                data.NextState = Mob.State.GoToTarget;

            if (canAttack && data.attacks.IsAttackReady)
                data.NextState = Mob.State.Attack;
        }
    }
}