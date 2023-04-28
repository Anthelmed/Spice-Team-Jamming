using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobQueueingState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.ShouldQueryAdvanceBlocked = true;
        data.ShouldQueryTargets = true;
    }

    public void Tick(MobAI.Data data)
    {
        if (data.LeaderDistance > data.smallTargetDistance)
        {
            data.NextState = MobAI.State.Regroup;
            return;
        }

        if (!data.AdvanceBlocked)
            data.NextState = MobAI.State.GoToTarget;
    }

    public void Exit(MobAI.Data data)
    {
        data.ShouldQueryAdvanceBlocked = false;
        data.ShouldQueryTargets = false;
    }
}
