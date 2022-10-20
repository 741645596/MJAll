using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHelper
{
    /// <summary>
    /// 创建系统原生空Button，主要用于点击空白区域关闭弹出框，或者是禁止按钮事件穿透到下个层级
    /// 当然也可以在Prefab挂个空按钮实现相同效果
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="clickCallback"></param>
    public static void CreateSwallowButton(Transform parent, Action clickCallback)
    {
        GameObject gObject = new GameObject("SwallowButton", typeof(RectTransform));
        gObject.layer = LayerMask.NameToLayer("UI");

        var rectTransform = gObject.transform as RectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.SetParent(parent, false);
        rectTransform.SetAsFirstSibling();

        var image = gObject.AddComponent<Image>();
        image.color = Color.clear;

        var bt = gObject.AddComponent<Button>();
        bt.targetGraphic = image;
        bt.onClick.AddListener(() =>
        {
            clickCallback?.Invoke();
        });
    }

    /// <summary>
    /// 创建空按钮
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static GameObject CreateEmptyButton(Vector2 size)
    {
        GameObject gObject = new GameObject("EmptyButton", typeof(RectTransform));
        gObject.layer = LayerMask.NameToLayer("UI");

        var half = new Vector2(0.5f, 0.5f);
        var rectTransform = gObject.transform as RectTransform;
        rectTransform.anchorMin = half;
        rectTransform.anchorMax = half;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = half;
        rectTransform.sizeDelta = size;

        var image = gObject.AddComponent<Image>();
        image.color = Color.clear;

        var bt = gObject.AddComponent<Button>();
        bt.targetGraphic = image;
        
        return gObject;
    }
}
