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
            data.visuals.SetAnimation(MobVisuals.AnimationID.Walk);

            if (!data.squad.Leader)
            {
                data.NextState = Mob.State.Idle;
                return;
            }

            data.locomotion.GoToPosition(data.squad.LeaderPosition);

            if (!data.squad.IsTooFar())
            {
                data.NextState = Mob.State.Idle;
                return;
            }
        }
    }
}