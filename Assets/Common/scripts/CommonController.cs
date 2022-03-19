using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Scenes 
{ 
    Titre = 0,
    Chapitre1 = 1, 
    Chapitre2 = 2,
    Chapitre3 = 3,
    Chapitre4 = 4,
    Chapitre5 = 5,
    Chapitre6 = 6,
    Chapitre7 = 7,
    Chapitre8 = 8,
    Credits = 9
}

public enum InteractionType 
{ 
    NotDefined,
    MoveCamera, 
    LeaveRoom, 
    Lamp_LightToggle, 
    Room_LightToggle, 
    Door, 
    AcceptMenu, 
    CancelMenu, 
    Ecran_Toggle,
    Ecran_Interaction,
    Placard_Interaction,
    Sortie,
    MoveCameraResetZoom,
    AttenteBus,
    DebutJeu,
    PrixFlamme,
    PrixMasque,
    PrixTotoro,
    Maxine
}

public enum ObjectType 
{ 
    NotDefined, 
    Lamp, 
    Menu, 
    Ecran,
    CarteMaxine,
    AttenteBus
}

public enum Emote 
{ 
    [Description("Croix")]Croix = 0,
    [Description("SmileySourire")] SmileySourire = 1,
    [Description("SmileyClinOeil")] SmileyClinOeil = 2,
    [Description("SmileyJoueRouge")] SmileyJoueRouge = 3,
    [Description("SmileyChoque")] SmileyChoque = 4,
    [Description("SmileyAppeti")] SmileyAppeti = 5,
    [Description("SmileyGene")] SmileyGene = 6,
    [Description("SmileyGrandSourire")] SmileyGrandSourire = 7,
    [Description("Calendrier")] Calendrier = 8,
    [Description("Cadeau")] Cadeau = 9,
    [Description("Chat")] Chat = 10,
    [Description("Soleil")] Soleil = 11,
    [Description("Crr")] Crr = 12,
}

public enum Cinematiques
{
    Chapitre1_Debut,
    Chapitre2_ArriveeMaxine,
    Chapitre2_Journee,
    Chapitre2_Boucle,
    Chapitre3_EntreePlacard,
    Chapitre3_SortiePlacard,
    Chapitre4_FuiteCouloir,
    Chapitre4_EntreeBanc,
    Chapitre4_Rencontre,
    Chapitre4_SautAraignee,
    Chapitre4_FinBanc,
    Chapitre5_Bus,
    Chapitre6_Debut,
    Chapitre7_Arrivee,
    Chapitre7_EntreeFete,
    Chapitre7_Rencontre,
    Chapitre7_EntreeJeu,
    Chapitre7_VictoireJeu,
    Chapitre7_FinJeu,
    Chapitre8_Fin,
    Credits_Credits,
    Titre_Titre
}

public enum Tutoriel
{
    Clic_Droit,
    Clic_Gauche,
    Hold_Droit,
    Hold_Gauche,
    Launch,
    Move,
    Drag,
    Check,
    Hold_Arrow_Down,
    Hold_Arrow_Up,
    Hold_Arrow_Right,
    Hold_Arrow_Left,
    Tuto_Clic_Droit_Back
}

public class CommonController : MonoBehaviour
{
    [SerializeField] protected Camera cam;
    [SerializeField] protected float CameraMoveTime = 0.5f;
    [SerializeField] protected MovingBody movingBody;//La plupart des chapitres ont un movingBody donc il est dans le controller
    [SerializeField] protected List<InteractableObject> lstInteractableObjects;//Liste de tous les objets interactables (si un objet n'est pas dans la liste, l'interaction ne fonctionnera pas)
    [SerializeField] protected Image fadePanel;//Panel utilisé pour les fondus
    [SerializeField] protected Animator animatorFadePanel;

    //Caméra
    private Vector3 CameraInitialPosition = new Vector3(0, 0, -10);//Position "standard" de la camera
    private Vector3 CameraDefaultPosition = new Vector3(0, 0, -10);//Position de la caméra au moment où on zoom
    private float CameraDefaultSize = 10.5f;//Taille de la caméra
    private bool bCameraIsMoving = false;

    //InterractableObject
    protected InteractableObject currentObject;

