using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacardController : MonoBehaviour
{
    private Coroutine coroutine = null;
    [SerializeField] private SpeakingBody speakingCtrl;

    // Start is called before the first frame update
    void Start()
    {
        coroutine = StartCoroutine(coroutine_SpawnSprites());
    }

    private IEnumerator coroutine_SpawnSprites()
    {
        while (true)
        {
            SpawnSprite();
            yield return new WaitForSeconds(1f);
        }
    }

    private void SpawnSprite()
    {
        speakingCtrl.Speak(Emote.Crr, 0, 0, BodyDirection.Gauche);

        MusicController.GetInstance().PlaySound(Sound.Placard, 0.5f);
    }
}
