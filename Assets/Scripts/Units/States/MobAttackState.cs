using System.Data;
using UnityEngine;

namespace Units
{
    public class MobAttackState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            data.frameStarted = Time.timeSinceLevelLoad;
            data.wasAttacking = false;
        }

        public void Exit(Mob.Data data)
        {
            data.attacks.EndMelee();
        }

        public void Tick(Mob.Data data)
        {
            data.visuals.SetAnimation(MobVisuals.AnimationID.Attack);

            var elapsed = Time.timeSinceLevelLoad - data.frameStarted;

            if (elapsed >= data.visuals.MeleeRange.x && elapsed <= data.visuals.MeleeRange.y)
            {
                if (!data.wasAttacking) data.attacks.StartMelee();
                data.wasAttacking = true;
            }
            else
            {
                if (data.wasAttacking) data.attacks.EndMelee();
                data.wasAttacking = false;
            }

            if (elapsed >= data.visuals.GetDuration(MobVisuals.AnimationID.Attack))
                data.NextState = Mob.State.CombatIdle;
        }
    }
}