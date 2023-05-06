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
                if (AudioManager.instance != null) AudioManager.instance.PlaySingleClip("playerHeal", SFXCategory.player, 0, 0);
                if (pickedUpVFX != null)
                {
                    var healFX = Instantiate(pickedUpVFX, other.transform.position, Quaternion.identity);
                 //   healFX.transform.SetParent(unit.transform);
                   
                }
      
                Destroy(gameObject);
            }
        }
    }
}
