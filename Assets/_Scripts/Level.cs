
using UnityEngine;


[System.Serializable]
public class Level : ScriptableObject
{
    public enum LevelMode { Classic, Star };
    public LevelMode mode;
    public int size = 4;
    public int targetMove;
    public TileP[] maps;
    public Vector3[] hintPath;
}
