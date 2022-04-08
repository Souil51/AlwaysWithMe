using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_8Controller : CommonController
{
    private enum State
    {
        Initial,
        Jeu,
        Fin
    }

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

    private State currentState = State.Initial;
    private float fTimeWithoutInteraction = 0;
    private float fTutoTimeElapsed = 0;
    private GameObject goTuto;

    protected override void ChildStart()
    {
        MusicController.GetInstance().ChangeClip(MusicController.Clips.Perso);

        StartCinematique(Cinematiques.Chapitre8_Initial);
    }

    protected override void ChildUpdate()
    {
        if (currentState == State.Jeu)
        {
            if (goTuto == null)
                fTimeWithoutInteraction += Time.deltaTime;
            else
                fTutoTimeElapsed += Time.deltaTime;

            if (goTuto == null && fTimeWithoutInteraction > 5)
            {
                fTimeWithoutInteraction = 0;
                goTuto = PlayTuto(Tutoriel.Clic_Gauche, new Vector3(5.82f, -0.8f, 0));
            }

            if(goTuto != null && fTutoTimeElapsed > 5)
            {
                fTutoTimeElapsed = 0;
                StopTuto(goTuto);
            }
        }
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
        switch (cinematique)
        {
            case Cinematiques.Chapitre8_Initial:
                {
                    StartCinematiqueInitial();
                }
                break;
            case Cinematiques.Chapitre8_Fin:
                {
                    StartCinematiqueFin();
                }
                break;
        }
    }

    #region Cinématique Initiale

    private void StartCinematiqueInitial()
    {
        movingBody.SetActive(false);

        StartCoroutine(coroutine_CinematiqueInitial());
    }

    private IEnumerator coroutine_CinematiqueInitial()
    {
        yield return new WaitForSeconds(1);

        movingBody.GoToPosition(new Vector3(17.9f, -11.45f, 1f));

        while (movingBody.IsGoingToPosition())
            yield return null;

        StopCinematiqueInitial();
    }

    private void StopCinematiqueInitial()
    {
        StopCinematique();
        currentState = State.Jeu;
    }

    #endregion

    #region Cinematique FIN

    private void StartCinematiqueFin()
    {
        currentState = State.Fin;
        if (goTuto != null) StopTuto(goTuto);

        StartCoroutine(coroutine_CinematiqueFin());
    }

    private IEnumerator coroutine_CinematiqueFin()
    {
        movingBody.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        movingBody.GoToPosition(new Vector3(10.4f, -5.86f, 1), 0.5f, false);
        while (movingBody.IsGoingToPosition())
            yield return null;

        movingBody.ChangeDirection(BodyDirection.Droite);

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

        speakingBody.SpeakRandom(12, -1f, 3f, BodyDirection.Gauche);

        yield return new WaitForSeconds(0.25f);

        speakingBody_maxine.SpeakRandom(12, -1f, 3f, BodyDirection.Droite);

        goAraignee.StartAnimation(AraigneeController.AnimationsAraignee.Chapitre8_Arrivee);

        while (!goAraignee.IsAnimationFinished(AraigneeController.AnimationsAraignee.Chapitre8_Arrivee))
            yield return null;

        yield return new WaitForSeconds(2f);

        goPerso_AraigneeAnimation.SetActive(true);

        goAraignee.FadeOut();

        yield return new WaitForSeconds(2f);

        MusicController.GetInstance().SmoothChangeVolume(0, 1f);

        yield return new WaitForSeconds(1f);

        StopCinematiqueFin();
    }

    private void StopCinematiqueFin()
    {
        SmoothChangeScene(Scenes.Credits);
        StopCinematique();
    }

    #endregion
}
