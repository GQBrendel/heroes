using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventListener : StateMachineBehaviour
{
    public virtual HeroController Hero { get; set; }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        {
            if (Hero != null)
            {
                Debug.Log("Finished Attack");
                Hero.FinishedAttack();
            }

        }
    }
}
