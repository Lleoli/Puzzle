using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LevelMoveLimitTable
{
    private const string CsvTablePath = "DataTable/level_move_limits_source";
    private const string EditorCsvRelativePath = "../ExcelData/level_move_limits.csv";

    private static Dictionary<string, int> moveLimits;

    public static int GetMoveLimit(string mode, int world, int level, int defaultMoveLimit)
    {
        EnsureLoaded();

        int moveLimit;
        if (moveLimits.TryGetValue(GetKey(mode, world, level), out moveLimit))
            return moveLimit;

        return defaultMoveLimit;
    }

    private static void EnsureLoaded()
    {
        if (moveLimits != null)
            return;

        moveLimits = new Dictionary<string, int>();
        string tableText = null;

#if UNITY_EDITOR
        string editorCsvPath = Path.Combine(Application.dataPath, EditorCsvRelativePath);
        if (File.Exists(editorCsvPath))
            tableText = File.ReadAllText(editorCsvPath);
#endif

        if (string.IsNullOrEmpty(tableText))
        {
            TextAsset asset = Resources.Load<TextAsset>(CsvTablePath);
            if (asset != null)
                tableText = asset.text;
        }

        if (!string.IsNullOrEmpty(tableText))
            LoadCsvRows(tableText);
    }

    private static void LoadCsvRows(string tableText)
    {
        using (StringReader reader = new StringReader(tableText))
        {
            string line;
            int lineIndex = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineIndex++;
                if (lineIndex <= 3 || string.IsNullOrEmpty(line.Trim()))
                    continue;

                string[] columns = line.Split(',');
                if (columns.Length < 4)
                    continue;

                string mode = columns[0].Trim();
                int world;
                int level;
                int moveLimit;

                if (string.IsNullOrEmpty(mode) ||
                    !int.TryParse(columns[1], out world) ||
                    !int.TryParse(columns[2], out level) ||
                    !int.TryParse(columns[3], out moveLimit))
                    continue;

                moveLimits[GetKey(mode, world, level)] = Mathf.Max(0, moveLimit);
            }
        }
    }

    private static string GetKey(string mode, int world, int level)
    {
        return mode + "_" + world + "_" + level;
    }
}