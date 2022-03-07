using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller permettant simplement de récupérer les évènements de l'animator de l'objet
public class Perso_Animation_1_Controller : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool bAnimationMainFinished;
    private bool bAnimationMainReverseFinished;

    public void animationCallback_MainAnimation()
    {
        bAnimationMainFinished = true;
    }

    public void animationCallback_MainAnimationReverse()
    {
        bAnimationMainReverseFinished = true;
    }


    public void animation_Main()
    {
        animator.SetTrigger("MainAnimation");
    }

    public bool animation_MainIsFinished()
    {
        return bAnimationMainFinished;
    }

    public void animation_MainReverse()
    {
        animator.SetTrigger("MainAnimationReverse");
    }

    public bool animation_MainReverseIsFinished()
    {
        return bAnimationMainReverseFinished;
    }

    public void ResetAnimatorTrigger()
    {
        animator.ResetTrigger("MainAnimation");
        animator.ResetTrigger("MainAnimationReverse");
    }
}
