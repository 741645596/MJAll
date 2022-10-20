using UnityEngine;
using UnityEngine.UI;

public static class CanvasHelper
{
    /// <summary>
    /// 动态创建空的Canvas
    /// </summary>
    /// <returns></returns>
    public static GameObject CreateEmptyCanvas()
    {
        GameObject gObject = new GameObject("Canvas", typeof(RectTransform));
        gObject.layer = LayerMask.NameToLayer("UI");

        var rectTransform = gObject.transform as RectTransform;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        gObject.AddComponent<Canvas>();

        return gObject;
    }

    /// <summary>
    /// 相当于CCLayerColor
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static GameObject CreateColorCanvas(Color color)
    {
        GameObject gObject = new GameObject("ColorCanvas", typeof(RectTransform));
        gObject.layer = LayerMask.NameToLayer("UI");

        var rectTransform = gObject.transform as RectTransform;
        rectTransform.pivot = layout.center;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchorMin = Vector2.zero;

        gObject.AddComponent<Canvas>();

        var image = gObject.AddComponent<Image>();
        image.raycastTarget = false;
        image.color = color;

        return gObject;
    }
}
