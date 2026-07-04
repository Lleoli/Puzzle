using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefs
{ 
    public static string currentMode
    {
        get { return PlayerPrefs.GetString("currentMode", Level.LevelMode.Classic.ToString()); }
        set { PlayerPrefs.SetString("currentMode", value); }
    }

    public static int currentWorld
    {
        get { return PlayerPrefs.GetInt("currentWorld"); }
        set { PlayerPrefs.SetInt("currentWorld", value); }
    }

    public static int currentLevel
    {
        get { return PlayerPrefs.GetInt("currentLevel"); }
        set { PlayerPrefs.SetInt("currentLevel", value); }
    }

    public static int unlockedLevel
    {
        get { return GetUnlockedLevel(currentMode, currentWorld); }
        set { SetUnlockedLevel(currentMode, currentWorld, value); }
    }

    public static int GetUnlockedLevel(string mode, int world)
    {
        return PlayerPrefs.GetInt("unlocked_level_" + mode + "_" + world);
    }

    public static void SetUnlockedLevel(string mode, int world, int value)
    {
        PlayerPrefs.SetInt("unlocked_level_" + mode + "_" + world, value);
    }

    public static int bestMove
    {
        get { return PlayerPrefs.GetInt("best_move_" + currentMode + "_" + currentWorld + "_" + currentLevel, -1); }
        set { PlayerPrefs.SetInt("best_move_" + currentMode + "_" + currentWorld + "_" + currentLevel, value); }
    }

    public static int GetNumStar(int world, int level)
    {
        return PlayerPrefs.GetInt("num_star_" + currentMode + "_" + world + "_" + level);
    }

    public static void SetNumStar(int world, int level, int numStar)
    {
        PlayerPrefs.SetInt("num_star_" + currentMode + "_" + world + "_" + level, numStar);
    }

    public static void UnlockWorld(string mode, int world)
    {
        PlayerPrefs.SetInt("unlock_world_" + mode + "_" + world, 1);
    }

    public static bool IsWorldUnlocked(string mode, int world)
    {
        if (world == 0) return true;
        return PlayerPrefs.GetInt("unlock_world_" + mode + "_" + world) == 1;
    }

    public static string continuePlayMode
    {
        get { return PlayerPrefs.GetString("continue_play_mode", "Star"); }
        set { PlayerPrefs.SetString("continue_play_mode", value); }
    }

    public static int continuePlayWorld
    {
        get { return PlayerPrefs.GetInt("continue_play_world"); }
        set { PlayerPrefs.SetInt("continue_play_world", value); }
    }

    public static int continuePlayLevel
    {
        get { return PlayerPrefs.GetInt("continue_play_level"); }
        set { PlayerPrefs.SetInt("continue_play_level", value); }
    }

    public static int noti3Ruby
    {
        get { return PlayerPrefs.GetInt("noti_3_ruby"); }
        set { PlayerPrefs.SetInt("noti_3_ruby", value); }
    }

    public static int noti4Ruby
    {
        get { return PlayerPrefs.GetInt("noti_4_ruby"); }
        set { PlayerPrefs.SetInt("noti_4_ruby", value); }
    }

    public static int noti5Ruby
    {
        get { return PlayerPrefs.GetInt("noti_5_ruby"); }
        set { PlayerPrefs.SetInt("noti_5_ruby", value); }
    }

    public static int noti6Ruby
    {
        get { return PlayerPrefs.GetInt("noti_6_ruby"); }
        set { PlayerPrefs.SetInt("noti_6_ruby", value); }
    }

    public static int noti7Ruby
    {
        get { return PlayerPrefs.GetInt("noti_7_ruby"); }
        set { PlayerPrefs.SetInt("noti_7_ruby", value); }
    }
}
