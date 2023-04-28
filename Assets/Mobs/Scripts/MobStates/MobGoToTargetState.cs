using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGoToTargetState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.agent.SetDestination(data.TargetTransform.position);
        data.agent.updateRotation = true;
        data.ShouldQueryAdvanceBlocked = true;
        data.ShouldQueryTargets = true;
        data.QueryNow = true;
    }
    public void Tick(MobAI.Data data)
    {
        if (!data.Target)
        {
            data.NextState = MobAI.State.Idle;
            return;
        }

        if (data.AdvanceBlocked)
        {
            data.NextState = MobAI.State.Queueing;
            return;
        }

        // Go to combat if we are close enough
        if (data.TargetDistance < data.meleeRange)
        {
            // Use a very small coldown to prevent both enemies attacking at the same time when they get in range
            if (data.CurrentAttackCooldown < 0f)
                data.CurrentAttackCooldown = Random.Range(0f, data.attackCooldown * 0.5f);
            data.NextState = MobAI.State.CombatIdle;
            return;
        }

        // Update the navigation path if the current one is too outdated
        if ((data.agent.destination - data.TargetTransform.position).sqrMagnitude > (data.agent.radius * data.agent.radius))
        {
            data.agent.SetDestination(data.TargetTransform.position);
        }
    }

    public void Exit(MobAI.Data data)
    {
        data.agent.ResetPath();
        data.agent.updateRotation = false;
        data.ShouldQueryTargets = false;
        data.ShouldQueryAdvanceBlocked = false;
    }
}
