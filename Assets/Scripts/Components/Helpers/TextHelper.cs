using UnityEngine;
using UnityEngine.UI;
using Unity.Core;

public static class TextHelper
{
    private static Font _regularFont = null;
    private static Font _blodFont = null;

    /// <summary>
    /// 初始化默认的字体。需要游戏开始时初始化一次
    /// </summary>
    /// <returns></returns>
    public static void InitDefaultFont(string assertName, string key)
    {
        _regularFont = AssetsManager.Load<Font>(assertName, key);
    }

    public static Font GetFont()
    {
        return _regularFont;
    }

    /// <summary>
    /// 初始化默认的粗体字体
    /// </summary>
    /// <returns></returns>
    public static void InitDefaultBlodFont(string assertName, string key)
    {
        _blodFont = AssetsManager.Load<Font>(assertName, key);
    }

    public static Font GetBlodFont()
    {
        return _blodFont;
    }

    /// <summary>
    /// 创建带有Text组件的GameObject，使用默认自定义字体
    /// </summary>
    /// <returns></returns>
    public static GameObject CreateText()
    {
        GameObject gObject = new GameObject("Text", typeof(RectTransform));
        gObject.layer = LayerMask.NameToLayer("UI");

        Vector2 half = new Vector2(0.5f, 0.5f);
        var rectTransform = gObject.transform as RectTransform;
        rectTransform.anchorMin = half;
        rectTransform.anchorMax = half;
        rectTransform.pivot = half;

        var txt = gObject.AddComponent<Text>();
        txt.raycastTarget = false;
        txt.supportRichText = false;
        txt.fontSize = 24;
        if (_regularFont == null)
        {
            Debug.LogWarning("提示：默认字体未初始化，请先调用TextHelper.InitDefaultFont初始化默认字体");
        }
        else
        {
            txt.font = _regularFont;
        }
        
        return gObject;
    }

    /// <summary>
    /// 创建带有粗体字体的GameObject
    /// </summary>
    /// <returns></returns>
    public static GameObject CreateBoldText()
    {
        GameObject gObject = new GameObject("BlodText", typeof(RectTransform));
        gObject.layer = LayerMask.NameToLayer("UI");

        Vector2 half = new Vector2(0.5f, 0.5f);
        var rectTransform = gObject.transform as RectTransform;
        rectTransform.anchorMin = half;
        rectTransform.anchorMax = half;
        rectTransform.pivot = half;

        var txt = gObject.AddComponent<Text>();
        txt.raycastTarget = false;
        txt.supportRichText = false;
        txt.fontSize = 24;

        if (_blodFont == null)
        {
            Debug.LogWarning("提示：默认字体未初始化，请先调用TextHelper.InitDefaultBlodFont初始化默认粗体字体");
        }
        else
        {
            txt.font = _blodFont;
        }

        return gObject;
    }
}
