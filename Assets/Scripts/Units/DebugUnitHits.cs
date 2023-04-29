using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class DebugUnitHits : MonoBehaviour
{
    public void LogHit(float damage, Unit other, Vector3 hitPosition)
    {
        var otherName = other ? other.name : "<null>";
        Debug.Log($"{name}: {damage} damage by {otherName} at {hitPosition}");
    }
    public void LogHeal(float damage)
    {
        Debug.Log($"{name}: {damage}");
    }

    public void LogDie()
    {
        Debug.Log($"{name}: It's dead!");
    }

    public void LogImmune()
    {
        Debug.Log($"{name}: Not affected by this");
    }
}
