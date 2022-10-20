// DOVisible.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/12
using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DOVisible : DOTweenerNode
{
	public bool isVisible;
	public override Tween GenerateTween(Transform transform)
	{
		var tween = transform.DOLocalRotate(Vector3.zero, time, RotateMode.LocalAxisAdd);
        tween.onPlay += ()=>
        {
			transform.gameObject.SetActive(isVisible);
		};
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

	public override void Rollback(Transform transform)
	{
		transform.gameObject.SetActive(!isVisible);
	}
}
