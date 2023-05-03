namespace Units
{
    public class MobIdleState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            data.perception.QueryTargetEnabled = true;
        }

        public void Exit(Mob.Data data)
        {
            data.perception.QueryTargetEnabled = false;
        }

        public void Tick(Mob.Data data)
        {
            data.visuals.SetAnimation(MobVisuals.AnimationID.Idle);
            if (data.squad && data.squad.IsTooFar())
            {
                data.NextState = Mob.State.Regroup;
                return;
            }

            if (data.perception && data.perception.Target)
            {
                data.NextState = Mob.State.GoToTarget;
                return;
            }
        }
    }
}