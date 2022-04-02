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

    private static MusicController _controller;

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
        float fCurrentVolume = Source.volume == 0 ? CommonController.VOLUME_BASE : Source.volume;

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
}
