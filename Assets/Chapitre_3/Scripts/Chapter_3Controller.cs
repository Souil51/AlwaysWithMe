using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_3Controller : CommonController
{
    private enum ChapitreState
    {
        Initial,
        CinematiquePlacard,
        Placard,
        CinematiqueSortiePlacard,
        ApresPlacard
    }

    [SerializeField] private SpriteRenderer lightBulb;
    [SerializeField] private GameObject lightRoom;
    [SerializeField] private Sprite sprt_lightOn;
    [SerializeField] private Sprite sprt_lightOff;
    private bool bLightOn = false;
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
    [SerializeField] private AraigneeController araignee;
    [SerializeField] private DynamicBodyController araigneeDynamicBody;
    [SerializeField] private float InitialAraigneeY;

    private int nIndexObjectAraignee;
    private GameObject goObjectToFollowAraignee;

    private ChapitreState currentState = ChapitreState.Initial;

    //Placard
    [SerializeField] private GameObject goChambreGroupe;
    [SerializeField] private GameObject goPlacardGroupe;

    private int nMaxOrderInLayer = 10;
    private float fMinDepth = 0;
    [SerializeField] private List<MovableObject> lstMovableObject = new List<MovableObject>();

    //Apr?s placard
    [SerializeField] private GameObject porteStatic;
    [SerializeField] private GameObject porteInteractable;
    [SerializeField] private GameObject placardStatic;
    [SerializeField] private GameObject placardInteractable;
    [SerializeField] private AraigneeController araigneeApresPlacard;
    [SerializeField] private SpriteRenderer sprtRendererBody;
    [SerializeField] private Sprite spriteBodySac;
    [SerializeField] private GameObject goSac;

    //Mouse shake detection
    private bool bAraigneeFollowObjet = true;

    private Vector3 startPosition;
    private Vector3 startPositionRoot;
    [SerializeField] private GameObject goRoot;

    private Coroutine coroutineSpeak;
    private Coroutine coroutineSpeakPlacard;

    private GameObject goDragTuto;
    private GameObject goShakeTuto;
    private GameObject goShakeTutoFollow;//Object suivi par le goCheckTuto
    private bool bTutoShakePlayed = false;

    private float timeWithoutSelectInteraction = 0f;
    private float timeWithoutShakeInteraction = 0f;

    protected override void ChildStart()
    {
        startPosition = araignee.transform.position;
        startPositionRoot = goRoot.transform.localPosition;

        nIndexObjectAraignee = Random.Range(0, lstMovableObject.Count);

        foreach (MovableObject movableObj in lstMovableObject)
        {
            movableObj.InitMovableObject();

            if (movableObj.GetObjectIndex() == nIndexObjectAraignee)
            {
                goObjectToFollowAraignee = movableObj.gameObject;
                araignee.transform.position = goObjectToFollowAraignee.transform.position;
            }

            movableObj.MovableObjectSelectedEvent += MovableObj_MovableObjectSelectedEvent;
            movableObj.MovableObjectShakedEvent += MovableObj_MovableObjectShakedEvent;
            movableObj.MovableObjectReleasedEvent += MovableObj_MovableObjectReleasedEvent;
        }

        movingBody.ChangeDirection(BodyDirection.Droite);
    }

    protected override void ChildUpdate()
    {
        if (currentState == ChapitreState.Placard)
        {
            if (bAraigneeFollowObjet)
            {
                Vector3 vPosObjToFollow = goObjectToFollowAraignee.transform.position;
                araignee.transform.position = new Vector3(vPosObjToFollow.x, vPosObjToFollow.y, 0.2f);
            }

            if(goDragTuto == null)
                timeWithoutSelectInteraction += Time.deltaTime;

            if(goShakeTuto == null)
                timeWithoutShakeInteraction += Time.deltaTime;

            if (timeWithoutSelectInteraction > 10f)
            {
                timeWithoutSelectInteraction = 0;

                goDragTuto = PlayTuto(Tutoriel.Drag, new Vector3(2.82f, 5.51f, 0f));
            }

            if (timeWithoutShakeInteraction > 10f)
            {
                timeWithoutShakeInteraction = 0;

                bTutoShakePlayed = false;
            }
        }

        if(goShakeTuto != null && goShakeTutoFollow != null)
        {
            goShakeTuto.transform.position = new Vector3(goShakeTutoFollow.transform.position.x, goShakeTutoFollow.transform.position.y + 3f, -1);
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

                    PlaySound(Sound.Interrupteur);
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

                    PlaySound(Sound.MenuFermer);

                    MenuLeaveAnimator.Play("LeaveDisappearAnimation");
                }
                break;
            case InteractionType.AcceptMenu:
                {
                    this.bMenuDisplayed = false;
                    MenuLeave.SetActive(false);

                    ResetFadeTriggers();

                    PlaySound(Sound.Porte);

                    SmoothChangeScene(Scenes.Chapitre4);
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

                    PlaySound(Sound.Interrupteur);
                }
                break;
            case InteractionType.Placard_Interaction:
                {
                    StartCinematique(Cinematiques.Chapitre3_EntreePlacard);
                }
                break;
        }
    }

    //G?re les interactions de ce chapitre
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
                }
                break;
        }
    }

    //G?re l'arr?t des interactions avec un objet du chapitre
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
                }
                break;
        }
    }

    protected override void StartChapterCinematique(Cinematiques cinematique)
    {
        switch (cinematique)
        {
            case Cinematiques.Chapitre3_EntreePlacard:
                {
                    StartCinematiquePlacard();
                }
                break;
            case Cinematiques.Chapitre3_SortiePlacard:
                {
                    StartCinematiqueSortiePlacard();
                }
                break;
        }
    }

    private void MovableObj_MovableObjectSelectedEvent(object sender, MovableObjectEventArg e)
    {
        PlaySound(Sound.SelectPeluche);

        timeWithoutSelectInteraction = 0;

        MovableObject objForeGround = lstMovableObject.Find(x => x.GetObjectIndex() == e.nIndex);

        if (goDragTuto != null)
        {
            StopTuto(goDragTuto);
            goDragTuto = null;
        }

        if (!bTutoShakePlayed)
        {
            goShakeTuto = PlayTuto(Tutoriel.Check, objForeGround.transform.position);
            goShakeTutoFollow = objForeGround.gameObject;
        }

        objForeGround.gameObject.GetComponent<SpriteRenderer>().sortingOrder = nMaxOrderInLayer++;
        objForeGround.gameObject.transform.position = new Vector3(objForeGround.gameObject.transform.position.x, fMinDepth, objForeGround.gameObject.transform.position.z);
        fMinDepth -= 0.01f;
    }

    private void MovableObj_MovableObjectReleasedEvent(object sender, MovableObjectEventArg e)
    {
        PlaySound(Sound.SelectPeluche);

        if (goShakeTuto != null)
        {
            StopTuto(goShakeTuto);
            goShakeTuto = null;
            goShakeTutoFollow = null;
        }
    }

    private void MovableObj_MovableObjectShakedEvent(object sender, MovableObjectEventArg e)
    {
        PlaySound(Sound.ShakePeluche);

        timeWithoutShakeInteraction = 0;

        if (goShakeTuto != null)
        {
            StopTuto(goShakeTuto);
            goShakeTuto = null;
            goShakeTutoFollow = null;
            bTutoShakePlayed = true;
        }

        if (!araignee.IsInit() && e.nIndex == nIndexObjectAraignee)
        {
            if(coroutineSpeak != null)
                StopCoroutine(coroutineSpeak);

            araignee.ShowHideSprite(true);
            araignee.InitDynamicBody();
            bAraigneeFollowObjet = false;
            araignee.AddForce(new Vector3(0, 0, 0));

            goObjectToFollowAraignee.GetComponent<MovableObject>().Launch(10);

            StartCoroutine(coroutine_ZoomOnAraignee());
        }
    }

    #region Cin?matique entr?e Placard

    private void StartCinematiquePlacard()
    {
        currentState = ChapitreState.CinematiquePlacard;

        StartCoroutine(coroutine_CinematiquePlacard());
    }

    private IEnumerator coroutine_CinematiquePlacard()
    {
        movingBody.StopMoving();

        yield return new WaitForSeconds(0.5f);

        araignee.ShowHideSprite(false);

        movingBody.GoToPosition(new Vector3(-13.71f, -3.06f, 256.98f));

        while (movingBody.IsGoingToPosition())
            yield return null;

        animatorFadePanel.SetTrigger("FadeIn");

        //On attend que l'animation termine
        while (!bFadeEnded)
            yield return null;

        animatorFadePanel.SetTrigger("FadeOut");
        animatorFadePanel.ResetTrigger("FadeIn");

        goChambreGroupe.gameObject.SetActive(false);
        goPlacardGroupe.gameObject.SetActive(true);
        coroutineSpeak = StartCoroutine(coroutine_AraigneeSpeak());

        goDragTuto = PlayTuto(Tutoriel.Drag, new Vector3(2.82f, 5.51f, 0f));

        yield return new WaitForSeconds(0.5f);

        StopCinematiquePlacard();
    }

    private void StopCinematiquePlacard()
    {
        currentState = ChapitreState.Placard;
        StopCinematique();

        movingBody.SetActive(false);
    }

    #endregion

    #region Cin?matique sortie Placard

    private void StartCinematiqueSortiePlacard()
    {
        currentState = ChapitreState.CinematiqueSortiePlacard;

        ResetFadeTriggers();

        StartCoroutine(coroutine_CinematiqueSortiePlacard());
    }

    private IEnumerator coroutine_CinematiqueSortiePlacard()
    {
        MoveCamera(new Vector3(-0.2f, 8.67f, -10), 10.5f);

        animatorFadePanel.SetTrigger("FadeIn");

        //On attend que l'animation termine
        while (!bFadeEnded)
            yield return null;

        movingBody.transform.position = new Vector3(-14.24f, -3.06f, 0);
        animatorFadePanel.SetTrigger("FadeOut");
        animatorFadePanel.ResetTrigger("FadeIn");

        if (coroutineSpeak != null)
            StopCoroutine(coroutineSpeak);

        goChambreGroupe.gameObject.SetActive(true);
        goPlacardGroupe.gameObject.SetActive(false);

        placardStatic.SetActive(true);
        placardInteractable.SetActive(false);
        
        araigneeApresPlacard.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        movingBody.GoToPosition(new Vector3(5.3f, -3.06f, movingBody.transform.position.z));

        yield return new WaitForSeconds(0.3f);

        MoveCamera(new Vector3(0.85f, 8.67f, -10), 3.4f);

        yield return new WaitForSeconds(0.3f);

        movingBody.ChangeDirection(BodyDirection.Droite);

        yield return new WaitForSeconds(0.5f);

        araigneeApresPlacard.SpeakRandom(4);

        while(araigneeApresPlacard.IsSpeaking())
            yield return null;

        MoveCamera(new Vector3(-0.2f, 8.67f, -10), 10.5f);

        yield return new WaitForSeconds(0.5f);

        movingBody.GoToPosition(new Vector3(8f, -3.06f, movingBody.transform.position.z), 0.3f);

        while (movingBody.IsGoingToPosition())
            yield return null;

        movingBody.ChangeDirection();

        yield return new WaitForSeconds(0.5f);

        araigneeApresPlacard.StartAnimation(AraigneeController.AnimationsAraignee.SautDansSac);

        while(!araigneeApresPlacard.IsAnimationFinished(AraigneeController.AnimationsAraignee.SautDansSac))
            yield return null;

        araigneeApresPlacard.gameObject.SetActive(false);

        movingBody.GoToPosition(new Vector3(2.5f, -3.06f, 256.98f));
        sprtRendererBody.gameObject.transform.position += new Vector3(0.8f, 0, 0);

        while (movingBody.IsGoingToPosition())
            yield return null;

        goSac.SetActive(false);
        sprtRendererBody.sprite = spriteBodySac;

        StopCinematiqueSortiePlacard();
    }

    private void StopCinematiqueSortiePlacard()
    {
        currentState = ChapitreState.ApresPlacard;

        porteStatic.SetActive(false);
        porteInteractable.SetActive(true);

        bInteractionsActives = true;

        MusicController.GetInstance().ChangeClip(MusicController.Clips.Perso);
        StopCinematique();
    }

    #endregion

    #region Scripts Placard

    private Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(1);
        Vector3 vRes = new Vector3(rayPoint.x, rayPoint.y, transform.position.z);

        return vRes;
    }

    private IEnumerator coroutine_ZoomOnAraignee()
    {
        MusicController.GetInstance().ChangeClip(MusicController.Clips.Araignee);

        if(goShakeTuto != null)
            StopTuto(goShakeTuto);

        if (goDragTuto != null)
            StopTuto(goDragTuto);

        while (!araigneeDynamicBody.IsBodyGettingUp())
            yield return null;

        foreach (MovableObject obj in lstMovableObject)
        {
            obj.ChangeOpacity(0.5f);
        }

        CreateSound(Sound.AraigneePlacard);

        Vector3 vPos = new Vector3(araignee.transform.position.x, 2.14f, araignee.transform.position.z);
        MoveCamera(vPos, 6f);

        while (!araigneeDynamicBody.IsBodyUp())
            yield return null;

        coroutineSpeakPlacard = StartCoroutine(coroutine_AraigneeSpeakPlacard());

        araignee.RestoreDynamicBodyPosition();

        araignee.ToggleAnimator(true);
        araignee.StartAnimation(AraigneeController.AnimationsAraignee.Chapitre3_Placard);

        while (!araignee.IsAnimationFinished(AraigneeController.AnimationsAraignee.Chapitre3_Placard))
            yield return null;

        yield return new WaitForSeconds(2f);

        float fYDifference = araignee.gameObject.transform.position.y - startPosition.y;
        float fXDifference = araignee.gameObject.transform.position.x - startPosition.x;

        araignee.gameObject.transform.position = new Vector3(startPosition.x + fXDifference, araignee.gameObject.transform.position.y, startPosition.z);
        goRoot.transform.localPosition = startPositionRoot;

        araignee.MoveAllTargets(fYDifference, fXDifference);

        yield return new WaitForSeconds(1f);

        StopCoroutine(coroutineSpeakPlacard);
        araignee.ToggleAnimator(false);

        StartCinematique(Cinematiques.Chapitre3_SortiePlacard);
    }

    private IEnumerator coroutine_AraigneeSpeak()
    {
        while (true)
        {
            araignee.SpeakRandom(1);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator coroutine_AraigneeSpeakPlacard()
    {
        while (true)
        {
            araignee.SpeakRandom(1);
            yield return new WaitForSeconds(0.5f);
            araignee.SpeakRandom(1);
            yield return new WaitForSeconds(0.5f);
        }
    }

    #endregion
}
