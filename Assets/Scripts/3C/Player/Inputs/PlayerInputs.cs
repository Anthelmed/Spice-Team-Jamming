using UnityEngine;
using Utilities.DataStructure;

namespace _3C.Player
{
    public enum InputType
    {
        MovementPerformed,
        MovementCanceled,
        MeleeAttackPerformed,
        DashPerformed,
        HeldMeleeAttackPerformed,
        AimPerformed,
        AimCanceled,
        RangeAttackPerformed,
        RangeAttackCanceled,
    }
    
    public class PlayerInputs
    {
        public Vector2 Movement  = Vector2.zero;
        public Vector2 AimDirection  = Vector2.zero;

        public DumbFixedSizeStack<InputType> InputStack;
        public bool IsUsingCursorPositionForAim = true;
    }
}