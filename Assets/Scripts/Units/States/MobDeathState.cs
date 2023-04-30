using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class MobDeathState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            if (data.visuals) data.visuals.TriggerDeath();
        }

        public void Exit(Mob.Data data)
        {
        }

        public void Tick(Mob.Data data)
        {
            if (!data.visuals || !data.mob.Visible || data.visuals.HasAnimationFinished())
                data.NextState = Mob.State.Destroy;
        }
    }
}