using System.Collections.Generic;
using UnityEngine;
using Unity.Core;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.Widget
{
    /// <summary>
    /// 管理UI的根节点以及查找接口文件
    /// </summary>
    public static class WDirector
    {
        public const int Swallow_Layer_ZOrder = -10000;

        private const string Swallow_Layer_Name = "__SwallowLayer__";

        private static GameObject _rootObject = null;
        private static Stack<WNode> _rootLayers = new Stack<WNode>();

        public static void Init()
        {
            // 初始UI根节点
            _InitUIRootObject();

            ResetRootLayer();
        }

        /// <summary>
        /// 默认UI根节点，我们规范该节点下的子节点都是Canvas组件(右键->UI->Weile->Canvas创建或是WLayer创建)，统一按z排序
        /// </summary>
        /// <returns></returns>
        public static WNode GetRootLayer()
        {
            return _rootLayers.Peek();
        }

        /// <summary>
        /// 通过索引找到指定layer，找不到返回空
        /// </summary>
        /// <param name="index"> 0是第一个，也是根节点 </param>
        /// <returns></returns>
        public static WNode GetRootLayer(int index)
        {
            var arr = _rootLayers.ToArray();

            // Stack控件0索引是最上面那个，我们需要获取底下那个
            var targetIndex = arr.Length - index - 1;
            if (arr.Length > targetIndex)
            {
                return arr[targetIndex];
            }
            return null;
        }

        /// <summary>
        /// 查找RootLayer的子节点Object，不会递归查找
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Transform FindInChildren(string childName)
        {
            return _rootLayers.Peek().transform.Find(childName);
        }

        /// <summary>
        /// 如果GameObject是通过WNode3D或继承WNode3D创建的，可以通过该方法找到创建GameObject的类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static T FindNode<T>(string childName) where T : WNode3D
        {
            return _rootLayers.Peek().transform.FindNode<T>(childName);
        }

        /// <summary>
        /// 获取指定RootLayer的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootLayerIndex"> 参考GetRootLayer() </param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static T FindNode<T>(int rootLayerIndex, string childName) where T : WNode3D
        {
            return GetRootLayer(rootLayerIndex)?.transform.FindNode<T>(childName);
        }

        /// <summary>
        /// 新增根节点，比如在大厅界面点击房间列表选择界面，此时房间列表会覆盖整个大厅界面，且不想删除大厅，那可以先Push一个新的根节点，关闭界面可以PopRootLayer
        /// </summary>
        /// <param name="hidePreRootLayer"> 隐藏上一个root节点，一般情况可以直接隐藏，除非需要配合做一些特殊动画 </param>
        public static void PushRootLayer(bool hidePreRootLayer = true)
        {
            _SetActive(!hidePreRootLayer);

            _CreateNewLayer();
        }

        /// <summary>
        /// 对应上面PushRootLayer
        /// </summary>
        public static void PopRootLayer()
        {
            var layer = _rootLayers.Pop();
            if (layer != null)
            {
                layer.RemoveFromParent();
            }

            _SetActive(true);
        }

        /// <summary>
        /// pop到只保留第一个
        /// </summary>
        public static void PopToRoot()
        {
            while (_rootLayers.Count > 1)
            {
                var n = _rootLayers.Pop();
                n.RemoveFromParent();
            }

            _SetActive(true);
        }

        /// <summary>
        /// 重新创建RootNode，相当于删除UI场景重新创建
        /// </summary>
        public static void ResetRootLayer()
        {
            _RemoveAllLayer();

            _CreateNewLayer();
        }

        /// <summary>
        /// UI的根节点，预留接口，一般用不上
        /// </summary>
        /// <returns></returns>
        public static GameObject GetUIRootObject()
        {
            return _rootObject;
        }

        /// <summary>
        /// 禁止所有点击事件，原理是在当前UI Root最前面创建一个Canvas吞噬所有点击事件
        /// </summary>
        public static void SetSwallowAllTouchEvent(bool isSwallow)
        {
            if (isSwallow)
            {
                _CreateSwallowLayer();
            }
            else
            {
                _RemoveSwallowLayer();
            } 
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        private static void _CreateSwallowLayer()
        {
            var trans = FindInChildren(Swallow_Layer_Name);
            if (trans == null)
            {
                WLayer.Create((l) => { })
                    .AddTo(GetRootLayer(), Swallow_Layer_ZOrder)
                    .SetName(Swallow_Layer_Name);
            }
        }

        private static void _RemoveSwallowLayer()
        {
            var trans = FindInChildren(Swallow_Layer_Name);
            if (trans != null)
            {
                GameObject.Destroy(trans.gameObject);
            }
        }

        private static void _SetActive(bool res)
        {
            var curLayer = _rootLayers.Peek();
            if (curLayer != null)
            {
                curLayer.SetActive(res);
            }
        }

        private static void _RemoveAllLayer()
        {
            while (_rootLayers.Count != 0)
            {
                var n = _rootLayers.Pop();
                n.RemoveFromParent();
            }
        }

        private static void _CreateNewLayer()
        {
            var name = _rootLayers.Count == 0 ? "WRootNode" : $"WRootNode_{_rootLayers.Count}";
            GameObject gObject = new GameObject(name, typeof(RectTransform));
            gObject.layer = LayerMask.NameToLayer("UI");
            var rectTransform = gObject.transform as RectTransform;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            var rootLayer = new WNode();
            rootLayer.InitGameObject(gObject);
            rootLayer.AddTo(_rootObject);

            _rootLayers.Push(rootLayer);
        }

        private static void _InitUIRootObject()
        {
            if (_rootObject != null && _rootObject.activeInHierarchy)
            {
                GameObject.Destroy(_rootObject);
            }

            // 根节点已创建不处理
            _rootObject = GameObject.Find("WUIRoot");
            if (_rootObject != null)
            {
                // 等高/等宽适配
                _InitDesignResolution();
                return;
            }

            // 创建UI根节点
            _CreateUIRootObject();

            // 等高/等宽适配
            _InitDesignResolution();

            // 点击事件
            _InitEventSystem();
        }

        private static void _CreateUIRootObject()
        {
            _rootObject = new GameObject("WUIRoot", typeof(Canvas));
            _rootObject.layer = LayerMask.NameToLayer("UI");
            _rootObject.transform.position = Vector3.zero;

            var canvas = _rootObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = CameraUtil.GetUICamera();
        }

        private static void _InitEventSystem()
        {
            // 增加点击事件
            if (GameObject.Find("EventSystem") == null)
            {
                _rootObject.AddComponent<EventSystem>();
                _rootObject.AddComponent<StandaloneInputModule>();
            }
        }

        private static void _InitDesignResolution()
        {
            var canvasScaler = _rootObject.EnsureComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            var size = DesignResolution.GetDesignSize();
            canvasScaler.referenceResolution = new Vector2(size.x, size.y);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = DesignResolution.Is2to1Screen() ? 1f : 0f;
        }
    }
}
