using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : CommonController
{
    private enum State
    {
        Initial,
        Click
    }

    [SerializeField] private GameObject goFilGroupe;
    [SerializeField] private GameObject goAnimationGroupe;
    [SerializeField] private GameObject goAraignee;

    private State currentState = State.Initial;
    private GameObject TutoClicGauche;

    protected override void ChildStart()
    {
        StartCinematique(Cinematiques.Credits_Credits);
    }

    protected override void ChildUpdate()
    {
        if(currentState == State.Click)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Début change scene
            }
        }
    }

    //Gère les interactions de ce chapitre
    protected override void ChapterZoomOnObject(InteractableObject interactionObject)
    {
        interactionObject.EnterZoom();
    }

    //Gère l'arrêt des interactions avec un objet du chapitre
    protected override void ChapterLeaveZoomOnObject(InteractableObject interactionObject)
    {
        interactionObject.LeaveZoom();
    }

    protected override void StartChapterCinematique(Cinematiques cinematique)
    {
        if (movingBody != null) movingBody.SetActive(false);

        switch (cinematique)
        {
            case Cinematiques.Credits_Credits:
                {
                    StartCinematiqueMenu();
                }
                break;
        }

        if (movingBody != null) movingBody.SetActive(true);
    }

    #region Cinematique Credit

    private void StartCinematiqueMenu()
    {
        StartCoroutine(coroutine_CinematiqueMenu());
    }

    private IEnumerator coroutine_CinematiqueMenu()
    {
        TutoClicGauche = PlayTuto(Tutoriel.Clic_Gauche, Vector3.zero);

        yield return new WaitForSeconds(2f);

        StopCinematiqueMenu();
    }

    private void StopCinematiqueMenu()
    {
        currentState = State.Click;
    }

    #endregion
}
