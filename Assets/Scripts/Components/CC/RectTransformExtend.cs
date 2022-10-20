using System;
using DG.Tweening;
using Unity.Widget;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 扩展RectTransform，使用方式同cocos
/// </summary>
public static class RectTransformExtend
{
    /// <summary>
    /// 以父节点左下角为原点(0,0)进行布局(锚点无关)
    /// </summary>
    /// <param name="t"></param>
    /// <param name="layout"></param>
    /// <param name="offset"></param>
    public static void Layout(this RectTransform t, Vector2 layout, Vector2 offset)
    {
        if (!(t.parent is RectTransform p))
        {
            WLDebug.LogWarning("RectTransformExtend.Layout: 父节点为空或不是RectTransform");
            return;
        }

        t.SetPositionInZero(p.CalculatePositionByLayout(layout, offset, false));
    }

    /// <summary>
    /// 以父节点左下角为原点(0,0)进行布局(锚点无关)
    /// </summary>
    /// <param name="t"></param>
    /// <param name="layout"></param>
    public static void Layout(this RectTransform t, Vector2 layout)
    {
        Layout(t, layout, Vector2.zero);
    }

    /// <summary>
    /// 以父节点左下角为原点(0,0)进行布局，考虑刘海屏(锚点无关)
    /// </summary>
    /// <param name="t"></param>
    /// <param name="layout"></param>
    /// <param name="offset"></param>
    public static void LayoutScreen(this RectTransform t, Vector2 layout, Vector2 offset)
    {
        if (!(t.parent is RectTransform p))
        {
            WLDebug.LogWarning("RectTransformExtend.LayoutScreen: 父节点为空或不是RectTransform");
            return;
        }

        var pos = DesignResolution.GetScreenPosition(layout, offset);
        t.SetPositionInZero(pos);
    }

    /// <summary>
    /// 以父节点左下角为原点(0,0)进行布局，考虑刘海屏(锚点无关)
    /// </summary>
    /// <param name="t"></param>
    /// <param name="layout"></param>
    public static void LayoutScreen(this RectTransform t, Vector2 layout)
    {
        LayoutScreen(t, layout, Vector2.zero);
    }

    public static void SetParent(this RectTransform t, WNode3D node)
    {
        t.SetParent(node.transform, false);
    }

    /// <summary>
    /// 设置以父节点左下角为原点(0,0)的位置(锚点无关)
    /// </summary>
    /// <param name="t"></param>
    /// <param name="position"></param>
    public static void SetPositionInZero(this RectTransform t, Vector2 position)
    {
        t.anchoredPosition = position - t.GetAnchorPositionInParentSpace();
    }

    /// <summary>
    /// 按原来的锚点设置位置
    /// </summary>
    /// <param name="t"></param>
    /// <param name="position"></param>
    public static void SetPosition(this RectTransform t, Vector2 position)
    {
        t.anchoredPosition = position;
    }

    /// <summary>
    /// 获取以父节点左下角为原点(0,0)的位置(锚点无关)
    /// </summary>
    /// <param name="t"></param>
    public static Vector2 GetPositionInZero(this RectTransform t)
    {
        return t.anchoredPosition + t.GetAnchorPositionInParentSpace();
    }

    public static Vector2 GetPosition(this RectTransform t)
    {
        return t.anchoredPosition;
    }

    /// <summary>
    /// 获取屏幕坐标
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector2 GetDisplayPosition(this RectTransform t)
    {
        if (!(t.parent is RectTransform p))
        {
            return t.position;
        }

        Rect prect = p.GetBoundingBox();

        return prect.position + t.GetPositionInZero();
    }

    /// <summary>
    /// 屏幕坐标转为局部坐标
    /// </summary>
    /// <param name="t"></param>
    /// <param name="displayPosition"></param>
    /// <returns></returns>
    public static Vector2 DisplayToLocal(this RectTransform t, Vector2 displayPos)
    {
        return displayPos - t.GetBoundingBox().position;
    }

