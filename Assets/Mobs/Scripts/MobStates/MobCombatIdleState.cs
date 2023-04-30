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
        if (data.LeaderDistance > data.regroupDistance)
        {
            data.NextState = MobAI.State.Regroup;
            return;
        }

        if (!data.Target)
        {
            data.NextState = MobAI.State.Idle;
            return;
        }

        if (data.TargetDistance > data.MaxRange)
        {
            data.NextState = MobAI.State.GoToTarget;
            return;
        }

        if (data.CurrentAttackCooldown < 0f && data.ToTargetCos > MobAI.COS_ATTACK)
        {
            data.CurrentAttackCooldown = data.attackCooldown;
            data.NextState = data.UseRanged ? MobAI.State.RangedAttack : MobAI.State.Attack;
        }
    }

    public void Exit(MobAI.Data data)
    {
        data.LookAtTarget = false;
    }
}
