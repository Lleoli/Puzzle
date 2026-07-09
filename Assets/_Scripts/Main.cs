using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public static Main Self;

    public RectTransform CanvasRectTransform;
    public CanvasScaler CanvasScaler;

    public RectTransform Target1;
    [SerializeField] private float firstLevelGuideMaskDuration = 2f;

    public void Awake()
    {
        Self = this;
        var guideMask = FindObjectOfType<GuideMask>(true);
        if (guideMask != null)
            guideMask.Init();
    }

    public IEnumerator PlayFirstLevelGuideMaskIfNeeded()
    {
        if (!IsFirstLevel() || Target1 == null || GuideMask.Self == null)
            yield break;

        GuideMask.Self.Play(Target1);
        yield return new WaitForSeconds(firstLevelGuideMaskDuration);
        GuideMask.Self.Close();
    }

    private bool IsFirstLevel()
    {
        return Prefs.currentMode == Level.LevelMode.Star.ToString() && Prefs.currentWorld == 0 && Prefs.currentLevel == 0;
    }
}