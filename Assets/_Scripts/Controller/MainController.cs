using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainController : BaseController
{
    public static MainController instance;
    public bool isComplete;
    public Transform skipLevelTr, hintTr;
    public Tutorial tutorialPrefab;

    private Tutorial firstLevelTutorial;
    private static readonly Vector3 FirstLevelTutorialTilePosition = new Vector3(2, 2, 0);
    private static readonly Vector3 FirstLevelTutorialMoveDirection = Vector3.left;

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

        StartCoroutine(ShowFirstLevelTutorialIfNeeded());
    }

    public void OnComplete()
    {
        isComplete = true;
        CompleteFirstLevelTutorial();

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

    public void OnMoveLimitExceeded()
    {
        if (isComplete)
            return;

        isComplete = true;
        StopFirstLevelTutorial();

        Timer.Schedule(this, 1.5f, () =>
        {
            WinDialog dialog = (WinDialog)DialogController.instance.GetDialog(DialogType.Win);
            dialog.SetMoveLimitExceededMode();
            DialogController.instance.ShowDialog(dialog);
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

    private IEnumerator ShowFirstLevelTutorialIfNeeded()
    {
        yield return null;

        if (!ShouldShowFirstLevelTutorial() || tutorialPrefab == null || Board.instance == null)
            yield break;

        Tile targetTile = Board.instance.GetTileAt(FirstLevelTutorialTilePosition);
        if (targetTile == null)
            yield break;

        firstLevelTutorial = Instantiate(tutorialPrefab);
        Transform tutorialParent = canvasRt != null ? canvasRt : Board.instance.transform.parent;
        if (tutorialParent != null)
            firstLevelTutorial.transform.SetParent(tutorialParent);

        firstLevelTutorial.transform.localScale = Vector3.one;
        firstLevelTutorial.transform.SetAsLastSibling();

        Vector3 dragOffset = Board.instance.transform.TransformVector(FirstLevelTutorialMoveDirection * targetTile.width);
        firstLevelTutorial.StartDragTutorial(targetTile.transform, dragOffset);
    }

    private bool ShouldShowFirstLevelTutorial()
    {
        return IsFirstLevelTutorialLevel() && !Tutorial.IsFirstLevelTutorialDone();
    }

    private bool IsFirstLevelTutorialLevel()
    {
        return Prefs.currentMode == Level.LevelMode.Classic.ToString() && Prefs.currentWorld == 0 && Prefs.currentLevel == 0;
    }

    private void StopFirstLevelTutorial()
    {
        if (firstLevelTutorial != null)
        {
            firstLevelTutorial.StopTutorial();
            firstLevelTutorial = null;
        }
    }

    private void CompleteFirstLevelTutorial()
    {
        if (!IsFirstLevelTutorialLevel())
            return;

        StopFirstLevelTutorial();
        Tutorial.SetFirstLevelTutorialDone(true);
    }
}
