using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_2Controller : CommonController
{
    public enum ChapitreState { Initial = 0, MaxineArrive = 1, MaxineLeft = 6, AnimationJournee = 2, FinJournee = 3, AnimationBoucle = 4, FinBoucle = 5 }

    private ChapitreState currentState = ChapitreState.Initial;

    [SerializeField] private MovingBody movingBody_2;
    [SerializeField] private GameObject MenuLeave;
    [SerializeField] private Animator MenuLeaveAnimator;
    [SerializeField] private HorlogeController Horloge;
    [SerializeField] private GameObject goPorteInteractable;
    [SerializeField] private GameObject goPorteSprite;

    //Cinématique Boucle
    [SerializeField] private GameObject goBoucle;
    [SerializeField] private GameObject chap_1_1;
    [SerializeField] private GameObject chap_2_1;
    [SerializeField] private GameObject chap_1_2;
    [SerializeField] private GameObject chap_2_2;
    [SerializeField] private float boucleSpeed = 10f;
    [SerializeField] private GameObject calendrierBoucle;
    [SerializeField] private GameObject goPerso_Animation_1;
    [SerializeField] private Animator Perso_Animation_1_Animator;
    private int nIndexBoucle = 1;
    private float fXStart = -15.5f;
    private float fYStart = 13.75f;

    //Cinématique Journée
    private Vector3 vInitialCameraPosition;
    private float fInitialCameraSize;
    private float fInitialHorlogeSpeed;

    protected override void ChildStart()
    {
        currentState = ChapitreState.Initial;
        movingBody_2.SetActive(false);
    }

    protected override void ChildUpdate()
    {
        if(currentState == ChapitreState.Initial)
        {
            if(movingBody.transform.position.x > 3f)//Condition de début de la cinématique
            {
                //StartCinematiqueMaxineArrive();
                StartCinematique(Cinematiques.Chapitre2_ArriveeMaxine);
            }
        }
    }

    protected override void ChapterInteraction(InteractionType type)
    {
        switch (type)
        {
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
                }
                break;
            case InteractionType.AcceptMenu:
                {
                    this.bMenuDisplayed = false;
                    MenuLeave.SetActive(false);

                    StartCinematique(Cinematiques.Chapitre2_Boucle);
                }
                break;
            case InteractionType.Ecran_Toggle:
                {

                }
                break;
            case InteractionType.Ecran_Interaction:
                {
                    StartCinematique(Cinematiques.Chapitre2_Journee);
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

        switch (interactionObject.GetObjectType())
        {
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
        switch (cinematique)
        {
            case Cinematiques.Chapitre2_ArriveeMaxine:
                {
                    StartCinematiqueMaxineArrive();
                }
                break;
            case Cinematiques.Chapitre2_Journee:
                {
                    StartCinematiqueJournee();
                }
                break;
            case Cinematiques.Chapitre2_Boucle:
                {
                    StartCinematiqueBoucle();
                }
                break;
        }
    }

    #region Cinématique Arrivée maxine
    
    public void StartCinematiqueMaxineArrive()
    {
        currentState = ChapitreState.MaxineArrive;

        StartCoroutine(coroutine_CinematiqueMaxineArrive());
    }

    public IEnumerator coroutine_CinematiqueMaxineArrive()
    {
        
        //movingBody.StopMoving();
        //movingBody.SetActive(false);
        movingBody_2.GoToPosition(new Vector3(7.2f, -3.06f, 0), 1.5f);

        MusicController.GetInstance().ChangeClip(MusicController.Clips.Maxine);

        while (movingBody_2.IsGoingToPosition())
            yield return null;

        //Discussion
        movingBody_2.SpeakRandom(4);
        yield return new WaitForSeconds(1f);
        movingBody.SpeakRandom(4);

        while (movingBody_2.IsSpeaking() || movingBody.IsSpeaking())
            yield return null;

        //Départ de Maxine
        movingBody_2.GoToPosition(new Vector3(23.57f, -3.06f, 0), 1.5f);

        //On attend qu'elle sorte de l'écran
        while (movingBody_2.IsGoingToPosition())
            yield return null;

        MusicController.GetInstance().ChangeClip(MusicController.Clips.Perso);

        //On réactive le joueur
        movingBody.SetActive(true);
        StopCinematiqueMaxineArrived();
    }

    private void StopCinematiqueMaxineArrived()
    {
        currentState = ChapitreState.MaxineLeft;
    }

    #endregion

    #region Cinématique Animation journée

    private void StartCinematiqueJournee()
    {
        currentState = ChapitreState.AnimationJournee;

        StartCoroutine(coroutine_CinematiqueJournee());
    }

    private IEnumerator coroutine_CinematiqueJournee()
    {
        movingBody.GoToPosition(new Vector3(14.89f, -3.06f, 0));

        while (movingBody.IsGoingToPosition())
            yield return null;

        Perso_Animation_1_Controller persoCtrl = goPerso_Animation_1.GetComponent<Perso_Animation_1_Controller>();
        movingBody.gameObject.SetActive(false);
        goPerso_Animation_1.SetActive(true);
        persoCtrl.StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.MainAnimation_Reverse);

        while (!persoCtrl.IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.MainAnimation_Reverse))
            yield return null;

        //Mise en place de la vue de l'animation
        vInitialCameraPosition = cam.transform.position;
        fInitialCameraSize = cam.orthographicSize;
        fInitialHorlogeSpeed = Horloge.GetSpeed();

        MoveCamera(new Vector3(8.5f, 12f, 0), 6f, true);
        Horloge.SetSpeed(1000);

        while (Horloge.GetHeureEulerAngle() <= 180 || Horloge.GetHeureEulerAngle() >= 185)
            yield return null;

        Horloge.SetSpeed(fInitialHorlogeSpeed);
        MoveCamera(vInitialCameraPosition, fInitialCameraSize, true);

        goPorteInteractable.SetActive(true);
        goPorteSprite.SetActive(false);

        persoCtrl.StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.Chapitre2_PersoAnimation);

        while (!persoCtrl.IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.Chapitre2_PersoAnimation))
            yield return null;

        movingBody.gameObject.SetActive(true);
        goPerso_Animation_1.SetActive(false);

        StopCinematiqueJournee();

        yield break;
    }

    private void StopCinematiqueJournee()
    {
        currentState = ChapitreState.FinJournee;
    }

    #endregion

    #region Cinématique Boucle

    private void StartCinematiqueBoucle()
    {
        currentState = ChapitreState.AnimationBoucle;

        StartCoroutine(coroutine_CinematiqueBoucle());
    }

    protected IEnumerator coroutine_CinematiqueBoucle()
    {
        //On lance l'animation de FadeIn
        animatorFadePanel.SetTrigger("FadeIn");

        //On attend que l'animation termine
        while (!bFadeEnded)
            yield return null;

        movingBody.transform.position = new Vector3(14.89f, -3.06f, 0);
        animatorFadePanel.SetTrigger("FadeOut");
        animatorFadePanel.ResetTrigger("FadeIn");


        goBoucle.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        calendrierBoucle.SetActive(true);

        yield return new WaitForSeconds(1f);

        while(nIndexBoucle < 20 || currentState == ChapitreState.FinBoucle)
        {
            chap_1_1.transform.position = new Vector3(chap_1_1.transform.position.x - boucleSpeed * Time.deltaTime, chap_1_1.transform.position.y, chap_1_1.transform.position.z);
            chap_2_1.transform.position = new Vector3(chap_2_1.transform.position.x - boucleSpeed * Time.deltaTime, chap_2_1.transform.position.y, chap_2_1.transform.position.z);
            chap_1_2.transform.position = new Vector3(chap_1_2.transform.position.x - boucleSpeed * Time.deltaTime, chap_1_2.transform.position.y, chap_1_2.transform.position.z);
            chap_2_2.transform.position = new Vector3(chap_2_2.transform.position.x - boucleSpeed * Time.deltaTime, chap_2_2.transform.position.y, chap_2_2.transform.position.z);

            bool bChangeSprite = false;

            if (chap_1_1.transform.position.x < -37.7f)
            {
                bChangeSprite = true;
                chap_1_1.transform.position = new Vector3(chap_1_1.transform.position.x + (37.7f * 4), chap_1_1.transform.position.y, chap_1_1.transform.position.z);
            }

            if (chap_2_1.transform.position.x < -37.7f)
            {
                bChangeSprite = true;
                chap_2_1.transform.position = new Vector3(chap_2_1.transform.position.x + (37.7f * 4), chap_2_1.transform.position.y, chap_2_1.transform.position.z);
            }

            if (chap_1_2.transform.position.x < -37.7f)
            {
                bChangeSprite = true;
                chap_1_2.transform.position = new Vector3(chap_1_2.transform.position.x + (37.7f * 4), chap_1_2.transform.position.y, chap_1_2.transform.position.z);
            }

            if (chap_2_2.transform.position.x < -37.7f)
            {
                bChangeSprite = true;
                chap_2_2.transform.position = new Vector3(chap_2_2.transform.position.x + (37.7f * 4), chap_2_2.transform.position.y, chap_2_2.transform.position.z);
            }

            if (bChangeSprite)
            {
                int i = nIndexBoucle % 5;
                int j = nIndexBoucle / 5;

                GameObject goCroix = (GameObject)Instantiate(Resources.Load("croix calendrier"));
                goCroix.transform.position = new Vector3(fXStart + 2f * i, fYStart - 1.5f * j, -8f);

                nIndexBoucle++;

                if (boucleSpeed < 200)
                    boucleSpeed += 10;
            }

            yield return null;
        }

        StopCinematiqueBoucle();
    }

    private void StopCinematiqueBoucle()
    {
        currentState = ChapitreState.FinBoucle;
        goBoucle.SetActive(false);

        SmoothChangeScene(Scenes.Chapitre3);
    }

    #endregion
}
