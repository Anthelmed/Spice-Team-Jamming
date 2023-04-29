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
        AimCanceled
    }
    
    public class PlayerInputs
    {
        public Vector2 Movement  = Vector2.zero;

        public DumbFixedSizeStack<InputType> InputStack;
    }
}