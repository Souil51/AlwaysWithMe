using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public enum Clips
    {
        Perso,
        Maxine,
        Araignee,
        Credits
    }

    [SerializeField] private AudioSource Source;

    [SerializeField] private AudioClip Theme_Perso;
    [SerializeField] private AudioClip Theme_Maxine;
    [SerializeField] private AudioClip Theme_Araignee;
    [SerializeField] private AudioClip Theme_Credits;

    [SerializeField] private List<AudioClip> ClipFX;
    [SerializeField] private AudioSource LoopFX;

    private GameObject goReference;
    private static MusicController _controller;
    private Coroutine coroutine_pitch;
    private static float LoopFX_Volume = 0.2f;

    public static MusicController GetInstance()
    {
        if(_controller == null)
        {
            GameObject goMusic = Instantiate(Resources.Load("MusicController")) as GameObject;
            goMusic.transform.position = Vector3.zero;
            _controller = goMusic.GetComponent<MusicController>();
            goMusic.GetComponent<AudioSource>().volume = CommonController.VOLUME_BASE;
        }

        return _controller;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (_controller == null)
            _controller = this;
    }

    public AudioClip GetAudioClip(Sound sound)
    {
        return ClipFX[(int)sound];
    }

    public void ChangeClip(Clips clip)
    {
        StartCoroutine(coroutine_SmoothChangeAudioClip(clip));
    }

    public void ChangeVolume(float fVolume)
    {
        if (fVolume < 0)
            Source.volume = 0;
        else if (fVolume > 1)
            Source.volume = 1;
        else
            Source.volume = fVolume;
    }

    public void SmoothChangeVolume(float fVolume, float fDuration = 1f)
    {
        StartCoroutine(coroutin_SmoothChangeVolume(fVolume, fDuration));
    }

    private IEnumerator coroutine_SmoothChangeAudioClip(Clips clip, float fDuration = 0.25f)
    {
        float fElapsedTime = 0;
        float fCurrentVolume = CommonController.VOLUME_BASE;

        while (fElapsedTime < fDuration && Source.volume > 0)
        {
            float fNewVolume = Mathf.Lerp(fCurrentVolume, 0, (fElapsedTime / fDuration));
            Source.volume = fNewVolume;

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        Source.volume = 0;

        switch (clip)
        {
            case Clips.Perso:
                Source.clip = Theme_Perso;
                break;
            case Clips.Maxine:
                Source.clip = Theme_Maxine;
                break;
            case Clips.Araignee:
                Source.clip = Theme_Araignee;
                break;
            case Clips.Credits:
                Source.clip = Theme_Credits;
                break;
        }

        Source.Play();

        fElapsedTime = 0;

        if (clip == Clips.Perso || clip == Clips.Credits)
            fCurrentVolume *= 1.5f;

        while (fElapsedTime < fDuration)
        {
            float fNewVolume = Mathf.Lerp(0, fCurrentVolume, (fElapsedTime / fDuration));
            Source.volume = fNewVolume;

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        Source.volume = fCurrentVolume;
    }

    private IEnumerator coroutin_SmoothChangeVolume(float fVolume, float fDuration)
    {
        float fElapsedTime = 0;
        float fCurrentVolume = Source.volume;

        while (fElapsedTime < fDuration)
        {
            float fNewVolume = Mathf.Lerp(fCurrentVolume, fVolume, (fElapsedTime / fDuration));
            Source.volume = fNewVolume;

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        Source.volume = fVolume;
    }

    #region FX

    public void SetGameObjectReference(GameObject go)
    {
        goReference = go;
    }

    public void PlaySound(Sound sound)
    {
        PlaySound(sound, 1f, Vector3.zero);
    }

    public void PlaySound(Sound sound, float volume)
    {
        PlaySound(sound, volume, Vector3.zero);
    }

    public void PlaySound(Sound sound, float volume, Vector3 position, bool forcePosition = false)
    {
        float fVolume = volume;

        switch (sound)
        {
            case Sound.Ballon: fVolume = volume * 0.2f; break;
            case Sound.MenuFermer: fVolume = volume * 0.5f; break;
            case Sound.Placard: fVolume = volume * 0.4f; break;
            case Sound.Tir: fVolume = volume * 0.2f; break;
            case Sound.JeuGagne: fVolume = volume * 0.3f; break;
            case Sound.BonnePeluche: fVolume = volume * 0.3f; break;
            case Sound.MauvaisePeluche: fVolume = volume * 0.3f; break;
            case Sound.Discussion_1: fVolume = volume * 0.15f; break;
            case Sound.Discussion_2: fVolume = volume * 0.15f; break;
        }

        Vector3 pos = position;

        if (goReference != null && !forcePosition)
            pos = goReference.transform.position;

        AudioSource.PlayClipAtPoint(ClipFX[(int)sound], pos, fVolume);
    }

    public void PlaySoundLoop()
    {
        LoopFX.volume = 0;
        LoopFX.Play();
        StartCoroutine(coroutine_SmoothPlaySound());
    }

    public void StopSoundLoop()
    {
        StartCoroutine(coroutine_SmoothStopSound());
    }

    public void ChangePitch(float toPitch, float fDuration = 0.5f)
    {
        if (coroutine_pitch != null) StopCoroutine(coroutine_pitch);

        coroutine_pitch = StartCoroutine(coroutine_SmoothChangePitch(toPitch, fDuration));
    }

    private IEnumerator coroutine_SmoothStopSound(float fDuration = 1f)
    {
        float fElapsedTime = 0;

        while (fElapsedTime < fDuration)
        {
            float fNewVolume = Mathf.Lerp(LoopFX_Volume, 0, (fElapsedTime / fDuration));
            LoopFX.volume = fNewVolume;

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        LoopFX.volume = 0;
        LoopFX.Stop();
    }

    private IEnumerator coroutine_SmoothPlaySound(float fDuration = 1f)
    {
        float fElapsedTime = 0;

        while (fElapsedTime < fDuration)
        {
            float fNewVolume = Mathf.Lerp(0, LoopFX_Volume, (fElapsedTime / fDuration));
            LoopFX.volume = fNewVolume;

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        LoopFX.volume = LoopFX_Volume;
    }

    private IEnumerator coroutine_SmoothChangePitch(float toPitch, float fDuration)
    {
        float fElapsedTime = 0;
        float fCurrentPitch = Source.pitch;

        while (fElapsedTime < fDuration)
        {
            float fNewVolume = Mathf.Lerp(fCurrentPitch, toPitch, (fElapsedTime / fDuration));
            Source.pitch = fNewVolume;

            fElapsedTime += Time.deltaTime;
            yield return null;
        }

        Source.pitch = toPitch;
    }

    #endregion
}
