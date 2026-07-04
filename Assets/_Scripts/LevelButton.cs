using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public Text levelName;
    public Image background;
    public Sprite passed, current, locked;
    public Button button;
    public GameObject star1, star2, star3;
    public GameObject starsBack, stars;

    int level;

    private void Start()
    {
        level = transform.GetSiblingIndex();
        levelName.text = (level + 1).ToString();

        int unlockedLevel = Prefs.unlockedLevel;
        if (level < unlockedLevel)
        {
            background.sprite = passed;
            button.interactable = true;
            levelName.gameObject.SetActive(true);
            starsBack.SetActive(true);
            stars.SetActive(true);
        }
        else if (level == unlockedLevel)
        {
            background.sprite = current;
            button.interactable = true;
            levelName.gameObject.SetActive(true);
            starsBack.SetActive(true);
            stars.SetActive(false);
        }
        else
        {
            background.sprite = locked;
            button.interactable = false;
            levelName.gameObject.SetActive(false);
            starsBack.SetActive(false);
            stars.SetActive(false);
        }

        int numStar = Prefs.GetNumStar(Prefs.currentWorld, level);
        star1.SetActive(numStar >= 1);
        star2.SetActive(numStar >= 2);
        star3.SetActive(numStar >= 3);

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        Prefs.currentLevel = level;
        CUtils.LoadScene(3, true);
        Sound.instance.PlayButton();
    }
}
