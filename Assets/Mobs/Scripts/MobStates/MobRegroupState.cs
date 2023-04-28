using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobRegroupState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.agent.SetDestination(data.leader.position);
        data.agent.updateRotation = true;
        data.ShouldQueryAdvanceBlocked = true;
    }

    public void Tick(MobAI.Data data)
    {
        if (!data.leader)
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
        if (data.LeaderDistance < data.smallTargetDistance * 0.5f)
        {
            data.NextState = MobAI.State.Idle;
            return;
        }

        // Update the navigation path if the current one is too outdated
        if ((data.agent.destination - data.leader.position).sqrMagnitude > (data.agent.radius * data.agent.radius))
        {
            data.agent.SetDestination(data.leader.position);
        }
    }

    public void Exit(MobAI.Data data)
    {
        data.agent.ResetPath();
        data.agent.updateRotation = false;
        data.ShouldQueryAdvanceBlocked = false;
    }
}
