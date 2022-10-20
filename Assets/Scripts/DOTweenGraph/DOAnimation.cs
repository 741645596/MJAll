// DOAnimation.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/12
using System;
using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DOAnimation : DOTweenerNode
{
    public AnimationClip clip;

	public override Tween GenerateTween(Transform transform)
	{
		time = clip.length;
		var tween = transform.DOLocalRotate(Vector3.zero, time, RotateMode.LocalAxisAdd);
        tween.onPlay += ()=>
        {
			var animation = transform.GetComponentInChildren<Animation>();
			Debug.Log("animation" + animation);
			var ani_clip = animation.GetClip(clip.name);
			if (ani_clip == null)
			{
				animation.AddClip(clip, clip.name);
				ani_clip = animation.GetClip(clip.name);
			}
			animation.Play(ani_clip.name);
		};
		tween.SetEase(ease);
		tween.SetDelay(delay);

#if UNITY_EDITOR  // 编辑器模式方便预设直接看效果
		if (EditorApplication.isPlaying == false)
		{
			tween.SetUpdate(UpdateType.Manual);
		}
#endif

		return tween;
	}
}
