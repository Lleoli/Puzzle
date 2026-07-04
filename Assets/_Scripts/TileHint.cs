using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TileHint : MonoBehaviour
{
    public float width;
    public Vector3 position;
    public Vector3 localPosition
    {
        get { return GetLocalPosition(position); }
    }

    public Image image;
    public Vector3[] paths;

    public Vector3 GetLocalPosition(Vector3 position)
    {
        return new Vector3((position.x + 0.5f) * width, (position.y + 0.5f) * width);
    }

    private void Start()
    {
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(width + 1, width + 1);
    }

    public void UpdateUI()
    {
        int index = 0;
        foreach (var path in MonoUtils.instance.hintPaths)
        {
            if (paths.Length == path.array.Length && paths.Except(path.array).Count() == 0)
            {
                break;
            }
            index++;
        }
        image.sprite = MonoUtils.instance.hintSprites[index];
    }
}
