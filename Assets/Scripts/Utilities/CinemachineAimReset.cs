using Cinemachine;
using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineAimReset : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<CinemachineVirtualCamera>().LookAt = null;
            Destroy(this);
        }
    }
}