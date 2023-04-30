using DefaultNamespace;
using UnityEngine;

namespace _3C.Player
{
    public class PlayerHealthReactor : MonoBehaviour
    {
        public void OnHealthChanged(float _health)
        {
            PlayerStaticEvents.s_PlayerHealthChanged?.Invoke(_health);
        }
    }
}