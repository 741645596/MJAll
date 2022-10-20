// DOUIColorNode.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/12
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DOUIColorNode : DOTweenerNode
{
	public Color color = Color.white;
	private Color _originalColor = Color.white;

	public override Tween GenerateTween(Transform transform)
	{
		var tween = DOTween.Sequence();
		var graphics = transform.GetComponentsInChildren<MaskableGraphic>();
		for (int j = 0; j < graphics.Length; j++)
		{
			// 只记录第一个的颜色，后续有特殊情况在改
			var graphic = graphics[j];
			if (j == 0)
            {
				_originalColor = graphic.color;
			}

			Tween docolor = graphic.DOColor(color, time);
			tween.Join(docolor);
		}
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
		var graphics = transform.GetComponentsInChildren<MaskableGraphic>();
		for (int j = 0; j < graphics.Length; j++)
		{
			// 只记录第一个的颜色，后续有特殊情况在改
			var graphic = graphics[j];
			graphic.color = _originalColor;
		}
	}
}
