using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorielController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool bHasToBeDestroy = false;//Les animations appelle une méthode dès qu'elles finissent, cette variable permet de détruire l'objet à la fin de l'animation en cours
    private Tutoriel tutoriel;

    public void PlayAnimation(Tutoriel tuto)
    {
        if (bHasToBeDestroy)
        {
            Destroy(gameObject);
            return;
        }

        tutoriel = tuto;

        switch (tutoriel)
        {
            case Tutoriel.Clic_Droit:
                {
                    animator.Play("tuto_clic_droit");
                }
                break;
            case Tutoriel.Clic_Gauche:
                {
                    animator.Play("tuto_clic_gauche");
                }
                break;
            case Tutoriel.Hold_Droit:
                {
                    animator.Play("tuto_hold_droit");
                }
                break;
            case Tutoriel.Hold_Gauche:
                {
                    animator.Play("tuto_hold_droit");
                }
                break;
            case Tutoriel.Drag:
                {
                    animator.Play("tuto_drag");
                }
                break;
            case Tutoriel.Launch:
                {
                    animator.Play("tuto_launch");
                }
                break;
            case Tutoriel.Move:
                {
                    animator.Play("tuto_move");
                }
                break;
            case Tutoriel.Check:
                {
                    animator.Play("tuto_check");
                }
                break;
        }
    }

    //Appelé par les animations du prefab
    public void animationCallback_ReplayAnimation()
    {
        PlayAnimation(tutoriel);
    }

    public void StopTutoriel()
    {
        bHasToBeDestroy = true;
    }
}
