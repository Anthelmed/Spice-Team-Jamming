using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class MobGoToTargetState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            // TODO: Set destination in locomotion
            // TODO: Locomotion look in direction of movement

            if (data.perception) data.perception.QueryTargetEnabled = true;
        }

        public void Exit(Mob.Data data)
        {
            // TODO: Locomotion clear destination
            // TODO: Locomotion stop rotation

            if (data.perception) data.perception.QueryTargetEnabled = false;
        }

        public void Tick(Mob.Data data)
        {
            // TODO: If leader too far, Regroup
            if (!data.perception || !data.perception.Target) data.NextState = Mob.State.Idle;
            // TODO: If close enough, fight
        }
    }
}