using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : CommonController
{
    [SerializeField] private GameObject goLettreTexte_1;
    [SerializeField] private GameObject goLettreTexte_2;
    [SerializeField] private GameObject goLettreTexte_3;
    [SerializeField] private GameObject goFilGroupe;
    [SerializeField] private GameObject goAnimationGroupe;
    [SerializeField] private GameObject goAraignee;

    [SerializeField] private GameObject goCreditTitre;
    [SerializeField] private GameObject goCreditSousTitre;
    [SerializeField] private GameObject goCreditsMoi;
    [SerializeField] private GameObject goCreditsSoftware;
    [SerializeField] private GameObject goCreditsThanks;

    [SerializeField] private GameObject particle_1;
    [SerializeField] private GameObject particle_2;
    [SerializeField] private GameObject particle_3;

    protected override void ChildStart()
    {
        StartCinematique(Cinematiques.Credits_Credits);
    }

    protected override void ChildUpdate()
    {
        
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
        if(movingBody != null) movingBody.SetActive(false);

        switch (cinematique)
        {
            case Cinematiques.Credits_Credits:
                {
                    StartCinematiqueCredits();
                }
                break;
        }

        if (movingBody != null) movingBody.SetActive(true);
    }

    #region Cinematique Credit

    private void StartCinematiqueCredits()
    {
        StartCoroutine(coroutine_CinematiqueCredits());
    }

    private IEnumerator coroutine_CinematiqueCredits()
    {
        yield return new WaitForSeconds(2f);

        SpriteRenderer sprtTexteTitre = goCreditTitre.GetComponent<SpriteRenderer>();

        float fElapsedTime = 0;
        float fDuration = 0.5f;

        while (fElapsedTime < fDuration)
        {
            float fNewAlpha = Mathf.Lerp(0, 1, (fElapsedTime / fDuration));
            sprtTexteTitre.color = new Color(sprtTexteTitre.color.r, sprtTexteTitre.color.g, sprtTexteTitre.color.b, fNewAlpha);

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        sprtTexteTitre.color = new Color(sprtTexteTitre.color.r, sprtTexteTitre.color.g, sprtTexteTitre.color.b, 1);

        SpriteRenderer sprtTexteSousTitre = goCreditSousTitre.GetComponent<SpriteRenderer>();
        SpriteRenderer sprtTexteMoi = goCreditsMoi.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        fElapsedTime = 0;

        while (fElapsedTime < fDuration)
        {
            float fNewAlpha = Mathf.Lerp(0, 1, (fElapsedTime / fDuration));
            sprtTexteSousTitre.color = new Color(sprtTexteSousTitre.color.r, sprtTexteSousTitre.color.g, sprtTexteTitre.color.b, fNewAlpha);
            sprtTexteMoi.color = new Color(sprtTexteMoi.color.r, sprtTexteMoi.color.g, sprtTexteMoi.color.b, fNewAlpha);

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        sprtTexteSousTitre.color = new Color(sprtTexteSousTitre.color.r, sprtTexteSousTitre.color.g, sprtTexteSousTitre.color.b, 1);
        sprtTexteMoi.color = new Color(sprtTexteMoi.color.r, sprtTexteMoi.color.g, sprtTexteMoi.color.b, 1);

        yield return new WaitForSeconds(2f);

        goFilGroupe.SetActive(true);
        goAnimationGroupe.GetComponent<Animator>().enabled = true;

        yield return new WaitForSeconds(3.5f);

        fElapsedTime = 0;

        while (fElapsedTime < fDuration)
        {
            float fNewAlpha = Mathf.Lerp(1, 0, (fElapsedTime / fDuration));
            sprtTexteSousTitre.color = new Color(sprtTexteSousTitre.color.r, sprtTexteSousTitre.color.g, sprtTexteTitre.color.b, fNewAlpha);
            sprtTexteMoi.color = new Color(sprtTexteMoi.color.r, sprtTexteMoi.color.g, sprtTexteMoi.color.b, fNewAlpha);

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        sprtTexteSousTitre.color = new Color(sprtTexteSousTitre.color.r, sprtTexteSousTitre.color.g, sprtTexteTitre.color.b, 0);
        sprtTexteMoi.color = new Color(sprtTexteMoi.color.r, sprtTexteMoi.color.g, sprtTexteMoi.color.b, 0);

        SpriteRenderer sprtSoftware = goCreditsSoftware.transform.Find("Sprite").GetComponent<SpriteRenderer>();

        fElapsedTime = 0;

        while (fElapsedTime < fDuration)
        {
            float fNewAlpha = Mathf.Lerp(0, 1, (fElapsedTime / fDuration));
            sprtSoftware.color = new Color(sprtSoftware.color.r, sprtSoftware.color.g, sprtSoftware.color.b, fNewAlpha);

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        sprtSoftware.color = new Color(sprtSoftware.color.r, sprtSoftware.color.g, sprtSoftware.color.b, 1);

        yield return new WaitForSeconds(2f);

        fElapsedTime = 0;

        while (fElapsedTime < fDuration)
        {
            float fNewAlpha = Mathf.Lerp(1, 0, (fElapsedTime / fDuration));
            sprtTexteTitre.color = new Color(sprtTexteTitre.color.r, sprtTexteTitre.color.g, sprtTexteTitre.color.b, fNewAlpha);
            sprtSoftware.color = new Color(sprtSoftware.color.r, sprtSoftware.color.g, sprtSoftware.color.b, fNewAlpha);

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        sprtTexteTitre.color = new Color(sprtTexteTitre.color.r, sprtTexteTitre.color.g, sprtTexteTitre.color.b, 0);
        sprtSoftware.color = new Color(sprtSoftware.color.r, sprtSoftware.color.g, sprtSoftware.color.b, 0);

        goAnimationGroupe.GetComponent<Animator>().enabled = false;
        goAraignee.transform.Find("root").GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(2f);

        fElapsedTime = 0;
        SpriteRenderer sprtThanks = goCreditsThanks.GetComponent<SpriteRenderer>();

        while (fElapsedTime < fDuration)
        {
            float fNewAlpha = Mathf.Lerp(0, 1, (fElapsedTime / fDuration));
            sprtThanks.color = new Color(sprtThanks.color.r, sprtThanks.color.g, sprtThanks.color.b, fNewAlpha);

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        sprtThanks.color = new Color(sprtThanks.color.r, sprtThanks.color.g, sprtThanks.color.b, 1);

        StopCinematiqueCredits();
    }

    private void StopCinematiqueCredits()
    {
        //Change scene chapitre 8
    }

    #endregion
}
