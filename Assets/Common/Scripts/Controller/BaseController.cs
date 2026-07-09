using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJSON;

public class BaseController : MonoBehaviour
{
    public GameObject gameMaster;
    public Music.Type music = Music.Type.None;

    public RectTransform canvasRt;
    public RectTransform affectedByAd;

    protected virtual void Awake()
    {
        if (GameMaster.instance == null && gameMaster != null)
            Instantiate(gameMaster);
        
        iTween.dimensionMode = CommonConst.ITWEEN_MODE;
        CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);
    }

    protected virtual void Start()
    {
        CPlayerPrefs.Save();

#if UNITY_WSA && !UNITY_EDITOR
        StartCoroutine(SavePrefs());
#endif
        Music.instance.Play(music);

    }

    private void OnBannerChanged(bool visible)
    {
        if (visible && affectedByAd != null)
        {
            float coef = canvasRt.rect.height / Screen.height;
            float adHeight = 50 * Screen.dpi / 160 * coef;
            affectedByAd.offsetMin = new Vector2(0, adHeight);
        }
    }

    public virtual void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            ClearAllLocalClientData();
            return;
        }

        CPlayerPrefs.Save();
    }

    public virtual void OnApplicationQuit()
    {
        ClearAllLocalClientData();
    }

    public static void ClearAllLocalClientData()
    {
        CPlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    private IEnumerator SavePrefs()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            CPlayerPrefs.Save();
        }
    }

    private void OnDestroy()
    {
    }
}
