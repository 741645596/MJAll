// @Author: tanjinhua
// @Date: 2021/8/23  15:21

using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public static class PointerEventDataExtension
{
    /// <summary>
    /// 适配后，正确的UI点击位置
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Vector2 LocationPosition(this PointerEventData data)
    {
        return data.position / display.srceenScaleFactor;
    }

    /// <summary>
    /// 前一次点击位置
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Vector2 PreLocationPosition(this PointerEventData data)
    {
        return (data.position - data.delta) / display.srceenScaleFactor;
    }
}
