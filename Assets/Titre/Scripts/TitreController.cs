using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitreController : CommonController
{
    [SerializeField] private Animator animatorAraigneePlafond;
    [SerializeField] private AraigneeController araigneeSaut;

    protected override void ChildStart()
    {
        StartCinematique(Cinematiques.Titre_Titre);
    }

    protected override void ChildUpdate()
    {

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
        araigneeSaut.StartAnimation(AraigneeController.AnimationsAraignee.SautsTitre);

        while (!araigneeSaut.IsAnimationFinished(AraigneeController.AnimationsAraignee.SautsTitre))
            yield return null;

        yield return new WaitForSeconds(2f);

        animatorAraigneePlafond.SetTrigger("Titre_1");

        yield return new WaitForSeconds(2f);

        animatorAraigneePlafond.ResetTrigger("Titre_1");
        animatorAraigneePlafond.SetTrigger("Titre_2");

        yield return new WaitForSeconds(2f);

        animatorAraigneePlafond.ResetTrigger("Titre_2");

        yield return new WaitForSeconds(5f);

        StopCinematiqueTitre();
    }

    private void StopCinematiqueTitre()
    {
        StartChapterCinematique(Cinematiques.Titre_Titre);
    }

    #endregion
}
