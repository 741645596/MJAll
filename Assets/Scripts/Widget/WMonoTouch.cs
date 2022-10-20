using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Unity.Widget
{
    /// <summary>
    /// 因为热更工程无法继承MonoBehaviour，所以需要弄个中间件桥接。用于处理需要滑来滑去的手势，以及事件是否向下传递
    /// </summary>
    public class WMonoTouch : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
    IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler
    {
        public Func<PointerEventData, bool> onTouchBegan;
        public Action<PointerEventData> onTouchMove;
        public Action<PointerEventData> onTouchEnd;

        private bool _isSwallowTouch;
        private GameObject _scrollGameObject;
        private GameObject _nextObject;
        private List<RaycastResult> _result = new List<RaycastResult>();

        public void OnPointerDown(PointerEventData eventData)
        {
            var r = onTouchBegan?.Invoke(eventData);
            _isSwallowTouch = r == null || r == true ? true : false;
           
            if (_isSwallowTouch)
            {
                return;
            }
            
            _nextObject = _NextEvent(eventData, ExecuteEvents.pointerDownHandler);
            if (_nextObject != null)
            {
                ExecuteEvents.Execute(_nextObject, eventData, ExecuteEvents.pointerDownHandler);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isSwallowTouch)
            {
                onTouchEnd.Invoke(eventData);
            }
            else
            {
                if (_nextObject != null)
                {
                    ExecuteEvents.Execute(_nextObject, eventData, ExecuteEvents.pointerUpHandler);
                }
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (_isSwallowTouch)
            {
                return;
            }
            if (_nextObject != null)
            {
                ExecuteEvents.Execute(_nextObject, eventData, ExecuteEvents.pointerClickHandler);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isSwallowTouch)
            {
                onTouchMove.Invoke(eventData);
            }
            else
            {
                ExecuteEvents.Execute(_scrollGameObject, eventData, ExecuteEvents.dragHandler);
            }
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (_isSwallowTouch)
            {
                return;
            }
            
            _scrollGameObject = _NextEvent(eventData, ExecuteEvents.initializePotentialDrag);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (_isSwallowTouch == true)
            {
                return;
            }
 
            if (_scrollGameObject != null)
            {
                ExecuteEvents.Execute(_scrollGameObject, eventData, ExecuteEvents.beginDragHandler);
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (_isSwallowTouch == true)
            {
                return;
            }

            if (_scrollGameObject != null)
            {
                ExecuteEvents.Execute(_scrollGameObject, eventData, ExecuteEvents.endDragHandler);
                _scrollGameObject = null;
            }
        }

        public virtual void OnScroll(PointerEventData eventData)
        {
            if (_isSwallowTouch == true)
            {
                return;
            }

            if (_scrollGameObject != null)
            {
                ExecuteEvents.Execute(_scrollGameObject, eventData, ExecuteEvents.scrollHandler);
                _scrollGameObject = null;
            }
        }


        private GameObject _NextEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
        {
            bool _begin = false;
            var pointerGo = eventData.pointerCurrentRaycast.gameObject ?? eventData.pointerDrag;
            EventSystem.current.RaycastAll(eventData, _result);
            foreach (var item in _result)
            {
                // 找到自己的下一个
                var go = item.gameObject;
                if (gameObject == go)
                {
                    _begin = true;
                    continue;
                }

                if (_begin)
                {
                    var excuteGo = ExecuteEvents.GetEventHandler<T>(go);
                    if (excuteGo)
                    {
                        return excuteGo;
                    }
                    else
                    {
                        if (go.TryGetComponent<UnityEngine.UI.Graphic>(out var com))
                        {
                            if (com.raycastTarget) return null;
                        }
                    }
                }
            }
            return null;
        }
    }

}