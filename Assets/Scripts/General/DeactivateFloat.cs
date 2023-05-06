using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateFloat : MonoBehaviour
{
    [SerializeField] bool deactivate;
    [SerializeField] bool destroy;
    [SerializeField] float delay = 1.2f;
    void Start()
    {
        Invoke("Deactivate", delay);
    }

   void Deactivate()
    {
        if (destroy) Destroy(gameObject);
        if (deactivate) gameObject.SetActive(false);
    }

}
