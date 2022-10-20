
using System;
using UnityEngine;

public static class display
{
    public static Vector2 size;
    public static float width = Screen.width;
    public static float height = Screen.height;
    public static float cx = width / 2;
    public static float cy = height / 2;
    public static float c_left = -width / 2;
    public static float c_right = width / 2;
    public static float c_top = height / 2;
    public static float c_bottom = -height / 2;
    public static float left = 0;
    public static float right = width;
    public static float top = height;
    public static float bottom = 0;
    public static float srceenScaleFactor = 1f;
    public static float aspect = width / height;
    public static Vector2 center = cc.p(width / 2, height / 2);
    public static Vector2 left_top = cc.p(0, height);
    public static Vector2 left_bottom = cc.p(0, 0);
    public static Vector2 left_center = cc.p(0, height / 2);
    public static Vector2 right_top = cc.p(width, height);
    public static Vector2 right_bottom = cc.p(width, 0);
    public static Vector2 right_center = cc.p(width, height / 2);
    public static Vector2 top_center = cc.p(width / 2, height);
    public static Vector2 top_bottom = cc.p(width / 2, 0);


    public static void Init()
    {
        var designSize = DesignResolution.GetDesignSize();
        var scrWidth = Math.Max(Screen.width, Screen.height);
        var scrHeight = Math.Min(Screen.width, Screen.height);
        float sx = scrWidth / designSize.x;
        float sy = scrHeight / designSize.y;
        float scale = DesignResolution.Is2to1Screen() ? sy : sx;    // 太宽则等高适配否则等宽适配
        width = scrWidth / scale;
        height = scrHeight / scale;
        srceenScaleFactor = scale;
        aspect = scrWidth / (float)scrHeight;
        size = new Vector2(width, height);
        cx = width / 2;
        cy = height / 2;
        c_left = -width / 2;
        c_right = width / 2;
        c_top = height / 2;
        c_bottom = -height / 2;
        left = 0;
        right = width;
        top = height;
        bottom = 0;
        center = cc.p(width / 2, height / 2);
        left_top = cc.p(0, height);
        left_bottom = cc.p(0, 0);
        left_center = cc.p(0, height / 2);
        right_top = cc.p(width, height);
        right_bottom = cc.p(width, 0);
        right_center = cc.p(width, height / 2);
        top_center = cc.p(width / 2, height);
        top_bottom = cc.p(width / 2, 0);
    }
}
