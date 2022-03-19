using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//Controller permettant simplement de récupérer les évènements de l'animator de l'objet
public class Perso_Animation_1_Controller : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public enum AnimationsPerso
    {
        [Description("MainAnimation")] MainAnimation,
        [Description("MainAnimationReverse")] MainAnimation_Reverse,
        [Description("maxine_animation_chapitre_4")] Chapitre4_MaxineAnimation,
        [Description("Perso_animation_chapitre_4")] Chapitre4_PersoAnimation,
        [Description("maxine_animation_reverse_chapitre_4")] Chapitre4_MaxineAnimationReverse,
        [Description("Perso_animation_reverse_chapitre_4")] Chapitre4_PersoAnimationReverse,
        [Description("Perso_Animation_1_chapitre_2")] Chapitre2_PersoAnimation,
        [Description("perso_animation_chapitre_5")] Chapitre5_PersoAnimation,
        [Description("perso_animation_reverse_chapitre_5")] Chapitre5_PersoAnimationReverse,
        [Description("perso_animation_chapitre_8")] Chapitre8_PersoAnimation,
        [Description("maxine_animation_chapitre_8")] Chapitre8_MaxineAnimation
    }

    private Dictionary<AnimationsPerso, bool> dicAnimationsFinished = new Dictionary<AnimationsPerso, bool>();

    private void Start()
    {
        LoadAnimations();
    }

    private void LoadAnimations()
    {
        if (dicAnimationsFinished.Count > 0) return;

        foreach (AnimationsPerso anims in (AnimationsPerso[])Enum.GetValues(typeof(AnimationsPerso)))
        {
            dicAnimationsFinished.Add(anims, false);
        }
    }

    public void StartAnimation(AnimationsPerso anim)
    {
        LoadAnimations();

        animator.SetTrigger(CommonController.GetEnumDescription(anim));
    }

    public bool IsAnimationFinished(AnimationsPerso anim)
    {
        return dicAnimationsFinished[anim];
    }

    public void AnimationFinished(AnimationsPerso anim)
    {
        dicAnimationsFinished[anim] = true;
    }

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
        animator.ResetTrigger("perso_animation_chapitre_8");
        animator.ResetTrigger("maxine_animation_chapitre_8");
    }
}
