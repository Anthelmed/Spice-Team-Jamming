﻿using Units;
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
            if (vegetation != null) vegetation.onHeal.AddListener(HandleTreeStateChange);

        }

        void HandleTreeStateChange(float amount)
        {
            switch (vegetation.Team)
            {
                case Faction.Nature:
                    {
                    PlayNatureStatusSound();
                    }
                    break;
                case Faction.Fire:
                    {
                     PlayBurnStatusSound();
                    }
                    break;
                case Faction.Ice:
                    {
                     PlayFreezeStatusSound(); 
                    }
                    break;
            }
        }

        public void PlayBurnStatusSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(burnsound, SFXCategory.player, 0.1f, 0.1f);
        }
        public void PlayFreezeStatusSound()
        {
            if (m_AudioManager == null) return;
            m_AudioManager.PlaySingleClip(freezeSound, SFXCategory.player, 0.1f, 0.1f);
        }
        public void PlayNatureStatusSound()
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