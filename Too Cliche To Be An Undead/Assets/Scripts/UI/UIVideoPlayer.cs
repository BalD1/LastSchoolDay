using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static UnityEditor.Progress;

public class UIVideoPlayer : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    public VideoPlayer GetVideoPlayer { get => videoPlayer; }

    [SerializeField] private RawImage image;
    public RawImage GetImage { get => image; }

    [SerializeField] private S_VideoByTag[] videosByTag;

    [System.Serializable]
    public struct S_VideoByTag
    {
#if UNITY_EDITOR
        [SerializeField] private string EDITOR_name; 
#endif
        [field: SerializeField] public E_VideoTag tag { get; private set;}
        [field: SerializeField] public VideoClip clip { get; private set;}
        [field: SerializeField] public bool loopVideo { get; private set; }
    }
    public enum E_VideoTag { SplashScreen, BookOpening, }

    public void SetClipWithoutPlaying(E_VideoTag searchedVideo)
    {
        foreach (var item in videosByTag)
        {
            if (item.tag == searchedVideo)
            {
                Color c = image.color;
                c.a = 0;
                image.color = c;

                videoPlayer.clip = item.clip;
                videoPlayer.isLooping = item.loopVideo;

                videoPlayer.Play();
                videoPlayer.Pause();
                return;
            }
        }
    }

    public void StartVideo(VideoPlayer.EventHandler onCompleteAction = null)
    {
        LeanTween.cancel(image.gameObject);

        videoPlayer.Stop();
        videoPlayer.Play();

        FadeVideo(true, .1f, null);

        videoPlayer.loopPointReached += onCompleteAction;
    }

    public void SetNewVideo(E_VideoTag searchedVideo, VideoPlayer.EventHandler onCompleteAction = null)
    {
        foreach (var item in videosByTag)
        {
            if (item.tag == searchedVideo)
            {
                LeanTween.cancel(image.gameObject);

                videoPlayer.Stop();
                videoPlayer.clip = item.clip;
                videoPlayer.isLooping = item.loopVideo;
                videoPlayer.Play();

                videoPlayer.loopPointReached += onCompleteAction;

                Color c = image.color;
                c.a = 1;
                image.color = c;
                return;
            }
        }
    }

    public void ResumeVideo()
    {
        videoPlayer.Play();
    }

    public void FadeVideo(bool fadeIn, float time, Action onCompleteAction = null)
    {
        LeanTween.value(image.color.a, fadeIn ? 1 : 0, time)
        .setOnUpdate(
        (float val) =>
        {
            Color c = image.color;
            c.a = val;
            image.color = c;
        }).setIgnoreTimeScale(true)
        .setOnComplete(onCompleteAction);
    }

    public IEnumerator WaitForAction(float timeToWait, Action actionToPlay)
    {
        yield return new WaitForSecondsRealtime(timeToWait);

        actionToPlay?.Invoke();
    }
}
