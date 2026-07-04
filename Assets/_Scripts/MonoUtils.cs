using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonoUtils : MonoBehaviour
{
    public Tile tilePrefab;
    public TileHint tileHintPrefab;
    public TileHint tileHintPrefab2;
    public Sprite[] tileSprites;
    public TileP[] tilePs;
    public Sprite[] hintSprites;
    public Vector3Array[] hintPaths;
    public GameObject ballPrefab;
    public GameObject starPrefab;

    public static MonoUtils instance;

    private void Awake()
    {
        instance = this;
    }
}
