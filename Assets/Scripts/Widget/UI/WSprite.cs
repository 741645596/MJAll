
using UnityEngine;
using UnityEngine.UI;
using Unity.Core;
using UnityEngine.Rendering;

namespace Unity.Widget
{
    public class WSprite : WNode
    {
        public enum FillMethod
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

        public Image image;

        public static WSprite Create()
        {
            return new WSprite();
        }

        public new static WSprite Create(string assetName, string key)
        {
            var sprite = new WSprite();
            sprite.SetTexture(assetName, key);
            return sprite;
        }

        public static WSprite CreateScale9Sprite(string assetName, string key)
        {
            var sprite = new WSprite();
            sprite.image.type = Image.Type.Sliced;
            sprite.SetTexture(assetName, key);
            return sprite;
        }

        public WSprite SetTexture(string assetName, string key)
        {
            var s = AssetsManager.Load<Sprite>(assetName, key);
            image.sprite = s;
            image.SetNativeSize();
            return this;
        }

        public WSprite SetOpacity(float opacity)
        {
            Color color = image.color;
            Color newColor = new Color(color.r, color.g, color.b, opacity / 255f);
            image.color = newColor;
            return this;
        }

        /// <summary>
        /// 设置填充方式
        /// </summary>
        /// <param name="fillMethod">参考顶部定义</param>
        /// <returns></returns>
        public WSprite SetFillMethod(FillMethod fillMethod)
        {
            switch (fillMethod)
            {
                case FillMethod.HorizontalLeft:
                    image.fillMethod = Image.FillMethod.Horizontal;
                    image.fillOrigin = (int)Image.OriginHorizontal.Left;
                    break;
                case FillMethod.HorizontalRight:
                    image.fillMethod = Image.FillMethod.Horizontal;
                    image.fillOrigin = (int)Image.OriginHorizontal.Right;
                    break;
                case FillMethod.VerticalTop:
                    image.fillMethod = Image.FillMethod.Vertical;
                    image.fillOrigin = (int)Image.OriginVertical.Top;
                    break;
                case FillMethod.VerticalBottom:
                    image.fillMethod = Image.FillMethod.Vertical;
                    image.fillOrigin = (int)Image.OriginVertical.Bottom;
                    break;
                case FillMethod.Radial90BottomLeft:
                    image.fillMethod = Image.FillMethod.Radial90;
                    image.fillOrigin = (int)Image.Origin90.BottomLeft;
                    break;
                case FillMethod.Radial90BottomRight:
                    image.fillMethod = Image.FillMethod.Radial90;
                    image.fillOrigin = (int)Image.Origin90.BottomRight;
                    break;
                case FillMethod.Radial90TopLeft:
                    image.fillMethod = Image.FillMethod.Radial90;
                    image.fillOrigin = (int)Image.Origin90.TopLeft;
                    break;
                case FillMethod.Radial90TopRight:
                    image.fillMethod = Image.FillMethod.Radial90;
                    image.fillOrigin = (int)Image.Origin90.TopRight;
                    break;
                case FillMethod.Radial180Bottom:
                    image.fillMethod = Image.FillMethod.Radial180;
                    image.fillOrigin = (int)Image.Origin180.Bottom;
                    break;
                case FillMethod.Radial180Top:
                    image.fillMethod = Image.FillMethod.Radial180;
                    image.fillOrigin = (int)Image.Origin180.Top;
                    break;
                case FillMethod.Radial180Left:
                    image.fillMethod = Image.FillMethod.Radial180;
                    image.fillOrigin = (int)Image.Origin180.Left;
                    break;
                case FillMethod.Radial180Right:
                    image.fillMethod = Image.FillMethod.Radial180;
                    image.fillOrigin = (int)Image.Origin180.Right;
                    break;
                case FillMethod.Radial360Bottom:
                    image.fillMethod = Image.FillMethod.Radial360;
                    image.fillOrigin = (int)Image.Origin360.Bottom;
                    break;
                case FillMethod.Radial360Top:
                    image.fillMethod = Image.FillMethod.Radial360;
                    image.fillOrigin = (int)Image.Origin360.Top;
                    break;
                case FillMethod.Radial360Left:
                    image.fillMethod = Image.FillMethod.Radial360;
                    image.fillOrigin = (int)Image.Origin360.Left;
                    break;
                case FillMethod.Radial360Right:
                    image.fillMethod = Image.FillMethod.Radial360;
                    image.fillOrigin = (int)Image.Origin360.Right;
                    break;
            }
            return this;
        }

        protected WSprite() : base()
        {
            var obj = ImageHelper.CreateEmptyImage();
            InitGameObject(obj);

            image = obj.GetComponent<Image>();
        }
    }
}