// @Author: tanjinhua
// @Date: 2021/4/1  11:43


using System;
using UnityEngine;
using UnityEngine.EventSystems;
using WLCore.Entity;

namespace MJCommon
{
    /// <summary>
    /// 协助手牌输入操作，用来响应点击空白区域
    /// </summary>
    public class MJBlankPlane : BaseEntity
    {

        public Action onClick;


        private Vector2 _beginPos;
        private PointerEventData _currentPointerData;


        public void Initialize(Camera eventCamera, TouchInput3D touchInput3D)
        {
            gameObject.layer = LayerMask.NameToLayer(MJDefine.MJHandMask);

            touchInput3D.AddListener(gameObject, eventCamera, TouchEvent3D.Begin, OnBegin);

            touchInput3D.AddListener(gameObject, eventCamera, TouchEvent3D.Click, OnClick);

            touchInput3D.AddListener(gameObject, eventCamera, TouchEvent3D.End, OnEnd);
        }


        private void OnBegin(TouchData3D data)
        {
            if (_currentPointerData != null)
            {
                return;
            }

            if (data.hitIndex != 0)
            {
                return;
            }

            _currentPointerData = data.eventData;

            _beginPos = data.eventData.position / display.srceenScaleFactor;
        }


        private void OnClick(TouchData3D data)
        {
            if (!IsCurrentTouch(data))
            {
                return;
            }

            if (data.hitIndex != 0)
            {
                return;
            }

            Vector2 pos = data.eventData.position / display.srceenScaleFactor;

            if (Vector2.Distance(_beginPos, pos) > 50f)
            {
                return;
            }

            onClick?.Invoke();
        }


        private void OnEnd(TouchData3D data)
        {
            if (!IsCurrentTouch(data))
            {
                return;
            }

            _currentPointerData = null;
        }


        private bool IsCurrentTouch(TouchData3D data)
        {
            if (_currentPointerData == null)
            {
                return false;
            }

            return _currentPointerData.pointerId == data.eventData.pointerId;
        }


        protected override GameObject CreateGameObject()
        {
            var obj = ObjectHelper.Instantiate("MJCommon/MJ/mj_cards", "prefabs/blank_plane.prefab");
            obj.name = "MJBlankPlane";
            return obj;
        }
    }
}
