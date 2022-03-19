using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AraigneeController : MonoBehaviour
{
    [SerializeField] private DynamicBodyController dynamicBodyController;
    [SerializeField] private Rigidbody2D rb2d_root;//root (racine du rig) de l'araignee
    [SerializeField] private List<GameObject> lstSprites;//Liste des sprites composants l'araignee
    [SerializeField] private SpeakingBody speakingCtrl;
    [SerializeField] private Animator holderAnimator;//Animator de l'objet parent

    public enum AnimationsAraignee
    {
        [Description("SautDansSac")] SautDansSac,
        [Description("SautFil")] SautFil,
        [Description("chapitre_3_SoriteEcran")] Chapitre3_SortieEcran,
        [Description("chapitre_3_FuiteCouloir")] Chapitre3_FuiteCouloir,
        [Description("chapitre_6_SautBus")] Chapitre6_SautBus,
        [Description("araignee_chapitre_8_arrivee")] Chapitre8_Arrivee,
        [Description("chapitre_3_placard")] Chapitre3_Placard,
        [Description("SautsTitre")] SautsTitre
    }

    private Dictionary<AnimationsAraignee, bool> dicAnimationsFinished = new Dictionary<AnimationsAraignee, bool>();

    void Start()
    {
        LoadAnimations();
    }

    void Update()
    {
        
    }

    private void LoadAnimations()
    {
        if (dicAnimationsFinished.Count > 0) return;

        foreach (AnimationsAraignee anims in (AnimationsAraignee[])Enum.GetValues(typeof(AnimationsAraignee)))
        {
            dicAnimationsFinished.Add(anims, false);
        }
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

    public void StartAnimation(AnimationsAraignee anim)
    {
        LoadAnimations();

        holderAnimator.SetTrigger(CommonController.GetEnumDescription(anim));
    }

    public bool IsAnimationFinished(AnimationsAraignee anim)
    {
        return dicAnimationsFinished[anim];
    }

    public void AnimationFinished(AnimationsAraignee anim)
    {
        dicAnimationsFinished[anim] = true;
    }

    public void ResetAllTriggers()
    {
        holderAnimator.ResetTrigger("SautDansSac");
        holderAnimator.ResetTrigger("SautFil");
        holderAnimator.ResetTrigger("chapitre_3_SoriteEcran");
        holderAnimator.ResetTrigger("chapitre_3_FuiteCouloir");
        holderAnimator.ResetTrigger("chapitre_6_SautBus");
        holderAnimator.ResetTrigger("araignee_chapitre_8_arrivee");
        holderAnimator.ResetTrigger("chapitre_3_placard");
        holderAnimator.ResetTrigger("SautsTitre");
    }

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
