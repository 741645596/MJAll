using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// 雷达型进度条，比如用在技能倒计时显示
/// </summary>
[AddComponentMenu("UI/Progress Radial")]
public class ProgressRadial : MonoBehaviour
{
    public float duration = 5;      // 播放倒计时时间
    public float elapseTime = 0;    // 从第几秒开始播放
    public bool isPlay = false;     // 是否立即播放

    private Action _finishCallback;
    private Image _image;

    /// <summary>
    /// 播放倒计时动画
    /// </summary>
    public void Play()
    {
        isPlay = true;
    }

    /// <summary>
    /// 指定播放时间和开始时间播放倒计时
    /// </summary>
    /// <param name="duration"> 播放时长 </param>
    /// <param name="elapseTime"> 从第几秒开始播放 </param>
    public void Play(float duration, float elapseTime)
    {
        this.duration = duration;
        this.elapseTime = elapseTime;
        isPlay = true;
    }

    /// <summary>
    /// 暂停动画，可以通过Play继续之前动画
    /// </summary>
    public void Pause()
    {
        isPlay = false;
    }

    /// <summary>
    /// 停止动画，重置状态为0
    /// </summary>
    public void Stop()
    {
        isPlay = false;
        elapseTime = 0;
        _image.fillAmount = 0;
    }

    /// <summary>
    /// 动画结束回调
    /// </summary>
    /// <param name="callback"></param>
    public void SetFinish(Action callback)
    {
        _finishCallback = callback;
    }

    protected void Start()
    {
        _image = GetComponent<Image>();
    }

    protected void Update()
    {
        if (isPlay == false)
        {
            return;
        }

        if (elapseTime > duration)
        {
            isPlay = false;
            elapseTime = 0;
            _finishCallback?.Invoke();
            return;
        }

        elapseTime += Time.deltaTime;
        var per = elapseTime / duration;
        _image.fillAmount = 1.0f - per;
    }

    /// <summary>
    /// 用于编辑器，右键创建默认模板
    /// </summary>
    /// <returns></returns>
    public static GameObject CreateTemplate()
    {
        GameObject obj = new GameObject("Progress Radial", typeof(Image));
        RectTransform trs = obj.transform as RectTransform;

        var image = obj.GetComponent<Image>();
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Radial360;
        image.fillOrigin = (int)Image.Origin360.Top;
        image.fillClockwise = false;

        return obj;
    }
}
