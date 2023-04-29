using DefaultNamespace.HealthSystem.Damageable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthHolder))]
[RequireComponent(typeof(Targetable))]
public class Vegetation : MonoBehaviour
{
    [SerializeField] private TreeStateExample m_states;
    [SerializeField] private Targetable m_targetable;
    [SerializeField] private HealthHolder m_health;

    private void Reset()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (!m_states) m_states = GetComponentInChildren<TreeStateExample>();
        if (!m_health) m_health = GetComponent<HealthHolder>();
        if (!m_targetable) m_targetable = GetComponent<Targetable>();
    }

    public void OnDeath(Targetable.Team enemyTeam)
    {
        m_health.Heal(m_health.MaxHealth);

        if (!m_states) return;

        if (enemyTeam.HasFlag(Targetable.Team.Nature) ||
            enemyTeam.HasFlag(Targetable.Team.Wizard))
        {
            m_states.SetNatureState();
            m_targetable.CurrentTeam = Targetable.Team.Nature;
        }
        else if (enemyTeam.HasFlag(Targetable.Team.Fire))
        {
            m_states.SetBurntState();
            m_targetable.CurrentTeam = Targetable.Team.Fire;
        }
        else if (enemyTeam.HasFlag(Targetable.Team.Ice))
        {
            m_states.SetFrozenState();
            m_targetable.CurrentTeam = Targetable.Team.Ice;
        }
    }
}
