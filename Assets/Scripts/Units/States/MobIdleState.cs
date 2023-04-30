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
            if (data.squad && data.squad.IsTooFar())
                data.NextState = Mob.State.Regroup;

            if (data.perception && data.perception.Target)
                data.NextState = Mob.State.GoToTarget;
        }
    }
}