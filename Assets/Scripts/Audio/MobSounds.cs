using UnityEngine;

namespace DefaultNamespace.Audio
{
    public class MobSounds : MonoBehaviour
    {
        [SerializeField] string attackOneSound = "";
        [SerializeField] string attackTwoSound = "";
        [SerializeField] string rangedSound = "";
        [SerializeField] string deathSound = "";
        [SerializeField] string damageSound = "";
        [SerializeField] string healSound = "";


        AudioManager m_AudioManager;

        private void Start()
        {
            if (AudioManager.instance != null) m_AudioManager = AudioManager.instance;

        }
        public void PlayAttackSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlayShuffledSound(attackOneSound, SFXCategory.mob, 0.05f, 0.05f);
        }

        public void PlayDashSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(attackTwoSound, SFXCategory.mob, 0.1f, 0.1f);
        }

        public void PlayRangeSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(rangedSound, SFXCategory.mob, 0.05f, 0.05f);
        }
        public void PlayDamageSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlayShuffledSound(damageSound, SFXCategory.mob, 0.1f, 0.1f);
        }
        public void PlayDeathSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(deathSound, SFXCategory.mob, 0, 0);
        }
        public void PlayHealSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(healSound, SFXCategory.mob, 0, 0);
        }


    }
}