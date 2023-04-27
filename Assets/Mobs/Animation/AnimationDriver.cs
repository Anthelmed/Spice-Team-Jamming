using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationDriver : MonoBehaviour
{
    public abstract void SetSpeed(float speed);
    public abstract void TriggerAttack();
    public abstract void TriggerHit();
    public abstract void TriggerDeath();
    public abstract bool HasAnimationFinished();
}
