using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AraigneeController : MonoBehaviour
{
    [SerializeField] private DynamicBodyController dynamicBodyController;
    [SerializeField] private Rigidbody2D rb2d_root;//root (racine du rig) de l'araignee
    [SerializeField] private List<GameObject> lstSprites;//Liste des sprites composants l'araignee
    [SerializeField] private SpeakingBody speakingCtrl;
    [SerializeField] private Animator holderAnimator;//Animator de l'objet parent

    //Variables permettant de savoir si une animation est terminée
    private bool bSautDansSacFinished = false;
    private bool bSautFilFinished = false;
    private bool bChapitre3SortieEcran = false;
    private bool bChapitre3FuiteCouloir = false;
    private bool bChapitre6SautBus = false;
    private bool bChapitre8Arrivee = false;
    private bool bChapitre3Placard = false;
    void Start()
    {
    }

    void Update()
    {
        
    }

    public void AddForce(Vector3 vDirection, float fCoefficien = 1f)
    {
        vDirection = new Vector2(vDirection.normalized.x, vDirection.normalized.y);
        dynamicBodyController.SetDynamicMode(vDirection * 100 * fCoefficien);
    }

    public void InitDynamicBody()
    {
        dynamicBodyController.InitBody();
    }

    public bool IsInit()
    {
        return dynamicBodyController.IsInit;
    }

    public void ShowHideSprite(bool bShow)
    {
        foreach(GameObject go in lstSprites)
        {
            go.SetActive(bShow);
        }
    }

    public void Speak(Emote emote)
    {
        List<Emote> lst = new List<Emote>();
        lst.Add(emote);

        speakingCtrl.Speak(lst, 0, 0, BodyDirection.Gauche);
    }

    public void Speak(List<Emote> emotes)
    {
        speakingCtrl.Speak(emotes, 0, 0, BodyDirection.Gauche);
    }

    public void SpeakRandom(int nombreEmotes)
    {
        speakingCtrl.SpeakRandom(nombreEmotes, 0, 0, BodyDirection.Gauche);
    }

    public bool IsSpeaking()
    {
        return speakingCtrl.IsSpeaking();
    }

    //Recentre la racine (root) : la position de la racine sera 0,0 et la position du parent sera l'ancienne de la racine
    public void RestoreDynamicBodyPosition()
    {
        Vector3 vCurrentPos = rb2d_root.transform.position;
        rb2d_root.transform.position = transform.position;
        transform.position = vCurrentPos;
    }

    public Vector3 GetRootPosition()
    {
        return rb2d_root.transform.position;
    }

    public void ToggleAnimator(bool bValue)
    {
        holderAnimator.enabled = bValue;
    }

    //Déplace toutes les cibles des pattes de l'araignee
    public void MoveAllTargets(float difference, float differenceX)
    {
        dynamicBodyController.MoveAllTargets(difference, differenceX);
    }

    #region animation SautDansSac

    public void animation_SautDansSac()
    {
        holderAnimator.SetTrigger("SautDansSac");
    }

    public void animation_SautDansSec_Finished()
    {
        bSautDansSacFinished = true;
    }

    public bool animation_SautDansSac_IsFinished()
    {
        return bSautDansSacFinished;
    }

    #endregion

    #region SautFil

    public void animation_SautFil()
    {
        holderAnimator.SetTrigger("SautFil");
    }

    public void animation_SautFil_Finished()
    {
        bSautFilFinished = true;
    }

    public bool animation_SautFil_IsFinished()
    {
        return bSautFilFinished;
    }

    #endregion

    #region chapitre_3_SortieEcran

    public void animation_chapitre_3_SortieEcran()
    {
        holderAnimator.SetTrigger("chapitre_3_SoriteEcran");
    }

    public void animation_chapitre_3_SortieEcran_Finished()
    {
        bChapitre3SortieEcran = true;
    }

    public bool animation_chapitre_3_SortieEcran_IsFinished()
    {
        return bChapitre3SortieEcran;
    }

    #endregion

    #region chapitre_3_FuiteCouloir

    public void animation_chapitre_3_FuiteCouloir()
    {
        holderAnimator.SetTrigger("chapitre_3_FuiteCouloir");
    }

    public void animation_chapitre_3_FuiteCouloir_Finished()
    {
        bChapitre3FuiteCouloir = true;
    }

    public bool animation_chapitre_3_FuiteCouloir_IsFinished()
    {
        return bChapitre3FuiteCouloir;
    }

    #endregion

    #region chapitre_6_SautBus

    public void animation_chapitre_6_SautBus()
    {
        holderAnimator.SetTrigger("chapitre_6_SautBus");
    }

    public void animation_chapitre__6_SautBus_Finished()
    {
        bChapitre6SautBus = true;
    }

    public bool animation_chapitre__6_SautBus_IsFinished()
    {
        return bChapitre6SautBus;
    }

    #endregion

    #region chapitre_8

    public void animation_araignee_chapitre_8_arrivee()
    {
        holderAnimator.SetTrigger("araignee_chapitre_8_arrivee");
    }

    public void animation_araignee_chapitre_8_arrivee_Finished()
    {
        bChapitre8Arrivee = true;
    }

    public bool animation_araignee_chapitre_8_arrivee_IsFinished()
    {
        return bChapitre8Arrivee;
    }

    #endregion

    #region Chapitre 3 Placard

    public void animation_araignee_chapitre_3_placard()
    {
        holderAnimator.SetTrigger("chapitre_3_placard");
    }

    public void animation_araignee_chapitre_3_placard_Finished()
    {
        bChapitre3Placard = true;
    }

    public bool animation_araignee_chapitre_3_placard_IsFinished()
    {
        return bChapitre3Placard;
    }

    #endregion

    #region Fade Out du sprite

    public void FadeOut()
    {
        StartCoroutine(coroutine_FadeOut());
    }

    private IEnumerator coroutine_FadeOut()
    {
        List<SpriteRenderer> lstSprtRenderer = new List<SpriteRenderer>();
        lstSprtRenderer.Add(transform.Find("Left_0").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("Left_1").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("Left_2").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("Left_3").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("Right_0").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("Right_1").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("Right_2").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("Right_3").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("Head").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("LeftEye").GetComponent<SpriteRenderer>());
        lstSprtRenderer.Add(transform.Find("RightEye").GetComponent<SpriteRenderer>());

        float fElapsedTime = 0;
        float fDuration = 1.5f;

        while (fElapsedTime < fDuration)
        {
            float fNewAlpha = Mathf.Lerp(1, 0, (fElapsedTime / fDuration));

            foreach(SpriteRenderer sprtR in lstSprtRenderer)
            {
                sprtR.color = new Color(sprtR.color.r, sprtR.color.g, sprtR.color.b, fNewAlpha);
            }

            fElapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    #endregion
}
