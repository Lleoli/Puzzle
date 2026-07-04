using UnityEngine;
using UnityEditor;

public class MakeScriptableObject
{
    [MenuItem("Tools/MyTool/Create My Scriptable Object")]
    static void DoIt()
    {
        Level asset = ScriptableObject.CreateInstance<Level>();
        AssetDatabase.CreateAsset(asset, "Assets/MyScriptableObject.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}