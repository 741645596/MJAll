﻿// DOJumpNode.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/10
using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DOJumpNode : DOTweenerNode
{
	public Vector2 target;
	public float jumpPower;
	public int numJumps;
	public bool relative;

	public override Tween GenerateTween(Transform transform)
	{
		Tween tween;
		if(transform is RectTransform)
        {
			tween = ((RectTransform)transform).DOJumpAnchorPos(target, jumpPower, numJumps, time);
        }
		else
        {
			tween = transform.DOLocalJump(target, jumpPower, numJumps, time);
        }
		tween.SetEase(ease);
		tween.SetDelay(delay);
		tween.SetRelative(relative);

#if UNITY_EDITOR	// 编辑器模式方便预设直接看效果
		if (EditorApplication.isPlaying == false)
        {
			tween.SetUpdate(UpdateType.Manual);
		}
#endif	
		return tween;
	}
}