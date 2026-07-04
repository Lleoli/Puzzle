using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SimpleAndroidNotifications;
using System;
using System.Text;

#pragma warning disable 0219

public class NotificationController : MonoBehaviour {

    private const double NOTI_DELAY_1 = 1 * 24;
    private const double NOTI_DELAY_2 = 2 * 24;
    private const double NOTI_DELAY_3 = 4 * 24;
    private const double NOTI_DELAY_4 = 7 * 24;
    private const double NOTI_DELAY_5 = 10 * 24;
    private const double NOTI_DELAY_6 = 15 * 24;
    private const double NOTI_DELAY_7 = 30 * 24;

    private bool ingame = false;

    private void Start()
    {
        InGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            OutGame();
        else
            InGame();
    }

    private void OnApplicationQuit()
    {
        OutGame();
    }

    private void InGame()
    {
        if (ingame) return;
        ingame = true;

        if (!PlayerPrefs.HasKey("out_game_time")) return;

        double restTimeSecs = CUtils.GetCurrentTime() - CUtils.GetActionTime("out_game");
        double restTime = restTimeSecs / 3600;

        if (restTime > NOTI_DELAY_3)
        {
            GiveFreeRuby(Prefs.noti3Ruby);
        }

        if (restTime > NOTI_DELAY_4)
        {
            GiveFreeRuby(Prefs.noti4Ruby);
        }

        if (restTime > NOTI_DELAY_5)
        {
            GiveFreeRuby(Prefs.noti5Ruby);
        }

        if (restTime > NOTI_DELAY_6)
        {
            GiveFreeRuby(Prefs.noti6Ruby);
        }

        if (restTime > NOTI_DELAY_7)
        {
            GiveFreeRuby(Prefs.noti7Ruby);
        }

        NotificationManager.CancelAll();
    }

    private void OutGame()
    {
        ingame = false;

        PushComeback(20);

        int num = PushFreeRuby(NOTI_DELAY_3);
        Prefs.noti3Ruby = num;

        num = PushFreeRuby(NOTI_DELAY_4);
        Prefs.noti4Ruby = num;

        num = PushFreeRuby(NOTI_DELAY_5);
        Prefs.noti5Ruby = num;

        num = PushFreeRuby(NOTI_DELAY_6);
        Prefs.noti6Ruby = num;

        num = PushFreeRuby(NOTI_DELAY_7);
        Prefs.noti7Ruby = num;

        CUtils.SetActionTime("out_game");
    }


    private void PushComeback(double delay)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string title = "Enjoy with us";
        string message = "Open the game and have fun together";
        NotificationManager.SendWithAppIcon(TimeSpan.FromHours(delay), title, message, new Color(0, 0.6f, 1), NotificationIcon.Message);
#endif
    }

    private int PushFreeRuby(double delay)
    {
        int num = CUtils.GetRandom(10, 15);
#if UNITY_ANDROID && !UNITY_EDITOR
        string message = "You got " + num + " free rubies. Let's play and challenge yourself";
        NotificationManager.SendWithAppIcon(TimeSpan.FromHours(delay), "Ruby gift", message, new Color(0, 0.6f, 1), NotificationIcon.Message);
#endif
        return num;
    }

    private void GiveFreeRuby(int num)
    {
        CurrencyController.CreditBalance(num);
    }
}
