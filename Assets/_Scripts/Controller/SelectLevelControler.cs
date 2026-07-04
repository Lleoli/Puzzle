using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLevelControler : BaseController {

    public RectTransform scrollContent, scrollRect;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(UpdateGrid());
    }

    private IEnumerator UpdateGrid()
	{
		yield return new WaitForEndOfFrame();

        int index = Mathf.Clamp(Prefs.unlockedLevel, 0, scrollContent.childCount - 1);
        Transform unlockedLevelTransform = scrollContent.GetChild(index);
        float newY = -unlockedLevelTransform.localPosition.y - scrollRect.rect.height / 2f;
        newY = Mathf.Clamp(newY, 0, scrollContent.rect.height);
        scrollContent.localPosition = new Vector3(scrollContent.localPosition.x, newY, scrollContent.localPosition.z);
    }
}
