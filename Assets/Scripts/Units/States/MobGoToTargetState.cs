using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class MobGoToTargetState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            if (data.perception) data.perception.QueryTargetEnabled = true;
        }

        public void Exit(Mob.Data data)
        {
            if (data.locomotion)
            {
                data.locomotion.Destination = null;
            }

            if (data.perception) data.perception.QueryTargetEnabled = false;
        }

        public void Tick(Mob.Data data)
        {
            // TODO: If leader too far, Regroup

            if (!data.perception || !data.perception.Target) 
                data.NextState = Mob.State.Idle;
            else 
                data.locomotion.Destination = data.perception.Target.transform;

            if (data.attacks)
            {
                var toTarget = (data.perception.Target.transform.position - data.transform.position).sqrMagnitude;

                var distance = data.attacks.MeleeRange + data.perception.Target.Radius;
                distance *= 0.9f;
                if (toTarget < distance * distance)
                {
                    if (data.attacks.IsAttackReady)
                        data.attacks.SetAttackDelay(Random.Range(0f, data.attacks.MeleeColdown * 0.5f));
                    data.NextState = Mob.State.CombatIdle;
                }
                else
                {
                    distance = data.attacks.MinRange - data.perception.Target.Radius;
                    var maxDistance = data.attacks.MaxRange  + data.perception.Target.Radius;
                    if (toTarget > distance * distance && toTarget < maxDistance * maxDistance)
                        data.NextState = Mob.State.CombatIdle;
                }

            }
        }
    }
}