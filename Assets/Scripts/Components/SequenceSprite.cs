using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 序列帧动画
/// </summary>
[AddComponentMenu("UI/Sequence Sprite")]
public class SequenceSprite: MonoBehaviour
{
    public float interval = 1f / 24f;   // 播放帧率
    public int loops = 0;               // 播放次数，默认0表示无限循环
    public bool isPlaying = true;       // 是否播放
    public Sprite[] sprites;            // 序列帧

    private Image _image;
    private float _elapseTime;
    private int _curIndex;
    private int _curLoop;
    private Action _finishCB;
    private bool _isActive;

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <returns></returns>
    public void Play()
    {
        _curIndex = 0;
        _curLoop = 0;
        _elapseTime = 0;
        isPlaying = true;
    }

    /// <summary>
    /// 从第Index帧开始播放，索引从0开始
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public void PlayFromIndex(int index)
    {
        _curIndex = sprites.Length > index ? index : 0;
        _curLoop = 0;
        _elapseTime = 0;
        isPlaying = true;
    }

    /// <summary>
    /// 停止播放动画
    /// </summary>
    public void Stop()
    {
        isPlaying = false;
    }

    /// <summary>
    /// 设置动画帧率
    /// </summary>
    /// <param name="time">time 动画间隔时间 单位：秒</param>
    /// <returns></returns>
    public void SetInterval(float time)
    {
        interval = time;
    }

    /// <summary>
    /// 动画结束回调
    /// </summary>
    /// <param name="callback"></param>
    public void SetFinishCallback(Action callback)
    {
        _finishCB = callback;
    }


    protected void Start()
    {
        _image = gameObject.GetComponent<Image>();
        if (_image == null)
        {
            Debug.LogWarning("错误提示：AnimationSprite 未配置Image组件，请检查组件");
        }

        if (sprites == null)
        {
            Debug.LogWarning("错误提示：AnimationSprite 未配置Sprites变量，请检查组件");
            sprites = new Sprite[0];
        }
    }

    protected void Update()
    {
        if (!_isActive)
        {
            return;
        }

        if (!isPlaying)
        {
            return;
        }

        _elapseTime += Time.deltaTime;
        if (_elapseTime < interval)
        {
            return;
        }
        _elapseTime -= interval;

        if (loops != 0 && _curLoop == loops)
        {
            _finishCB?.Invoke();
            Stop();
            return;
        }

        _image.sprite = sprites[_curIndex];
        _curIndex++;
        if (_curIndex == sprites.Length)
        {
            _curIndex = 0;
            _curLoop++;
        }
    }

    protected void OnEnable()
    {
        _isActive = true;
    }

    protected void OnDisable()
    {
        _isActive = false;
    }
}
