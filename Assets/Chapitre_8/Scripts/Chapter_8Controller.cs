using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_8Controller : CommonController
{
    [SerializeField] private MovingBody movingBodyMaxine;
    [SerializeField] private GameObject goPerso_Animation_Maxine;
    [SerializeField] private GameObject goPerso_Animation;
    [SerializeField] private GameObject goPerso_AraigneeAnimation;
    [SerializeField] private GameObject goMaxineInteraction;
    [SerializeField] private AraigneeController goAraignee;
    [SerializeField] private ParticleSystem particleSystemDisappear;

    [SerializeField] private SpeakingBody speakingBody;
    [SerializeField] private SpeakingBody speakingBody_maxine;

    protected override void ChapterInteraction(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Maxine:
                {
                    goMaxineInteraction.gameObject.SetActive(false);
                    StartCinematique(Cinematiques.Chapitre8_Fin);
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
    }

    protected override void StartChapterCinematique(Cinematiques cinematique)
    {
        movingBody.SetActive(false);

        switch (cinematique)
        {
            case Cinematiques.Chapitre8_Fin:
                {
                    StartCinematiqueFin();
                }
                break;
        }

        movingBody.SetActive(true);
    }

    #region Cinematique FIN

    private void StartCinematiqueFin()
    {
        StartCoroutine(coroutine_CinematiqueFin());
    }

    private IEnumerator coroutine_CinematiqueFin()
    {
        movingBody.SetActive(false);

        movingBody.GoToPosition(new Vector3(10.4f, -5.86f, 1));
        while (movingBody.IsGoingToPosition())
            yield return null;

        movingBody.GoToPosition(new Vector3(10.4f - 0.01f, -5.86f, 1));
        while (movingBody.IsGoingToPosition())
            yield return null;

        yield return new WaitForSeconds(0.5f);

        Vector3 vMaxinePos = movingBodyMaxine.transform.position;
        movingBodyMaxine.GoToPosition(new Vector3(vMaxinePos.x + 0.01f, vMaxinePos.y, 1));
        while (movingBodyMaxine.IsGoingToPosition())
            yield return null;

        List<Emote> lstEmotes = new List<Emote>();
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote2);
        lstEmotes.Add(Emote.Emote1);
        lstEmotes.Add(Emote.Emote2);

        movingBody.Speak(lstEmotes);
        movingBodyMaxine.Speak(lstEmotes);

        yield return new WaitForSeconds(2f);

        movingBody.GoToPosition(new Vector3(-4.76f, -10.54f, 1));
        movingBodyMaxine.GoToPosition(new Vector3(-8.31f, -11f, 1));

        while (movingBodyMaxine.IsGoingToPosition() || movingBody.IsGoingToPosition())
            yield return null;

        //Banc
        movingBodyMaxine.gameObject.SetActive(false);
        goPerso_Animation_Maxine.SetActive(true);
        Perso_Animation_1_Controller ctrl_maxine = goPerso_Animation_Maxine.GetComponent<Perso_Animation_1_Controller>();
        ctrl_maxine.animation_MainReverse();

        movingBody.gameObject.SetActive(false);
        goPerso_Animation.SetActive(true);
        Perso_Animation_1_Controller ctrl = goPerso_Animation.GetComponent<Perso_Animation_1_Controller>();
        ctrl.animation_MainReverse();

        while (!ctrl_maxine.animation_MainReverseIsFinished() || !ctrl.animation_MainReverseIsFinished())
            yield return null;

        speakingBody.Speak(lstEmotes, -1f, 3f, BodyDirection.Gauche);
        speakingBody_maxine.Speak(lstEmotes, -1f, 3f, BodyDirection.Droite);

        goAraignee.animation_araignee_chapitre_8_arrivee();

        while (!goAraignee.animation_araignee_chapitre_8_arrivee_IsFinished())
            yield return null;

        yield return new WaitForSeconds(2f);

        goPerso_AraigneeAnimation.SetActive(true);

        yield return new WaitForSeconds(2f);

        goAraignee.FadeOut();
        particleSystemDisappear.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        StopCinematiqueFin();
    }

    private void StopCinematiqueFin()
    {
        
    }

    #endregion
}
