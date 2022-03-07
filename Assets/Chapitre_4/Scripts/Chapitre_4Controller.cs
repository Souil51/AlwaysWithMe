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

    protected override void ChildStart()
    {
        currentState = ChapitreState.Couloir;
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
            }
            else if (Input.GetMouseButton(0))
            {
                if (fil_groupe.transform.position.y > fMinY)
                {
                    fil_groupe.transform.position = new Vector3(fil_groupe.transform.position.x, fil_groupe.transform.position.y - (fSpeed * Time.deltaTime), fil_groupe.transform.position.z);
                }
            }

            if (fil_groupe.transform.position.y < fMinY + 0.5f)
            {
                currentState = ChapitreState.Banc_Fil;

                fil_araignee.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                banc_araigneeRoot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
        }
        else if(currentState == ChapitreState.Banc_Fil)
        {
            banc_araigneeRoot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

            if (Input.GetMouseButton(0))
            {
                banc_araigneeRoot.GetComponent<Rigidbody2D>().AddForce(new Vector2(-2, 0), ForceMode2D.Force);
            }
            else if (Input.GetMouseButton(1))
            {
                banc_araigneeRoot.GetComponent<Rigidbody2D>().AddForce(new Vector2(2, 0), ForceMode2D.Force);
            }

            if(fil_araignee.transform.eulerAngles.z < 335f && fil_araignee.transform.eulerAngles.z > 300f && currentState == ChapitreState.Banc_Fil)
            {
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
                    StartCinematique(Cinematiques.Chapitre4_EntreeBanc);
                }
                break;
        }
    }

    //G�re les interactions de ce chapitre
    protected override void ChapterZoomOnObject(InteractableObject interactionObject)
    {
        interactionObject.EnterZoom();
    }

    //G�re l'arr�t des interactions avec un objet du chapitre
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
        movingBody.SetActive(false);

        switch (cinematique)
        {
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

        movingBody.SetActive(true);
    }

    #region Cin�matique Araignee Fuite Couloir

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
        araignee_couloir.animation_chapitre_3_FuiteCouloir();

        while (!araignee_couloir.animation_chapitre_3_FuiteCouloir_IsFinished())
            yield return null;

        StopCinematiqueFuiteCouloir();
    }

    private void StopCinematiqueFuiteCouloir()
    { 
        movingBody.SetActive(true);
        araignee_couloir.gameObject.SetActive(false);
    
        currentState = ChapitreState.Couloir_Apres_Fuite;
    }

    #endregion

    #region Cin�matique Entr�e banc

    private void StartCinematiqueEntreeBanc()
    {
        currentState = ChapitreState.CinematiqueEntreeBanc;

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
        movingBody_Maxine.GoToPosition(new Vector3(3.87f, -11.14f, 256.9276f));

        while (movingBody_Maxine.IsGoingToPosition())
            yield return null;

        List<Emote> lstEmotes = new List<Emote>();
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote2);
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote2);

        movingBody_Maxine.Speak(lstEmotes);
        yield return new WaitForSeconds(0.5f);
        movingBody.Speak(lstEmotes);
        yield return new WaitForSeconds(0.5f);

        movingBody_Maxine.gameObject.SetActive(false);
        goPerso_Animation_Maxine.SetActive(true);

        Perso_Animation_1_Controller ctrl_maxine = goPerso_Animation_Maxine.GetComponent<Perso_Animation_1_Controller>();
        ctrl_maxine.animation_MainReverse();

        while (!ctrl_maxine.animation_MainReverseIsFinished())
            yield return null;

        yield return new WaitForSeconds(1f);

        //Perso principal
        movingBody.GoToPosition(new Vector3(-4.96f, -11.14f, 256.9276f));

        while (movingBody.IsGoingToPosition())
            yield return null;

        movingBody.gameObject.SetActive(false);
        goPerso_Animation_1.SetActive(true);

        coroutineBoucleSpeak = StartCoroutine(coroutine_BoucleSpeak());//Pendant la pahse avec l'araign�, les persos vont parler en boucle

        Perso_Animation_1_Controller ctrl = goPerso_Animation_1.GetComponent<Perso_Animation_1_Controller>();
        ctrl.animation_MainReverse();

        while(!ctrl.animation_MainReverseIsFinished())
            yield return null;

        //Apparition de l'araign�e
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

        //---//
        StopCinematiqueRencontre();
    }

    private IEnumerator coroutine_BoucleSpeak()
    {
        while (true)
        {
            int nIndex = Random.Range(0, SpeakingBody.lstSprites.Count);
            goPerso_Animation_Maxine_SpeakingBody.Speak((Emote)nIndex, -1f, 3f, BodyDirection.Droite);

            yield return new WaitForSeconds(0.5f);

            nIndex = Random.Range(0, SpeakingBody.lstSprites.Count);
            goPerso_Animation_1_SpeakingBody.Speak((Emote)nIndex, 0, 3f, BodyDirection.Gauche);

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void StopCinematiqueRencontre()
    {
        //Visibilit� de l'araignee
        currentState = ChapitreState.Banc_Araignee;
    }

    #endregion

    #region Cin�matique AraigneeSaut

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
        Vector3 vDestination = new Vector3(3.88f, -2.7f, currentPosition.z);

        Vector3 currentRotation = araignee.transform.eulerAngles;

        araignee.GetComponent<AraigneeController>().animation_SautFil();

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(currentPosition, vDestination, (fElapsedTime / fDuration));
            araignee.transform.position = vNewPos;

            Vector3 vNewEulerAngles = Vector3.Lerp(currentRotation, new Vector3(0, 0, 2), (fElapsedTime / fDuration));
            araignee.transform.eulerAngles = vNewEulerAngles;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        while (!araignee.GetComponent<AraigneeController>().animation_SautFil_IsFinished())
            yield return null;

        StopCinematiqueAraigneeSaut();
    }

    private void StopCinematiqueAraigneeSaut()
    {
        MoveCamera(banc_araigneeRoot.transform.position, 2f);
        SetDefaultCameraPosition(new Vector3(0, 0, -10));

        carte_maxine.SetActive(true);
    }

    #endregion

    #region Cin�matique BancFin

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

        goPerso_Animation_1.GetComponent<Perso_Animation_1_Controller>().animation_Main();
        goPerso_Animation_Maxine.GetComponent<Perso_Animation_1_Controller>().animation_Main();

        yield return new WaitForSeconds(0.5f);

        //Fuite de l'araignee
        araignee.GetComponent<AraigneeController>().animation_chapitre_3_SortieEcran();

        while (!goPerso_Animation_1.GetComponent<Perso_Animation_1_Controller>().animation_MainIsFinished()
            || !goPerso_Animation_Maxine.GetComponent<Perso_Animation_1_Controller>().animation_MainIsFinished())
            yield return null;

        movingBody_Maxine.gameObject.SetActive(true);
        goPerso_Animation_Maxine.SetActive(false);

        movingBody.gameObject.SetActive(true);
        goPerso_Animation_1.SetActive(false);

        //Maxine se l�ve
        movingBody_Maxine.GoToPosition(new Vector3(24.59f, -11.14f, movingBody_Maxine.transform.position.z)); ;

        while(movingBody_Maxine.IsGoingToPosition())
            yield return null;

        StopCoroutine(coroutineBoucleSpeak);

        yield return new WaitForSeconds(3f);

        StopCinematiqueBancFin();
    }

    private void StopCinematiqueBancFin()
    {
        ChangeScene(Scenes.Chapitre5);
    }

    #endregion
}
