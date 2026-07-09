using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackImageSequence : MonoBehaviour {
    public string folderName;
    public float fps = 30;
    public bool loop = true;
    public bool playOnStart = true;
    public float startDelay = 0;
    public bool pingPong = true;
    public bool accelerate = true;
    public float endSpeedMultiplier = 3f;

    public Image image;
    private Sprite[] sprites;
    private Coroutine playCoroutine;

    private void Start()
    {
        sprites = Resources.LoadAll<Sprite>(folderName);

        if (playOnStart)
        {
            Invoke("Play", startDelay);
        }
    }

    public void Play()
    {
        if (playCoroutine != null)
            StopCoroutine(playCoroutine);

        playCoroutine = StartCoroutine(IEPlay());
    }

    public void Stop()
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
    }

    private IEnumerator IEPlay()
    {
        if (sprites == null || sprites.Length == 0 || image == null || fps <= 0)
            yield break;

        do
        {
            yield return PlayRange(0, sprites.Length - 1, 1);

            if (pingPong && sprites.Length > 1)
                yield return PlayRange(sprites.Length - 2, 0, -1);
        } while (loop);

        playCoroutine = null;
    }

    private IEnumerator PlayRange(int startIndex, int endIndex, int step)
    {
        int count = Mathf.Abs(endIndex - startIndex) + 1;
        for (int i = 0, index = startIndex; i < count; i++, index += step)
        {
            image.sprite = sprites[index];
            image.SetNativeSize();
            yield return new WaitForSeconds(GetFrameDelay(index));
        }
    }

    private float GetFrameDelay(int spriteIndex)
    {
        float baseDelay = 1f / fps;
        if (!accelerate || sprites == null || sprites.Length <= 1)
            return baseDelay;

        float progress = spriteIndex / (float)(sprites.Length - 1);
        float speedMultiplier = Mathf.Lerp(1f, Mathf.Max(1f, endSpeedMultiplier), progress);
        return baseDelay / speedMultiplier;
    }
}