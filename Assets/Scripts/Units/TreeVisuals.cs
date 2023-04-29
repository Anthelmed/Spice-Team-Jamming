using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

namespace Units
{
    public class TreeVisuals : UnitVisuals
    {
        public Renderer trunkRenderer;
        public Renderer leavesRenderer;
        public Transform leavesTransform;
        public Transform[] iceTransforms;
        public ParticleSystem fireParticles;

        private void Start()
        {
            foreach (var iceCube in iceTransforms)
            {
                iceCube.localScale = Vector3.zero;

            }
            trunkRenderer.material.SetFloat("_TrunkState", 0);
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_unit)
                m_unit.onHeal.AddListener(OnHeal);
        }

        private void OnHeal(float amount)
        {
            switch (m_unit.Team)
            {
                case Faction.Nature:
                    SetNatureState();
                    break;
                case Faction.Fire:
                    SetBurntState();
                    break;
                case Faction.Ice:
                    SetFrozenState();
                    break;
            }
        }

        [Button]
        public void SetNatureState()
        {
            StopAllCoroutines();
            StartCoroutine(NatureCoroutine());
        }

        [Button]
        public void SetFrozenState()
        {
            StopAllCoroutines();
            StartCoroutine(FrozenCoroutine());
        }
        [Button]
        public void SetBurntState()
        {
            StopAllCoroutines();
            StartCoroutine(BurntCoroutine());
        }

        IEnumerator BurntCoroutine()
        {
            float time = 0;

            leavesRenderer.material.SetFloat("_IceFireSwitch", 1);
            fireParticles.Play();
            float startState = trunkRenderer.material.GetFloat("_TrunkState");

            leavesTransform.DOKill();
            leavesTransform.DOScale(new Vector3(0, 1, 0), 0.4f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.2f);
            foreach (var iceCube in iceTransforms)
            {
                iceCube.DOKill();
                iceCube.DOScale(Vector3.zero, Random.Range(0.4f, 0.6f)).SetDelay(Random.Range(0f, 0.8f)).SetEase(Ease.InBack);

            }
            while (time < 1)
            {
                trunkRenderer.material.SetFloat("_TrunkState", Mathf.Lerp(startState, 1, time));
                //    leavesMaterial.SetFloat("_Dissolve", Mathf.Lerp(0, 1, time));
                time += Time.deltaTime / 2;
                yield return null;
            }
        }
        IEnumerator FrozenCoroutine()
        {
            fireParticles.Stop();
            float time = 0;
            leavesRenderer.material.SetFloat("_IceFireSwitch", 0);
            float startState = trunkRenderer.material.GetFloat("_TrunkState");

            leavesTransform.DOKill();
            leavesTransform.DOScale(new Vector3(0, 1, 0), 0.4f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.2f);
            foreach (var iceCube in iceTransforms)
            {
                iceCube.DOKill();
                iceCube.DOScale(Vector3.one, Random.Range(0.4f, 0.6f)).SetDelay(Random.Range(0f, 0.8f)).SetEase(Ease.OutBack);

            }

            while (time < 1)
            {
                trunkRenderer.material.SetFloat("_TrunkState", Mathf.Lerp(startState, -1, time));
                //  leavesMaterial.SetFloat("_Dissolve", Mathf.Lerp(0, 1, time));
                time += Time.deltaTime / 2;
                yield return null;
            }
        }
        IEnumerator NatureCoroutine()
        {
            float time = 0;
            fireParticles.Stop();
            float startState = trunkRenderer.material.GetFloat("_TrunkState");
            leavesTransform.DOKill();
            leavesRenderer.material.SetFloat("_Dissolve", 0);
            leavesTransform.localScale = new Vector3(0, 1, 0);
            leavesTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetDelay(1.5f);
            foreach (var iceCube in iceTransforms)
            {
                iceCube.DOKill();
                iceCube.DOScale(Vector3.zero, Random.Range(0.4f, 0.6f)).SetDelay(Random.Range(0f, 0.8f)).SetEase(Ease.InBack);

            }
            while (time < 1f)
            {
                trunkRenderer.material.SetFloat("_TrunkState", Mathf.Lerp(startState, 0, time));
                time += Time.deltaTime / 2;
                yield return null;
            }
        }
    }
}