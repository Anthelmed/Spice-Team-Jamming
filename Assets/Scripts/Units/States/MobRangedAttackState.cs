namespace Units
{
    public class MobRangedAttackState : Mob.IState
    {
        public void Enter(Mob.Data data)
        {
            if (data.mob.Visible && data.visuals) data.visuals.TriggerRangedAttack();
            else data.attacks.DoRanged();
        }

        public void Exit(Mob.Data data)
        {
        }

        public void Tick(Mob.Data data)
        {
            if (!data.mob.Visible || !data.visuals || data.visuals.HasAnimationFinished())
            {
                data.NextState = Mob.State.CombatIdle;
                return;
            }

            if (data.visuals.IsDamagingFrame() && data.perception.Target)
                data.attacks.DoRanged();
        }
    }
}