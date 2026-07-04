using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorController : MonoBehaviour
{
    [HideInInspector]
    public int boardSize = 4;

    public InputField worldInput, levelInput, boardsizeInput, targetMoveInput;
    public Dropdown modeDropdown;
    public Text loadLevelText;

    [HideInInspector]
    public int worldIndex = 0;
    [HideInInspector]
    public int levelIndex = 0;
    [HideInInspector]
    public int targetMove = 0;
    [HideInInspector]
    public Level.LevelMode levelmode;

    public static LevelEditorController instance;
    private Level level;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        worldInput.text = PlayerPrefs.GetInt("level_editor_world_index").ToString();
        levelInput.text = PlayerPrefs.GetInt("level_editor_level_index").ToString();
        boardsizeInput.text = PlayerPrefs.GetInt("level_editor_board_size", 4).ToString();
        targetMoveInput.text = targetMove.ToString();
        modeDropdown.value = PlayerPrefs.GetInt("level_editor_level_mode");
    }

    public void OnInputValueChanged()
    {
        if (worldInput.text == "-") worldInput.text = "";
        if (levelInput.text == "-") levelInput.text = "";
        if (targetMoveInput.text == "-") targetMoveInput.text = "";
        if (boardsizeInput.text == "-" || boardsizeInput.text == "0") boardsizeInput.text = "";

        if (!string.IsNullOrEmpty(worldInput.text))
        {
            int.TryParse(worldInput.text, out worldIndex);
        }

        if (!string.IsNullOrEmpty(levelInput.text))
        {
            int.TryParse(levelInput.text, out levelIndex);
        }

        if (!string.IsNullOrEmpty(boardsizeInput.text))
        {
            int.TryParse(boardsizeInput.text, out boardSize);
        }

        if (!string.IsNullOrEmpty(targetMoveInput.text)) int.TryParse(targetMoveInput.text, out targetMove);

        UpdateLoadLevelText();
    }

    public void OnModeValueChanged()
    {
        levelmode = (Level.LevelMode)modeDropdown.value;
        PlayerPrefs.SetInt("level_editor_level_mode", (int)levelmode);

        UpdateLoadLevelText();
    }

    public void OnLoadClick()
    {
        if (level == null)
            BoardEditor.instance.AddLevel();
        else
            BoardEditor.instance.LoadLevel(level);


        PlayerPrefs.SetInt("level_editor_world_index", worldIndex);
        PlayerPrefs.SetInt("level_editor_level_index", levelIndex);
        PlayerPrefs.SetInt("level_editor_board_size", boardSize);
    }

    public void UpdateLoadLevelText()
    {
        level = Superpow.Utils.GetLevel(levelmode.ToString(), worldIndex, levelIndex);
        loadLevelText.text = level == null ? "add" : "load";
    }
}
