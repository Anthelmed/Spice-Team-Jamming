using System.Collections;
using DefaultNamespace.Audio;
using UnityEngine;

namespace _3C.Player
{
    public interface IStateHandler
    {
        void OnStateEnded();
        Coroutine StartCoroutine(IEnumerator _coroutine);
        void StopCoroutine(Coroutine _coroutine);
        void StopAllCoroutines();
        GameObject gameObject { get; }
        PlayerSounds PlayerSoundsInstance { get; }

        void OnMovementStateChanged(bool _state);
    }
}