using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : BaseController
{
    public RectTransform rootCanvas;
    public Transform scrollContent;

    public static WorldController instance;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();

        foreach (Transform item in scrollContent)
        {
            item.localPosition = new Vector3(rootCanvas.rect.width, item.localPosition.y, 0);
        }

        StartCoroutine(ItemAnimation());
    }

    private IEnumerator ItemAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (Transform item in scrollContent)
        {
            iTween.MoveTo(item.gameObject, iTween.Hash("position", new Vector3(0, item.localPosition.y, 0), "isLocal", true, "time", 0.25f));
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void UpdateUI()
    {
        foreach(Transform child in scrollContent)
        {
            child.GetComponent<WorldItem>().UpdateUI();
        }
    }
}
