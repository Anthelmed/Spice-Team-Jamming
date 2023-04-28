using DefaultNamespace.HealthSystem.Damageable;
using DefaultNamespace.HealthSystem.Damager;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    [Flags]
    public enum Team
    {
        Nature = 0x01,
        Fire = 0x02,
        Ice = 0x04,
        Wizard = 0x08,
    }

    public Team team = Team.Nature;
    public bool isMain;

    private static readonly Team[] m_teams = new Team[] { Team.Nature, Team.Fire, Team.Ice, Team.Wizard };

    private static List<Targetable>[] m_targets = new List<Targetable>[]
    {
        new List<Targetable>(),
        new List<Targetable>(),
        new List<Targetable>(),
        new List<Targetable>(),
    };

    private static List<Targetable>[] m_mainTargets = new List<Targetable>[]
    {
        new List<Targetable>(),
        new List<Targetable>(),
        new List<Targetable>(),
        new List<Targetable>(),
    };

    // Reuse this one to avoid allocations
    private static List<Targetable> m_reusableListForQueries = new List<Targetable>();

    public static List<Targetable> QueryTargets(Vector3 center, float maxDistance, bool mainTargets, Team teams)
    {
        m_reusableListForQueries.Clear();
        var maxDistanceSq = maxDistance * maxDistance;

        for (int i = 0; i < m_teams.Length; ++i)
        {
            if (!teams.HasFlag(m_teams[i])) continue;
            if (mainTargets)
                FindTargetsInRange(center, maxDistanceSq, m_mainTargets[i]);
            else
                FindTargetsInRange(center, maxDistanceSq, m_targets[i]);
        }

        return m_reusableListForQueries;
    }

    private static void FindTargetsInRange(Vector3 center, float maxDistanceSq, List<Targetable> targets)
    {
        for (int i = 0; i < targets.Count; ++i)
        {
            if ((targets[i].transform.position - center).sqrMagnitude < maxDistanceSq)
                m_reusableListForQueries.Add(targets[i]);
        }
    }

    public static Targetable QueryClosestTarget(Vector3 pos, float maxDistance, bool mainTarget, Team teams, out float distance)
    {
        Targetable result = null;
        Targetable newResult = null;

        var maxDistanceSq = maxDistance * maxDistance;
        float newDistSq;

        for (int i = 0; i < m_teams.Length; ++i)
        {
            if (!teams.HasFlag(m_teams[i])) continue;
            if (mainTarget)
                newResult = FindClosestTargetRange(pos, maxDistanceSq, m_mainTargets[i], out newDistSq);
            else
                newResult = FindClosestTargetRange(pos, maxDistanceSq, m_targets[i], out newDistSq);

            if (newResult)
            {
                result = newResult;
                maxDistanceSq = newDistSq;
            }
        }

        distance = Mathf.Sqrt(maxDistanceSq);
        return result;
    }

    private static Targetable FindClosestTargetRange(Vector3 center, float maxDistanceSq, List<Targetable> targets, out float distSq)
    {
        distSq = maxDistanceSq;
        Targetable result = null;

        for (int i = 0; i < targets.Count; ++i)
        {
            var newDistSq = (targets[i].transform.position - center).sqrMagnitude;
            if (newDistSq < distSq)
            {
                distSq = newDistSq;
                result = targets[i];
            }
        }

        return result;
    }

    private void Reset()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        foreach (var damager in GetComponentsInChildren<ColliderDamager>())
        {
            damager.team = team;
        }

        foreach (var health in GetComponentsInChildren<HealthHolder>())
        {
            health.team = team;
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < m_teams.Length; ++i)
        {
            if (team.HasFlag(m_teams[i]))
            {
                m_targets[i].Add(this);
                if (isMain)
                    m_mainTargets[i].Add(this);
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < m_teams.Length; ++i)
        {
            if (team.HasFlag(m_teams[i]))
            {
                m_targets[i].Remove(this);
                if (isMain)
                    m_mainTargets[i].Remove(this);
            }
        }
    }
}
