using UnityEngine;
using System.Collections;
using System;

public class Sound : MonoBehaviour
{
    public AudioSource audioSource, loopAudioSource;
    public enum Button { Default };
    public enum Others { Slide, Win, GetStar, BallEnd, Star1, Star2, Star3 };
    public enum Cue { TileMove, ButtonClick, WinDialog, MoveLimitExceeded, StarCollected };

    public static Action<Cue> onCueRequested;

    [HideInInspector]
    public AudioClip[] buttonClips;
    [HideInInspector]
    public AudioClip[] otherClips;

    public static Sound instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateSetting();
    }

    public bool IsMuted()
    {
        return !IsEnabled();
    }

    public bool IsEnabled()
    {
        return CPlayerPrefs.GetBool("sound_enabled", true);
    }

    public void SetEnabled(bool enabled)
    {
        CPlayerPrefs.SetBool("sound_enabled", enabled);
        UpdateSetting();
    }

    public void PlayTileMove()
    {
        RequestCue(Cue.TileMove);
        Play(Others.Slide);
    }

    public void PlayButtonClick()
    {
        RequestCue(Cue.ButtonClick);
        PlayButton();
    }

    public void PlayWinDialog()
    {
        RequestCue(Cue.WinDialog);
        Play(Others.Win);
    }

    public void PlayMoveLimitExceeded()
    {
        RequestCue(Cue.MoveLimitExceeded);
    }

    public void PlayStarCollected()
    {
        RequestCue(Cue.StarCollected);
        Play(Others.GetStar);
    }

    private void RequestCue(Cue cue)
    {
        if (onCueRequested != null)
            onCueRequested(cue);
    }

    public void Play(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }

    public void Play(AudioSource audioSource)
    {
        if (audioSource == null)
            return;

        if (IsEnabled())
        {
            audioSource.Play();
        }
    }

    public void PlayButton(Button type = Button.Default)
    {
        int index = (int)type;
        if (audioSource == null || buttonClips == null || index < 0 || index >= buttonClips.Length || buttonClips[index] == null)
            return;

        audioSource.PlayOneShot(buttonClips[index]);
    }

    public void Play(Others type, float volume = 1)
    {
        int index = (int)type;
        if (audioSource == null || otherClips == null || index < 0 || index >= otherClips.Length || otherClips[index] == null)
            return;

        audioSource.volume = volume;
        audioSource.PlayOneShot(otherClips[index]);
    }

    public void PlayLooping(Others type, float volume = 1)
    {
        int index = (int)type;
        if (loopAudioSource == null || otherClips == null || index < 0 || index >= otherClips.Length || otherClips[index] == null)
            return;

        loopAudioSource.volume = volume;
        loopAudioSource.PlayOneShot(otherClips[index]);
    }

    public void StopLooping()
    {
        if (loopAudioSource != null)
            loopAudioSource.Stop();
    }

    public void UpdateSetting()
    {
        if (audioSource != null)
            audioSource.mute = IsMuted();
        if (loopAudioSource != null)
            loopAudioSource.mute = IsMuted();
    }
}