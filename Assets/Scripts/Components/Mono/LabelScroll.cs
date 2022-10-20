using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 可滚动的文本控件，一般用于显示公告这类长文本
/// 使用方法，鼠标右键：UI -> Weile -> Label Scroll会帮忙创建所需组件
/// </summary>
public class LabelScroll : MonoBehaviour
{
    private RectTransform _transform;
    private RectTransform _textTrans;
    private Text _text;

    private void Start()
    {
        _transform = transform as RectTransform;

        var textObj = transform.FindReference("Text");
        if (textObj == null)
        {
            WLDebug.LogWarning("错误提示：LabelScroll 找不到Text子节点，请检查预设");
            return;
        }

        _textTrans = textObj.transform as RectTransform;
        _text = _textTrans.GetComponent<Text>();
        if (_text == null)
        {
            WLDebug.LogWarning("错误提示：LabelScroll Text子节点未配置Text组件，请检查预设");
            return;
        }

        // 重新设置内容，自动调整对象大小
        _text.RegisterDirtyVerticesCallback(() =>
        {
            _textTrans.sizeDelta = new Vector2(0, _text.preferredHeight - _transform.sizeDelta.y);
        });

        // TextObject是等比适配，需要减去初始大小
        _textTrans.sizeDelta = new Vector2(0, _text.preferredHeight - _transform.sizeDelta.y);
    }

    /// <summary>
    /// 用于右键创建模板使用
    /// </summary>
    /// <returns></returns>
    public static GameObject CreateTemplate()
    {
        GameObject obj = new GameObject("Label Scroll", typeof(RectTransform));

        // 增加Image组件
        var image = obj.AddComponent<Image>();
        image.raycastTarget = true;

        // 增加ScrollRect
        ScrollRect sr = obj.AddComponent<ScrollRect>();
        sr.horizontal = false;

        // Mask组件
        var mask = obj.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        // 创建Text
        GameObject text = TextHelper.CreateText();
        var rectTransform = text.transform as RectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0, 1);

        rectTransform.SetParent(obj.transform, false);
        sr.content = rectTransform;

        // 创建自动适配文字长度脚本
        obj.AddComponent<LabelScroll>();

        return obj;
    }
}
