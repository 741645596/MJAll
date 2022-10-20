// DOTweenAnimation.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/11
using System.Collections;
using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class DOTweenAnimation : MonoBehaviour
{
    public DOTweenGraph graph;
    public bool playImmediate;      // 是否创建后立即播放

    private bool _isRecord;
    private Vector3 _localPosition;
    private Vector3 _localScale;
    private Quaternion _localRotation;
    private Sequence _sequence;

    void Start()
    {
        if (playImmediate)
        {
            Play();
        }
    }

    public void ResetTransform()
    {
        if (_sequence != null)
        {
            _sequence.Kill();
            _sequence = null;
        }

        _Rollback();

        transform.localPosition = _localPosition;
        transform.localScale = _localScale;
        transform.localRotation = _localRotation;

#if UNITY_EDITOR
        DOTween.Clear();
        EditorApplication.update -= __DoTweenManualUpdate;
#endif
    }

    public void Play()
    {
        if(graph == null)
        {
            WLDebug.LogWarning($"graph is null, name = ", transform.name);
            return;
        }

        var root = graph.GetRootNode();
        if (root == null)
        {
            WLDebug.LogWarning($"graph root is null, name = ", transform.name);
            return;
        }

        // 记录初始数据，重置状态用
        _RecordAndReset();

        // 解析动画
        _ParseSeqeue(root);
    }

#if UNITY_EDITOR 
    static void __DoTweenManualUpdate()
    {
        DOTween.ManualUpdate(Time.deltaTime, 1f);
    }
#endif

    private void _ParseSeqeue(DORootNode root)
    {
        _sequence = transform.CreateTweenGraphSequence(root);
        _sequence.Play();

#if UNITY_EDITOR    // 编辑器模式方便预设直接看效果
        if (EditorApplication.isPlaying == false)
        {
            _sequence.SetUpdate(UpdateType.Manual);
            EditorApplication.update -= __DoTweenManualUpdate;
            EditorApplication.update += __DoTweenManualUpdate;
        }
#endif
    }

    private void _Rollback()
    {
        if (graph == null) { return; }
        var root = graph.GetRootNode();
        if (root == null) { return; }

        var tweenNode = root.GetOutputPort("next").Connection.node as DOBaseNode;
        while (tweenNode != null)
        {
            tweenNode.Rollback(transform);
            tweenNode = tweenNode.GetNextNode();
        }
    }

    private void _RecordAndReset()
    {
        if (_isRecord == false)
        {
            _isRecord = true;
            _localPosition = transform.localPosition;
            _localRotation = transform.localRotation;
            _localScale = transform.localScale;
        }
        else
        {
            ResetTransform();
        }
    }

#if UNITY_EDITOR
    protected void OnDestroy()
    {
        DOTween.Clear();
        EditorApplication.update -= __DoTweenManualUpdate;
    }
#endif
}
