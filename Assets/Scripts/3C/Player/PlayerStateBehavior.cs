using UnityEngine;

namespace _3C.Player
{
    public class PlayerStateBehavior : MonoBehaviour
    {
        public virtual void StartState()
        {
            enabled = true;
        }

        public virtual void StopState()
        {
            enabled = false;
        }
    }
}