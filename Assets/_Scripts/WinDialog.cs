using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinDialog : Dialog {
    public GameObject[] stars;
    public Text levelName;

    private bool hideNextButton;

    protected override void Start()
    {
        base.Start();

        levelName.text = "level " + (Prefs.currentLevel + 1);
        ApplyNextButtonState();
        StartCoroutine(ShowStars());
    }

    public override void Show()
    {
        ApplyNextButtonState();
        base.Show();
    }

    public void SetMoveLimitExceededMode()
    {
        hideNextButton = true;
        ApplyNextButtonState();
    }

    private void ApplyNextButtonState()
    {
        GameObject nextButton = FindChildByName(transform, "Next");
        GameObject ImageNext = FindChildByName(transform, "ImageNext");
        if (nextButton != null)
            nextButton.SetActive(!hideNextButton);
        if (ImageNext != null)
            ImageNext.SetActive(!hideNextButton);
    }

    private GameObject FindChildByName(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child.gameObject;

            GameObject result = FindChildByName(child, childName);
            if (result != null)
                return result;
        }

        return null;
    }

    private IEnumerator ShowStars()
    {
        int numStars = Prefs.GetNumStar(Prefs.currentWorld, Prefs.currentLevel);
        stars[0].SetActive(false);
        stars[1].SetActive(false);
        stars[2].SetActive(false);

        yield return new WaitForSeconds(0.5f);

        float time = 0.3f;

        Sound.Others[] starSounds = { Sound.Others.Star1, Sound.Others.Star2, Sound.Others.Star3 };

        for (int i = 0; i < numStars; i++)
        {
            GameObject star = stars[i];
            star.SetActive(true);
            var localPosition = star.transform.localPosition;
            star.transform.localPosition += Vector3.up * 40;
            star.transform.localScale = Vector3.one * 7;
            star.GetComponent<Image>().canvasRenderer.SetAlpha(0.3f);

            iTween.MoveTo(star, iTween.Hash("position", localPosition, "isLocal", true, "time", time));
            iTween.ScaleTo(star, iTween.Hash("scale", Vector3.one , "isLocal", true, "time", time));
            star.GetComponent<Image>().CrossFadeAlpha(1, time, true);

            Sound.instance.Play(starSounds[i]);

            yield return new WaitForSeconds(time);
        }
    }

    public void OnNextClick()
    {
        if (hideNextButton)
            return;

        if (Prefs.currentLevel < Const.NUMLEVEL - 1)
        {
            Prefs.currentLevel++;
            CUtils.LoadScene(2, true);
        }
        else
        {
            CUtils.LoadScene(1, true);
        }

        Close();
    }
}