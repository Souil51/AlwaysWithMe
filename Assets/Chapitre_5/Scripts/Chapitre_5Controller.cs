using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapitre_5Controller : CommonController
{
    [SerializeField] private Perso_Animation_1_Controller Perso_Animation_1;
    [SerializeField] private GameObject goBus;
    [SerializeField] private GameObject goEntreeBus;

    protected override void ChildStart()
    {
        StartCinematiqueInitial();

        MusicController.GetInstance().ChangeClip(MusicController.Clips.Perso);
    }

    protected override void ChapterInteraction(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.AttenteBus:
                {
                    StartCinematique(Cinematiques.Chapitre5_Bus);
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
            case Cinematiques.Chapitre5_Initial:
                {
                    StartCinematiqueInitial();
                }
                break;
            case Cinematiques.Chapitre5_Bus:
                {
                    StartCinematiqueBus();
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

        movingBody.GoToPosition(new Vector3(15.36f, -4.17f, 1f));

        while (movingBody.IsGoingToPosition())
            yield return null;

        StopCinematiqueInitial();
    }

    private void StopCinematiqueInitial()
    {
        StopCinematique();
    }

    #endregion

    #region Cinématique bus

    private void StartCinematiqueBus()
    {
        StartCoroutine(coroutine_CinematiqueBus());
    }

    private IEnumerator coroutine_CinematiqueBus()
    {
        movingBody.StopMoving();

        yield return new WaitForSeconds(0.5f);

        movingBody.GoToPosition(new Vector3(7.17f, -3.40f, 1));

        while (movingBody.IsGoingToPosition())
            yield return null;

        movingBody.StopMoving();
        movingBody.SetActive(false);

        movingBody.gameObject.SetActive(false);
        Perso_Animation_1.gameObject.SetActive(true);

        Perso_Animation_1.StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.Chapitre5_PersoAnimation);

        while (!Perso_Animation_1.IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.Chapitre5_PersoAnimation))
            yield return null;

        //Attente du bus
        yield return new WaitForSeconds(2f);
        
        //Arrivée du bus
        float fElapsedTime = 0;
        float fDuration = 1f;
        Vector3 vBusCurrentPos = goBus.transform.position;
        Vector3 vBusDestination = new Vector3(vBusCurrentPos.x - 35.86f, -4.12f, 0);

        Vector3 vEntreeBusCurrentPos = goEntreeBus.transform.position;
        Vector3 vEntreeBusDestination = new Vector3(vEntreeBusCurrentPos.x - 35.86f, -0.75f, 0);

        MusicController.GetInstance().PlaySoundLoop();

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(vBusCurrentPos, vBusDestination, (fElapsedTime / fDuration));
            goBus.transform.position = vNewPos;

            Vector3 vNewPosEntree = Vector3.Lerp(vEntreeBusCurrentPos, vEntreeBusDestination, (fElapsedTime / fDuration));
            goEntreeBus.transform.position = vNewPosEntree;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        //Mouvement vers le bus
        Perso_Animation_1.ResetAnimatorTrigger();
        Perso_Animation_1.StartAnimation(Perso_Animation_1_Controller.AnimationsPerso.Chapitre5_PersoAnimationReverse);

        while (!Perso_Animation_1.IsAnimationFinished(Perso_Animation_1_Controller.AnimationsPerso.Chapitre5_PersoAnimationReverse))
            yield return null;

        movingBody.gameObject.SetActive(true);
        Perso_Animation_1.gameObject.SetActive(false);

        movingBody.GoToPosition(new Vector3(-7.92f, -8.45f, 1));

        while (movingBody.IsGoingToPosition())
            yield return null;

        yield return new WaitForSeconds(0.5f);

        goEntreeBus.GetComponent<SpriteRenderer>().sortingOrder = 2;

        PlaySound(Sound.PorteBus);

        yield return new WaitForSeconds(0.5f);

        movingBody.GoToPosition(new Vector3(16.09f, -8.45f, 1));

        while (movingBody.IsGoingToPosition())
            yield return null;

        movingBody.GoToPosition(new Vector3(movingBody.transform.position.x - 0.01f, -9.52f, 1));

        yield return new WaitForSeconds(0.25f);

        while (movingBody.IsGoingToPosition())
            yield return null;

        fElapsedTime = 0;
        vBusCurrentPos = goBus.transform.position;
        vBusDestination = new Vector3(vBusCurrentPos.x - 40f, -4.12f, 0);

        vEntreeBusCurrentPos = goEntreeBus.transform.position;
        vEntreeBusDestination = new Vector3(vEntreeBusCurrentPos.x - 40f, -0.75f, 0);

        Vector3 movingBodyCurrentPos = movingBody.transform.position;
        Vector3 movingBodyDestination = new Vector3(movingBodyCurrentPos.x - 40f, -9.52f, 0);

        while (fElapsedTime < fDuration)
        {
            Vector3 vNewPos = Vector3.Lerp(vBusCurrentPos, vBusDestination, (fElapsedTime / fDuration));
            goBus.transform.position = vNewPos;

            Vector3 vNewPosEntree = Vector3.Lerp(vEntreeBusCurrentPos, vEntreeBusDestination, (fElapsedTime / fDuration));
            goEntreeBus.transform.position = vNewPosEntree;

            vNewPos = Vector3.Lerp(movingBodyCurrentPos, movingBodyDestination, (fElapsedTime / fDuration));
            movingBody.transform.position = vNewPos;

            fElapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        StopCinematiqueBus();
    }

    private void StopCinematiqueBus()
    {
        SmoothChangeScene(Scenes.Chapitre6);

        MusicController.GetInstance().StopSoundLoop();

        StopCinematique();
    }

    #endregion
}
