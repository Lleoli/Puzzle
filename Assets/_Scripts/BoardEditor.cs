using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoardEditor : MonoBehaviour, IPointerDownHandler {

    private Dictionary<Vector3, Tile> tiles = new Dictionary<Vector3, Tile>();
    private Dictionary<Vector3, TileHint> hints = new Dictionary<Vector3, TileHint>();
    private int size;
    private float tileWidth;
    public Transform backsRegion, tilesRegion, hintRegion;
    public GameObject backTilePrefab, selectedTilePrefab;
    public Tile tilePrefab;
    public GameObject listTilesScreen, listHintsScreen;
    public Transform listTilesRoot;
    public Text addTileText, addStarText;
    public GameObject targetMoveInput;
    public GameObject buttonAddStar;
    public Transform gridTiles, gridHints;

    private RectTransform rt;
    private GameObject selectedTile;
    private Vector3 selectedPosition;
    private List<Tile> pathTiles;
    private Tile startTile, goalTile;
    private int numStarLoaded;
    private bool addLevel, loadLevel, hintGenerated;

    [HideInInspector]
    public Level level;

    public static BoardEditor instance;

    private void Awake()
    {
        instance = this;
    }

    private void ResetVariables()
    {
        Destroy(selectedTile);
        foreach (Transform child in tilesRegion) Destroy(child.gameObject);
        foreach (Transform child in hintRegion) Destroy(child.gameObject);
        foreach (Transform child in backsRegion) Destroy(child.gameObject);

        pathTiles = new List<Tile>();
        startTile = null;
        goalTile = null;
        numStarLoaded = 0;

        addTileText.text = "add tile";
        addStarText.text = "add star";
        targetMoveInput.GetComponent<InputField>().text = "0";

        tiles.Clear();
        hints.Clear();
    }

    private void LoadBaseBoard()
    {
        rt = GetComponent<RectTransform>();
        tileWidth = rt.rect.width / size;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject tile = Instantiate(backTilePrefab);
                tile.transform.SetParent(backsRegion);
                tile.transform.localScale = Vector3.one;
                tile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileWidth, tileWidth);

                tile.transform.localPosition = GetLocalPosition(new Vector3(j, i));
            }
        }

        selectedTile = Instantiate(selectedTilePrefab);
        selectedTile.transform.SetParent(transform);
        selectedTile.transform.localScale = Vector3.one;
        selectedTile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileWidth, tileWidth);

        selectedTile.transform.localPosition = GetLocalPosition(new Vector3(0, 0));
        selectedPosition = Vector3.zero;

        if (level.mode == Level.LevelMode.Classic)
        {
            buttonAddStar.SetActive(false);
            targetMoveInput.SetActive(true);
        }
        else
        {
            buttonAddStar.SetActive(true);
            targetMoveInput.SetActive(false);
        }
    }

    public void AddLevel()
    {
        ResetVariables();

#if UNITY_EDITOR
        level = ScriptableObject.CreateInstance<Level>();
        level.size = size = LevelEditorController.instance.boardSize;
        level.mode = LevelEditorController.instance.levelmode;

        int worldIndex = LevelEditorController.instance.worldIndex;
        string folderPath = "Assets/Resources/" + level.mode + "/World_" + worldIndex;
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/Resources/" + level.mode, "World_" + worldIndex);
#endif

        LoadBaseBoard();
        addLevel = true;
    }

    public void LoadLevel(Level level)
    {
        this.level = level;
        ResetVariables();

        size = level.size;
        targetMoveInput.GetComponent<InputField>().text = level.targetMove.ToString();

        LoadBaseBoard();

        int index = 0;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                if (index < level.maps.Length)
                {
                    TileP p = level.maps[index];
                    if (p.type != Tile.Type.None)
                    {
                        Tile tile = Instantiate(MonoUtils.instance.tilePrefab);
                        tile.transform.SetParent(tilesRegion);
                        tile.transform.localScale = Vector3.one;
                        tile.p = p;
                        tile.position = new Vector3(col, row);
                        tile.width = rt.rect.width / size;
                        tile.onTileMoveComplete += OnMoveTileComplete;

                        tile.transform.localPosition = tile.localPosition;
                        tiles.Add(tile.position, tile);

                        if (p.type == Tile.Type.Start)
                        {
                            startTile = tile;
                        }
                        else if (p.type == Tile.Type.Goal)
                        {
                            goalTile = tile;
                        }

                        if (numStarLoaded == 3 && p.hasStar)
                        {
                            p.hasStar = false;
                        }

                        if (p.hasStar)
                        {
                            GameObject star = Instantiate(MonoUtils.instance.starPrefab);
                            star.transform.SetParent(tile.transform);
                            star.transform.localScale = Vector3.one;
                            star.transform.localPosition = Vector3.zero;
                            tile.star = star;
                            SetStarPosition(tile, star);
                            numStarLoaded++;
                        }
                    }
                }
                index++;
            }
        }

        List<Vector3[]> hintPaths = new List<Vector3[]>();
        for (int i = 0; i < level.hintPath.Length; i++)
        {
            if (i == 0)
            {
                hintPaths.Add(new Vector3[1] { level.hintPath[i + 1] - level.hintPath[i] });
            }
            else if (i == level.hintPath.Length - 1)
            {
                hintPaths.Add(new Vector3[1] { level.hintPath[i - 1] - level.hintPath[i] });
            }
            else
            {
                hintPaths.Add(new Vector3[2] { level.hintPath[i - 1] - level.hintPath[i], level.hintPath[i + 1] - level.hintPath[i] });
            }
        }

        index = 0;
        foreach (var hintPos in level.hintPath)
        {
            TileHint tileHint = Instantiate(MonoUtils.instance.tileHintPrefab);
            tileHint.transform.SetParent(hintRegion);
            tileHint.transform.localScale = Vector3.one;
            tileHint.width = rt.rect.width / size;

            tileHint.position = hintPos;
            tileHint.transform.localPosition = tileHint.localPosition;

            tileHint.paths = hintPaths[index];
            tileHint.UpdateUI();

            hints.Add(hintPos, tileHint);
            index++;
        }

        ShowHint();

        UpdateAddStarText();
        UpdateAddTileText();

        loadLevel = true;
    }

    private void SetStarPosition(Tile tile, GameObject star)
    {
        if (tile.p.paths.Length < 2)
            star.transform.localPosition = Vector3.zero;
        else
        {
            var path1 = GetPathVector(tile.p.paths[0]);
            var path2 = GetPathVector(tile.p.paths[1]);

            if (path1 + path2 == Vector3.zero)
            {
                star.transform.localPosition = Vector3.zero;
            }
            else
            {
                star.transform.localPosition = (path1 + path2) * 0.13f * tile.width;
            }
        }
    }

    private Vector3 GetLocalPosition(Vector3 position)
    {
        return new Vector3((position.x + 0.5f) * tileWidth, (position.y + 0.5f) * tileWidth);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3 localPosition = transform.InverseTransformPoint(worldPos);

        float x = (int)(localPosition.x / tileWidth);
        float y = (int)(localPosition.y / tileWidth);

        selectedPosition = new Vector3(x, y);
        selectedTile.transform.localPosition = GetLocalPosition(selectedPosition);

        UpdateAddTileText();
        UpdateAddStarText();
    }

    private void UpdateAddTileText()
    {
        if (tiles.ContainsKey(selectedPosition))
        {
            addTileText.text = "edit tile";
        }
        else
        {
            addTileText.text = "add tile";
        }
    }

    private void UpdateAddStarText()
    {
        if (tiles.ContainsKey(selectedPosition) && tiles[selectedPosition].p.hasStar)
        {
            addStarText.text = "del star";
        }
        else
        {
            addStarText.text = "add star";
        }
    }

    private bool CheckAddorLoadLevel()
    {
        if (addLevel == false && loadLevel == false)
        {
            Debug.LogError("You need to add or load level first");
            return false;
        }
        return true;
    }

    public void CloseListTileScreen()
    {
        listTilesScreen.SetActive(false);
    }

    public void OpenListTileScreen()
    {
        if (!CheckAddorLoadLevel()) return;
        listTilesScreen.SetActive(true);
    }

    public void CloseListHintScreen()
    {
        listHintsScreen.SetActive(false);
    }

    public void OpenListHintScreen()
    {
        if (!CheckAddorLoadLevel()) return;
        listHintsScreen.SetActive(true);
    }

    public void AddStar()
    {
        if (!CheckAddorLoadLevel()) return;

        HideHint();
        if (tiles.ContainsKey(selectedPosition))
        {
            var tile = tiles[selectedPosition];
            if (!tile.p.hasStar && tile.p.paths.Length > 0)
            {
                if (GetNumStarAdded() == 3)
                {
                    Debug.LogError("The maximum number of star is 3");
                }
                else
                {
                    tile.p.hasStar = true;
                    GameObject star = Instantiate(MonoUtils.instance.starPrefab);
                    star.transform.SetParent(tile.transform);
                    star.transform.localScale = Vector3.one;
                    star.transform.localPosition = Vector3.zero;
                    tile.star = star;
                    SetStarPosition(tile, star);
                }
            }
            else if (tile.p.hasStar)
            {
                tile.p.hasStar = false;
                Destroy(tile.star);
            }
        }

        UpdateAddStarText();
    }

    private int GetNumStarAdded()
    {
        int count = 0;
        foreach(var tile in tiles.Values)
        {
            if (tile.p.hasStar) count++;
        }
        return count;
    }

    public void GenerateHintPath()
    {
        if (!CheckAddorLoadLevel()) return;

        pathTiles = new List<Tile>();
        bool connected = CheckConnected();
        if (!connected)
        {
            if (startTile == null)
                Debug.LogError("Can not generate hint path because there is no start tile");
            else if (goalTile == null)
                Debug.LogError("Can not generate hint path because there is no goal tile");
            else
                Debug.LogError("Can not generate hint path because tiles are not connected");
            return;
        }


        foreach(Transform child in hintRegion)
        {
            Destroy(child.gameObject);
            hints.Remove(child.GetComponent<TileHint>().position);
        }

        var hintPath = pathTiles.Select(x => x.position).ToArray();

        List<Vector3[]> hintPaths = new List<Vector3[]>();
        for (int i = 0; i < hintPath.Length; i++)
        {
            if (i == 0)
            {
                hintPaths.Add(new Vector3[1] { hintPath[i + 1] - hintPath[i] });
            }
            else if (i == hintPath.Length - 1)
            {
                hintPaths.Add(new Vector3[1] { hintPath[i - 1] - hintPath[i] });
            }
            else
            {
                hintPaths.Add(new Vector3[2] { hintPath[i - 1] - hintPath[i], hintPath[i + 1] - hintPath[i] });
            }
        }

        int index = 0;
        foreach (var hintPos in hintPath)
        {
            TileHint tileHint = Instantiate(MonoUtils.instance.tileHintPrefab);
            tileHint.transform.SetParent(hintRegion);
            tileHint.transform.localScale = Vector3.one;
            tileHint.width = rt.rect.width / size;

            tileHint.position = hintPos;
            tileHint.transform.localPosition = tileHint.localPosition;

            tileHint.paths = hintPaths[index];
            tileHint.UpdateUI();

            hints.Add(hintPos, tileHint);
            index++;
        }

        ShowHint();
        hintGenerated = true;
    }

    private void SaveToAsset()
    {
#if UNITY_EDITOR
        CreateOrReplaceAsset(level, "Assets/Resources/" + level.mode + "/World_" + LevelEditorController.instance.worldIndex + "/Level_" + LevelEditorController.instance.levelIndex + ".asset");
        AssetDatabase.SaveAssets();
#endif

        LevelEditorController.instance.UpdateLoadLevelText();
    }

    private T CreateOrReplaceAsset<T>(T asset, string path) where T : ScriptableObject
    {
#if UNITY_EDITOR
        T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (existingAsset == null)
        {
            AssetDatabase.CreateAsset(asset, path);
            existingAsset = asset;
        }
        else
        {
            EditorUtility.CopySerialized(asset, existingAsset);
        }

        return existingAsset;
#else
        return null;
#endif
    }

    public void SaveLevel()
    {
        if (!CheckAddorLoadLevel()) return;

        level.size = size;

        var hintPath = new List<Vector3>();
        foreach (Transform child in hintRegion) hintPath.Add(child.GetComponent<TileHint>().position);

        if (hintPath.Count == 0)
        {
            Debug.LogError("You need to generate hints");
            return;
        }
        else if (hintPath.Count == 1)
        {
            Debug.LogError("Something wrong with hint path");
            return;
        }

        if (level.mode == Level.LevelMode.Classic)
        {
            if (LevelEditorController.instance.targetMove == 0)
            {
                Debug.LogWarning("You need to input the target move");
            }
            level.targetMove = LevelEditorController.instance.targetMove;
        }

        level.hintPath = hintPath.ToArray();

        List<TileP> list = new List<TileP>();

        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                Vector3 position = new Vector3(x, y);
                TileP p = new TileP();

                if (tiles.ContainsKey(position))
                {
                    var tile = tiles[position];
                    p = tile.p;
                }
                else
                {
                    p.type = Tile.Type.None;
                }

                list.Add(p);
            }
        }

        level.maps = list.ToArray();

        SaveToAsset();

        int numStarAdded = GetNumStarAdded();
        if (numStarAdded > 0 && level.mode == Level.LevelMode.Classic)
        {
            Debug.LogError("No need star in Classic mode");
            return;
        }

        if (numStarAdded != 3 && level.mode == Level.LevelMode.Star)
        {
            Debug.LogError("We need 3 stars in Star mode");
            return;
        }

        Debug.Log("SAVED");
    }

    public void OnTileSelected(int i)
    {
        CloseListTileScreen();

        Transform button = gridTiles.GetChild(i);

        Sprite sprite = button.GetComponent<Image>().sprite;

        if (sprite == null)
        {
            if (tiles.ContainsKey(selectedPosition))
            {
                var tile = tiles[selectedPosition];
                tiles.Remove(selectedPosition);

                if (tile == startTile) startTile = null;
                else if (tile == goalTile) goalTile = null;

                Destroy(tile.gameObject);
                addTileText.text = "add tile";
            }
        }
        else
        {
            int index = 0;
            foreach (var s in MonoUtils.instance.tileSprites)
            {
                if (s == sprite)
                {
                    break;
                }
                index++;
            }

            var monoP = MonoUtils.instance.tilePs[index];
            var p = new TileP
            {
                type = monoP.type,
                paths = monoP.paths,
                hasStar = monoP.hasStar
            };

            if (p.type == Tile.Type.Start && startTile != null && startTile.position != selectedPosition)
            {
                Debug.LogError("Start tile already exists");
                return;
            }
            else if (p.type == Tile.Type.Goal && goalTile != null && goalTile.position != selectedPosition)
            {
                Debug.LogError("Goal tile already exists");
                return;
            }

            Tile tile;
            if (tiles.ContainsKey(selectedPosition))
            {
                tile = tiles[selectedPosition];

                if (tile == startTile) startTile = null;
                else if (tile == goalTile) goalTile = null;

                p.hasStar = tile.p.hasStar;

                tile.p = p;
                tile.UpdateUI();
            }
            else
            {
                tile = Instantiate(tilePrefab);
                tile.transform.SetParent(tilesRegion);
                tile.transform.localScale = Vector3.one;
                tile.width = tileWidth;
                tile.position = selectedPosition;
                tile.transform.localPosition = tile.localPosition;
                tile.onTileMoveComplete += OnMoveTileComplete;

                tiles.Add(selectedPosition, tile);
                tile.p = p;
            }

            if (p.type == Tile.Type.Start)
            {
                startTile = tile;
            }
            else if (p.type == Tile.Type.Goal)
            {
                goalTile = tile;
            }

            addTileText.text = "edit tile";
        }
    }

    public void AddHint()
    {
        if (!hints.ContainsKey(selectedPosition))
        {
            TileHint tileHint = Instantiate(MonoUtils.instance.tileHintPrefab2);
            tileHint.transform.SetParent(hintRegion);
            tileHint.transform.localScale = Vector3.one;
            tileHint.width = tileWidth;

            tileHint.position = selectedPosition;
            tileHint.transform.localPosition = tileHint.localPosition;

            hints.Add(selectedPosition, tileHint);
        }

        ShowHint();
    }

    public void RemoveHint()
    {
        if (hints.ContainsKey(selectedPosition))
        {
            var hint = hints[selectedPosition];
            hints.Remove(selectedPosition);

            Destroy(hint.gameObject);
        }
    }

    public void OnMoveTileComplete(Tile tile, Vector3 newPosition)
    {
        ChangeTilePosition(tile, newPosition);
        UpdateAddTileText();

        if (hintGenerated)
        {
            int targetMove = LevelEditorController.instance.targetMove;
            targetMove++;
            targetMoveInput.GetComponent<InputField>().text = targetMove.ToString();
        }
    }

    public bool CanMove(Tile tile, Vector3 direct)
    {
        Vector3 newPosition = tile.position + direct;
        if (newPosition.x >= size || newPosition.x < 0 || newPosition.y >= size || newPosition.y < 0) return false;
        return !tiles.ContainsKey(newPosition);
    }

    private void ChangeTilePosition(Tile tile, Vector3 newPosition)
    {
        tiles.Remove(tile.position);
        tile.position = newPosition;
        tiles.Add(newPosition, tile);
    }

    private bool CheckConnected()
    {
        if (startTile == null || goalTile == null) return false;

        Tile cursor = startTile;
        var outPath = cursor.p.paths[0];
        pathTiles.Add(cursor);

        while (true)
        {
            var nextTile = GetTile(cursor.position + GetPathVector(outPath));
            if (nextTile == null || nextTile.p.paths.Length == 0) return false;

            bool match = false;
            Tile.Path newOutpath = Tile.Path.Down;

            foreach (var path in nextTile.p.paths)
            {
                if (GetPathVector(path) + GetPathVector(outPath) == Vector3.zero)
                {
                    match = true;
                }
                else
                {
                    newOutpath = path;
                }
            }

            outPath = newOutpath;
            cursor = nextTile;
            pathTiles.Add(cursor);

            if (match == false) return false;
            else if (nextTile == goalTile) return true;
        }
    }

    private Vector3 GetPathVector(Tile.Path path)
    {
        return path == Tile.Path.Up ? Vector3.up :
                path == Tile.Path.Down ? Vector3.down :
                path == Tile.Path.Left ? Vector3.left : Vector3.right;
    }

    private Tile GetTile(Vector3 position)
    {
        return !tiles.ContainsKey(position) ? null : tiles[position];
    }

    [HideInInspector]
    public bool hintShowing;
    public void ShowHint()
    {
        hintRegion.gameObject.SetActive(true);
        hintShowing = true;
    }

    public void HideHint()
    {
        hintRegion.gameObject.SetActive(false);
        hintShowing = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnTileSelected(18);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            OnTileSelected(0);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            AddHint();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveHint();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Timer.Schedule(this, 0.1f, () =>
            {
                OpenListTileScreen();
            });
        }
    }
}
