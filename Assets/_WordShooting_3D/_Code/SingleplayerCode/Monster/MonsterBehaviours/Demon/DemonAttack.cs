using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonAttack : StateMachineBehaviour
{
    [SerializeField] private float timer;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer >= 3)
        {
            timer = 0;

            int attackType = Random.Range(1, 4);
            // int attackType = 1;
            switch (attackType)
            {
                case 1:
                    animator.SetTrigger("attack1");
                    break;
                case 2:
                    animator.SetTrigger("attack2");
                    break;
                case 3:
                    animator.SetTrigger("attack3");
                    break;
            }
        }
    }
    // // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {

    // }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
