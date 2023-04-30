using System;
using System.Collections;
using DefaultNamespace;
using NaughtyAttributes;
using Runtime.Utilities;
using UnityEngine;

namespace _3C.Player
{
    [Serializable]
    public class DashBehavior : PlayerStateBehavior
    {
        // [AnimatorParam("m_Animator")]
        // [SerializeField] private int m_DashTriggerParam;
        // [SerializeField] private Animator m_Animator;
        [SerializeField] private float m_ManaCost;

        public override float BaseManaPoints => m_ManaCost;


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
        [SerializeField] private float m_PostDashDelay;

        [SerializeField] private ParticleSystem m_VFX;


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
            //m_Animator?.SetTrigger(m_DashTriggerParam);
        }

        public override void StopState()
        {
            m_StateHandler.StopAllCoroutines();
        }

        private IEnumerator c_Dashing()
        {
            m_StateHandler.OnMovementStateChanged(false);
            m_StateHandler.SetOrientationToUseMovement();
            Vector3 start = m_Transform.position;
            Vector3 end = m_Transform.position + m_Transform.forward * m_Distance;
            m_VFX.Play();

            if (m_HasOvershootFrame)
            {
                m_Transform.position -= m_Transform.forward * m_OvershootDistance;
                yield return null;
                if (m_HasOvershootPauseFrame)
                {
                    yield return null;
                }
            }
            
            m_StateHandler.PlayerSoundsInstance.PlayDashSound();

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
            if (m_PostDashDelay == 0)
            {
                PostDashCleaning();
            }
            else
            {
                m_StateHandler.StartCoroutine(c_PostDashDelay());
            }
        }

        private void PostDashCleaning()
        {
            m_StateHandler.OnMovementStateChanged(true);
            m_StateHandler.OnStateEnded();
        }

        private IEnumerator c_PostDashDelay()
        {
            yield return new WaitForSeconds(m_PostDashDelay);
            PostDashCleaning();
        }

        public override void OnValidate()
        {
            UpdateInnerValues();
        }
    }
}