using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HomeController : BaseController {
    private const int CLASSIC = 1;
    private const int STAR = 2;
    private const int FACEBOOK = 3;
    private const string StartAnimationPlayedKey = "home_start_animation_played";

    public GameObject playButton;
    public Image playIcon;
    public Sprite starIcon, classicIcon;

    private bool startAnimationPrepared;
    private bool startAnimationPlayed;
    private bool hasPlayButtonFinalPosition;
    private Vector3 playButtonFinalPosition;

    protected override void Start()
    {
        base.Start();
        playIcon.sprite = Prefs.continuePlayMode == "Star" ? starIcon : classicIcon;

        CachePlayButtonFinalPosition();
        if (HasStartAnimationCache())
        {
            ShowStartAnimationFinalState();
        }
        else
        {
            PrepareStartAnimation();
            if (FindObjectOfType<VideoPlay>() == null)
                PlayStartAnimation();
        }

        Superpow.Utils.SetMusic();
    }

    public static bool HasStartAnimationCache()
    {
        return PlayerPrefs.GetInt(StartAnimationPlayedKey) == 1;
    }

    public void PlayStartAnimation()
    {
        if (startAnimationPlayed || playButton == null)
            return;

        if (HasStartAnimationCache())
        {
            ShowStartAnimationFinalState();
            return;
        }

        PrepareStartAnimation();
        startAnimationPlayed = true;
        SaveStartAnimationCache();
        iTween.MoveBy(playButton, iTween.Hash("amount", Vector3.right * 5, "easetype", iTween.EaseType.easeOutBack, "time", 0.4f, "delay", 0.4f));
    }

    public void ShowStartAnimationFinalState()
    {
        if (playButton == null)
            return;

        CachePlayButtonFinalPosition();
        iTween.Stop(playButton);
        playButton.transform.position = playButtonFinalPosition;
        startAnimationPrepared = false;
        startAnimationPlayed = true;
    }

    private void PrepareStartAnimation()
    {
        if (startAnimationPrepared || playButton == null)
            return;

        CachePlayButtonFinalPosition();
        playButton.transform.position = playButtonFinalPosition - Vector3.right * 5;
        startAnimationPrepared = true;
    }

    private void CachePlayButtonFinalPosition()
    {
        if (hasPlayButtonFinalPosition || playButton == null)
            return;

        playButtonFinalPosition = playButton.transform.position;
        hasPlayButtonFinalPosition = true;
    }

    private void SaveStartAnimationCache()
    {
        PlayerPrefs.SetInt(StartAnimationPlayedKey, 1);
        PlayerPrefs.Save();
    }

    public void OnClick(int index)
    {
        switch (index)
        {
            case CLASSIC:
                Prefs.currentMode = "Classic";
                CUtils.LoadScene(1, true);
                break;
            case STAR:
                Prefs.currentMode = "Star";
                CUtils.LoadScene(1, true);
                break;
            case FACEBOOK:
                CUtils.LikeFacebookPage(ConfigController.Config.facebookPageID);
                break;
        }
        Sound.instance.PlayButton();
    }

    public void OnPlayClick()
    {
        Prefs.currentMode = Prefs.continuePlayMode;
        Prefs.currentWorld = Prefs.continuePlayWorld;
        Prefs.currentLevel = Prefs.continuePlayLevel;

        iTween.MoveBy(playButton, iTween.Hash("amount", Vector3.right * 5, "easetype", iTween.EaseType.easeInBack, "time", 0.4f));

        CUtils.LoadScene(2, true);
        Sound.instance.PlayButton();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}