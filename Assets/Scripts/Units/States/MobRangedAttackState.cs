using UnityEngine;

namespace Units
{
    public class MobRangedAttackState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            data.frameStarted = Time.timeSinceLevelLoad;
            data.wasAttacking = false;
        }

        public void Exit(Mob.Data data)
        {
        }

        public void Tick(Mob.Data data)
        {
            data.visuals.SetAnimation(MobVisuals.AnimationID.RangedAttack);

            var elapsed = Time.timeSinceLevelLoad - data.frameStarted;

            if (!data.wasAttacking && elapsed >= data.visuals.RangedDelay)
                data.attacks.DoRanged();

            if (elapsed >= data.visuals.GetDuration(MobVisuals.AnimationID.RangedAttack))
            {
                data.NextState = Mob.State.CombatIdle;
                return;
            }
        }
    }
}