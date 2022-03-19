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

    [SerializeField] private GameObject spriteBodyMaxine;

    protected override void ChildStart()
    {
        MusicController.GetInstance().ChangeClip(MusicController.Clips.Perso);
    }

    protected override void ChapterInteraction(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Maxine:
                {
                    goMaxineInteraction.gameObject.SetActive(false);
                    spriteBodyMaxine.gameObject.SetActive(true);
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
        MusicController.GetInstance().ChangeClip(MusicController.Clips.Maxine);

        movingBody.SetActive(false);

        movingBody.GoToPosition(new Vector3(10.4f, -5.86f, 1));
        while (movingBody.IsGoingToPosition())
            yield return null;

        movingBody.GoToPosition(new Vector3(10.4f - 0.01f, -5.86f, 1));
        while (movingBody.IsGoingToPosition())
            yield return null;

        movingBody.SpeakRandom(6);
        yield return new WaitForSeconds(0.25f);
        movingBodyMaxine.SpeakRandom(6);

        while (movingBody.IsSpeaking() || movingBodyMaxine.IsSpeaking())
            yield return null;

        movingBody.GoToPosition(new Vector3(-4.76f, -10.54f, 1));
        movingBodyMaxine.GoToPosition(new Vector3(-8.31f, -11f, 1));

        while (movingBodyMaxine.IsGoingToPosition() || movingBody.IsGoingToPosition())
            yield return null;

        //Banc
        movingBodyMaxine.gameObject.SetActive(false);
        goPerso_Animation_Maxine.SetActive(true);
        Perso_Animation_1_Controller ctrl_maxine = goPerso_Animation_Maxine.GetComponent<Perso_Animation_1_Controller>();
        ctrl_maxine.StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.Chapitre8_MaxineAnimation);

        movingBody.gameObject.SetActive(false);
        goPerso_Animation.SetActive(true);
        Perso_Animation_1_Controller ctrl = goPerso_Animation.GetComponent<Perso_Animation_1_Controller>();
        ctrl.StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.Chapitre8_PersoAnimation);

        while (!ctrl_maxine.IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.Chapitre8_MaxineAnimation) 
            || !ctrl.IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.Chapitre8_PersoAnimation))
            yield return null;

        speakingBody.SpeakRandom(6, -1f, 3f, BodyDirection.Gauche);
        speakingBody_maxine.SpeakRandom(6, -1f, 3f, BodyDirection.Droite);

        goAraignee.StartAnimation(AraigneeController.AnimationsAraignee.Chapitre8_Arrivee);

        while (!goAraignee.IsAnimationFinished(AraigneeController.AnimationsAraignee.Chapitre8_Arrivee))
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
        SmoothChangeScene(Scenes.Credits);
    }

    #endregion
}
