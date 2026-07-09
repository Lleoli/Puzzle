using System;
using UnityEngine;

public static class StarCurrencyController
{
    private const string SpentStarsKey = "spent_stars";
    private const string BonusStarsKey = "bonus_stars";
    private const string BonusRewardClaimedPrefix = "bonus_reward_claimed_";

    public static Action onBalanceChanged;

    public static int GetBalance()
    {
        return Mathf.Max(0, GetTotalEarnedStars() + GetBonusStars() - GetSpentStars());
    }

    public static void CreditBonusStars(int value)
    {
        if (value <= 0)
            return;

        SetBonusStars(GetBonusStars() + value);
        NotifyBalanceChanged();
    }

    public static bool TryCreditBonusStarsOnce(string rewardKey, int value)
    {
        if (string.IsNullOrEmpty(rewardKey) || value <= 0)
            return false;

        string claimedKey = GetBonusRewardClaimedKey(rewardKey);
        if (PlayerPrefs.GetInt(claimedKey) == 1)
            return false;

        PlayerPrefs.SetInt(claimedKey, 1);
        SetBonusStars(GetBonusStars() + value);
        NotifyBalanceChanged();
        return true;
    }

    public static bool IsBonusRewardClaimed(string rewardKey)
    {
        if (string.IsNullOrEmpty(rewardKey))
            return false;

        return PlayerPrefs.GetInt(GetBonusRewardClaimedKey(rewardKey)) == 1;
    }

    public static bool DebitBalance(int value)
    {
        if (value <= 0)
            return true;

        if (GetBalance() < value)
            return false;

        SetSpentStars(GetSpentStars() + value);
        NotifyBalanceChanged();
        return true;
    }

    public static void NotifyBalanceChanged()
    {
        if (onBalanceChanged != null)
            onBalanceChanged();
    }

    private static int GetTotalEarnedStars()
    {
        int total = 0;
        string mode = Level.LevelMode.Star.ToString();
        for (int world = 0; world < Const.NUMWORLD; world++)
        {
            for (int level = 0; level < Const.NUMLEVEL; level++)
            {
                total += PlayerPrefs.GetInt("num_star_" + mode + "_" + world + "_" + level);
            }
        }

        return total;
    }

    private static string GetBonusRewardClaimedKey(string rewardKey)
    {
        return BonusRewardClaimedPrefix + rewardKey;
    }

    private static int GetBonusStars()
    {
        return PlayerPrefs.GetInt(BonusStarsKey);
    }

    private static void SetBonusStars(int value)
    {
        PlayerPrefs.SetInt(BonusStarsKey, Mathf.Max(0, value));
        PlayerPrefs.Save();
    }

    private static int GetSpentStars()
    {
        return PlayerPrefs.GetInt(SpentStarsKey);
    }

    private static void SetSpentStars(int value)
    {
        PlayerPrefs.SetInt(SpentStarsKey, Mathf.Max(0, value));
        PlayerPrefs.Save();
    }
}
