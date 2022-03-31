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
    private List<GameObject> lstTutosZoom = new List<GameObject>();
    private bool bTutoZoomedDisplayed = false;

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
            lstTutosZoom.Add(PlayTuto(Tutoriel.Clic_Gauche, new Vector3(6.28f, 8.17f, 0)));
            lstTutosZoom.Add(PlayTuto(Tutoriel.Clic_Gauche, new Vector3(-6.59f, 8.93f, 0)));
            lstTutosZoom.Add(PlayTuto(Tutoriel.Clic_Gauche, new Vector3(12.89f, 9.52f, 0)));
            lstTutosZoom.Add(PlayTuto(Tutoriel.Clic_Gauche, new Vector3(14.77f, 11.60f, 0)));

            bTutoZoomedDisplayed = true;
        }
    }

    protected override void ChapterInteraction(InteractionType type)
    {
        if(lstTutosZoom.Count > 0)//Si on affiche les tuto Clic, on les enlève à la première interaction
        {
            foreach(GameObject go in lstTutosZoom)
            {
                go.GetComponent<TutorielController>().StopTutoriel();
            }

            lstTutosZoom.Clear();
        }

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
                }
                break;
            case InteractionType.Door:
                {
                    this.bMenuDisplayed = true;
                    MenuLeave.SetActive(true);
                }
                break;
            case InteractionType.CancelMenu:
                {
                    this.bMenuDisplayed = false;

                    MenuLeaveAnimator.Play("LeaveDisappearAnimation");
                    //MenuLeave.SetActive(false);
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

        if (lstTutosZoom.Count > 0)//Si on affiche les tuto Clic, on les enlève à la première interaction
        {
            foreach (GameObject go in lstTutosZoom)
            {
                go.GetComponent<TutorielController>().StopTutoriel();
            }

            lstTutosZoom.Clear();

            //Affichage du tuto pour dézoom
            GameObject goTuto1 = PlayTuto(Tutoriel.Tuto_Clic_Droit_Back, new Vector3(10.59f, 8.22f, 0));
            goTuto1.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

            GameObject goTuto2 = PlayTuto(Tutoriel.Tuto_Clic_Droit_Back, new Vector3(-0.89f, 8.43f, 0));
            goTuto2.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

            lstTutosZoom.Add(goTuto1);
            lstTutosZoom.Add(goTuto2);
        }

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
                }
                break;
        }
    }

    //Gère l'arrêt des interactions avec un objet du chapitre
    protected override void ChapterLeaveZoomOnObject(InteractableObject interactionObject)
    {
        interactionObject.LeaveZoom();

        if (lstTutosZoom.Count > 0)//Si on affiche les tuto Clic, on les enlève à la première interaction
        {
            foreach (GameObject go in lstTutosZoom)
            {
                go.GetComponent<TutorielController>().StopTutoriel();
            }

            lstTutosZoom.Clear();
        }

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
