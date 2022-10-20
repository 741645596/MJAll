using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 实现如，防作弊排队中...，指定...跳动动画效果
/// 因为用的比较少就不加到右键菜单栏里
/// 使用方法：第一层子节点为控制目标，然后参考下面参数配置
/// </summary>
public class SequeueText : MonoBehaviour
{
    public int jumpFromIndex = 0;       // 从第几个开始跳，默认索引0为第一个
    public int jumpToIndex = -1;        // -1表示最后一个

    public float jumpHeight = 40;       // 跳动高度
    public float jumpDuration = 0.3f;   // 跳动动画时长
    public float jumpNextInterval = 0.5f;   // 距离下一个跳动的时间

    public float turnInterval = 3f;     // 总的一轮时间
    public bool playImmediately = true; // 是否立即播放

    private RectTransform _thisTrans;

    public void Play()
    {
        var count = transform.childCount;
        var startIndex = Mathf.Min(count-1, jumpFromIndex);

        jumpToIndex = jumpToIndex == -1 ? count - 1 : jumpToIndex;
        var endIndex = Mathf.Min(count-1, jumpToIndex);

        for (int i = startIndex; i <= endIndex; i++)
        {
            var r = transform.GetChild(i) as RectTransform;
            var pos = r.anchoredPosition;

            CCAction.NewSequence()
                .DelayTime((i - startIndex) * jumpNextInterval)
                .MoveBy(jumpDuration, cc.p(0, jumpHeight))
                .MoveBy(jumpDuration + 0.05f, cc.p(0, -jumpHeight - 4))
                .MoveBy(0.1f, cc.p(0, 4))
                .Run(r);
        }

        CCAction.NewSequence()
            .DelayTime(turnInterval)
            .CallFunc(() =>
            {
                Play();
            })
            .Run(_thisTrans);
    }

    protected void Start()
    {
        _thisTrans = transform as RectTransform;
        Debug.Assert(_thisTrans != null, "SequeueText root must RectTransform, Please check!!");

        if (playImmediately)
        {
            Play();
        }
    }
}
