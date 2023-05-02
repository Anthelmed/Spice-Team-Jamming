using UnityEngine;

namespace Units
{
    public class MobDeathState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            data.frameStarted = Time.timeSinceLevelLoad;
        }

        public void Exit(Mob.Data data)
        {
        }

        public void Tick(Mob.Data data)
        {
            data.visuals.SetAnimation(MobVisuals.AnimationID.Death);

            if (Time.timeSinceLevelLoad >= (data.frameStarted + data.visuals.GetDuration(MobVisuals.AnimationID.Death)))
                data.NextState = Mob.State.Destroy;
        }
    }
}