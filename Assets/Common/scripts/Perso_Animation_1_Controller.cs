using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller permettant simplement de récupérer les évènements de l'animator de l'objet
public class Perso_Animation_1_Controller : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool bAnimationMainFinished;
    private bool bAnimationMainFinishedChapitre2;
    private bool bAnimationMainReverseFinished;
    private bool bAnimationChapitre4Finished;
    private bool bAnimationMaxineChapitre4Finished;
    private bool bAnimationReverseChapitre4Finished;
    private bool bAnimationMaxineReverseChapitre4Finished;
    private bool bAnimationPersoChapitre5Finished;
    private bool bAnimationPersoReverseChapitre5Finished;

    #region Animation Callback

    public void animationCallback_MainAnimation()
    {
        bAnimationMainFinished = true;
    }

    public void animationCallback_MainChapitre2Animation()
    {
        bAnimationMainFinishedChapitre2 = true;
    }

    public void animationCallback_MainAnimationReverse()
    {
        bAnimationMainReverseFinished = true;
    }

    public void animationCallback_AnimationPersoChapitre4()
    {
        bAnimationChapitre4Finished = true;
    }

    public void animationCallback_AnimationMaxineChapitre4()
    {
        bAnimationMaxineChapitre4Finished = true;
    }

    public void animationCallback_AnimationPersoReverseChapitre4()
    {
        bAnimationReverseChapitre4Finished = true;
    }

    public void animationCallback_AnimationMaxineReverseChapitre4()
    {
        bAnimationMaxineReverseChapitre4Finished = true;
    }

    public void animationCallback_AnimationPersoChapitre5()
    {
        bAnimationPersoChapitre5Finished = true;
    }

    public void animationCallback_AnimationPersoReverseChapitre5()
    {
        bAnimationPersoReverseChapitre5Finished = true;
    }

    #endregion

    #region Animation Main

    public void animation_Main()
    {
        animator.SetTrigger("MainAnimation");
    }

    public bool animation_MainIsFinished()
    {
        return bAnimationMainFinished;
    }

    #endregion

    #region Animation Main Chapitre 2

    public void animation_Main_Chapitre_2()
    {
        animator.SetTrigger("Perso_Animation_1_chapitre_2");
    }

    public bool animation_MainChapitre2IsFinished()
    {
        return bAnimationMainFinishedChapitre2;
    }

    #endregion

    #region Animation MainReverse

    public void animation_MainReverse()
    {
        animator.SetTrigger("MainAnimationReverse");
    }

    public bool animation_MainReverseIsFinished()
    {
        return bAnimationMainReverseFinished;
    }

    #endregion

    #region Animation PersoChapitre4

    public void animation_PersoChapitre4()
    {
        animator.SetTrigger("Perso_animation_chapitre_4");
    }

    public bool animation_PersoChapitre4Finished()
    {
        return bAnimationChapitre4Finished;
    }

    #endregion

    #region Animation PersoReverseChapitre4

    public void animation_PersoReverseChapitre4()
    {
        animator.SetTrigger("Perso_animation_reverse_chapitre_4");
    }

    public bool animation_PersoReverseChapitre4Finished()
    {
        return bAnimationReverseChapitre4Finished;
    }

    #endregion

    #region Animation MaxineChapitre4

    public void animation_MaxineChapitre4()
    {
        animator.SetTrigger("maxine_animation_chapitre_4");
    }

    public bool animation_MaxineChapitre4Finished()
    {
        return bAnimationMaxineChapitre4Finished;
    }

    #endregion

    #region Animation MaxineReverseChapitre4

    public void animation_MaxineReverseChapitre4()
    {
        animator.SetTrigger("maxine_animation_reverse_chapitre_4");
    }

    public bool animation_MaxineReverseChapitre4Finished()
    {
        return bAnimationMaxineReverseChapitre4Finished;
    }

    #endregion

    #region Animation Perso Chapitre 5

    public void animation_PersoChapitre5()
    {
        animator.SetTrigger("perso_animation_chapitre_5");
    }

    public bool animation_PersoChapitre5Finished()
    {
        return bAnimationPersoChapitre5Finished;
    }

    #endregion

    #region Animation Perso Reverse Chapitre 5

    public void animation_PersoReverseChapitre5()
    {
        animator.SetTrigger("perso_animation_reverse_chapitre_5");
    }

    public bool animation_PersoReverseChapitre5Finished()
    {
        return bAnimationPersoReverseChapitre5Finished;
    }

    #endregion

    public void ResetAnimatorTrigger()
    {
        animator.ResetTrigger("MainAnimation");
        animator.ResetTrigger("MainAnimationReverse");
        animator.ResetTrigger("maxine_animation_chapitre_4");
        animator.ResetTrigger("Perso_animation_chapitre_4");
        animator.ResetTrigger("maxine_animation_reverse_chapitre_4");
        animator.ResetTrigger("Perso_animation_reverse_chapitre_4");
        animator.ResetTrigger("Perso_Animation_1_chapitre_2");
        animator.ResetTrigger("perso_animation_chapitre_5");
        animator.ResetTrigger("perso_animation_reverse_chapitre_5");
    }
}
