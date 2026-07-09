using UnityEngine;
using UnityEngine.Video;

public class VideoPlay : MonoBehaviour
{
    [SerializeField] private GameObject videoRoot;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool hideAfterFinished = true;
    [SerializeField] private bool playStartAnimationAfterFinished = true;

    private VideoPlayer video;
    private bool completed;
    private bool pendingPlay;

    private void Awake()
    {
        EnsureVideoReference();
        if (videoRoot == null)
            videoRoot = gameObject;
    }

    private void OnEnable()
    {
        EnsureVideoReference();
        if (video != null)
        {
            video.loopPointReached -= OnVideoFinished;
            video.loopPointReached += OnVideoFinished;
            video.prepareCompleted -= OnVideoPrepared;
            video.prepareCompleted += OnVideoPrepared;
        }
    }

    private void OnDisable()
    {
        if (video != null)
        {
            video.loopPointReached -= OnVideoFinished;
            video.prepareCompleted -= OnVideoPrepared;
        }
    }

    private void Start()
    {
        if (playOnStart)
        {
            PlayVideo();
        }
        else if (videoRoot != null)
        {
            videoRoot.SetActive(false);
        }
    }

    public void PlayVideo()
    {
        completed = false;
        pendingPlay = true;

        if (videoRoot != null && !videoRoot.activeSelf)
            videoRoot.SetActive(true);

        EnsureVideoReference();

        if (video == null)
        {
            pendingPlay = false;
            if (playStartAnimationAfterFinished)
                StartGameAnimation();
            return;
        }

        video.Stop();
        video.time = 0;
        video.frame = 0;
        video.waitForFirstFrame = true;
        video.Prepare();
    }

    public void StopVideo()
    {
        pendingPlay = false;

        EnsureVideoReference();
        if (video != null)
        {
            video.Stop();
            video.time = 0;
            video.frame = 0;
        }

        if (videoRoot != null)
            videoRoot.SetActive(false);
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        if (!pendingPlay)
            return;

        pendingPlay = false;
        source.time = 0;
        source.frame = 0;
        source.Play();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        if (completed)
            return;

        completed = true;
        pendingPlay = false;

        if (playStartAnimationAfterFinished)
            StartGameAnimation();

        if (hideAfterFinished && videoRoot != null)
            videoRoot.SetActive(false);
    }

    private void EnsureVideoReference()
    {
        if (video == null)
            video = GetComponent<VideoPlayer>();
    }

    private void StartGameAnimation()
    {
        HomeController homeController = FindObjectOfType<HomeController>();
        if (homeController != null)
            homeController.PlayStartAnimation();
    }
}