    //Click : utilisé pour ajouter un délai avant de bouger le personnage (pour ne pas bouger le perso directement si on click sur un objet interacatble)
    private float fDelayToMove = 0.1f;
    private float fCurrentClickedDelay = 0;
    private bool bStartMoveDelay = false;

    //Etats
    protected bool bFadeEnded = false;
    protected bool bMenuDisplayed = false;
    private bool bClickHasBeenInteraction = false;

    //Tuto
    protected List<GameObject> lstPlayingTuto = new List<GameObject>();

    //Scene
    private bool IsChangingScene = false;

    void Start()
    {
        CameraDefaultSize = cam.orthographicSize;

        foreach (InteractableObject obj in lstInteractableObjects)
        {
            obj.startInteractionDelegate += InteractableObject_startInteractionDelegate;
        }

        //Méthode virtuelle appelée par la classe fille du controller du chapitre
        ChildStart();
    }

    void Update()
    {
        //Pour empêcher de bouger quand on veut juste cliquer sur un object "interactable", il y a un délai avant de déclencher la marche du personnage
        if (bStartMoveDelay)
        {
            fCurrentClickedDelay += Time.deltaTime;

            if(fCurrentClickedDelay > fDelayToMove && !bClickHasBeenInteraction)
            {
                if(movingBody != null) movingBody.SetMoving(true);
                bStartMoveDelay = false;
                fCurrentClickedDelay = 0;
            }
        }

        if (Input.GetMouseButtonDown(0) && currentObject == null)
        {
            bStartMoveDelay = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            bClickHasBeenInteraction = false;
            bStartMoveDelay = false;
            fCurrentClickedDelay = 0;
            if (movingBody != null) movingBody.SetMoving(false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentObject != null)
            {
                if (currentObject.GetInteractionType() == InteractionType.MoveCamera || currentObject.GetInteractionType() == InteractionType.MoveCameraResetZoom)
                {
                    if (!bCameraIsMoving)
                    {
                        MoveCamera(CameraDefaultPosition, CameraDefaultSize, true);

                        ChapterLeaveZoomOnObject(currentObject);
                    }
                }
            }
        }

        //Méthode virtuelle appelée par la classe fille du controller du chapitre
        ChildUpdate();

        if (movingBody != null)
        {
            foreach (InteractableObject interObj in lstInteractableObjects)
            {
                float fDistance = interObj.transform.position.x > movingBody.transform.position.x ? interObj.transform.position.x - movingBody.transform.position.x : movingBody.transform.position.x - interObj.transform.position.x;
                interObj.SetIsTooFar(fDistance > 5.5f);
            }
        }
    }

    protected void InteractableObject_startInteractionDelegate(object sender, EventArgs e)
    {
        InteractableObject interObject = (InteractableObject)sender;

        if (bMenuDisplayed)
        {
            ObjectType objectType = interObject.GetObjectType();

            if (objectType != ObjectType.Menu)
                return;
        }

        bClickHasBeenInteraction = true;
        bool bCurrentObjectSet = currentObject != null;

        if (!interObject.IsInner())
            currentObject = interObject;

        InteractionType type = interObject.GetInteractionType();

        if (type == InteractionType.MoveCamera || type == InteractionType.MoveCameraResetZoom)
        {
            if (!bCurrentObjectSet)
            {
                CameraDefaultPosition = type == InteractionType.MoveCamera ? cam.transform.position : CameraInitialPosition;
                MoveCamera(interObject.transform.position, interObject.GetSizeCamera());

                if (movingBody != null)
                {
                    movingBody.StopMoving();
                    movingBody.SetActive(false);
                }

                ChapterZoomOnObject(interObject);
            }
        }
        else
        {
            ChapterInteraction(type);
            ClearInteraction(interObject);
        }
    }

    //Lance la coroutine de mouvement de la caméra
    protected void MoveCamera(Vector3 position, float size, bool bClearInteraction = false)
    {
        StartCoroutine(SmoothMoveCamera(new Vector3(position.x, position.y, cam.transform.position.z), size, bClearInteraction));
    }

    protected void ResetCameraPosition()
    {
        MoveCamera(CameraInitialPosition, 10.5f);
    }

