using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_1Controller : CommonController
{

    [SerializeField] private SpriteRenderer lightBulb;
    [SerializeField] private GameObject lightRoom;
    [SerializeField] private Sprite sprt_lightOn;
    [SerializeField] private Sprite sprt_lightOff;
    [SerializeField] private GameObject MenuLeave;
    [SerializeField] private Animator MenuLeaveAnimator;
    [SerializeField] private SpriteRenderer sptrRdr_EcranZoom;
    [SerializeField] private InteractableObject interObj_Ecran;
    [SerializeField] private Sprite sprt_EcranZoomOn;
    [SerializeField] private Sprite sprt_EcranZoomOff;
    [SerializeField] private Sprite sprt_Ecran1On;
    [SerializeField] private Sprite sprt_Ecran2On;
    [SerializeField] private Sprite sprt_Ecran1Off;
    [SerializeField] private Sprite sprt_Ecran2Off;
    [SerializeField] private bool bEcranOn = true;
    [SerializeField] private Perso_Animation_1_Controller Perso_Animation_1;

    private bool bLightOn = false;

    //Tutos
    private GameObject goTutoMove = null;
    private bool bTutoZoomedDisplayed = false;

    private GameObject goTutoEcran;
    private GameObject goTutoDezoomEcran;
    private GameObject goTutoCalendrier;
    private GameObject goTutoDezoomCalendrier;
    private GameObject goTutoInterrupteur;
    private GameObject goTutoPorte;

    protected override void ChildStart()
    {
        SetInteractionsActives(false);

        StartCinematique(Cinematiques.Chapitre1_Debut);
    }

    protected override void ChildUpdate()
    {
        if(!bTutoZoomedDisplayed && movingBody.GetMovingTime() > 0.5f)
        {
            StopTuto(goTutoMove);

            //Tuto sur les objets interactables
            goTutoCalendrier = PlayTuto(Tutoriel.Clic_Gauche, new Vector3(4.76f, 8.49f, 0));
            goTutoEcran = PlayTuto(Tutoriel.Clic_Gauche, new Vector3(-6.59f, 8.93f, 0));
            goTutoInterrupteur = PlayTuto(Tutoriel.Clic_Gauche, new Vector3(12.44f, 9.8f, 0));
            goTutoPorte = PlayTuto(Tutoriel.Clic_Gauche, new Vector3(14.5f, 11.76f, 0));

            bTutoZoomedDisplayed = true;
        }
    }

    protected override void ChapterInteraction(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Room_LightToggle:
                {
                    if (bLightOn)
                    {
                        lightBulb.sprite = sprt_lightOff;
                        lightRoom.SetActive(false);
                        bLightOn = false;
                    }
                    else
                    {
                        lightBulb.sprite = sprt_lightOn;
                        lightRoom.SetActive(true);
                        bLightOn = true;
                    }

                    if(goTutoInterrupteur != null)
                    {
                        StopTuto(goTutoInterrupteur);
                        goTutoInterrupteur = null;
                    }
                }
                break;
            case InteractionType.Door:
                {
                    this.bMenuDisplayed = true;
                    MenuLeave.SetActive(true);

                    if (goTutoPorte != null)
                    {
                        StopTuto(goTutoPorte);
                        goTutoPorte = null;
                    }
                }
                break;
            case InteractionType.CancelMenu:
                {
                    this.bMenuDisplayed = false;

                    MenuLeaveAnimator.Play("LeaveDisappearAnimation");
                }
                break;
            case InteractionType.AcceptMenu:
                {
                    this.bMenuDisplayed = false;
                    MenuLeave.SetActive(false);
                    SmoothChangeScene(Scenes.Chapitre2);
                }
                break;
            case InteractionType.Ecran_Toggle:
                {
                    if (bEcranOn)
                    {
                        bEcranOn = false;
                        sptrRdr_EcranZoom.sprite = sprt_EcranZoomOff;
                        interObj_Ecran.ChangeSprite_1(sprt_Ecran1Off);
                        interObj_Ecran.ChangeSprite_2(sprt_Ecran2Off);
                    }
                    else
                    {
                        bEcranOn = true;
                        sptrRdr_EcranZoom.sprite = sprt_EcranZoomOn;
                        interObj_Ecran.ChangeSprite_1(sprt_Ecran1On);
                        interObj_Ecran.ChangeSprite_2(sprt_Ecran2On);
                    }
                }
                break;
        }
    }

    //Gère les interactions de ce chapitre
    protected override void ChapterZoomOnObject(InteractableObject interactionObject)
    {
        interactionObject.EnterZoom();

        switch (interactionObject.GetObjectType())
        {
            case ObjectType.Lamp:
                {
                    interactionObject.GetComponent<PolygonCollider2D>().enabled = false;

                    GameObject goLamp_Button = (GameObject)interactionObject.transform.Find("Lamp_Button").gameObject;
                    goLamp_Button.SetActive(true);
                }
                break;
            case ObjectType.Ecran:
                {
                    interactionObject.GetComponent<PolygonCollider2D>().enabled = false;

                    GameObject goEcran_Bouton = (GameObject)interactionObject.transform.Find("Ecran_Bouton").gameObject;
                    goEcran_Bouton.SetActive(true);

                    if (goTutoEcran != null)
                    {
                        StopTuto(goTutoEcran);
                        goTutoEcran = null;

                        goTutoDezoomEcran = PlayTuto(Tutoriel.Tuto_Clic_Droit_Back, new Vector3(-0.81f, 10.8f, 0));
                        goTutoDezoomEcran.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
                    }
                }
                break;
            case ObjectType.Calendrier:
                {
                    if (goTutoCalendrier != null)
                    {
                        StopTuto(goTutoCalendrier);
                        goTutoCalendrier = null;

                        goTutoDezoomCalendrier = PlayTuto(Tutoriel.Tuto_Clic_Droit_Back, new Vector3(10.43f, 10.66f, 0));
                        goTutoDezoomCalendrier.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
                    }
                }
                break;
        }
    }

    //Gère l'arrêt des interactions avec un objet du chapitre
    protected override void ChapterLeaveZoomOnObject(InteractableObject interactionObject)
    {
        interactionObject.LeaveZoom();

        switch (interactionObject.GetObjectType())
        {
            case ObjectType.Lamp:
                {
                    interactionObject.GetComponent<PolygonCollider2D>().enabled = true;

                    GameObject goLamp_Button = (GameObject)interactionObject.transform.Find("Lamp_Button").gameObject;
                    goLamp_Button.SetActive(false);
                }
                break;
            case ObjectType.Ecran:
                {
                    interactionObject.GetComponent<PolygonCollider2D>().enabled = true;

                    GameObject goEcran_Bouton = (GameObject)interactionObject.transform.Find("Ecran_Bouton").gameObject;
                    goEcran_Bouton.SetActive(false);

                    if (goTutoDezoomEcran != null)
                    {
                        StopTuto(goTutoDezoomEcran);
                        goTutoDezoomEcran = null;
                    }
                }
                break;
            case ObjectType.Calendrier:
                {
                    if (goTutoDezoomCalendrier != null)
                    {
                        StopTuto(goTutoDezoomCalendrier);
                        goTutoDezoomCalendrier = null;
                    }
                }
                break;
        }
    }

    protected override void StartChapterCinematique(Cinematiques cinematique)
    {
        switch(cinematique)
        {
            case Cinematiques.Chapitre1_Debut :
            {
                    StartCinematiqueDebut();
            }
            break;
        }
    }

    #region Cinematique début

    private void StartCinematiqueDebut()
    {
        StartCoroutine(coroutine_CinematiqueDebut());
    }

    private IEnumerator coroutine_CinematiqueDebut()
    {
        yield return new WaitForSeconds(3f);

        Perso_Animation_1.StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.MainAnimation);

        while (!Perso_Animation_1.IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.MainAnimation))
            yield return null;

        movingBody.gameObject.SetActive(true);
        Perso_Animation_1.gameObject.SetActive(false);

        SetInteractionsActives(true);

        goTutoMove = PlayTuto(Tutoriel.Move, new Vector3(5.81f, 5.57f, 0));

        StopCinematiqueDebut();
    }

    private void StopCinematiqueDebut()
    {
        StopCinematique();
    }

    #endregion
}
