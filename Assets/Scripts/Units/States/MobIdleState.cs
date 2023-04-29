using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class MobIdleState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            if (data.perception)
                data.perception.QueryTargetEnabled = true;
        }

        public void Exit(Mob.Data data)
        {
            if (data.perception)
                data.perception.QueryTargetEnabled = false;
        }

        public void Tick(Mob.Data data)
        {
            // TODO too far from leader, go there
            // TODO has target, to there
        }
    }
}