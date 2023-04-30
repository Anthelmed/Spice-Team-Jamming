using System.Collections;
using _3C.Player.Weapons;
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
        PlayerAiming PlayerAimingInstance { get; }

        void OnMovementStateChanged(bool _state);

        void OnAimingStateChanged(bool _state);
        void SetOrientationToUseMovement();

        void ChangeMovementSpeedModifier(float _modifier);
        
        T Instantiate<T>(T prefab)
            where T : Object;

        void ResetMovementSpeedModifier()
        {
            ChangeMovementSpeedModifier(1);
        }
    }
}