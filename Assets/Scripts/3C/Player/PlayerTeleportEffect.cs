using UnityEngine;
using DefaultNamespace.Audio;

public class PlayerTeleportEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_TeleportInVFX;
    [SerializeField] private ParticleSystem m_TeleportOutVFX;
    [SerializeField] PlayerSounds m_Sounds;
    public void PlayTeleportIn()
    {
        m_TeleportInVFX.Play();
        m_Sounds.PlayTeleportSound();
    }

    public void PlayTeleportOut()
    {
        m_TeleportOutVFX.Play();
        m_Sounds.PlayTeleportSound();
    }
}
