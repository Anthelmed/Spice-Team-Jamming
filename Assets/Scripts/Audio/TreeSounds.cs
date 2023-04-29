using UnityEngine;

namespace DefaultNamespace.Audio
{
    public class TreeSounds : MonoBehaviour
    {
        [SerializeField] string burnsound;
        [SerializeField] string freezeSound;
        [SerializeField] string natureSound;
        [SerializeField] string takeFireDamage;
        [SerializeField] string takeIceDamage;
        [SerializeField] string restore;


        AudioManager m_AudioManager;

        [SerializeField] Vegetation vegetation;

        

        private void Start()
        {
            if (AudioManager.instance != null) m_AudioManager = AudioManager.instance;
            if (vegetation != null) vegetation.OnTreeStateChanged += HandleTreeStateChange;

        }

        void HandleTreeStateChange(Targetable.Team team)
        {
            switch (team)
            {
                case Targetable.Team.Nature:
                    {
                   //    PlayNatureSound();
                    }
                    break;
                case Targetable.Team.Fire:
                    {
                    //    PlayFireDamageSound();
                    }
                    break;
                case Targetable.Team.Ice:
                    {
                    //    PlayIceDamageSound(); 
                    }
                    break;
                case Targetable.Team.Wizard:
                    break;
                default:
                    break;
            }
        }

        public void PlayBurnSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(burnsound, SFXCategory.player, 0.1f, 0.1f);
        }
        public void PlayFreezeSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(freezeSound, SFXCategory.player, 0.1f, 0.1f);
        }
        public void PlayNatureSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(natureSound, SFXCategory.player, 0.1f, 0.1f);
        }
        public void PlayFireDamageSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(takeFireDamage, SFXCategory.player, 0.1f, 0.1f);
        }
        public void PlayIceDamageSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(takeIceDamage, SFXCategory.player, 0.1f, 0.1f);
        }
        public void PlayRestoreSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(restore, SFXCategory.player, 0.1f, 0.1f);
        }


    }
}