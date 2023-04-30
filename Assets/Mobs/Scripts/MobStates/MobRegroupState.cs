public class MobRegroupState : MobAI.IMobState
{
    public void Enter(MobAI.Data data)
    {
        data.agent.SetDestination(data.leader.position);
        data.agent.updateRotation = true;
    }

    public void Tick(MobAI.Data data)
    {
        if (!data.leader)
        {
            data.NextState = MobAI.State.Idle;
            return;
        }

        // Go to combat if we are close enough
        if (data.LeaderDistance < data.regroupDistance * 0.5f)
        {
            data.NextState = MobAI.State.Idle;
            return;
        }

        // Update the navigation path if the current one is too outdated
        if ((data.agent.destination - data.RegroupPosition).sqrMagnitude > (data.agent.radius * data.agent.radius))
        {
            data.agent.SetDestination(data.RegroupPosition);
        }
    }

    public void Exit(MobAI.Data data)
    {
        data.agent.ResetPath();
        data.agent.updateRotation = false;
    }
}
