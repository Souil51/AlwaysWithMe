using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitreController : CommonController
{
    [SerializeField] private Animator animatorAraigneePlafond;
    [SerializeField] private AraigneeController araigneeSaut;

    protected override void ChildStart()
    {
        MusicController musicController = MusicController.GetInstance();
        musicController.ChangeClip(MusicController.Clips.Perso);

        StartCinematique(Cinematiques.Titre_Titre);

        PlayTuto(Tutoriel.Clic_Gauche, new Vector3(9.32f, -7.74f, 0), 2);
    }

    protected override void ChildUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            SmoothChangeScene(Scenes.Chapitre1);
        }
    }

    protected override void ChapterZoomOnObject(InteractableObject interactionObject)
    {
        interactionObject.EnterZoom();
    }

    protected override void ChapterLeaveZoomOnObject(InteractableObject interactionObject)
    {
        interactionObject.LeaveZoom();
    }

    protected override void StartChapterCinematique(Cinematiques cinematique)
    {
        if (movingBody != null) movingBody.SetActive(false);

        switch (cinematique)
        {
            case Cinematiques.Titre_Titre:
                {
                    StartCinematiqueTitre();
                }
                break;
        }

        if (movingBody != null) movingBody.SetActive(true);
    }

    #region Cinematique Credit

    private void StartCinematiqueTitre()
    {
        StartCoroutine(coroutine_CinematiqueTitre());
    }

    private IEnumerator coroutine_CinematiqueTitre()
    {
        yield return new WaitForSeconds(5f);

        araigneeSaut.StartAnimation(AraigneeController.AnimationsAraignee.SautsTitre);

        while (!araigneeSaut.IsAnimationFinished(AraigneeController.AnimationsAraignee.SautsTitre))
            yield return null;

        yield return new WaitForSeconds(4f);

        animatorAraigneePlafond.SetTrigger("Titre_2");

        yield return new WaitForSeconds(4f);

        animatorAraigneePlafond.ResetTrigger("Titre_2");
        animatorAraigneePlafond.SetTrigger("Titre_1");

        yield return new WaitForSeconds(4f);

        animatorAraigneePlafond.ResetTrigger("Titre_1");

        yield return new WaitForSeconds(5f);

        StopCinematiqueTitre();
    }

    private void StopCinematiqueTitre()
    {
        animatorAraigneePlafond.ResetTrigger("Titre_1");
        animatorAraigneePlafond.ResetTrigger("Titre_2");
        araigneeSaut.ResetAllTriggers();

        StartChapterCinematique(Cinematiques.Titre_Titre);
    }

    #endregion
}
