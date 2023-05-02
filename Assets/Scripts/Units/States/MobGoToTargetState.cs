using UnityEngine;

namespace Units
{
    public class MobGoToTargetState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            data.perception.QueryTargetEnabled = true;
        }

        public void Exit(Mob.Data data)
        {
            data.locomotion.Destination = null;
            data.perception.QueryTargetEnabled = false;
        }

        public void Tick(Mob.Data data)
        {
            data.visuals.SetAnimation(MobVisuals.AnimationID.Walk);

            if (data.squad.IsTooFar())
            {
                data.NextState = Mob.State.Regroup;
                return;
            }

            if (!data.perception.Target)
            {
                data.NextState = Mob.State.Idle;
                return;
            }
            else
            {
                data.locomotion.Destination = data.perception.Target.transform;
            }

            if (data.attacks.IsInMeleeRange(data.perception.Target) ||
                (data.attacks.HasRangedAttack && data.attacks.IsInRangedRange(data.perception.Target)))
            {
                if (data.attacks.IsAttackReady)
                    data.attacks.SetAttackDelay(Random.Range(0f, data.attacks.MeleeColdown * 0.5f));

                data.NextState = Mob.State.CombatIdle;
            }
        }
    }
}