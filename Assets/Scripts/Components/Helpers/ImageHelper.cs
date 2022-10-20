using Unity.Core;
using UnityEngine;
using UnityEngine.UI;

public static class ImageHelper
{
    /// <summary>
    /// 创建带有Image组件的空Object
    /// </summary>
    /// <returns></returns>
    public static GameObject CreateEmptyImage()
    {
        GameObject gObject = new GameObject("Image", typeof(RectTransform));
        gObject.layer = LayerMask.NameToLayer("UI");

        var half = new Vector2(0.5f, 0.5f);
        var rectTransform = gObject.transform as RectTransform;
        rectTransform.anchorMin = half;
        rectTransform.anchorMax = half;
        rectTransform.pivot = half;

        var image = gObject.AddComponent<Image>();
        image.raycastTarget = false;

        return gObject;
    }

    /// <summary>
    /// 创建带有图片的Image
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="assetKey"></param>
    /// <returns></returns>
    public static GameObject CreateImage(string assetName, string key)
    {
        GameObject gObject = new GameObject("Image", typeof(RectTransform));
        gObject.layer = LayerMask.NameToLayer("UI");

        var half = new Vector2(0.5f, 0.5f);
        var rectTransform = gObject.transform as RectTransform;
        rectTransform.anchorMin = half;
        rectTransform.anchorMax = half;
        rectTransform.pivot = half;

        var image = gObject.AddComponent<Image>();
        image.raycastTarget = false;
        image.sprite = AssetsManager.Load<Sprite>(assetName, key);
        image.SetNativeSize();

        return gObject;
    }

    public enum FixedSceenType
    {
        Auto,       // 等比缩放且保证不会看到空白，默认使用该模式
        Height,     // 按等高缩放
        Width,      // 按等宽缩放
        Both,       // 按等高/等宽填满屏幕，会有压缩
    }

    /// <summary>
    /// 将背景图拉伸跟屏幕一样大
    /// </summary>
    /// <param name="imgTrans"> 图片的Object对象 </param>
    /// <param name="t"> 拉伸类型，参考FixedSceenType </param>
    public static void FixedScreen(RectTransform imgTrans, FixedSceenType t = FixedSceenType.Auto)
    {
        var size = imgTrans.sizeDelta;
        float scaleX = display.width / size.x;
        float scaleY = display.height / size.y;

        if (FixedSceenType.Auto == t)
        {
            var scale = Mathf.Max(scaleX, scaleY);
            imgTrans.localScale = new Vector3(scale, scale, 1);
        }
        else if (FixedSceenType.Height == t)
        {
            imgTrans.localScale = new Vector3(scaleY, scaleY, 1);
        }
        else if (FixedSceenType.Width == t)
        {
            imgTrans.localScale = new Vector3(scaleX, scaleX, 1);
        }
        else
        {
            imgTrans.localScale = new Vector3(scaleX, scaleY, 1);
        }
    }
}
