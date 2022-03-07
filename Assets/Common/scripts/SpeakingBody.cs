using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakingBody : MonoBehaviour
{
    public static List<Sprite> lstSprites;//Liste des toutes les emotes
    private bool bIsSpeaking = false;

    // Start is called before the first frame update
    void Start()
    {
        if (lstSprites == null)
            LoadEmotes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void LoadEmotes()
    {
        lstSprites = new List<Sprite>();

        lstSprites.Add((Sprite)Resources.Load<Sprite>("EmoteSprite/emote_1"));
        lstSprites.Add((Sprite)Resources.Load<Sprite>("EmoteSprite/emote_2"));
    }

    private void SpawnEmote(Emote emote, float fXOffset, float fYOffset, BodyDirection bodyDirection = BodyDirection.Droite)
    {
        GameObject goEmote = (GameObject)Instantiate(Resources.Load("emote_holder"));
        goEmote.transform.position = new Vector3(fXOffset + transform.position.x, fYOffset + transform.position.y, -1f);

        SpriteRenderer sprtRenderer = goEmote.GetComponentInChildren<SpriteRenderer>();

        int nEmote = (int)emote;

        if (nEmote < 0 || nEmote >= lstSprites.Count) return;

        sprtRenderer.sprite = lstSprites[nEmote];

        Animator animatorEmote = goEmote.GetComponentInChildren<Animator>();

        if (bodyDirection == BodyDirection.Gauche)
        {
            animatorEmote.SetTrigger("Anim_2");
        }
        else
        {
            animatorEmote.SetTrigger("Anim_1");
        }
    }

    public void Speak(List<Emote> emotes, float fXOffset, float fYOffset, BodyDirection bodyDirection = BodyDirection.Droite, float fDelay = 0.5f)
    {
        if (lstSprites == null) LoadEmotes();

        if (bIsSpeaking) return;

        StartCoroutine(coroutine_Speak(emotes, fXOffset, fYOffset, bodyDirection));
    }

    public void Speak(Emote emote, float fXOffset, float fYOffset, BodyDirection bodyDirection = BodyDirection.Droite, float fDelay = 0.5f)
    {
        if (lstSprites == null) LoadEmotes();

        if (bIsSpeaking) return;

        List<Emote> lstEmotes = new List<Emote>();
        lstEmotes.Add(emote);

        Speak(lstEmotes, fXOffset, fYOffset, bodyDirection);
    }

    private IEnumerator coroutine_Speak(List<Emote> emotes, float fXOffset, float fYOffset, BodyDirection bodyDirection = BodyDirection.Droite, float fDelay = 0.5f)
    {
        bIsSpeaking = true;

        foreach (Emote emote in emotes)
        {
            SpawnEmote(emote, fXOffset, fYOffset, bodyDirection);
            yield return new WaitForSeconds(fDelay);
        }

        bIsSpeaking = false;
    }

    public bool IsSpeaking()
    {
        return bIsSpeaking;
    }
}
