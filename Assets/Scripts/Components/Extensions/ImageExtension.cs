// @Author: tanjinhua
// @Date: 2021/1/4  10:34


using Unity.Core;
using UnityEngine;
using UnityEngine.UI;

public enum ImageFillMethod
{
    /// <summary>
    /// 水平方向从左到右填充
    /// </summary>
    HorizontalLeft,
    /// <summary>
    /// 水平方向从右到左填充
    /// </summary>
    HorizontalRight,

    /// <summary>
    /// 垂直方向从上到下填充
    /// </summary>
    VerticalTop,
    /// <summary>
    /// 垂直方向从下到上填充
    /// </summary>
    VerticalBottom,

    /// <summary>
    /// 90度从左下角开始填充
    /// </summary>
    Radial90BottomLeft,
    /// <summary>
    /// 90度从左上角开始填充
    /// </summary>
    Radial90TopLeft,
    /// <summary>
    /// 90度从右上角开始填充
    /// </summary>
    Radial90TopRight,
    /// <summary>
    /// 90度从右下角开始填充
    /// </summary>
    Radial90BottomRight,

    /// <summary>
    /// 180度从底部开始填充
    /// </summary>
    Radial180Bottom,
    /// <summary>
    /// 180度从左边开始填充
    /// </summary>
    Radial180Left,
    /// <summary>
    /// 180度从顶部开始填充
    /// </summary>
    Radial180Top,
    /// <summary>
    /// 180度从右边开始填充
    /// </summary>
    Radial180Right,

    /// <summary>
    /// 360度从底部开始填充
    /// </summary>
    Radial360Bottom,
    /// <summary>
    /// 360度从右边开始填充
    /// </summary>
    Radial360Right,
    /// <summary>
    /// 360度从顶部开始填充
    /// </summary>
    Radial360Top,
    /// <summary>
    /// 360度从左边开始填充
    /// </summary>
    Radial360Left,
}

public static class ImageExtension
{
    /// <summary>
    /// 重置图片
    /// </summary>
    /// <param name="image"></param>
    /// <param name="assetName"></param>
    /// <param name="key"></param>
    public static void SetTexture(this Image image, string assetName, string key)
    {
        var s = AssetsManager.Load<Sprite>(assetName, key);
        image.sprite = s;
    }

    /// <summary>
    /// 设置图片置灰
    /// </summary>
    /// <param name="image"></param>
    /// <param name="isGery"></param>
    public static void SetGery(this Image image, bool isGery)
    {
        if(isGery)
        {
            image.material = new Material(Shader.Find("UI/Grey"));
        }
        else
        {
            image.material = new Material(Shader.Find("UI/Default"));
        }
    }

    /// <summary>
    /// 设置填充方式
    /// </summary>
    /// <param name="image"></param>
    /// <param name="method"></param>
    public static void SetFillMethod(this Image image, ImageFillMethod method)
    {
        image.type = Image.Type.Filled;

        switch (method)
        {
            case ImageFillMethod.HorizontalLeft:
                image.fillMethod = Image.FillMethod.Horizontal;
                image.fillOrigin = (int)Image.OriginHorizontal.Left;
                break;
            case ImageFillMethod.HorizontalRight:
                image.fillMethod = Image.FillMethod.Horizontal;
                image.fillOrigin = (int)Image.OriginHorizontal.Right;
                break;
            case ImageFillMethod.VerticalTop:
                image.fillMethod = Image.FillMethod.Vertical;
                image.fillOrigin = (int)Image.OriginVertical.Top;
                break;
            case ImageFillMethod.VerticalBottom:
                image.fillMethod = Image.FillMethod.Vertical;
                image.fillOrigin = (int)Image.OriginVertical.Bottom;
                break;
            case ImageFillMethod.Radial90BottomLeft:
                image.fillMethod = Image.FillMethod.Radial90;
                image.fillOrigin = (int)Image.Origin90.BottomLeft;
                break;
            case ImageFillMethod.Radial90BottomRight:
                image.fillMethod = Image.FillMethod.Radial90;
                image.fillOrigin = (int)Image.Origin90.BottomRight;
                break;
            case ImageFillMethod.Radial90TopLeft:
                image.fillMethod = Image.FillMethod.Radial90;
                image.fillOrigin = (int)Image.Origin90.TopLeft;
                break;
            case ImageFillMethod.Radial90TopRight:
                image.fillMethod = Image.FillMethod.Radial90;
                image.fillOrigin = (int)Image.Origin90.TopRight;
                break;
            case ImageFillMethod.Radial180Bottom:
                image.fillMethod = Image.FillMethod.Radial180;
                image.fillOrigin = (int)Image.Origin180.Bottom;
                break;
            case ImageFillMethod.Radial180Top:
                image.fillMethod = Image.FillMethod.Radial180;
                image.fillOrigin = (int)Image.Origin180.Top;
                break;
            case ImageFillMethod.Radial180Left:
                image.fillMethod = Image.FillMethod.Radial180;
                image.fillOrigin = (int)Image.Origin180.Left;
                break;
            case ImageFillMethod.Radial180Right:
                image.fillMethod = Image.FillMethod.Radial180;
                image.fillOrigin = (int)Image.Origin180.Right;
                break;
            case ImageFillMethod.Radial360Bottom:
                image.fillMethod = Image.FillMethod.Radial360;
                image.fillOrigin = (int)Image.Origin360.Bottom;
                break;
            case ImageFillMethod.Radial360Top:
                image.fillMethod = Image.FillMethod.Radial360;
                image.fillOrigin = (int)Image.Origin360.Top;
                break;
            case ImageFillMethod.Radial360Left:
                image.fillMethod = Image.FillMethod.Radial360;
                image.fillOrigin = (int)Image.Origin360.Left;
                break;
            case ImageFillMethod.Radial360Right:
                image.fillMethod = Image.FillMethod.Radial360;
                image.fillOrigin = (int)Image.Origin360.Right;
                break;
        }
    }
}
