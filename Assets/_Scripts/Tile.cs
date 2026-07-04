using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Tile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum Type { Start, Goal, Fixed, Normal, None };
    public enum Path { Up, Down, Left, Right };

    [HideInInspector]
    public TileP p;

    public float width;
    public Vector3 position;
    public Vector3 localPosition
    {
        get { return GetLocalPosition(position); }
    }

    public Image image;

    [HideInInspector]
    public GameObject star;
    public Action<Tile, Vector3> onTileMoveComplete;

    private Vector3 moveDirect;

    private void Start()
    {
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 2.5f, width - 2.5f);
        UpdateUI();
    }

    public void UpdateUI()
    {
        var path1 = p.paths;
        int index = 0;
        foreach (var tileP in MonoUtils.instance.tilePs)
        {
            var path2 = tileP.paths;
            if (tileP.type == p.type && path1.Length == path2.Length && path1.Except(path2).Count() == 0)
            {
                break;
            }
            index++;
        }

        image.sprite = MonoUtils.instance.tileSprites[index];
    }

    public Vector3 GetLocalPosition(Vector3 position)
    {
        return new Vector3((position.x + 0.5f) * width, (position.y + 0.5f) * width);
    }

    private bool dragging;
    private Vector3 beginMousePosition;
    private float beginDragTime;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (MainController.instance != null && MainController.instance.isComplete) return;
        if (Board.instance != null && Board.instance.hintShowing) Board.instance.HideHint();
        if (BoardEditor.instance != null && BoardEditor.instance.hintShowing) BoardEditor.instance.HideHint();

        dragging = true;
        beginDragTime = Time.time;
        beginMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragging == false) return;

        var delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - beginMousePosition;
        moveDirect = Vector2.zero;
        float moveLength = 0;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            moveDirect = delta.x > 0 ? Vector2.right : Vector2.left;
            moveLength = Mathf.Abs(delta.x);
        }
        else
        {
            moveDirect = delta.y > 0 ? Vector2.up : Vector2.down;
            moveLength = Mathf.Abs(delta.y);
        }

        if (CanMove(moveDirect) && dragging)
        {
            transform.localPosition = localPosition + moveDirect * moveLength * 100;
            if (Vector3.Distance(transform.localPosition, localPosition) > width / 5)
            {
                Vector2 newPosition = position + moveDirect;
                iTween.MoveTo(gameObject, iTween.Hash("position", GetLocalPosition(newPosition), "isLocal", true, "time", 0.03f, "oncomplete", "OnTileMoveComplete"));
                dragging = false;

                if (Sound.instance != null)
                    Sound.instance.Play(Sound.Others.Slide);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragging)
        {
            float distance = Vector3.Distance(transform.localPosition, localPosition);
            float speed = distance / (Time.time - beginDragTime);
            if (speed > 0 && speed < 100)
            {
                iTween.MoveTo(gameObject, iTween.Hash("position", localPosition, "isLocal", true, "time", 0.02f));
            }
            else if (speed > 100 && CanMove(moveDirect))
            {
                Vector2 newPosition = position + moveDirect;
                iTween.MoveTo(gameObject, iTween.Hash("position", GetLocalPosition(newPosition), "isLocal", true, "time", 0.03f, "oncomplete", "OnTileMoveComplete"));

                if (Sound.instance != null)
                    Sound.instance.Play(Sound.Others.Slide);
            }
        }
        dragging = false;
    }
       
    public bool CanMove(Vector2 moveDirect)
    {
        if (p.type != Type.Normal) return false;

        if (Board.instance != null)
            return Board.instance.CanMove(this, moveDirect);
        else
            return BoardEditor.instance.CanMove(this, moveDirect);
    }

    private void OnTileMoveComplete()
    {
        var newPosition = position + moveDirect;
        onTileMoveComplete(this, newPosition);
    }
}
