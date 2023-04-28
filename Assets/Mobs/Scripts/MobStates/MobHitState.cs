using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHitState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.animator.TriggerHit();
        if (data.sounds) data.sounds.PlayDamageSound();
    }

    public void Tick(MobAI.Data data)
    {
        if (data.animator.HasAnimationFinished())
            data.NextState = MobAI.State.CombatIdle;
    }

    public void Exit(MobAI.Data data)
    {
    }
}
