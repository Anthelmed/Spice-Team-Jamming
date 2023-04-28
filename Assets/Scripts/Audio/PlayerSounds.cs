using UnityEngine;

namespace DefaultNamespace.Audio
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] string attackOneSound = "PlayerAttackOne";
        [SerializeField] string dashSound = "playerDash";
        [SerializeField] string rangedSound = "";
        [SerializeField] string deathSound = "playerMagicalDeath";
        [SerializeField] string damageSound = "";
        [SerializeField] string healSound = "";


        AudioManager m_AudioManager;

        private void Start()
        {
            if (AudioManager.instance != null) m_AudioManager = AudioManager.instance;

        }
        public void PlayAttackSound()
        {
            m_AudioManager.PlayShuffledSound(attackOneSound, SFXCategory.player, 0.05f, 0.05f);
        }

        public void PlayDashSound()
        {
           m_AudioManager.PlaySingleClip(dashSound, SFXCategory.player, 0.1f, 0.1f);
        }

        public void PlayRangeSound()
        {
            m_AudioManager.PlaySingleClip(rangedSound, SFXCategory.player, 0.05f, 0.05f);
        }
        public void PlayDamageSound()
        {
            m_AudioManager.PlaySingleClip(damageSound, SFXCategory.player, 0.1f, 0.1f);
        }
        public void PlayDeathSound()
        {
            m_AudioManager.PlaySingleClip(deathSound, SFXCategory.player, 0, 0);
        }
        public void PlayHealSound()
        {
            m_AudioManager.PlaySingleClip(healSound, SFXCategory.player, 0, 0);
        }


    }
}