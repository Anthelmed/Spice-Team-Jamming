using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Units;

namespace DefaultNamespace
{


    public class Heal : MonoBehaviour
    {
        [SerializeField] float healAmt = 10;
        [SerializeField] GameObject pickedUpVFX;
        private void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent(out Unit unit);
           if (unit != null && unit.UnitType == Unit.Type.Player)
            {
                unit.Heal(healAmt);
                if (pickedUpVFX != null)
                {
                    var healFX = Instantiate(pickedUpVFX);
                    healFX.transform.SetParent(unit.transform);
                }
      
                Destroy(gameObject);
            }
        }
    }
}
