// DOUIColorNode.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/12
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DOUIFadeNode : DOTweenerNode
{
	public float alpha = 1f;
	private float _origionAlpha = 1f;
	
	public override Tween GenerateTween(Transform transform)
	{
		// 优先判断CanvasGroup，这个可以在预设里浏览，MaskableGraphic看不出效果
		var cg = transform.GetComponent<CanvasGroup>();
		if (cg != null)
		{
			_origionAlpha = cg.alpha;
			var tw = cg.DOFade(alpha, time);
			return tw;
		}

		var tween = DOTween.Sequence();
		var graphics = transform.GetComponentsInChildren<MaskableGraphic>();
		for (int j = 0; j < graphics.Length; j++)
		{
			// 只记录第一个的颜色，后续有特殊情况在改
			var graphic = graphics[j];
            if (j == 0)
            {
				_origionAlpha = graphic.color.a;
            }

            Tween docolor = graphic.DOFade(alpha, time);
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
		// 优先判断CanvasGroup
		var cg = transform.GetComponent<CanvasGroup>();
		if (cg != null)
		{
			cg.alpha = _origionAlpha;
			return;
		}

		var graphics = transform.GetComponentsInChildren<MaskableGraphic>();
		for (int j = 0; j < graphics.Length; j++)
		{
            // 只记录第一个的颜色，后续有特殊情况在改
            var graphic = graphics[j];
			var color = graphic.color;
			color.a = _origionAlpha;
			graphic.color = color;
        }
	}
}
