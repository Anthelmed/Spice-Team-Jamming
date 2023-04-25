using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace _3C.Player
{
    public class DashBehavior : PlayerStateBehavior
    {
        [SerializeField] private bool m_HasOvershootFrame;
        [ShowIf("m_HasOvershootFrame")]
        [SerializeField] private float m_OvershootDistance;
        
        [SerializeField] private bool m_HasBounceFrame;
        [ShowIf("m_HasBounceFrame")]
        [SerializeField] private float m_BounceDistance;
        
        [SerializeField] private float m_Duration;
        [SerializeField] private float m_Distance;
        [SerializeField] private AnimationCurve m_DistanceCurve;
        
        private float m_InverseDuration;

        public Action OnDashEnded;

        private void Awake()
        {
            UpdateInnerValues();
        }

        private void UpdateInnerValues()
        {
            m_InverseDuration = 1 / m_Duration;
        }

        private void OnEnable()
        {
            StartCoroutine(c_Dashing());
        }

        private IEnumerator c_Dashing()
        {
            Vector3 start = transform.position;
            Vector3 end = transform.position + transform.forward * m_Distance;

            if (m_HasOvershootFrame)
            {
                transform.position -= transform.forward * m_OvershootDistance;
                yield return null;
            }
            
            for (float time = 0; time < m_Duration; time += Time.deltaTime)
            {
                float ratio = time * m_InverseDuration;
                float remappedRatio = m_DistanceCurve.Evaluate(ratio);

                transform.position = Vector3.LerpUnclamped(start, end, remappedRatio);
                yield return null;
            }
            
            if (m_HasBounceFrame)
            {
                transform.position += transform.forward * m_BounceDistance;
                yield return null;
            }
            
            transform.position = end;
            OnDashEnded?.Invoke();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnValidate()
        {
            UpdateInnerValues();
        }
    }
}