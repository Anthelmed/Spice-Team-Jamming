using DefaultNamespace;
using UnityEngine;

namespace _3C.Player
{
    public class OnAttackAnimationEndBehavior : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GameplayData.s_PlayerStateHandler.MeleeAttack.OnAttackAnimationEnded();
        }
    }
}