    public void SetDefaultCameraPosition(Vector3 vPosition)
    {
        CameraDefaultPosition = vPosition;
    }

    //Supprime la référence à l'objet suivi et permet au joueur de bouger
    //L'objet en paramètre perme de savoir si on a cliqué sur un objet "interne" d'un object "interactable"
    protected void ClearInteraction(InteractableObject interObject = null)
    {
        if (interObject != null && interObject.IsInner()) return;

        if(movingBody != null) movingBody.SetActive(true);
        currentObject = null;
    }

    //Déplace la caméra en lissant le mouvement et en changeant la taille
    protected IEnumerator SmoothMoveCamera(Vector3 newPosition, float newSize, bool bClearInteraction)
    {
        bCameraIsMoving = true;
        float elapsedTime = 0;

        Vector3 vCurrentPos = cam.transform.position;
        float fCurrentSize = cam.orthographicSize;

        while (elapsedTime < CameraMoveTime)
        {
            cam.transform.position = Vector3.Lerp(vCurrentPos, newPosition, (elapsedTime / CameraMoveTime));
            cam.orthographicSize = Mathf.Lerp(fCurrentSize, newSize, (elapsedTime / CameraMoveTime));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        cam.transform.position = newPosition;
        cam.orthographicSize = newSize;

        if (bClearInteraction)
            ClearInteraction();

        bCameraIsMoving = false;
    }

    public bool IsTooFarFromMovingBody(Vector3 vPos)
    {
        if (movingBody == null)
            return false;
        else
        {
            float fMagnitude = (vPos - movingBody.transform.position).magnitude;
            return fMagnitude > 5.5f;
        }
    }

    protected void StartCinematique(Cinematiques cinematique)
    {
        if (movingBody != null)
        {
            movingBody.StopMoving();
            movingBody.SetActive(false);
        }

        StartChapterCinematique(cinematique);
    }

    protected void StopCinematique()
    {
        movingBody.SetActive(true);
    }

    #region Empty Virtual Methods

    protected virtual void ChapterInteraction(InteractionType type) { }

    protected virtual void ChapterZoomOnObject(InteractableObject interactionObject) { }

    protected virtual void ChapterLeaveZoomOnObject(InteractableObject interactionObject) { }

    protected virtual void ChildStart() { }

    protected virtual void ChildUpdate() { }

    protected virtual void StartChapterCinematique(Cinematiques cinematique) { }

    #endregion

    #region Scene Manager

    protected void SmoothChangeScene(Scenes sceneIndex)
    {
        if (IsChangingScene) return;

        IsChangingScene = true;

        StartCoroutine(coroutine_SmoothChangeScene(sceneIndex));
    }

    private void ChangeScene(Scenes sceneIndex)
    {
        SceneManager.LoadScene((int)sceneIndex);
    }

    protected IEnumerator coroutine_SmoothChangeScene(Scenes sceneIndex)
    {
        //On lance l'animation de FadeIn
        animatorFadePanel.SetTrigger("FadeIn");

        //On attend que l'animation termine
        while (!bFadeEnded)
            yield return null;

        ChangeScene(sceneIndex);
    }

    public void SetFadeEnded()
    {
        bFadeEnded = true;
    }

    #endregion

    #region Tuto

    protected GameObject PlayTuto(Tutoriel tuto, Vector3 vPos, float fScale = 1)
    {
        GameObject goTuto = (GameObject)Instantiate(Resources.Load("tuto_holder"));

        lstPlayingTuto.Add(goTuto);

        goTuto.transform.localScale *= fScale;

        goTuto.transform.position = vPos;
        TutorielController tutoCtrl = goTuto.GetComponent<TutorielController>();
        tutoCtrl.PlayAnimation(tuto);

        return goTuto;
    }

    protected void StopTuto(GameObject goTuto)
    {
        if (goTuto == null || lstPlayingTuto.Count == 0) return;

        TutorielController tutoCtrl = goTuto.GetComponent<TutorielController>();
        tutoCtrl.StopTutoriel();

        lstPlayingTuto.Remove(goTuto);
    }

    #endregion

    #region

    public static string GetEnumDescription(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (attributes != null && attributes.Any())
        {
            return attributes.First().Description;
        }

        return value.ToString();
    }

    #endregion
}
