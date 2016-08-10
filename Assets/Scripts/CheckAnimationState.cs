using UnityEngine;
using System.Collections;

public class CheckAnimationState : StateMachineBehaviour
{
    public string stateName;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponentInParent<IGameActor>().RecieveAnimationState(stateName, true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponentInParent<IGameActor>().RecieveAnimationState(stateName, false);
    }

}
