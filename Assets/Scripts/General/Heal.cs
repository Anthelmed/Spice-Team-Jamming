using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Units;
using DG.Tweening;

namespace DefaultNamespace
{


    public class Heal : MonoBehaviour
    {
        [SerializeField] float healAmt = 10;
        [SerializeField] GameObject pickedUpVFX;
        [SerializeField] GameObject petals;
        [SerializeField] Material petalMat;
        [SerializeField] GameObject button;
        [SerializeField] Vector3 punchScaleAmount;
        [SerializeField] float outTime = 1f;

        private void Start()
        {
            var go = petals.gameObject;
            petalMat = go.GetComponent<MeshRenderer>().material;
        }
        private void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent(out Unit unit);
           if (unit != null && unit.UnitType == Unit.Type.Player)
            {
                unit.Heal(healAmt);
                SpawnPickupFX();

            }
        }

        [ContextMenu("spawn heal Stuff")]
        private void SpawnPickupFX()
        {
            if (AudioManager.instance != null) AudioManager.instance.PlaySingleClip("playerHeal", SFXCategory.player, 0, 0);


            if (pickedUpVFX != null) Instantiate(pickedUpVFX, transform.position, Quaternion.identity);
            petalMat.DOFloat(0f, "_cullVertices", (outTime * 0.75f)).SetEase(Ease.InOutQuad);

            button.transform.DOScale(0, (outTime * 0.8f)).SetEase(Ease.InElastic).OnComplete(() =>
            {

                Destroy(gameObject);

            });
        }
    }
}
