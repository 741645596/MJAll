using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Unity.Widget
{
    /// <summary>
    /// UI GameObject常用方法封装
    /// </summary>
    public class WNode : WNode3D
    {
        public RectTransform rectTransform;

        private List<Sequence> _seqList;

        /// <summary>
        /// 创建UI预设控件
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public static new WNode Create(string assetName, string assetKey)
        {
            var node = new WNode();
            node.InitGameObject(assetName, assetKey);
            return node;
        }

        /// <summary>
        /// 通过GameObject初始化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static new WNode Create(GameObject obj)
        {
            var node = new WNode();
            node.InitGameObject(obj);
            return node;
        }

        /// <summary>
        /// 初始Object
        /// </summary>
        /// <param name="obj"></param>
        public new void InitGameObject(GameObject obj)
        {
            base.InitGameObject(obj);

            rectTransform = obj.transform as RectTransform;
            Debug.Assert(rectTransform != null, "错误提示：WNode.InitGameObject参数obj的根节点不是RectTransform");
        }

        /// <summary>
        /// 通过AB包资源初始化
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetKey"></param>
        public new void InitGameObject(string assetName, string assetKey)
        {
            base.InitGameObject(assetName, assetKey);
            
            rectTransform = transform as RectTransform;
            Debug.Assert(rectTransform != null, $"错误提示：{assetName}/{assetKey}资源根节点不是RectTransform，请检查");
        }

        /// <summary>
        /// 添加到父节点
        /// </summary>
        /// <param name="obj"></param>
        public WNode AddTo(GameObject obj)
        {
            rectTransform.SetParent(obj.transform, false);
            return this;
        }

        public WNode AddTo(Transform t)
        {
            rectTransform.SetParent(t, false);
            return this;
        }

        public WNode AddTo(WNode node)
        {
            rectTransform.SetParent(node.transform, false);
            return this;
        }

        /// <summary>
        /// 建议使用Position调位置，适配模式在编辑器应该是设置好的了
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public WNode Layout(Vector2 layout, Vector2 offset)
        {
            rectTransform.Layout(layout, offset);
            return this;
        }

        public WNode Layout(Vector2 layout)
        {
            rectTransform.Layout(layout);
            return this;
        }

        public WNode LayoutScreen(Vector2 layout, Vector2 offset)
        {
            rectTransform.LayoutScreen(layout, offset);
            return this;
        }

        public WNode LayoutScreen(Vector2 layout)
        {
            rectTransform.LayoutScreen(layout);
            return this;
        }

        /// <summary>
        /// 设置坐标位置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public WNode SetPosition(Vector2 pos)
        {
            rectTransform.anchoredPosition = pos;
            return this;
        }

        public WNode SetPosition(float x, float y)
        {
            rectTransform.anchoredPosition = new Vector2(x, y);
            return this;
        }

        public WNode SetPositionX(float x)
        {
            var p = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(x, p.y);
            return this;
        }

        public WNode SetPositionY(float y)
        {
            var p = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(p.x, y);
            return this;
        }

        public Vector2 GetPosition()
        {
            return rectTransform.anchoredPosition;
        }

        /// <summary>
        /// 以父节点左下角为原点(0,0)设置位置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public WNode SetPositionInZero(Vector2 pos)
        {
            rectTransform.SetPositionInZero(pos);
            return this;
        }

        /// <summary>
        /// 获取当前节点在屏幕上的位置
        /// </summary>
        /// <returns></returns>
        public Vector2 GetDisplayPosition()
        {
            return rectTransform.GetDisplayPosition();
        }

        /// <summary>
        /// 将屏幕坐标转为当前节点内的坐标
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public Vector2 DisplayToLocal(Vector2 worldPos)
        {
            return rectTransform.DisplayToLocal(worldPos);
        }

        public WNode SetScale(float s)
        {
            rectTransform.localScale = new Vector3(s, s, 1);
            return this;
        }

        public WNode SetScale(float x, float y)
        {
            rectTransform.localScale = new Vector3(x, y, 1);
            return this;
        }

        public WNode SetScaleX(float scale)
        {
            var s = rectTransform.localScale;
            rectTransform.localScale = new Vector3(scale, s.y, s.z);
            return this;
        }

        public WNode SetScaleY(float scale)
        {
            var s = rectTransform.localScale;
            rectTransform.localScale = new Vector3(s.x, scale, s.z);
            return this;
        }

        public WNode SetAnchor(Vector2 anchor)
        {
            rectTransform.pivot = anchor;
            return this;
        }

        public WNode SetRotation(float rotation)
        {
            var r = rectTransform.eulerAngles;
            rectTransform.eulerAngles = new Vector3(r.x, r.y, rotation * -1f);
            return this;
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="opacity"> 0 - 255 </param>
        /// <param name="effectChilds"> 是否影响所有子节点 </param>
        /// <returns></returns>
        public WNode SetOpacity(int opacity, bool effectChilds = false)
        {
            rectTransform.SetOpacity(opacity, effectChilds);
            return this;
        }

        public WNode SetContentSize(Vector2 size)
        {
            rectTransform.sizeDelta = size;
            return this;
        }

        public WNode SetContentSize(float width, float height)
        {
            rectTransform.sizeDelta = new Vector2(width, height);
            return this;
        }

        public void RunAction(CCActionData action)
        {
            if (action.type == CCActionType.Sequence
                || action.type == CCActionType.Spawn
                || action.type == CCActionType.RepeatForever
                || action.type == CCActionType.EaseExponentialOut
                || action.type == CCActionType.EaseExponentialIn)
            {
                var sequence = CCActionHelper.GetSequence(rectTransform, action, action.type);
                sequence.onComplete = () => {
                    _seqList.Remove(sequence);
                };
                sequence.Play();

                if (_seqList == null)
                {
                    _seqList = new List<Sequence>();
                }
                _seqList.Add(sequence);
            }
        }

        public void StopAllActions()
        {
            if (null == _seqList)
            {
                return;
            }

            for (int i = 0; i < _seqList.Count; i++)
            {
                _seqList[i].Kill();
            }
            _seqList.Clear();
        }

        public float GetScale()
        {
            return rectTransform.localScale.x;
        }

        public Vector2 GetContentSize()
        {
            return rectTransform.sizeDelta;
        }

        
    }
}
