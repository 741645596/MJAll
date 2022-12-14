// DOParallel.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/10
using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DOMoveNode : DOTweenerNode
{
	public Vector3 target;
	public bool relative;

	public override Tween GenerateTween(Transform transform)
	{
		Tweener tween;
		if(transform is RectTransform)
        {
			tween = ((RectTransform)transform).DOAnchorPos(target, time);
        }
		else
        {
			tween = transform.DOLocalMove(target, time);
        }
		tween.SetEase(ease);
		tween.SetDelay(delay);
		tween.SetRelative(relative);

#if UNITY_EDITOR    // 编辑器模式方便预设直接看效果
		if (EditorApplication.isPlaying == false)
		{
			tween.SetUpdate(UpdateType.Manual);
		}
#endif

		return tween;
	}
}