using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobRangedAttackState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.projectile.transform.localPosition = Vector3.zero;
        data.projectile.target = data.TargetTransform.position;
        data.projectile.gameObject.SetActive(true);

        data.animator.TriggerRangedAttack();
        if (data.sounds) data.sounds.PlayRangeSound();
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
