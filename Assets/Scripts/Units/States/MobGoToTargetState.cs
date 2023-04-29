using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class MobGoToTargetState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            if (data.locomotion && data.perception && data.perception.Target)
            {
                
            // TODO: Locomotion look in direction of movement
            }

            if (data.perception) data.perception.QueryTargetEnabled = true;
        }

        public void Exit(Mob.Data data)
        {
            if (data.locomotion)
            {
                data.locomotion.Destination = null;
                // TODO: Locomotion stop rotation
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

            // TODO: If close enough, fight
        }
    }
}