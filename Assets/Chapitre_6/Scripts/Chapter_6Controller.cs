using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_6Controller : CommonController
{
    private enum ChapitreState
    {
        Initial,
        Araignee
    }

    private ChapitreState currentState = ChapitreState.Initial;

    //Jeu araignee
    [SerializeField] private AraigneeController araignee;
    [SerializeField] private GameObject goRootAraignee;
    [SerializeField] private LineRenderer lineRenderer;
    private Vector3 ClickStartingPosition = Vector3.zero;
    private bool IsClicked = false;

    //Obstacles
    [SerializeField] private GameObject goHalo_1;
    [SerializeField] private GameObject goHalo_2;
    //CHangement de sprite tous les fTimePerSprite secondes
    private SpriteRenderer sprtRenderer;
    private float fTimePerSprite = 0.25f;
    private float fTimeCount = 0;
    private GameObject goCurrentHalo;

    private float CameraMoveSpeed = 5f;
    private float XMinCamera =  -20.46f;
    private float XMaxCamera = -0.27f;

    private GameObject TutoLaunch;

    protected override void ChildStart()
    {
        araignee.ShowHideSprite(false);

        StartCinematique(Cinematiques.Chapitre6_Debut);
    }

    protected override void ChildUpdate()
    {
        //Changement de sprite des obstacles
        if (fTimeCount > fTimePerSprite)
        {
            if (goCurrentHalo == goHalo_1)
            {
                goHalo_2.SetActive(true);
                goCurrentHalo = goHalo_2;
                goHalo_1.SetActive(false);
            }
            else
            {
                goHalo_1.SetActive(true);
                goCurrentHalo = goHalo_1;
                goHalo_2.SetActive(false);
            }

            fTimeCount = 0;
        }

        fTimeCount += Time.deltaTime;

        if (currentState == ChapitreState.Araignee)
        {
            Vector3 vMousePos = GetWorldMousePosition();

            if (Input.GetMouseButtonDown(0))
            {
                lineRenderer.enabled = true;
                InitMovement(vMousePos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                lineRenderer.enabled = false;
                StartMovement(vMousePos);
            }

            if (IsClicked)
            {
                Vector3 vDifferrence = vMousePos - ClickStartingPosition;

                float fDistance = Mathf.Abs(vDifferrence.magnitude);
                if (fDistance > 10)
                    fDistance = 10;

                fDistance /= 10;

                Color c = new Color(1, 1 - fDistance, 1 - fDistance);

                lineRenderer.SetPosition(0, ClickStartingPosition);
                lineRenderer.SetPosition(1, vMousePos);
                lineRenderer.startColor = c;
                lineRenderer.endColor = c;
            }

            float fNewCameraX = Mathf.Lerp(cam.transform.position.x, goRootAraignee.transform.position.x, CameraMoveSpeed * Time.deltaTime);
            
            if(fNewCameraX > XMinCamera && fNewCameraX < XMaxCamera)
                cam.transform.position = new Vector3(fNewCameraX, cam.transform.position.y, cam.transform.position.z);

            //Déclenchement de la fin du chapitre
            Vector3 vRootPosition = araignee.GetRootPosition();
            if (vRootPosition.x < -26.27f && vRootPosition.y > -2.58f)
            {
                SmoothChangeScene(Scenes.Chapitre7);
            }
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
            case Cinematiques.Chapitre6_Debut:
                {
                    StartCinematiqueInitiale();
                }
                break;
        }

        movingBody.SetActive(true);
    }

    #region Jeu Araignee

    private void InitMovement(Vector3 vPos)
    {
        IsClicked = true;
        ClickStartingPosition = vPos;
    }

    private void StartMovement(Vector3 vPos)
    {
        IsClicked = false;

        Vector3 vDifferrence = vPos - ClickStartingPosition;

        float fDistance = Mathf.Abs(vDifferrence.magnitude);
        if (fDistance > 5)
            fDistance = 5;

        araignee.AddForce(vDifferrence, fDistance);

        if(fDistance > 1 && TutoLaunch != null)
        {
            StopTuto(TutoLaunch);
        }

        ClickStartingPosition = Vector3.zero;
    }

    public Vector3 GetWorldMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.GetPoint(1);
    }

    #endregion

    #region Cinématique initiale

    private void StartCinematiqueInitiale()
    {
        StartCoroutine(coroutine_CinematiqueInitiale());
    }

    private IEnumerator coroutine_CinematiqueInitiale()
    {
        yield return new WaitForSeconds(0f);

        MusicController.GetInstance().ChangeClip(MusicController.Clips.Araignee);

        //araignee.ShowHideSprite(true);
        araignee.StartAnimation(AraigneeController.AnimationsAraignee.Chapitre6_SautBus);

        while (!araignee.IsAnimationFinished(AraigneeController.AnimationsAraignee.Chapitre6_SautBus))
            yield return null;

        //araignee.RestoreDynamicBodyPosition();

        TutoLaunch = PlayTuto(Tutoriel.Launch, new Vector3(0.97f, -2.66f, -8.61f));

        StopCinematiqueInitiale();
    }

    private void StopCinematiqueInitiale()
    {
        //araignee.RestoreDynamicBodyPosition();
        currentState = ChapitreState.Araignee;
        araignee.ToggleAnimator(false);
        araignee.InitDynamicBody();
    }

    #endregion
}
