using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Units
{
    public class MobAttackState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            data.framesLeft = 5;
            if (data.mob.Visible && data.visuals) data.visuals.TriggerAttack();
            else data.attacks.StartMelee();
        }

        public void Exit(Mob.Data data)
        {
            data.attacks.EndMelee();
        }

        public void Tick(Mob.Data data)
        {
            data.framesLeft--;
            if (!data.mob.Visible || !data.visuals || data.visuals.HasAnimationFinished())
            {
                if (data.framesLeft <= 0)
                    data.NextState = Mob.State.CombatIdle;

                return;
            }

            if (data.visuals.IsDamagingFrame())
                data.attacks.StartMelee();
            else
                data.attacks.EndMelee();
        }
    }
}