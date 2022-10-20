// @Author: tanjinhua
// @Date: 2021/8/23  15:21

using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Widget;

public static class GameObjectExtension
{
    /// <summary>
    /// 设置层，applyToAllChildren是否递归所有子节点，默认递归
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layer"></param>
    /// <param name="applyToAllChildren"></param>
    public static void SetLayer(this GameObject gameObject, int layer, bool applyToAllChildren = true)
    {
        if (!applyToAllChildren)
        {
            gameObject.layer = layer;
            return;
        }

        List<Transform> allChildren = gameObject.transform.GetAllChildren();
        allChildren.ForEach(c => c.gameObject.layer = layer);
    }

    /// <summary>
    /// 添加到父节点
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject AddTo(this GameObject obj, Transform parent)
    {
        obj.transform.SetParent(parent, false);
        return obj;
    }

    public static GameObject AddTo(this GameObject obj, GameObject parent)
    {
        obj.transform.SetParent(parent.transform, false);
        return obj;
    }

    public static GameObject AddTo(this GameObject obj, WNode3D parent)
    {
        obj.transform.SetParent(parent.transform, false);
        return obj;
    }

    public static GameObject Layout(this GameObject obj, Vector2 layout, Vector2 offset)
    {
        var t = obj.transform as RectTransform;
        t.Layout(layout, offset);
        return obj;
    }

    public static GameObject Layout(this GameObject obj, Vector2 layout)
    {
        var t = obj.transform as RectTransform;
        t.Layout(layout);
        return obj;
    }

    /// <summary>
    /// 获取第一层的所有子节点
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<Transform> GetChildren(this GameObject obj)
    {
        return obj.transform.GetChildren();
    }

    /// <summary>
    /// 获取所有子节点
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<Transform> GetAllChildren(this GameObject obj)
    {
        return obj.transform.GetAllChildren();
    }

    public static Transform FindInChildren(this GameObject obj, string childName)
    {
        return obj.transform.FindInChildren(childName);
    }

    /// <summary>
    /// 详细见TransformExtension.FindInChildren
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static T FindInChildren<T>(this GameObject obj, string childName) where T : Component
    {
        return obj.transform.FindInChildren<T>(childName);
    }

    /// <summary>
    /// 详细见TransformExtension.FindInChildren
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static Transform FindInChildren(this GameObject obj, Func<Transform, bool> condition)
    {
        return obj.transform.FindInChildren(condition);
    }

    /// <summary>
    /// 详细见TransformExtension.FindInChildren
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static T FindInChildren<T>(this GameObject obj, Func<Transform, bool> condition) where T : Component
    {
        return obj.transform.FindInChildren<T>(condition);
    }

    /// <summary>
    /// 这个方法会递归查找，优先建议使用FindInChildren
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static Transform FindReference(this GameObject obj, string childName)
    {
        return obj.transform.FindReference(childName);
    }

    /// <summary>
    /// 这个方法会递归查找，优先建议使用FindInChildren
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static Transform FindReference(this GameObject obj, Func<Transform, bool> condition)
    {
        return obj.transform.FindReference(condition);
    }

    /// <summary>
    /// 这个方法会递归查找，优先建议使用FindInChildren
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindReference<T>(this GameObject obj, string name) where T : Component
    {
        return obj.transform.FindReference<T>(name);
    }

    /// <summary>
    /// 这个方法会递归查找，优先建议使用FindInChildren
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static T FindReference<T>(this GameObject obj, Func<Transform, bool> condition) where T : Component
    {
        return obj.transform.FindReference<T>(condition);
    }

    /// <summary>
    /// 如果GameObject是通过WNode3D或继承WNode3D创建的，可以通过该方法反向找到继承的类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static T FindNode<T>(this GameObject obj, string childName) where T : WNode3D
    {
        return obj.transform.FindNode<T>(childName);
    }

    /// <summary>
    /// 组件如果存在则直接返回，否则创建一个新的，确保组件一定存在
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T EnsureComponent<T>(this GameObject obj) where T : Component
    {
        T r = obj.GetComponent<T>();
        if (r != null)
        {
            return r;
        }
        return obj.AddComponent<T>();
    }
}
