using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class DebugUnitHits : MonoBehaviour
{
    public void LogHit(float damage, Unit other, Vector3 hitPosition)
    {
        var otherName = other ? other.name : "<null>";
        Debug.Log($"{name}: {damage} points by {otherName} at {hitPosition}");
    }
}
