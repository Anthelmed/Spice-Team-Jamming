using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MobCombatIdleState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.LookAtTarget = true;
    }

    public void Tick(MobAI.Data data)
    {
        if (!data.Target)
        {
            data.NextState = MobAI.State.Idle;
            return;
        }

        if (data.TargetDistance > data.attackRange.y)
        {
            data.NextState = MobAI.State.GoToTarget;
            return;
        }

        if (data.CurrentAttackCooldown < 0f && data.ToTargetCos > MobAI.COS_ATTACK)
        {
            data.CurrentAttackCooldown = data.attackCooldown;
            data.NextState = MobAI.State.Attack;
        }
    }

    public void Exit(MobAI.Data data)
    {
        data.LookAtTarget = false;
    }
}
