using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HomeController : BaseController {
    private const int CLASSIC = 1;
    private const int STAR = 2;
    private const int FACEBOOK = 3;

    public GameObject playButton;
    public Image playIcon;
    public Sprite starIcon, classicIcon;

    private bool startAnimationPrepared;
    private bool startAnimationPlayed;

    protected override void Start()
    {
        base.Start();
        playIcon.sprite = Prefs.continuePlayMode == "Star" ? starIcon : classicIcon;

        PrepareStartAnimation();
        if (FindObjectOfType<VideoPlay>() == null)
            PlayStartAnimation();

        Superpow.Utils.SetMusic();
    }

    public void PlayStartAnimation()
    {
        if (startAnimationPlayed || playButton == null)
            return;

        PrepareStartAnimation();
        startAnimationPlayed = true;
        iTween.MoveBy(playButton, iTween.Hash("amount", Vector3.right * 5, "easetype", iTween.EaseType.easeOutBack, "time", 0.4f, "delay", 0.4f));
    }

    private void PrepareStartAnimation()
    {
        if (startAnimationPrepared || playButton == null)
            return;

        playButton.transform.position = playButton.transform.position - Vector3.right * 5;
        startAnimationPrepared = true;
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