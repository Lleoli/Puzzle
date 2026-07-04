using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldItem : MonoBehaviour
{
    public GameObject lockedImage;
    public Text progress;
    private int world;
    private bool isUnlocked;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        world = transform.GetSiblingIndex();

        UpdateUI();
    }

    public void UpdateUI()
    {
        isUnlocked = Prefs.IsWorldUnlocked(Prefs.currentMode, world);
        if (isUnlocked)
        {
            lockedImage.SetActive(false);
            progress.text = Prefs.GetUnlockedLevel(Prefs.currentMode, world) + "/" + Const.NUMLEVEL;
        }
        else
        {
            lockedImage.SetActive(true);
        }
    }

    public void OnClick()
    {
        if (isUnlocked)
        {
            Prefs.currentWorld = world;
            CUtils.LoadScene(2, true);
            Sound.instance.PlayButton();
        }
        else
        {
            var dialog = (UnlockPackageDialog)DialogController.instance.GetDialog(DialogType.UnlockPackage);
            dialog.worldIndex = world;
            DialogController.instance.ShowDialog(dialog);
        }
    }
}
