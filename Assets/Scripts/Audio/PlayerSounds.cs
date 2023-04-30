using DefaultNamespace.HealthSystem.Damageable;
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


        HealthHolder playerHealth;
        AudioManager m_AudioManager;

        private void Start()
        {
            if (AudioManager.instance != null) m_AudioManager = AudioManager.instance;
          //  playerHealth = GetComponentInParent<HealthHolder>();
            
        }

       public void UpdateTensionLevel(float currentValue)
        {
 
        }

        public void PlayAttackSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlayShuffledSound(attackOneSound, SFXCategory.player, 0.05f, 0.05f);
        }

        public void PlayDashSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(dashSound, SFXCategory.player, 0.1f, 0.1f);
        }

        public void PlayRangeSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlayShuffledSound(rangedSound, SFXCategory.player, 0.1f, 0.05f);
        }
        public void PlayDamageSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlayShuffledSound(damageSound, SFXCategory.player, 0.15f, 0.05f);
            //if (playerHealth != null)
            //{
            //    print("player health found" + playerHealth.MaxHealth + "/" + playerHealth.);

            //}
        }
        public void PlayDeathSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(deathSound, SFXCategory.player, 0, 0);
        }
        public void PlayHealSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(healSound, SFXCategory.player, 0, 0);
        }


    }
}