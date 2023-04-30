using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class MobRegroupState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
        }

        public void Exit(Mob.Data data)
        {
            data.locomotion.ClearPosition();
        }

        public void Tick(Mob.Data data)
        {
            if (!data.squad || !data.squad.Leader)
            {
                data.NextState = Mob.State.Idle;
                return;
            }

            // Go to combat if we are close enough
            if (!data.squad.IsTooFar())
            {
                data.NextState = Mob.State.Idle;
                return;
            }

            data.locomotion.GoToPosition(data.squad.LeaderPosition);
        }
    }
}