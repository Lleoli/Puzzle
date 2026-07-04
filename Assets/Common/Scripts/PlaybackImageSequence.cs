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

    public Image image;
    private Sprite[] sprites;

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
        StartCoroutine(IEPlay());
    }

    private IEnumerator IEPlay()
    {
        do
        {
            foreach(var sprite in sprites)
            {
                image.sprite = sprite;
                image.SetNativeSize();
                yield return new WaitForSeconds(1 / fps);
            }
        } while (loop);
    }
}
