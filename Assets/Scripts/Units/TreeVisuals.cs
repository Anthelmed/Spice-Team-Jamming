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
        public Renderer[] additionalTrunks;

        private bool m_needsFire = false;

        private void Start()
        {
            foreach (var iceCube in iceTransforms)
            {
                iceCube.localScale = Vector3.zero;

            }
            foreach (var renderer in additionalTrunks)
            {
                renderer.material.SetFloat("_TrunkState", 0);
            }
            trunkRenderer.material.SetFloat("_TrunkState", 0);
        }

        private void OnEnable()
        {
            if (m_needsFire)
                fireParticles.Play();
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
        public void SetNatureState(bool immediate = false)
        {
            m_needsFire = false;
            if ((!m_unit || m_unit.Visible) && gameObject.activeInHierarchy && !immediate)
            {
                StopAllCoroutines();
                StartCoroutine(NatureCoroutine());
            }
            else
                NatureImmediate();
        }


        [Button]
        public void SetFrozenState()
        {
            m_needsFire = false;
            if ((!m_unit || m_unit.Visible) && gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(FrozenCoroutine());
            }
            else
                FrozenImmediate();
        }
        [Button]
        public void SetBurntState()
        {
            m_needsFire = true;
            if ((!m_unit || m_unit.Visible) && gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(BurntCoroutine());
            }
            else 
                BurnImmediate();
        }

        private void BurnImmediate()
        {
            leavesRenderer.material.SetFloat("_IceFireSwitch", 1);
            leavesTransform.localScale = new Vector3(0, 1, 0);
            foreach (var iceCube in iceTransforms)
            {
                iceCube.localScale = Vector3.zero;
            }
            foreach (var renderer in additionalTrunks)
            {
                renderer.material.SetFloat("_TrunkState", 1);
            }
            trunkRenderer.material.SetFloat("_TrunkState", 1);
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
                foreach (var renderer in additionalTrunks)
                {
                    renderer.material.SetFloat("_TrunkState", Mathf.Lerp(startState, 1, time));
                }
                //    leavesMaterial.SetFloat("_Dissolve", Mathf.Lerp(0, 1, time));
                time += Time.deltaTime / 2;
                yield return null;
            }
        }

        private void FrozenImmediate()
        {
            leavesRenderer.material.SetFloat("_IceFireSwitch", 0);
            leavesTransform.localScale = new Vector3(0, 1, 0);
            foreach (var iceCube in iceTransforms)
            {
                iceCube.localScale = Vector3.one;
            }
            trunkRenderer.material.SetFloat("_TrunkState", -1);
             foreach (var renderer in additionalTrunks)
                {
                    renderer.material.SetFloat("_TrunkState", -1);
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
                 foreach (var renderer in additionalTrunks)
                {
                    renderer.material.SetFloat("_TrunkState", Mathf.Lerp(startState, -1, time));
                }
                //  leavesMaterial.SetFloat("_Dissolve", Mathf.Lerp(0, 1, time));
                time += Time.deltaTime / 2;
                yield return null;
            }
        }

        private void NatureImmediate()
        {
            leavesRenderer.material.SetFloat("_Dissolve", 0);
            leavesTransform.localScale = Vector3.one;
            foreach (var iceCube in iceTransforms)
            {
                iceCube.localScale = Vector3.zero;
            }
            trunkRenderer.material.SetFloat("_TrunkState", 0);
             foreach (var renderer in additionalTrunks)
                {
                    renderer.material.SetFloat("_TrunkState", 0);
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
                 foreach (var renderer in additionalTrunks)
                {
                    renderer.material.SetFloat("_TrunkState", Mathf.Lerp(startState, 0, time));
                }
                time += Time.deltaTime / 2;
                yield return null;
            }
        }
    }
}