// DOSequence.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/10
using DG.Tweening;
using UnityEngine;
using XNode;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DOSequence : DOTweenerNode
{
    [Input(dynamicPortList = true)]
    public string[] DOTweeners;

    public override Tween GenerateTween(Transform transform)
    {
        var sequence = DOTween.Sequence();

        for (int i = 0; i < DOTweeners.Length; i++)
        {
            var key = $"DOTweeners {i}";

            var p = GetInputPort(key);
            var n = p.Connection.node as DOTweenerNode;
            sequence.Append(n.GenerateTween(transform));
        }
        sequence.SetEase(ease);
        sequence.SetDelay(delay);

#if UNITY_EDITOR    // 编辑器模式方便预设直接看效果
        if (EditorApplication.isPlaying == false)
        {
            sequence.SetUpdate(UpdateType.Manual);
        }
#endif

        return sequence;
    }
}
