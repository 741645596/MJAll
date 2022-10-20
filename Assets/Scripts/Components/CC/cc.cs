
using System;
using UnityEngine;

public static class cc
{
    public static Color RED;
    public static Color GREEN;
    public static Color BLUE;
    public static Color BLACK;
    public static Color WHITE;
    public static Color YELLOW;
    public static Color GRAY;

    public static void Init()
    {
        cc.RED = c3b(255, 0, 0);
        cc.GREEN = c3b(0, 255, 0);
        cc.BLUE = c3b(0, 0, 255);
        cc.BLACK = c3b(0, 0, 0);
        cc.WHITE = c3b(255, 255, 255);
        cc.YELLOW = c3b(255, 255, 0);
        cc.GRAY = c3b(155, 155, 155);
        
        display.Init();
        layout.Init();
    }

    public static Vector2 p(float x, float y)
    {
        return new Vector2(x, y);
    }

    public static Color c4b(int r, int g, int b, int a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static Color c4b(int r, int g, int b, float a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a);
    }

    public static Color c3b(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public static Color color(string color)
    {
        Color nowColor;
        ColorUtility.TryParseHtmlString(color, out nowColor);
        return nowColor;
    }

    public static Vector2 pAdd(Vector2 a, Vector2 b)
    {
        return a + b;
    }

    public static Vector2 pAdd(Vector2 a, float x, float y)
    {
        return new Vector2(a.x + x, a.y + y);
    }

    public static Vector2 pSub(Vector2 a, Vector2 b)
    {
        return a - b;
    }

    public static Vector2 pSub(Vector2 a, float x, float y)
    {
        return new Vector2(a.x - x, a.y - y);
    }

    public static double pGetLength(Vector2 pos)
    {
        return Math.Sqrt(pos.x * pos.x + pos.y * pos.y);
    }

    public static double pGetDistance(Vector2 startP, Vector2 endP)
    {
        return cc.pGetLength(cc.pSub(startP, endP));
    }

    public static Rect rect(float x, float y, float width, float height)
    {
        return new Rect(x, y, width, height);
    }

    public static bool rectContainsPoint(Rect rect, Vector2 point)
    {
        return (point.x >= rect.x && point.x <= (rect.x + rect.width)) &&
            (point.y >= rect.y && point.y <= (rect.y + rect.height));
    }

    public static bool rectIntersectsRect(Rect rect1, Rect rect2)
    {
        return !(rect1.x > rect2.x + rect2.width ||
        rect1.x + rect1.width < rect2.x ||
        rect1.y > rect2.y + rect2.height ||
        rect1.y + rect1.height < rect2.y);
    }
}

