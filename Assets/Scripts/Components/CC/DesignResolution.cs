
using System;
using UnityEngine;

public static class DesignResolution
{
    private static int _currentResolutionWidth = 0;
    private static int _currentResolutionHeight = 0;
    private static float _screenOffsetL = 0;
    private static float _screenOffsetR = 0;

    /// <summary>
    /// 设计分辨率大小，比率1.777
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetDesignSize()
    {
        return new Vector2(1920, 1080);
    }

    /// <summary>
    /// 是否2 ：1的屏幕
    /// </summary>
    /// <returns></returns>
    public static bool Is2to1Screen()
    {
        var factor = GetFactor();
        return factor > 1.85f;
    }

    /// <summary>
    /// 是否超宽屏 
    /// </summary>
    /// <returns></returns>
    public static bool IsSuper2to1Screen()
    {
        return GetFactor() > 2.07;
    }


    public static Vector2 GetScreenPosition(Vector2 layout, Vector2 offset)
    {
        var layoutPos = cc.p(display.size.x * layout.x, display.size.y * layout.y);
        var pos = cc.pAdd(layoutPos, offset);

        if (layout.x == 0)
        {
            pos.x += GetScreenOffsetL();
        }
        else if (layout.x == 1)
        {
            pos.x -= GetScreenOffsetR();
        }
        return pos;
    }

    /// <summary>
    /// 自定义宽屏手机左右两边偏移位置，比如iPhone X当刘海在左边则（56, 0），刘海在右边则（0, 56）
    /// 该接口为新增接口，需要大版本更新后调用或用反射调用
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    public static void SetScreenOffset(float l, float r)
    {
        _screenOffsetL = l;
        _screenOffsetR = r;
    }

    // 获取上面接口数据
    public static float GetScreenOffsetL()
    {
        return _screenOffsetL;
    }

    public static float GetScreenOffsetR()
    {
        return _screenOffsetR;
    }

    /// <summary>
    /// 设置屏幕比，性能差的手机降低比例对于性能帮助很大，一般情况：
    /// 高质量：1.0；中：0.85；低质量：0.7
    /// </summary>
    /// <param name="ratio"></param>
    public static void SetScreenResolution(float ratio)
    {
        if (ratio >= 1)
        {
            return;
        }

        if (_currentResolutionWidth == 0 && _currentResolutionHeight == 0)
        {
            _currentResolutionWidth = Screen.currentResolution.width;
            _currentResolutionHeight = Screen.currentResolution.height; 
        }
        int scaleWidth = Mathf.FloorToInt(_currentResolutionWidth * ratio);
        int scaleHeight = Mathf.FloorToInt(_currentResolutionHeight * ratio);
        Screen.SetResolution(scaleWidth, scaleHeight, true);
        WLDebug.Log($"设置完后，当前屏幕分辨率{scaleWidth}-{scaleWidth}");
    }

    // 屏幕分辨率比
    private static float GetFactor()
    {
        float width = Screen.width;
        float height = Screen.height;
        return width > height ? width / height : height / width;
    }
}
