using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobIdleState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.ShouldQueryTargets = true;
        data.QueryNow = true;
    }

    public void Tick(MobAI.Data data)
    {
        if (data.LeaderDistance > data.regroupDistance)
        {
            data.NextState = MobAI.State.Regroup;
            return;
        }

        if (data.Target)
            data.NextState = MobAI.State.GoToTarget;
    }

    public void Exit(MobAI.Data data)
    {
        data.ShouldQueryTargets = false;
    }
}
