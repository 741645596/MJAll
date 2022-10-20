using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// 使用Webm格式播放透明视频时，在Android真机上会有黑屏闪一下，该组件主要用来修复该问题
/// 需要挂在VideoPlayer和RawImage
/// </summary>
public class VideoPlayerBugFix : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;

    /// <summary>
    /// 重播
    /// </summary>
    public void Replay()
    {
        videoPlayer.frame = 0;
        videoPlayer.Play();
    }

    /// <summary>
    /// 播放帧数量
    /// </summary>
    /// <returns></returns>
    public ulong GetFrameCout()
    {
        return videoPlayer.frameCount;
    }

    /// <summary>
    /// 视频总时长
    /// </summary>
    /// <returns></returns>
    public double GetDuration()
    {
        return videoPlayer.length;
    }

    protected void Start()
    {
        // 先隐藏，开始在播放
        rawImage.enabled = false;
        videoPlayer.started += (vp) =>
        {
            rawImage.enabled = true;
        };
    }
}
