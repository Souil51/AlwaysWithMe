using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter_7Controller : CommonController
{
    private enum ChapitreState
    {
        Initial,
        CinematiqueArrivee,
        Arrivee,
        CinematiqueEntreeFete,
        Fete,
        CinematiqueRencontre,
        ApresRencontre,
        CinematiqueDebutJeu,
        Jeu,
        CinematiqueVictoireJeu,
        ChoixPrix,
        CinematiqueFinJeu
    }

    private ChapitreState currentState = ChapitreState.Initial;

    [SerializeField] private MovingBody moveingBodyMaxine;
    [SerializeField] private GameObject goBus;
    [SerializeField] private GameObject entree_groupe;
    [SerializeField] private GameObject fete_groupe;
    [SerializeField] private GameObject jeu_groupe;
    [SerializeField] private GameObject interaction_jeu;

    //Jeu
    [SerializeField] private GameObject goTarget;
    [SerializeField] private List<BallonController> lstBallons;
    [SerializeField] private GameObject goBallonsHolder;
    [SerializeField] private GameObject goPrix;
    [SerializeField] private GameObject fil_groupe;
    [SerializeField] private AraigneeController araignee;

    [SerializeField] private ParticleSystem particleSystemVictoire;

    [SerializeField] private GameObject PrixFlamme;
    [SerializeField] private GameObject PrixMasque;
    [SerializeField] private GameObject PrixTotoro;

    [SerializeField] private List<Sprite> lstSpritesChiffres;
    [SerializeField] private Image ImagePoint_1;
    [SerializeField] private Image ImagePoint_2;
    [SerializeField] private Image ImageGoal_1;
    [SerializeField] private Image ImageGoal_2;

    [SerializeField] private SpriteRenderer peluche;

    private int nObjectif = 1;
    private int nPoints = 0;
    private Coroutine coroutineDescenteAraignee = null;

    protected override void ChildStart()
    {
        //StartCinematique(Cinematiques.Chapitre7_Arrivee);

        //StartCinematique(Cinematiques.Chapitre7_EntreeFete);
        StartCinematique(Cinematiques.Chapitre7_EntreeJeu);
    }

    protected override void ChildUpdate()
    {
        if(currentState == ChapitreState.Fete)
        {
            if(movingBody.transform.position.x > -4.6f)
            {
                StartCinematique(Cinematiques.Chapitre7_Rencontre);
            }
        }
        else if(currentState == ChapitreState.Jeu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                BallonController ballonDisabled = null;

                foreach (BallonController ballon in lstBallons)
                {
                    if (ballon.IsTargeted())
                    {
                        ballonDisabled = ballon;
                        ballon.SetEnabled(false);
                    }
                }

                if(ballonDisabled != null)
                {
                    nPoints++;
                    UpdateJeuPoints();

                    if (nPoints == nObjectif)
                    {
                        StartCinematique(Cinematiques.Chapitre7_VictoireJeu);
                    }
                    else
                    {

                        lstBallons.Remove(ballonDisabled);
                        Destroy(ballonDisabled);

                        GameObject goNewBallon = (GameObject)Instantiate(Resources.Load("ballon"));
                        goNewBallon.transform.SetParent(goBallonsHolder.transform);

                        lstBallons.Add(goNewBallon.GetComponent<BallonController>());

                        float fX = Random.Range(-7.8f, 8.4f);
                        float fY = Random.Range(-0.3f, 7.7f);

                        goNewBallon.transform.position = new Vector3(fX, fY, 0);
                    }
                }
            }
        }
        else if(currentState == ChapitreState.ChoixPrix)
        {

        }
    }

    protected override void ChapterInteraction(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Sortie:
                {
                    StartCinematique(Cinematiques.Chapitre7_EntreeFete);
                }
                break;
            case InteractionType.DebutJeu:
                {
                    StartCinematique(Cinematiques.Chapitre7_EntreeJeu);
                }
                break;
            case InteractionType.PrixFlamme:
                {
                    coroutineDescenteAraignee = StartCoroutine(coroutine_DescenteAraigneeFaux());
                }
                break;
            case InteractionType.PrixMasque:
                {
                    coroutineDescenteAraignee = StartCoroutine(coroutine_DescenteAraigneeFaux());
                }
                break;
            case InteractionType.PrixTotoro:
                {
                    StartCinematique(Cinematiques.Chapitre7_FinJeu);
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
    }

    private void UpdateJeuPoints()
    {
        if(nPoints >= 10)
        {
            ImagePoint_1.sprite = lstSpritesChiffres[1];
        }

        ImagePoint_2.sprite = lstSpritesChiffres[nPoints % 10];
    }

    protected override void StartChapterCinematique(Cinematiques cinematique)
    {
        switch (cinematique)
        {
            case Cinematiques.Chapitre7_Arrivee:
                {
                    StartCinematiqueArrivee();
                }
                break;
            case Cinematiques.Chapitre7_EntreeFete:
                {
                    StartCinematiqueEntreeFete();
                }
                break;
            case Cinematiques.Chapitre7_Rencontre:
                {
                    StartCinematiqueRencontre();
                }
                break;
            case Cinematiques.Chapitre7_EntreeJeu:
                {
                    StartCinematiqueDebutJeu();
                }
                break;
            case Cinematiques.Chapitre7_VictoireJeu:
                {
                    StartCinematiqueVictoireJeu();
                }
                break;
            case Cinematiques.Chapitre7_FinJeu:
                {
                    StartCinematiqueFinJeu();
                }
                break;
        }
    }

    #region Cinematique Arrivee

    private void StartCinematiqueArrivee()
    {
        currentState = ChapitreState.CinematiqueArrivee;
        movingBody.transform.position = new Vector3(49.4f, -10.6f, 1);
        goBus.transform.position = new Vector3(35.5f, -5.07f, 0);

        StartCoroutine(coroutine_CinematiqueArrivee());
    }

    private IEnumerator coroutine_CinematiqueArrivee()
    {
        movingBody.SetActive(false);
        movingBody.ChangeDirection(BodyDirection.Gauche);

        float fElapsedTime = 0;
        float fDuration = 1f;

        Vector3 vBusCurrentPos = goBus.transform.position;
        Vector3 vBusDestination = new Vector3(vBusCurrentPos.x - 33f, -5.07f, 0);

        Vector3 movingBodyCurrentPos = movingBody.transform.position;
        Vector3 movingBodyDestination = new Vector3(movingBodyCurrentPos.x - 33f, -10.8f, 0);

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(vBusCurrentPos, vBusDestination, (fElapsedTime / fDuration));
            goBus.transform.position = vNewPos;

            vNewPos = Vector3.Lerp(movingBodyCurrentPos, movingBodyDestination, (fElapsedTime / fDuration));
            movingBody.transform.position = vNewPos;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        Vector3 vMovingBodyPosition = movingBody.gameObject.transform.position;
        movingBody.GoToPosition(new Vector3(vMovingBodyPosition.x, vMovingBodyPosition.y + 1f, vMovingBodyPosition.z));

        while (movingBody.IsGoingToPosition())
            yield return null;

        vMovingBodyPosition = movingBody.gameObject.transform.position;
        movingBody.GoToPosition(new Vector3(vMovingBodyPosition.x - 25f, vMovingBodyPosition.y, vMovingBodyPosition.z));

        while (movingBody.IsGoingToPosition())
            yield return null;

        yield return new WaitForSeconds(1f);

        vBusCurrentPos = goBus.transform.position;
        vBusDestination = new Vector3(vBusCurrentPos.x - 40f, -5.07f, 0);
        fElapsedTime = 0;

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(vBusCurrentPos, vBusDestination, (fElapsedTime / fDuration));
            goBus.transform.position = vNewPos;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        StopCinematiqueArrivee();
    }

    private void StopCinematiqueArrivee()
    {
        currentState = ChapitreState.Arrivee;
    }

    #endregion

    #region Cinematique Entree fete

    private void StartCinematiqueEntreeFete()
    {
        currentState = ChapitreState.CinematiqueEntreeFete;
        StartCoroutine(coroutine_CinematiqueEntreeFete());
    }

    private IEnumerator coroutine_CinematiqueEntreeFete()
    {
        animatorFadePanel.SetTrigger("FadeIn");

        //On attend que l'animation termine
        while (!bFadeEnded)
            yield return null;

        movingBody.transform.position = new Vector3(-12.1f, movingBody.transform.position.y, movingBody.transform.position.z);
        movingBody.GoToPosition(new Vector3(movingBody.transform.position.x + 0.1f, movingBody.transform.position.y, movingBody.transform.position.z));

        while (movingBody.IsGoingToPosition())
            yield return null;

        movingBody.transform.position = new Vector3(-11.08f, -11.44f, 0);
        animatorFadePanel.SetTrigger("FadeOut");
        animatorFadePanel.ResetTrigger("FadeIn");

        entree_groupe.gameObject.SetActive(false);
        fete_groupe.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        StopCinematiqueEntreeFete();
    }

    private void StopCinematiqueEntreeFete()
    {
        StopCinematique();
        currentState = ChapitreState.Fete;
    }

    #endregion

    #region Cinematique Rencontre

    private void StartCinematiqueRencontre()
    {
        currentState = ChapitreState.CinematiqueEntreeFete;

        moveingBodyMaxine.transform.position = new Vector3(24.2f, -11.44f, 1f);

        StartCoroutine(coroutine_CinematiqueRencontre());
    }

    private IEnumerator coroutine_CinematiqueRencontre()
    {
        movingBody.StopMoving();
        movingBody.SetActive(false);

        moveingBodyMaxine.gameObject.SetActive(true);
        moveingBodyMaxine.GoToPosition(new Vector3(-0.46f, -11.44f, 1f));

        while (moveingBodyMaxine.IsGoingToPosition())
            yield return null;

        List<Emote> lstEmotes = new List<Emote>();
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote2);
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote2);
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote2);

        moveingBodyMaxine.Speak(lstEmotes);

        yield return new WaitForSeconds(1f);

        movingBody.Speak(lstEmotes);

        yield return new WaitForSeconds(1.5f);

        moveingBodyMaxine.GoToPosition(new Vector3(3.83f, -9.34f, 1f));

        while (moveingBodyMaxine.IsGoingToPosition())
            yield return null;

        interaction_jeu.SetActive(true);

        StopCinematiqueRencontre();
    }

    private void StopCinematiqueRencontre()
    {
        movingBody.SetActive(true);
        currentState = ChapitreState.ApresRencontre;
    }

    #endregion

    #region Cinematique Entree Jeu

    private void StartCinematiqueDebutJeu()
    {
        currentState = ChapitreState.CinematiqueDebutJeu;

        movingBody.gameObject.SetActive(false);
        moveingBodyMaxine.gameObject.SetActive(false);

        StartCoroutine(coroutine_CinematiqueDebutJeu());
    }

    private IEnumerator coroutine_CinematiqueDebutJeu()
    {
        animatorFadePanel.SetTrigger("FadeIn");

        //On attend que l'animation termine
        while (!bFadeEnded)
            yield return null;

        animatorFadePanel.SetTrigger("FadeOut");
        animatorFadePanel.ResetTrigger("FadeIn");

        ImageGoal_1.gameObject.SetActive(true);
        ImageGoal_2.gameObject.SetActive(true);
        ImagePoint_1.gameObject.SetActive(true);
        ImagePoint_2.gameObject.SetActive(true);

        fete_groupe.gameObject.SetActive(false);
        jeu_groupe.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        StopCinematiqueDebutJeu();
    }

    private void StopCinematiqueDebutJeu()
    {
        currentState = ChapitreState.Jeu;
    }

    #endregion

    #region Cinematique Jeu Gagn�

    private void StartCinematiqueVictoireJeu()
    {
        currentState = ChapitreState.CinematiqueVictoireJeu;

        goTarget.SetActive(false);
        goBallonsHolder.SetActive(false);

        StartCoroutine(coroutine_CinematiqueVictoireJeu());
    }

    private IEnumerator coroutine_CinematiqueVictoireJeu()
    {
        particleSystemVictoire.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        particleSystemVictoire.gameObject.SetActive(true);

        yield return null;

        StopCinematiqueVictoireJeu();
    }

    private void StopCinematiqueVictoireJeu()
    {
        goPrix.SetActive(true);

        currentState = ChapitreState.ChoixPrix;
    }

    #endregion

    #region Choix Prix

    private IEnumerator coroutine_DescenteAraigneeFaux()
    {
        ToggleChoixPrixObjets(false);

        float fElapsedTime = 0;
        float fDuration = 0.5f;

        Vector3 vFilGroupe = fil_groupe.transform.position;
        Vector3 vFilGroupeDestination = new Vector3(vFilGroupe.x, vFilGroupe.y - 9f, vFilGroupe.z);

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(vFilGroupe, vFilGroupeDestination, (fElapsedTime / fDuration));
            fil_groupe.transform.position = vNewPos;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        List<Emote> lstEmotes = new List<Emote>();
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote1);

        araignee.Speak(lstEmotes);

        yield return new WaitForSeconds(1.5f);

        fElapsedTime = 0;
        vFilGroupe = fil_groupe.transform.position;
        vFilGroupeDestination = new Vector3(vFilGroupe.x, vFilGroupe.y + 9f, vFilGroupe.z);

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(vFilGroupe, vFilGroupeDestination, (fElapsedTime / fDuration));
            fil_groupe.transform.position = vNewPos;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        ToggleChoixPrixObjets(true);
    }

    private void ToggleChoixPrixObjets(bool bValue)
    {
        PrixFlamme.GetComponent<InteractableObject>().DisableHilight();
        PrixMasque.GetComponent<InteractableObject>().DisableHilight();
        PrixTotoro.GetComponent<InteractableObject>().DisableHilight();

        PrixFlamme.SetActive(bValue);
        PrixMasque.SetActive(bValue);
        PrixTotoro.SetActive(bValue);
    }

    #endregion

    #region Cinematique Fin Jeu

    private void StartCinematiqueFinJeu()
    {
        currentState = ChapitreState.CinematiqueFinJeu;

        StartCoroutine(coroutine_CinematiqueFinJeu());
    }

    private IEnumerator coroutine_CinematiqueFinJeu()
    {
        //Fondu vers l'�cran de fin
        animatorFadePanel.SetTrigger("FadeIn");

        while (!bFadeEnded)
            yield return null;

        animatorFadePanel.SetTrigger("FadeOut");
        animatorFadePanel.ResetTrigger("FadeIn");

        ImageGoal_1.gameObject.SetActive(false);
        ImageGoal_2.gameObject.SetActive(false);
        ImagePoint_1.gameObject.SetActive(false);
        ImagePoint_2.gameObject.SetActive(false);

        fete_groupe.gameObject.SetActive(true);
        jeu_groupe.gameObject.SetActive(false);

        movingBody.gameObject.SetActive(true);
        moveingBodyMaxine.gameObject.SetActive(true);

        peluche.gameObject.SetActive(true);

        interaction_jeu.SetActive(false);

        movingBody.gameObject.transform.position = new Vector3(-3.72f, -8.98f, 1);
        moveingBodyMaxine.gameObject.transform.position = new Vector3(3.83f, -9.34f, 1);

        //Le fondu est termin� -> d�but de l'animation
        movingBody.GoToPosition(new Vector3(-0.71f, -9.34f, 1));

        while (moveingBodyMaxine.IsGoingToPosition())
            yield return null;

        peluche.sortingOrder = 5;

        moveingBodyMaxine.GoToPosition(new Vector3(3.5f, -11.27f, 1));
        movingBody.GoToPosition(new Vector3(-0.11f, -11.27f, 1));

        while (moveingBodyMaxine.IsGoingToPosition() || movingBody.IsGoingToPosition())
        {
            Vector3 rootPosition = movingBody.GetRootPosition();
            peluche.gameObject.transform.position = new Vector3(rootPosition.x, rootPosition.y + 2f, rootPosition.z);
            yield return null;
        }

        movingBody.ExtendArms();

        //Passage de la peluche
        float fElapsedTime = 0;
        float fDuration = 0.5f;

        Vector3 vCurrentPeluchePosition = peluche.transform.position;

        Vector3 rootMaxinePosition = moveingBodyMaxine.GetRootPosition();
        Vector3 vPelucheDestination = new Vector3(rootMaxinePosition.x, rootMaxinePosition.y + 2f, rootMaxinePosition.z);

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(vCurrentPeluchePosition, vPelucheDestination, (fElapsedTime / fDuration));
            peluche.transform.position = vNewPos;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        movingBody.UnextendedArms();

        List<Emote> lstEmotes = new List<Emote>();
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote2);
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote2);

        moveingBodyMaxine.Speak(lstEmotes);
        movingBody.Speak(lstEmotes);

        yield return new WaitForSeconds(2f);

        moveingBodyMaxine.GoToPosition(new Vector3(23.81f, -11.23f, 1));

        while (moveingBodyMaxine.IsGoingToPosition())
        {
            Vector3 rootPosition = moveingBodyMaxine.GetRootPosition();
            peluche.gameObject.transform.position = new Vector3(rootPosition.x, rootPosition.y + 2f, rootPosition.z);
            yield return null;
        }

        StopCinematiqueFinJeu();
    }

    private void StopCinematiqueFinJeu()
    {
        //Change scene chapitre 8
    }

    #endregion
}
