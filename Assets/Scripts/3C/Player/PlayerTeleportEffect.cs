using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleportEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_TeleportInVFX;
    [SerializeField] private ParticleSystem m_TeleportOutVFX;
    public void PlayTeleportIn()
    {
        m_TeleportInVFX.Play();
    }

    public void PlayTeleportOut()
    {
        m_TeleportOutVFX.Play();
    }
}