    /// <summary>
    /// 局部坐标转屏幕坐标
    /// </summary>
    /// <param name="t"></param>
    /// <param name="localPos"></param>
    /// <returns></returns>
    public static Vector2 LocalToDisplay(this RectTransform t, Vector2 localPos)
    {
        return t.GetBoundingBox().position + localPos;
    }

    /// <summary>
    /// 设置轴心
    /// </summary>
    /// <param name="t"></param>
    /// <param name="anchor"></param>
    public static void SetAnchor(this RectTransform t, Vector2 anchor)
    {
        t.pivot = anchor;
    }

    /// <summary>
    /// 获取轴心
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector2 GetAnchor(this RectTransform t)
    {
        return t.pivot;
    }

    /// <summary>
    /// 设置矩形尺寸(锚点无关)
    /// </summary>
    /// <param name="t"></param>
    /// <param name="size"></param>
    public static void SetContentSizeInZero(this RectTransform t, Vector2 size)
    {
        if (t.parent is RectTransform p)
        {
            Vector2 anchoredSize = (t.anchorMax - t.anchorMin) * p.GetContentSizeInZero();
            t.sizeDelta = size - anchoredSize;
        }
        else
        {
            t.sizeDelta = size;
        }
    }

    public static void SetContentSize(this RectTransform t, Vector2 size)
    {
        t.sizeDelta = size;
    }

    public static void SetContentSize(this RectTransform t, float width, float height)
    {
        t.sizeDelta = new Vector2(width, height);
    }

    /// <summary>
    /// 获取矩形尺寸(锚点无关)
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector2 GetContentSizeInZero(this RectTransform t)
    {
        return t.rect.size;
    }

    public static Vector2 GetContentSize(this RectTransform t)
    {
        return t.sizeDelta;
    }

    /// <summary>
    /// 设置透明度
    /// </summary>
    /// <param name="t"></param>
    /// <param name="opacity">透明度，值在0-255之间</param>
    /// <param name="effectChilds">是否对所有子节点都生效</param>
    public static void SetOpacity(this Transform t, int opacity, bool effectChilds = false)
    {
        var obj = t.gameObject;
        // 优先判断是否有CanvasGroup，该组件会管理所有子节点的透明度
        var canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = opacity / 255f;
            return;
        }

        var img = obj.GetComponent<Image>();
        if (img != null)
        {
            Color color = img.color;
            Color newColor = new Color(color.r, color.g, color.b, opacity / 255f);
            img.color = newColor;
        }

        var txt = obj.GetComponent<Text>();
        if (txt != null)
        {
            Color color = txt.color;
            Color newColor = new Color(color.r, color.g, color.b, opacity / 255f);
            txt.color = newColor;
        }

