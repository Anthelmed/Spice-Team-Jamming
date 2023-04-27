using UnityEngine;

namespace DefaultNamespace.Audio
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource m_AttackSound;
        [SerializeField] private AudioSource m_DashSound;
        [SerializeField] private AudioSource m_RangeSound;

        public AudioSource AttackSound => m_AttackSound;

        public AudioSource DashSound => m_DashSound;

        public AudioSource RangeSound => m_RangeSound;

        public void PlayAttackSound()
        {
            PlaySoundIfPossible(m_AttackSound);
        }

        public void PlayDashSound()
        {
            PlaySoundIfPossible(m_DashSound);
        }

        public void PlayRangeSound()
        {
            PlaySoundIfPossible(m_RangeSound);
        }

        private void PlaySoundIfPossible(AudioSource _source)
        {
            if (_source != null)
            {
                _source.Play();
            }
        }
    }
}