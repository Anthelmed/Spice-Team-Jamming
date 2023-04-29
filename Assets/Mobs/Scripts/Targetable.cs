using DefaultNamespace.HealthSystem.Damageable;
using DefaultNamespace.HealthSystem.Damager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;

[SelectionBase]
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

    [FormerlySerializedAs("team")]
    [SerializeField] private Team m_team = Team.Nature;

    public Team CurrentTeam
    {
        get => m_team;
        set
        {
            m_team = value;
            OnValidate();
        }
    }
    
    public enum Priority
    {
        Low = 0,
        Medium,
        High
    }

    public Priority priority = Priority.Medium;
    public float radius = 0.5f;

    private static List<Targetable> m_allTargets = new List<Targetable>();

    // Reuse this one to avoid allocations
    private static List<Targetable> m_reusableListForQueries = new List<Targetable>();

    public static List<Targetable> QueryTargets(Vector3 center, float maxDistance, Team teams, 
        Priority minPriority = Priority.Low, Priority maxPriority = Priority.High)
    {
        m_reusableListForQueries.Clear();
        var maxDistanceSq = maxDistance * maxDistance;

        for (int i = 0; i < m_allTargets.Count; ++i)
        {
            var target = m_allTargets[i];
            if ((target.m_team & teams) != 0 &&
                target.priority >= minPriority && target.priority <= maxPriority &&
                (target.transform.position - center).sqrMagnitude <= maxDistanceSq)
            {
                m_reusableListForQueries.Add(target);
            }
        }

        return m_reusableListForQueries;
    }

    public static Targetable QueryClosestTarget(Vector3 center, float maxDistance, out float distance, Team teams,
        Priority minPriority = Priority.Low, Priority maxPriority = Priority.High)
    {
        Targetable result = null;

        var maxDistanceSq = maxDistance * maxDistance;
        float newDistSq;

        for (int i = 0; i < m_allTargets.Count; ++i)
        {
            var target = m_allTargets[i];
            if ((target.m_team & teams) != 0 &&
                target.priority >= minPriority && target.priority <= maxPriority)
            {
                newDistSq = (target.transform.position - center).sqrMagnitude;
                if (newDistSq < maxDistanceSq)
                {
                    result = target;
                    maxDistanceSq = newDistSq; ;
                }
            }
        }

        distance = Mathf.Sqrt(maxDistanceSq);
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
            damager.team = m_team;
        }

        foreach (var health in GetComponentsInChildren<HealthHolder>())
        {
            health.team = m_team;
        }
    }

    private void OnEnable()
    {
        m_allTargets.Add(this);
    }

    private void OnDisable()
    {
        m_allTargets.Remove(this);
    }
}
