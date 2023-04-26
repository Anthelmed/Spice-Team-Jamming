using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace _3C.Player
{
    [Serializable]
    public class DashBehavior : PlayerStateBehavior
    {
        [SerializeField] private bool m_HasOvershootFrame;
        [ShowIf("m_HasOvershootFrame")]
        [SerializeField] private float m_OvershootDistance;
        
        [ShowIf("m_HasOvershootFrame")]
        [SerializeField] private bool m_HasOvershootPauseFrame;
        
        
        [SerializeField] private bool m_HasBounceFrame;
        [ShowIf("m_HasBounceFrame")]
        [SerializeField] private float m_BounceDistance;
        
        [SerializeField] private float m_Duration;
        [SerializeField] private float m_Distance;
        [SerializeField] private AnimationCurve m_DistanceCurve;
        
        private float m_InverseDuration;
        private Transform m_Transform;

        protected override void Init(IStateHandler _stateHandler)
        {
            UpdateInnerValues();
            m_Transform = _stateHandler.gameObject.transform;
        }

        private void UpdateInnerValues()
        {
            m_InverseDuration = 1 / m_Duration;
        }

        public override void StartState()
        {
            m_StateHandler.StartCoroutine(c_Dashing());
        }

        public override void StopState()
        {
            m_StateHandler.StopAllCoroutines();
        }

        private IEnumerator c_Dashing()
        {
            Vector3 start = m_Transform.position;
            Vector3 end = m_Transform.position + m_Transform.forward * m_Distance;

            if (m_HasOvershootFrame)
            {
                m_Transform.position -= m_Transform.forward * m_OvershootDistance;
                yield return null;
                if (m_HasOvershootPauseFrame)
                {
                    yield return null;
                }
            }
            
            for (float time = 0; time < m_Duration; time += Time.deltaTime)
            {
                float ratio = time * m_InverseDuration;
                float remappedRatio = m_DistanceCurve.Evaluate(ratio);

                m_Transform.position = Vector3.LerpUnclamped(start, end, remappedRatio);
                yield return null;
            }
            
            if (m_HasBounceFrame)
            {
                m_Transform.position += m_Transform.forward * m_BounceDistance;
                yield return null;
            }
            
            m_Transform.position = end;
            m_StateHandler.OnStateEnded();
        }

        public override void OnValidate()
        {
            UpdateInnerValues();
        }
    }
}