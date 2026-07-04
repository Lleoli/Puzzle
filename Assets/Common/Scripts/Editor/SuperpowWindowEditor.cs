using UnityEngine;
using UnityEditor;

public class SuperpowWindowEditor
{
    [MenuItem("Superpow/Clear all playerprefs")]
    static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("Superpow/Unlock all levels")]
    static void UnlockAllLevel()
    {
        CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);
        for (int i = 0; i < Const.NUMWORLD; i++)
        {
            Prefs.UnlockWorld("Classic", i);
            Prefs.SetUnlockedLevel("Classic", i, Const.NUMLEVEL);
        }

        for (int i = 0; i < Const.NUMWORLD; i++)
        {
            Prefs.UnlockWorld("Star", i);
            Prefs.SetUnlockedLevel("Star", i, Const.NUMLEVEL);
        }
    }

    [MenuItem("Superpow/Credit balance (ruby, coin..)")]
    static void AddRuby()
    {
        CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);
        CurrencyController.CreditBalance(1000);
    }

    [MenuItem("Superpow/Set balance to 0")]
    static void SetBalanceZero()
    {
        CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);
        CurrencyController.SetBalance(0);
    }
}