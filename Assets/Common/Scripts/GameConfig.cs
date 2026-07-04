using UnityEngine;
using System;

[System.Serializable]
public class GameConfig
{
    public Admob admob;

    [Header("")]
    public int interstitialAdPeriod;
    public int rewardedVideoAmount;
    public string iosAppID;
    public string facebookPageID;

    [Header("")]
    public int skipLevelCost;
}

[System.Serializable]
public class Admob
{
    [Header("Banner")]
    public string androidBanner;
    public string iosBanner;

    [Header("Interstitial")]
    public string androidInterstitial;
    public string iosInterstitial;

    [Header("RewardedVideo")]
    public string androidRewarded;
    public string iosRewarded;
}