        // 递归所有子节点
        if (effectChilds)
        {
            var children = t.GetChildren();
            foreach (var c in children)
            {
                c.SetOpacity(opacity, effectChilds);
            }
        }
    }

    /// <summary>
    /// 设置颜色
    /// </summary>
    /// <param name="t"></param>
    /// <param name="c"></param>
    public static void SetColor(this Transform t, Color c)
    {
        var img = t.gameObject.GetComponent<Image>();
        if (img != null)
        {
            Color color = img.color;
            Color newColor = new Color(c.r, c.g, c.b, color.a);
            img.color = newColor;
        }

        var txt = t.gameObject.GetComponent<Text>();
        if (txt != null)
        {
            Color color = txt.color;
            Color newColor = new Color(c.r, c.g, c.b, color.a);
            txt.color = newColor;
        }
    }

    /// <summary>
    /// 缩放大小
    /// </summary>
    /// <param name="t"></param>
    /// <param name="scale"></param>
    public static void SetScale(this RectTransform t, float scale)
    {
        t.localScale = new Vector3(scale, scale, 1);
    }

    /// <summary>
    /// x、y轴缩放
    /// </summary>
    /// <param name="t"></param>
    /// <param name="sx"></param>
    /// <param name="sy"></param>
    public static void SetScale(this RectTransform t, float scalex, float scaley)
    {
        t.localScale = new Vector3(scalex, scaley, 1);
    }

    /// <summary>
    /// 设置旋转角度，rotation = 0 ~ 360
    /// </summary>
    /// <param name="t"></param>
    /// <param name="rotation"></param>
    public static void SetRotation(this RectTransform t, float rotation)
    {
        var r = t.eulerAngles;
        t.eulerAngles = new Vector3(r.x, r.y, rotation * -1f);
    }

    /// <summary>
    /// 吞噬所有点击事件，防止事件往下传递
    /// </summary>
    public static void SwallowTouchEvents(this RectTransform t)
    {
        ButtonHelper.CreateSwallowButton(t, null);
    }

    /// <summary>
    /// 截取事件回调
    /// </summary>
    /// <param name="t"></param>
    /// <param name="clickCallback"></param>
    public static void EnableTouchEvent(this RectTransform t, Action clickCallback)
    {
        ButtonHelper.CreateSwallowButton(t, clickCallback);
    }

    /// <summary>
    /// 控件在屏幕的矩阵大小（非上级目录的矩阵大小）
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Rect GetBoundingBox(this RectTransform t)
    {
        // 需要考虑父节点缩放问题
        Vector3[] corners = {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero
        };
        t.GetWorldCorners(corners);

        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float maxY = -Mathf.Infinity;
        var c = CameraUtil.GetUICamera();
        for (int i = 0; i < corners.Length; i++)
        {
            var p = c.WorldToScreenPoint(corners[i]);
            minX = p.x <= minX ? p.x : minX;
            maxX = p.x > maxX ? p.x : maxX;
            minY = p.y <= minY ? p.y : minY;
            maxY = p.y > maxY ? p.y : maxY;
        }
        minX /= display.srceenScaleFactor;
        minY /= display.srceenScaleFactor;
        maxX /= display.srceenScaleFactor;
        maxY /= display.srceenScaleFactor;
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    /// <summary>
    /// 仿cocos动画
    /// </summary>
    /// <param name="t"></param>
    /// <param name="action"></param>
    public static void RunAction(this RectTransform t, CCActionData action)
    {
        var sequence = CCActionHelper.GetSequence(t, action, action.type);
        sequence.Play();
    }

    /// <summary>
    /// 获取父节点空间下实际的锚点位置
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector2 GetAnchorPositionInParentSpace(this RectTransform t)
    {
        if (!(t.parent is RectTransform p))
        {
            WLDebug.LogWarning("RectTransformExtend.GetAnchorPositionInParentSpace: 父节点为空或不是RectTransform");
            return Vector2.zero;
        }

        Vector2 anchor = (t.anchorMax - t.anchorMin) * t.pivot + t.anchorMin;

        return p.rect.size * anchor;
    }

    /// <summary>
    /// 根据布局信息计算以左下角为原点(0,0)的位置(锚点无关)
    /// 备注：如果是
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="offset"></param>
    /// <param name="useSafeArea"></param>
    /// <returns></returns>
    public static Vector2 CalculatePositionByLayout(this RectTransform t, Vector2 layout, Vector2 offset, bool useSafeArea = false)
    {
        var size = useSafeArea ? display.size : t.GetContentSizeInZero();
        float expectedX = size.x * layout.x + offset.x;
        if (useSafeArea)
        {
            expectedX += layout.x == 0 ? DesignResolution.GetScreenOffsetL() : layout.x == 1 ? -DesignResolution.GetScreenOffsetR() : 0;
        }
        float expectedY = size.y * layout.y + offset.y;

        return new Vector2(expectedX, expectedY);
    }
}


