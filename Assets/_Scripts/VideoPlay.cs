using UnityEngine;
using UnityEngine.Video;

public class VideoPlay : MonoBehaviour
{
    private const float RevealFallbackDelay = 0.12f;

    [SerializeField] private GameObject videoRoot;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool hideAfterFinished = true;
    [SerializeField] private bool playStartAnimationAfterFinished = true;

    private VideoPlayer video;
    private CanvasGroup videoCanvasGroup;
    private Coroutine revealFallbackCoroutine;
    private bool completed;
    private bool pendingPlay;
    private bool waitingFirstFrame;

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
            video.frameReady -= OnFrameReady;
            video.frameReady += OnFrameReady;
        }
    }

    private void OnDisable()
    {
        StopRevealFallback();

        if (video != null)
        {
            video.loopPointReached -= OnVideoFinished;
            video.prepareCompleted -= OnVideoPrepared;
            video.frameReady -= OnFrameReady;
        }
    }

    private void Start()
    {
        if (ShouldSkipCachedHomeStartVideo())
        {
            StopVideo();
            HomeController homeController = FindObjectOfType<HomeController>();
            if (homeController != null)
                homeController.ShowStartAnimationFinalState();
            return;
        }

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
        waitingFirstFrame = false;
        StopRevealFallback();

        if (videoRoot != null && !videoRoot.activeSelf)
            videoRoot.SetActive(true);

        SetVideoVisible(false);
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
        video.sendFrameReadyEvents = true;
        video.Prepare();
    }

    public void StopVideo()
    {
        pendingPlay = false;
        waitingFirstFrame = false;
        StopRevealFallback();

        EnsureVideoReference();
        if (video != null)
        {
            video.sendFrameReadyEvents = false;
            video.Stop();
            video.time = 0;
            video.frame = 0;
        }

        SetVideoVisible(false);

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
        waitingFirstFrame = true;
        source.Play();
        revealFallbackCoroutine = StartCoroutine(RevealVideoAfterFallback(source));
    }

    private void OnFrameReady(VideoPlayer source, long frameIdx)
    {
        if (!waitingFirstFrame)
            return;

        RevealVideo();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        if (completed)
            return;

        completed = true;
        pendingPlay = false;
        waitingFirstFrame = false;
        StopRevealFallback();

        if (playStartAnimationAfterFinished)
            StartGameAnimation();

        if (hideAfterFinished && videoRoot != null)
            videoRoot.SetActive(false);
    }

    private bool ShouldSkipCachedHomeStartVideo()
    {
        return playOnStart && playStartAnimationAfterFinished && HomeController.HasStartAnimationCache() && FindObjectOfType<HomeController>() != null;
    }

    private void EnsureVideoReference()
    {
        if (video == null)
            video = GetComponent<VideoPlayer>();
    }

    private void EnsureVideoCanvasGroup()
    {
        if (videoRoot == null || videoCanvasGroup != null)
            return;

        videoCanvasGroup = videoRoot.GetComponent<CanvasGroup>();
        if (videoCanvasGroup == null)
            videoCanvasGroup = videoRoot.AddComponent<CanvasGroup>();
    }

    private void SetVideoVisible(bool visible)
    {
        EnsureVideoCanvasGroup();
        if (videoCanvasGroup == null)
            return;

        videoCanvasGroup.alpha = visible ? 1f : 0f;
    }

    private void RevealVideo()
    {
        waitingFirstFrame = false;
        StopRevealFallback();

        if (video != null)
            video.sendFrameReadyEvents = false;

        SetVideoVisible(true);
    }

    private System.Collections.IEnumerator RevealVideoAfterFallback(VideoPlayer source)
    {
        yield return new WaitForSecondsRealtime(RevealFallbackDelay);

        if (waitingFirstFrame && source != null && source.isPlaying)
            RevealVideo();
    }

    private void StopRevealFallback()
    {
        if (revealFallbackCoroutine == null)
            return;

        StopCoroutine(revealFallbackCoroutine);
        revealFallbackCoroutine = null;
    }

    private void StartGameAnimation()
    {
        HomeController homeController = FindObjectOfType<HomeController>();
        if (homeController != null)
            homeController.PlayStartAnimation();
    }
}