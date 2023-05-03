using UnityEditor;
using UnityEngine;

namespace Units
{
    public class MobHitState : Mob.IState
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
            data.visuals.SetAnimation(MobVisuals.AnimationID.Hit);

            if (Time.timeSinceLevelLoad >= (data.frameStarted + data.visuals.GetDuration(MobVisuals.AnimationID.Hit)))
                data.NextState = Mob.State.CombatIdle;
        }
    }
}