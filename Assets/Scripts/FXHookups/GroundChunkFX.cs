using UnityEngine;


public class GroundChunkFX : MonoBehaviour
{
    [SerializeField] GameObject RTCam;

    public void ActivateChunkFX()
    {
        RTCam.SetActive(true);
    }
    public void DeactivateChunkFX()
    {
        RTCam.SetActive(false);
    }

}
