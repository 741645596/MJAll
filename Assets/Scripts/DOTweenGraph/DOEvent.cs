// DOEvent.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/20
using System;
using DG.Tweening;
using UnityEngine;

public class DOEvent : DOBaseNode
{
    public string eventName;

	public override Tween GenerateTween(Transform transform)
	{
		var tween = transform.DOLocalRotate(Vector3.zero, 0.0001f, RotateMode.LocalAxisAdd);
        tween.onComplete += ()=>
        {
            var action = GetEvent(eventName);
            action?.Invoke();
		};
		return tween;
	}

    public Action GetEvent(string key)
    {
        var g = graph as DOTweenGraph;
        if (g != null)
        {
            return g.GetEvent(key);
        }

        return null;
    }
}
