public class MobAttackState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.animator.TriggerAttack();
        if (data.sounds) data.sounds.PlayAttackSound();
    }

    public void Tick(MobAI.Data data)
    {
        if (data.attackCollider)
            data.attackCollider.enabled = data.animator.IsDamagingFrame();

        if (data.animator.HasAnimationFinished())
            data.NextState = MobAI.State.CombatIdle;
    }

    public void Exit(MobAI.Data data)
    {
        if (data.attackCollider)
            data.attackCollider.enabled = false;
    }
}
