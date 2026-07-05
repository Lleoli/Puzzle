using UnityEngine;
using UnityEditor;

public class SuperpowWindowEditor : EditorWindow
{
    private int currencyAmount = CurrencyController.DEFAULT_CURRENCY;

    [MenuItem("Superpow/玩家数据工具")]
    static void OpenPlayerDataTools()
    {
        GetWindow<SuperpowWindowEditor>("玩家数据");
    }

    private void OnGUI()
    {
        GUILayout.Label("玩家数据", EditorStyles.boldLabel);

        if (GUILayout.Button("重置关卡进度"))
        {
            if (EditorUtility.DisplayDialog("重置关卡进度", "确定要重置关卡进度、星星、最佳步数和当前关卡吗？", "重置", "取消"))
            {
                ResetLevelProgress();
                SavePlayerPrefs();
                Debug.Log("关卡进度已重置。");
            }
        }

        EditorGUILayout.Space();
        currencyAmount = EditorGUILayout.IntField("货币数量", currencyAmount);

        if (GUILayout.Button("设置货币数量"))
        {
            SetCurrency(currencyAmount);
            SavePlayerPrefs();
            Debug.Log("货币数量已设置为 " + currencyAmount + "。");
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("重置关卡和货币"))
        {
            if (EditorUtility.DisplayDialog("重置玩家数据", "确定要重置关卡进度，并把货币设置为上面的数量吗？", "重置", "取消"))
            {
                ResetLevelProgress();
                SetCurrency(currencyAmount);
                SavePlayerPrefs();
                Debug.Log("关卡进度已重置，货币数量已设置为 " + currencyAmount + "。");
            }
        }
    }

    [MenuItem("Superpow/清空所有本地存档")]
    static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("Superpow/解锁所有关卡")]
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

    [MenuItem("Superpow/增加货币 1000")]
    static void AddRuby()
    {
        CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);
        CurrencyController.CreditBalance(1000);
    }

    [MenuItem("Superpow/货币清零")]
    static void SetBalanceZero()
    {
        CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);
        CurrencyController.SetBalance(0);
    }

    [MenuItem("Superpow/重置关卡和货币")]
    static void ResetLevelsAndCurrency()
    {
        if (!EditorUtility.DisplayDialog("重置玩家数据", "确定要重置关卡进度，并把货币恢复为默认值吗？", "重置", "取消"))
        {
            return;
        }

        ResetLevelProgress();
        SetCurrency(CurrencyController.DEFAULT_CURRENCY);
        SavePlayerPrefs();
        Debug.Log("关卡进度已重置，货币数量已设置为 " + CurrencyController.DEFAULT_CURRENCY + "。");
    }

    private static void ResetLevelProgress()
    {
        string[] modes = { "Classic", "Star" };

        PlayerPrefs.SetString("currentMode", "Classic");
        PlayerPrefs.SetInt("currentWorld", 0);
        PlayerPrefs.SetInt("currentLevel", 0);
        PlayerPrefs.SetString("continue_play_mode", "Classic");
        PlayerPrefs.SetInt("continue_play_world", 0);
        PlayerPrefs.SetInt("continue_play_level", 0);

        foreach (string mode in modes)
        {
            for (int world = 0; world < Const.NUMWORLD; world++)
            {
                PlayerPrefs.DeleteKey("unlock_world_" + mode + "_" + world);
                PlayerPrefs.DeleteKey("unlocked_level_" + mode + "_" + world);

                for (int level = 0; level < Const.NUMLEVEL; level++)
                {
                    PlayerPrefs.DeleteKey("num_star_" + mode + "_" + world + "_" + level);
                    PlayerPrefs.DeleteKey("best_move_" + mode + "_" + world + "_" + level);
                }
            }
        }
    }

    private static void SetCurrency(int amount)
    {
        CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);
        CurrencyController.SetBalance(amount);
    }

    private static void SavePlayerPrefs()
    {
        CPlayerPrefs.Save();
        PlayerPrefs.Save();
    }
}
