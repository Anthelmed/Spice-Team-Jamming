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
            if (data.squad && data.squad.IsTooFar())
                data.NextState = Mob.State.Regroup;

            if (!data.perception.Target)
            {
                data.NextState = Mob.State.Idle;
                return;
            }

            var toTarget = data.perception.Target.transform.position - data.transform.position;
            var cos = Vector3.Dot(toTarget.normalized, data.transform.forward);
            bool canAttack = cos > Mob.COS_ATTACK;
            
            if (data.attacks.HasRangedAttack && data.attacks.IsInRangedRange(data.perception.Target))
            {
                if (canAttack && data.attacks.IsAttackReady)
                    data.NextState = Mob.State.RangedAttack;
                return;
            }

            if (!data.attacks.IsInMeleeRange(data.perception.Target))
            {
                data.NextState = Mob.State.GoToTarget;
            }

            if (canAttack && data.attacks.IsAttackReady)
                data.NextState = Mob.State.Attack;
        }
    }
}