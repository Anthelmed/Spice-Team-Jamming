using System.Collections;
using UnityEngine;

namespace _3C.Player
{
    public interface IStateHandler
    {
        void OnStateEnded();
        void StartCoroutine(IEnumerator _coroutine);
        void StopAllCoroutines();
        GameObject gameObject { get; }
    }
}