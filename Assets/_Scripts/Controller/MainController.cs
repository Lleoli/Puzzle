using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainController : BaseController
{
    public static MainController instance;
    public bool isComplete;
    public Transform skipLevelTr, hintTr;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();

        var level = Superpow.Utils.GetLevel(Prefs.currentMode, Prefs.currentWorld, Prefs.currentLevel);
        Board.instance.LoadLevel(level);

        Prefs.continuePlayMode = Prefs.currentMode;
        Prefs.continuePlayWorld = Prefs.currentWorld;
        Prefs.continuePlayLevel = Prefs.currentLevel;

        Superpow.Utils.SetMusic();

        Timer.Schedule(this, 0, () =>
        {
            skipLevelTr.transform.SetX(hintTr.transform.position.x);
        });
    }

    public void OnComplete()
    {
        isComplete = true;

        if (Prefs.currentLevel == Prefs.unlockedLevel)
        {
            Prefs.unlockedLevel++;
            if (Prefs.currentLevel == Const.NUMLEVEL - 1)
            {
                int nextWorld = Prefs.currentWorld + 1;
                Prefs.UnlockWorld(Prefs.currentMode, nextWorld);
            }
        }

        Prefs.continuePlayMode = Prefs.currentMode;
        if (Prefs.currentLevel == Const.NUMLEVEL - 1)
        {
            Prefs.continuePlayWorld = Prefs.currentWorld + 1;
            Prefs.continuePlayLevel = 0;
        }
        else
        {
            Prefs.continuePlayWorld = Prefs.currentWorld;
            Prefs.continuePlayLevel = Prefs.currentLevel + 1;
        }
    }

    public void OnBallToGoal()
    {
        Timer.Schedule(this, 0.5f, () =>
        {
            DialogController.instance.ShowDialog(DialogType.Win);
            Sound.instance.Play(Sound.Others.Win);
        });
    }

    public void SkipLevel()
    {
        if (Prefs.unlockedLevel == Prefs.currentLevel)
        {
            SkipLevelDialog dialog = (SkipLevelDialog)DialogController.instance.GetDialog(DialogType.SkipLevel);
            DialogController.instance.ShowDialog(dialog);
        }
        else
        {
            OnComplete();
            OnBallToGoal();
        }
    }
}
