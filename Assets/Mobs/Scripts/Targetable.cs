using DefaultNamespace.HealthSystem.Damageable;
using DefaultNamespace.HealthSystem.Damager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public enum Team
    {
        Nature,
        Fire,
        Ice,
        Wizard,
    }

    public Team team = Team.Nature;
    public bool isMain;

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

    public static List<Targetable> QueryTargets(Vector3 center, float maxDistance, bool mainTargets, params Team[] teams)
    {
        if (mainTargets)
            QueryTargets_internal(center, maxDistance, m_mainTargets, teams);
        else
            QueryTargets_internal(center, maxDistance, m_targets, teams);

        return m_reusableListForQueries;
    }

    private static void QueryTargets_internal(Vector3 center, float maxDistance, List<Targetable>[] targets, Team[] teams)
    {
        m_reusableListForQueries.Clear();

        var maxDistanceSq = maxDistance * maxDistance;
        for (int i = 0; i < teams.Length; ++i)
        {
            FindTargetsInRange(center, maxDistanceSq, targets[(int)teams[i]]);
        }
    }

    private static void FindTargetsInRange(Vector3 center, float maxDistanceSq, List<Targetable> targets)
    {
        for (int i = 0; i < targets.Count; ++i)
        {
            if ((targets[i].transform.position - center).sqrMagnitude < maxDistanceSq)
                m_reusableListForQueries.Add(targets[i]);
        }
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
        m_targets[(int)team].Add(this);
        if (isMain)
            m_mainTargets[(int)team].Add(this);
    }

    private void OnDisable()
    {
        m_targets[(int)team].Remove(this);
        if (isMain)
            m_mainTargets[(int)team].Remove(this);
    }
}
