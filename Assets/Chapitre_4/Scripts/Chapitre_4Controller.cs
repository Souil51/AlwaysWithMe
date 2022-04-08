using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Chapitre_4Controller : CommonController
{
    private enum ChapitreState
    {
        Couloir,
        Couloir_Fuite,
        Couloir_Apres_Fuite,
        CinematiqueEntreeBanc,
        Banc,
        Banc_Rencontre,
        Banc_Araignee,
        Banc_Fil,
        Banc_SautAraignee,
        Banc_Fin
    }

    private ChapitreState currentState = ChapitreState.Couloir;

    //Couloir
    [SerializeField] private GameObject couloir_groupe;
    [SerializeField] private GameObject banc_groupe;
    [SerializeField] private AraigneeController araignee_couloir;

    //Banc rencontre
    [SerializeField] private GameObject goPerso_Animation_1;
    [SerializeField] private SpeakingBody goPerso_Animation_1_SpeakingBody;
    [SerializeField] private MovingBody movingBody_Maxine;
    [SerializeField] private GameObject goPerso_Animation_Maxine;
    [SerializeField] private SpeakingBody goPerso_Animation_Maxine_SpeakingBody;
    private Coroutine coroutineBoucleSpeak;

    //Banc_Araignee
    [SerializeField] private GameObject fil_groupe;
    [SerializeField] private GameObject fil_araignee;
    [SerializeField] private GameObject banc_araigneeRoot;
    [SerializeField] private GameObject carte_maxine;
    [SerializeField] private GameObject araignee;

    [SerializeField] private float fMinY;
    [SerializeField] private float fMaxY;
    [SerializeField] private float fSpeed;

    private GameObject TutoHoldArrowDown;
    private GameObject TutoHoldArrowUp;
    private GameObject TutoHoldArrowLeft;
    private GameObject TutoHoldArrowRight;


    protected override void ChildStart()
    {
        currentState = ChapitreState.Couloir;

        StartCinematique(Cinematiques.Chapitre4_Initial);
    }

    protected override void ChildUpdate()
    {
        if(currentState == ChapitreState.Couloir)
        {
            if (movingBody.transform.position.x > -0.91f)
            {
                StartCinematique(Cinematiques.Chapitre4_FuiteCouloir);
            }
        }
        else if(currentState == ChapitreState.Banc)
        {
            if(movingBody.transform.position.x > -6.80f)
            {
                StartCinematique(Cinematiques.Chapitre4_Rencontre);
            }
        }
        else if(currentState == ChapitreState.Banc_Araignee)
        {
            if (Input.GetMouseButton(1))
            {
                if (fil_groupe.transform.position.y < fMaxY)
                {
                    fil_groupe.transform.position = new Vector3(fil_groupe.transform.position.x, fil_groupe.transform.position.y + (fSpeed * Time.deltaTime), fil_groupe.transform.position.z);
                }

                StopTuto(TutoHoldArrowUp);
            }
            else if (Input.GetMouseButton(0))
            {
                if (fil_groupe.transform.position.y > fMinY)
                {
                    fil_groupe.transform.position = new Vector3(fil_groupe.transform.position.x, fil_groupe.transform.position.y - (fSpeed * Time.deltaTime), fil_groupe.transform.position.z);
                }

                StopTuto(TutoHoldArrowDown);
            }

            if (fil_groupe.transform.position.y < fMinY + 0.5f)
            {
                currentState = ChapitreState.Banc_Fil;

                fil_araignee.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                banc_araigneeRoot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

                TutoHoldArrowRight = PlayTuto(Tutoriel.Hold_Arrow_Right, new Vector3(9.51f, -1.48f, -0.91f));
                TutoHoldArrowLeft = PlayTuto(Tutoriel.Hold_Arrow_Left, new Vector3(14.85f, -1.48f, -0.91f));
            }
        }
        else if(currentState == ChapitreState.Banc_Fil)
        {
            banc_araigneeRoot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

            if (Input.GetMouseButton(0))
            {
                banc_araigneeRoot.GetComponent<Rigidbody2D>().AddForce(new Vector2(-2, 0), ForceMode2D.Force);

                StopTuto(TutoHoldArrowRight);

                if (TutoHoldArrowDown != null) StopTuto(TutoHoldArrowDown);
                if (TutoHoldArrowUp != null) StopTuto(TutoHoldArrowUp);
            }
            else if (Input.GetMouseButton(1))
            {
                banc_araigneeRoot.GetComponent<Rigidbody2D>().AddForce(new Vector2(2, 0), ForceMode2D.Force);

                StopTuto(TutoHoldArrowLeft);

                if (TutoHoldArrowDown != null) StopTuto(TutoHoldArrowDown);
                if (TutoHoldArrowUp != null) StopTuto(TutoHoldArrowUp);
            }

            if(fil_araignee.transform.eulerAngles.z < 335f && fil_araignee.transform.eulerAngles.z > 300f && currentState == ChapitreState.Banc_Fil)
            {
                if (TutoHoldArrowRight != null) StopTuto(TutoHoldArrowRight);
                if (TutoHoldArrowLeft != null) StopTuto(TutoHoldArrowLeft);


                StartCinematique(Cinematiques.Chapitre4_SautAraignee);
            }
        }
    }

    protected override void ChapterInteraction(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Sortie:
                {
                    PlaySound(Sound.Porte);

                    StartCinematique(Cinematiques.Chapitre4_EntreeBanc);
                }
                break;
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

        switch (interactionObject.GetObjectType())
        {
            case ObjectType.CarteMaxine:
                {
                    StartCinematique(Cinematiques.Chapitre4_FinBanc);
                }
                break;
        }
    }

    protected override void StartChapterCinematique(Cinematiques cinematique)
    {
        switch (cinematique)
        {
            case Cinematiques.Chapitre4_Initial:
                StartCinematiqueInitial();
                break;
            case Cinematiques.Chapitre4_FuiteCouloir:
                {
                    StartCinematiqueFuiteCouloir();
                }
                break;
            case Cinematiques.Chapitre4_EntreeBanc:
                {
                    StartCinematiqueEntreeBanc();
                }
                break;
            case Cinematiques.Chapitre4_Rencontre:
                {
                    StartCinematiqueRencontre();
                }
                break;
            case Cinematiques.Chapitre4_SautAraignee:
                {
                    StartCinematiqueAraigneeSaut();
                }
                break;
            case Cinematiques.Chapitre4_FinBanc:
                {
                    StartCinematiqueBancFin();
                }
                break;
        }
    }

    #region Cinématique Initiale

    private void StartCinematiqueInitial()
    {
        currentState = ChapitreState.Couloir;

        movingBody.SetActive(false);

        StartCoroutine(coroutine_CinematiqueInitial());
    }

    private IEnumerator coroutine_CinematiqueInitial()
    {
        yield return new WaitForSeconds(1);

        movingBody.GoToPosition(new Vector3(-7.38f, -11.14f, 256.92f));
        
        while (movingBody.IsGoingToPosition())
            yield return null;

        StopCinematiqueInitial();
    }

    private void StopCinematiqueInitial()
    {
        StopCinematique();
    }

    #endregion

    #region Cinématique Araignee Fuite Couloir

    private void StartCinematiqueFuiteCouloir()
    {
        currentState = ChapitreState.Couloir_Fuite;

        movingBody.StopMoving();
        movingBody.SetActive(false);

        araignee_couloir.gameObject.SetActive(true);

        StartCoroutine(coroutine_CinematiqueFuiteCouloir());
    }

    private IEnumerator coroutine_CinematiqueFuiteCouloir()
    {
        araignee_couloir.StartAnimation(AraigneeController.AnimationsAraignee.Chapitre3_FuiteCouloir);

        while (!araignee_couloir.IsAnimationFinished(AraigneeController.AnimationsAraignee.Chapitre3_FuiteCouloir))
            yield return null;

        StopCinematiqueFuiteCouloir();
    }

    private void StopCinematiqueFuiteCouloir()
    {
        StopCinematique();
        araignee_couloir.gameObject.SetActive(false);
    
        currentState = ChapitreState.Couloir_Apres_Fuite;
    }

    #endregion

    #region Cinématique Entrée banc

    private void StartCinematiqueEntreeBanc()
    {
        currentState = ChapitreState.CinematiqueEntreeBanc;

        ResetFadeTriggers();

        StartCoroutine(coroutine_CinematiqueEntreeBanc());
    }

    private IEnumerator coroutine_CinematiqueEntreeBanc()
    {
        animatorFadePanel.SetTrigger("FadeIn");

        //On attend que l'animation termine
        while (!bFadeEnded)
            yield return null;

        movingBody.transform.position = new Vector3(-11.08f, -11.14f, 0);
        animatorFadePanel.SetTrigger("FadeOut");
        animatorFadePanel.ResetTrigger("FadeIn");

        couloir_groupe.gameObject.SetActive(false);
        banc_groupe.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        StopCinematiqueEntreeBanc();
    }

    private void StopCinematiqueEntreeBanc()
    {
        currentState = ChapitreState.Banc;
        movingBody_Maxine.gameObject.SetActive(true);
        StopCinematique();
    }

    #endregion

    #region Cinematique Rencontre

    private void StartCinematiqueRencontre()
    {
        currentState = ChapitreState.Banc_Rencontre;

        StartCoroutine(coroutine_CinematiqueRencontre());
    }

    private IEnumerator coroutine_CinematiqueRencontre()
    {
        movingBody.StopMoving();
        movingBody.SetActive(false);

        //Maxine
        movingBody_Maxine.ChangeDirection(BodyDirection.Gauche);
        movingBody_Maxine.GoToPosition(new Vector3(3.87f, -11.14f, 256.9276f), 1f);

        MusicController.GetInstance().ChangeClip(MusicController.Clips.Maxine);

        while (movingBody_Maxine.IsGoingToPosition())
            yield return null;

        movingBody_Maxine.SpeakRandom(4);
        yield return new WaitForSeconds(0.25f);
        movingBody.SpeakRandom(4);
        yield return new WaitForSeconds(0.75f);

        movingBody_Maxine.gameObject.SetActive(false);
        goPerso_Animation_Maxine.SetActive(true);

        Perso_Animation_1_Controller ctrl_maxine = goPerso_Animation_Maxine.GetComponent<Perso_Animation_1_Controller>();
        ctrl_maxine.StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.Chapitre4_MaxineAnimation);

        while (!ctrl_maxine.IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.Chapitre4_MaxineAnimation))
            yield return null;

        yield return new WaitForSeconds(1f);

        //Perso principal
        movingBody.GoToPosition(new Vector3(-1.56f, -11.14f, 256.9276f));

        while (movingBody.IsGoingToPosition())
            yield return null;

        yield return new WaitForSeconds(0.25f);

        movingBody.gameObject.SetActive(false);
        goPerso_Animation_1.SetActive(true);

        coroutineBoucleSpeak = StartCoroutine(coroutine_BoucleSpeak());//Pendant la pahse avec l'araigné, les persos vont parler en boucle

        Perso_Animation_1_Controller ctrl = goPerso_Animation_1.GetComponent<Perso_Animation_1_Controller>();
        ctrl.StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.Chapitre4_PersoAnimation);

        while(!ctrl.IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.Chapitre4_PersoAnimation))
            yield return null;

        //Apparition de l'araignée
        MoveCamera(new Vector3(5.02f, 2.78f, -10), 8);
        fil_groupe.SetActive(true);

        float fElapsedTime = 0;
        float fDuration = 0.5f;

        float currentY = fil_groupe.transform.position.y;

        while (fElapsedTime < fDuration)
        {
            float vNewY = Mathf.Lerp(currentY, currentY - 3, (fElapsedTime / fDuration));
            fil_groupe.transform.position = new Vector3(fil_groupe.transform.position.x, vNewY, fil_groupe.transform.position.z);

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        TutoHoldArrowDown = PlayTuto(Tutoriel.Hold_Arrow_Down, new Vector3(10.64f, 7.59f, -0.91f));
        TutoHoldArrowUp = PlayTuto(Tutoriel.Hold_Arrow_Up, new Vector3(14.85f, 7.59f, -0.91f));

        MusicController.GetInstance().ChangeClip(MusicController.Clips.Araignee);

        //---//
        StopCinematiqueRencontre();
    }

    private IEnumerator coroutine_BoucleSpeak()
    {
        while (true)
        {
            goPerso_Animation_Maxine_SpeakingBody.SpeakRandom(1, 1.5f, 7.5f, BodyDirection.Droite);

            yield return new WaitForSeconds(0.5f);

            goPerso_Animation_1_SpeakingBody.SpeakRandom(1, 2.5f, 7.5f, BodyDirection.Gauche);

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void StopCinematiqueRencontre()
    {
        StopCinematique();
        currentState = ChapitreState.Banc_Araignee;
    }

    #endregion

    #region Cinématique AraigneeSaut

    private void StartCinematiqueAraigneeSaut()
    {
        currentState = ChapitreState.Banc_SautAraignee;

        Vector3 vRoot = banc_araigneeRoot.transform.position;
        banc_araigneeRoot.transform.position = Vector3.zero;
        araignee.transform.position = vRoot;

        banc_araigneeRoot.GetComponent<HingeJoint2D>().enabled = false;
        banc_araigneeRoot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        araignee.GetComponent<Animator>().enabled = true;
        araignee.GetComponent<IKManager2D>().enabled = true;

        StartCoroutine(coroutine_AraigneSaut());
    }

    private IEnumerator coroutine_AraigneSaut(float fDuration = 0.5f)
    {
        float fElapsedTime = 0;

        Vector3 currentPosition = araignee.transform.position;
        Vector3 vDestination = new Vector3(4.38f, -2.5f, currentPosition.z);

        Vector3 currentRotation = araignee.transform.eulerAngles;
        Vector3 currentRootRotation = banc_araigneeRoot.transform.eulerAngles;

        araignee.GetComponent<AraigneeController>().StartAnimation(AraigneeController.AnimationsAraignee.SautFil);

        bool bSound = false;

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(currentPosition, vDestination, (fElapsedTime / fDuration));
            araignee.transform.position = vNewPos;

            Vector3 vNewEulerAngles = Vector3.Lerp(currentRotation, new Vector3(0, 0, 0), (fElapsedTime / fDuration));
            araignee.transform.eulerAngles = vNewEulerAngles;

            Vector3 vNewRootEulerAngles = Vector3.Lerp(currentRotation, new Vector3(0, 0, 90), (fElapsedTime / fDuration));
            banc_araigneeRoot.transform.eulerAngles = vNewRootEulerAngles;

            fElapsedTime += Time.deltaTime;

            if(fElapsedTime > fDuration / 2 && !bSound)
            {
                PlaySound(Sound.AraigneePlacard);
                bSound = true;
            }

            yield return null;
        }

        araignee.transform.eulerAngles = Vector3.zero;
        banc_araigneeRoot.transform.eulerAngles = new Vector3(0, 0, 90);
        araignee.transform.position = vDestination;

        while (!araignee.GetComponent<AraigneeController>().IsAnimationFinished(AraigneeController.AnimationsAraignee.SautFil))
            yield return null;

        StopCinematiqueAraigneeSaut();
    }

    private void StopCinematiqueAraigneeSaut()
    {
        MoveCamera(banc_araigneeRoot.transform.position, 2f);
        SetDefaultCameraPosition(new Vector3(0, 0, -10));

        carte_maxine.SetActive(true);
        StopCinematique();
    }

    #endregion

    #region Cinématique BancFin

    private void StartCinematiqueBancFin()
    {
        currentState = ChapitreState.Banc_Fin;

        StartCoroutine(coroutine_BancFin());
    }

    private IEnumerator coroutine_BancFin()
    {
        yield return new WaitForSeconds(1f);

        StopCoroutine(coroutineBoucleSpeak);

        carte_maxine.SetActive(false);

        //Fuite de l'araignee
        araignee.GetComponent<AraigneeController>().StartAnimation(AraigneeController.AnimationsAraignee.Chapitre3_SortieEcran);

        yield return new WaitForSeconds(0.2f);

        PlaySound(Sound.ShakePeluche);

        yield return new WaitForSeconds(0.8f);

        MusicController.GetInstance().ChangeClip(MusicController.Clips.Maxine);

        goPerso_Animation_1.GetComponent<Perso_Animation_1_Controller>().StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.Chapitre4_PersoAnimationReverse);
        goPerso_Animation_Maxine.GetComponent<Perso_Animation_1_Controller>().StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.Chapitre4_MaxineAnimationReverse);

        while (!goPerso_Animation_1.GetComponent<Perso_Animation_1_Controller>().IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.Chapitre4_PersoAnimationReverse)
            || !goPerso_Animation_Maxine.GetComponent<Perso_Animation_1_Controller>().IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.Chapitre4_MaxineAnimationReverse))
            yield return null;

        yield return new WaitForSeconds(1f);

        movingBody_Maxine.gameObject.SetActive(true);
        goPerso_Animation_Maxine.SetActive(false);

        movingBody.gameObject.SetActive(true);
        goPerso_Animation_1.SetActive(false);

        movingBody.SetActive(false);

        movingBody.StopSpeaking();
        movingBody_Maxine.StopSpeaking();

        movingBody.SpeakRandom(3);

        yield return new WaitForSeconds(0.25f);

        movingBody_Maxine.SpeakRandom(3);

        while (movingBody.IsSpeaking() || movingBody_Maxine.IsSpeaking())
            yield return null;

        //Maxine se lève
        movingBody_Maxine.GoToPosition(new Vector3(24.59f, -11.14f, movingBody_Maxine.transform.position.z), 1f); ;
        movingBody.SetActive(true);

        while (movingBody_Maxine.IsGoingToPosition())
            yield return null;

        StopCoroutine(coroutineBoucleSpeak);

        StopCinematique();

        yield return new WaitForSeconds(3f);

        StopCinematiqueBancFin();
    }

    private void StopCinematiqueBancFin()
    {
        ResetFadeTriggers();

        SmoothChangeScene(Scenes.Chapitre5);
    }

    #endregion
}
