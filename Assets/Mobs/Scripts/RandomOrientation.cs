using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomOrientation : MonoBehaviour
{
    void Start()
    {
        transform.localRotation = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
    }
}
