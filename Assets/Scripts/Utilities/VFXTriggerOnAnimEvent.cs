using UnityEngine;

namespace Utilities
{
    public class VFXTriggerOnAnimEvent : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] m_ParticleSystems;

        public void TriggerParticles(int _index)
        {
            m_ParticleSystems[_index].Play();
        }
    }
}