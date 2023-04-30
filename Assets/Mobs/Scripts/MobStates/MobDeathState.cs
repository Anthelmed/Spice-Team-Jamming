public class MobDeathState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.animator.TriggerDeath();
        if (data.sounds) data.sounds.PlayDeathSound();
    }

    public void Tick(MobAI.Data data)
    {
        if (data.animator.HasAnimationFinished())
            data.Destroy = true;
    }

    public void Exit(MobAI.Data data)
    {
    }
}
