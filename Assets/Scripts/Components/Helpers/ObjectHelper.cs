using System;
using Unity.Core;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


public static class ObjectHelper
{
    /// <summary>
    /// 创建预设对象
    /// </summary>
    /// <param name="asset"> ab包路径名 </param>
    /// <param name="key"> 资源路径名 </param>
    /// <returns></returns>
    public static GameObject Instantiate(string asset, string key)
    {
        var prefab = AssetsManager.Load<GameObject>(asset, key);
        if (prefab != null)
        {
            GameObject go = Object.Instantiate(prefab);
            go.name = go.name.Replace("(Clone)", "");
            return go;
        }

        WLDebug.LogWarning($"Instantiate return null, asset:{asset}, key:{key}，please check asset path");
        return null;
    }

    /// <summary>
    /// 创建个RectTransform空对象
    /// </summary>
    /// <returns></returns>
    public static GameObject CreateEmptyObject()
    {
        GameObject gObject = new GameObject("EmptyGameObject", typeof(RectTransform));
        gObject.layer = LayerMask.NameToLayer("UI");

        Vector2 half = new Vector2(0.5f, 0.5f);
        var rectTransform = gObject.transform as RectTransform;
        rectTransform.anchorMin = half;
        rectTransform.anchorMax = half;
        rectTransform.pivot = half;
        return gObject;
    }
}

