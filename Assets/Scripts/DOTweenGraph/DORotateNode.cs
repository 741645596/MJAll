// DORotateNode.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/10
using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DORotateNode : DOTweenerNode
{
	public Vector3 target;
	public RotateMode mode;

	public override Tween GenerateTween(Transform transform)
	{
		var tween = transform.DOLocalRotate(target, time, mode);
		tween.SetEase(ease);
		tween.SetDelay(delay);

#if UNITY_EDITOR    // 编辑器模式方便预设直接看效果
		if (EditorApplication.isPlaying == false)
		{
			tween.SetUpdate(UpdateType.Manual);
		}
#endif

		return tween;
	}
}
