using System;
using UnityEngine;

public static class layout
{
    public static Vector2 left_top;
    public static Vector2 left_center;
    public static Vector2 left_bottom;
    public static Vector2 center;
    public static Vector2 center_bottom;
    public static Vector2 center_top;
    public static Vector2 right_top;
    public static Vector2 right_center;
    public static Vector2 right_bottom;

    public static void Init()
    {
        layout.left_top = cc.p(0, 1);
        layout.left_center = cc.p(0, 0.5f);
        layout.left_bottom = cc.p(0, 0);
        layout.center = cc.p(0.5f, 0.5f);
        layout.center_bottom = cc.p(0.5f, 0);
        layout.center_top = cc.p(0.5f, 1);
        layout.right_top = cc.p(1, 1);
        layout.right_center = cc.p(1, 0.5f);
        layout.right_bottom = cc.p(1, 0);
    }
}