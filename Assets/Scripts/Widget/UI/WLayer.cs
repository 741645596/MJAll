
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.Widget
{
    public class WLayer : WNode
    {
        /// <summary>
        /// ui层级管理，请参考文档 规范文档/程序规范文档/4. UI层级规范.docx
        /// </summary>
        public static WLayer Create()
        {
            var layer = new WLayer();
            layer.Init();
            return layer;
        }

        /// <summary>
        /// 通过预设直接创建
        /// 备注：如果要管理点击需要在预设里添加组件GraphicRaycaster和Image
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public static new WLayer Create(string assetName, string assetKey)
        {
            var layer = new WLayer();
            layer.InitGameObject(assetName, assetKey);
            return layer;
        }

        /// <summary>
        /// 创建带有背景颜色的Canvas，点击事件可以往下传递
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static WLayer Create(Color color)
        {
            var layer = new WLayer();
            layer.Init(color);
            return layer;
        }

        /// <summary>
        /// 创建带有背景颜色，且点击背景有回调
        /// </summary>
        /// <param name="color"></param>
        /// <param name="clickCallback"></param>
        /// <returns></returns>
        public static WLayer Create(Color color, Action<WLayer> clickCallback)
        {
            var layer = new WLayer();
            layer.Init(color, clickCallback);
            return layer;
        }

        /// <summary>
        /// 创建背景颜色是透明，且点击背景有回调，事件不会往下传递
        /// </summary>
        /// <param name="clickCallback"></param>
        /// <returns></returns>
        public static WLayer Create(Action<WLayer> clickCallback)
        {
            return Create(Color.clear, clickCallback);
        }

        /// <summary>
        /// 如果继承WLayer，在构造方法需要先调用Init或者InitGameObject初始化Object对象
        /// </summary>
        public void Init(string name = "WLayer")
        {
            var obj = CanvasHelper.CreateEmptyCanvas();
            obj.name = name;
            obj.AddComponent<GraphicRaycaster>();
            InitGameObject(obj);
        }

        public void Init(Color color, string name = "WColorLayer")
        {
            var obj = CanvasHelper.CreateColorCanvas(color);
            obj.AddComponent<GraphicRaycaster>();
            obj.name = name;
            InitGameObject(obj);
        }

        public void Init(Color color, Action<WLayer> clickCallback, string name = "WColorLayer")
        {
            var obj = CanvasHelper.CreateEmptyCanvas();
            obj.AddComponent<GraphicRaycaster>();

            // 图片
            var image = obj.AddComponent<Image>();
            image.raycastTarget = true;
            image.color = color;

            // 按钮
            var bt = obj.AddComponent<Button>();
            bt.targetGraphic = image;
            bt.onClick.AddListener(() =>
            {
                clickCallback?.Invoke(this);
            });

            obj.name = "WLayer";
            InitGameObject(obj);
        }

        /// <summary>
        /// 我们规范只有Canvas可以设置z坐标
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="zOrder"></param>
        /// <returns></returns>
        public WLayer AddTo(GameObject obj, int zOrder)
        {
            rectTransform.SetParent(obj.transform, false);
            SetPosiitonZ(zOrder);
            return this;
        }

        public WLayer AddTo(Transform t, int zOrder)
        {
            rectTransform.SetParent(t, false);
            SetPosiitonZ(zOrder);
            return this;
        }

        public WLayer AddTo(WNode node, int zOrder)
        {
            rectTransform.SetParent(node.transform, false);
            SetPosiitonZ(zOrder);
            return this;
        }

        /// <summary>
        /// 设置Z坐标位置，需要添加到父节点设置才能生效
        /// </summary>
        /// <param name="z"></param>
        public void SetPosiitonZ(float z)
        {
            Debug.Assert(rectTransform.parent != null, "错误提示：在调用该方法之前必须添加到父节点，否则设置无效");

            var canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                WLDebug.LogWarning($"错误提示：你的预设不带有Canvas组件，请查看{gameObject.name}");
                return;
            }

            canvas.overrideSorting = true;
            var pos = rectTransform.localPosition;
            pos.z = z;
            rectTransform.localPosition = pos;
        }

        /// <summary>
        /// 开启该方法，重写OnTouchBegan、OnTouchMove、OnTouchEnd三个方法；
        /// </summary>
        public void EnableTouchEvent()
        {
            var touch = gameObject.EnsureComponent<WMonoTouch>();
            touch.onTouchBegan = OnTouchBegan;
            touch.onTouchMove = OnTouchMove;
            touch.onTouchEnd = OnTouchEnd;

            var image = gameObject.EnsureComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
        }
        protected virtual bool OnTouchBegan(PointerEventData d) { return true; }
        protected virtual void OnTouchMove(PointerEventData d) {}
        protected virtual void OnTouchEnd(PointerEventData d) {}

    }
}
