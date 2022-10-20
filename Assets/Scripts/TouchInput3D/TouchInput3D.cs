// @Author: tanjinhua
// @Date: 2021/1/2  0:56


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 3D对象触摸事件类型
/// </summary>
public enum TouchEvent3D
{
    /// <summary>
    /// 手指按下时在对象范围内或从外部划动进入对象范围时响应
    /// </summary>
    Enter,

    /// <summary>
    /// Enter响应后手指划动离开对象范围时响应
    /// </summary>
    Exit,

    /// <summary>
    /// 手指按下时响应，需要通过回传参数TouchData3D的hinIndex判断是否击中和第几个击中
    /// </summary>
    Begin,

    /// <summary>
    /// 划动手指时响应，需要通过回传参数TouchData3D的hinIndex判断是否击中和第几个击中
    /// </summary>
    Move,

    /// <summary>
    /// 手指离开屏幕时响应，需要通过回传参数TouchData3D的hinIndex判断是否击中和第几个击中
    /// </summary>
    End,

    /// <summary>
    /// Begin和End都在对象范围内时响应
    /// </summary>
    Click
}

//public enum TouchPhase
//{
//    Begin,
//    Move,
//    End
//}

public class TouchInput3D
{
    private class Listener
    {
        public Camera eventCamera;
        public GameObject target;
        public Collider collider;
        public Action<TouchData3D> onEnter;
        public Action<TouchData3D> onExit;
        public Action<TouchData3D> onBegin;
        public Action<TouchData3D> onMove;
        public Action<TouchData3D> onEnd;
        public Action<TouchData3D> onClick;
        //public Dictionary<int, bool> beginStates;
        public Dictionary<int, bool> enterStates;
        public Dictionary<int, bool> exitStates;

        public bool hasAnyHandler
        {
            get
            {
                return onEnter != null || onExit != null || onBegin != null || onMove != null || onEnd != null || onClick != null;
            }
        }
    }

    private List<Camera> eventCameras;
    private Dictionary<Collider, Listener> colliderListenerMap;

    public TouchInput3D()
    {
        eventCameras = new List<Camera>();
        colliderListenerMap = new Dictionary<Collider, Listener>();
    }

    public void AddListener(GameObject target, Camera eventCamera, TouchEvent3D type, Action<TouchData3D> eventHandler)
    {
        if (!target.TryGetComponent(out Collider collider))
        {
            WLDebug.LogWarning("TouchInput3D.AddListener: 注册监听3D物体触摸事件需要添加Collider组件");
            return;
        }
        Listener listener;
        if (colliderListenerMap.ContainsKey(collider))
        {
            listener = colliderListenerMap[collider];
            if (listener.eventCamera != eventCamera)
            {
                throw new Exception("TouchInput3D.AddListener: 同一个对象不允许同时使用不同的Camera注册触摸事件");
            }
        }
        else
        {
            listener = new Listener
            {
                eventCamera = eventCamera,
                target = target,
                collider = collider,
                //beginStates = new Dictionary<int, bool>(),
                enterStates = new Dictionary<int, bool>(),
                exitStates = new Dictionary<int, bool>()
            };
            colliderListenerMap[collider] = listener;
        }

        BindEventHandler(target, listener, type, eventHandler);

        if (!eventCameras.Contains(eventCamera))
        {
            eventCameras.Add(eventCamera);
        }
    }

    public void RemoveListener(GameObject target, TouchEvent3D type)
    {
        if (!target.TryGetComponent(out Collider collider))
        {
            return;
        }

        if (!colliderListenerMap.ContainsKey(collider))
        {
            return;
        }

        Listener listener = colliderListenerMap[collider];

        switch (type)
        {
            case TouchEvent3D.Enter:
                listener.onEnter = null;
                break;
            case TouchEvent3D.Exit:
                listener.onExit = null;
                break;
            case TouchEvent3D.Begin:
                listener.onBegin = null;
                break;
            case TouchEvent3D.Move:
                listener.onMove = null;
                break;
            case TouchEvent3D.End:
                listener.onEnd = null;
                break;
            case TouchEvent3D.Click:
                listener.onClick = null;
                break;
        }

        if (!listener.hasAnyHandler)
        {
            RemoveListener(collider);
        }
    }

    public void RemoveListener(GameObject target)
    {
        if (!target.TryGetComponent(out Collider collider))
        {
            return;
        }
        RemoveListener(collider);
    }

    public void RemoveListener(Collider collider)
    {
        if (colliderListenerMap.ContainsKey(collider))
        {
            Camera eventCamera = colliderListenerMap[collider].eventCamera;

            CheckCameraUsage(eventCamera);

            colliderListenerMap.Remove(collider);
        }
    }

    public void Clear()
    {
        eventCameras.Clear();
        colliderListenerMap.Clear();
    }

    private void BindEventHandler(GameObject target, Listener listener, TouchEvent3D type, Action<TouchData3D> eventHandler)
    {
        switch (type)
        {
            case TouchEvent3D.Enter:
                if (listener.onEnter != null)
                {
                    throw new Exception($"TouchInput3D.AddListener: [{target.gameObject.name}] 重复注册了[{type}]事件");
                }
                listener.onEnter = eventHandler;
                break;

            case TouchEvent3D.Exit:
                if (listener.onExit != null)
                {
                    throw new Exception($"TouchInput3D.AddListener: [{target.gameObject.name}] 重复注册了[{type}]事件");
                }
                listener.onExit = eventHandler;
                break;

            case TouchEvent3D.Begin:
                if (listener.onBegin != null)
                {
                    throw new Exception($"TouchInput3D.AddListener: [{target.gameObject.name}] 重复注册了[{type}]事件");
                }
                listener.onBegin = eventHandler;
                break;

            case TouchEvent3D.Move:
                if (listener.onMove != null)
                {
                    throw new Exception($"TouchInput3D.AddListener: [{target.gameObject.name}] 重复注册了[{type}]事件");
                }
                listener.onMove = eventHandler;
                break;

            case TouchEvent3D.End:
                if (listener.onEnd != null)
                {
                    throw new Exception($"TouchInput3D.AddListener: [{target.gameObject.name}] 重复注册了[{type}]事件");
                }
                listener.onEnd = eventHandler;
                break;

            case TouchEvent3D.Click:
                if (listener.onClick != null)
                {
                    throw new Exception($"TouchInput3D.AddListener: [{target.gameObject.name}] 重复注册了[{type}]事件");
                }
                listener.onClick = eventHandler;
                break;
        }
    }

    private void ClearInvalidTargets()
    {
        if (colliderListenerMap.Count == 0)
        {
            return;
        }

        List<Collider> keys = new List<Collider>(colliderListenerMap.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            Collider collider = keys[i];

            if (collider == null)
            {
                colliderListenerMap.Remove(collider);
            }
        }

        for (int i = eventCameras.Count - 1; i >= 0; i--)
        {
            Camera eventCamera = eventCameras[i];

            if (eventCamera == null)
            {
                eventCameras.RemoveAt(i);
            }
            else
            {
                CheckCameraUsage(eventCamera);
            }
        }
    }

    private void CheckCameraUsage(Camera camera)
    {
        bool cameraUsed = false;
        foreach (var pair in colliderListenerMap)
        {
            if (pair.Value.eventCamera == camera)
            {
                cameraUsed = true;
                break;
            }
        }

        if (!cameraUsed)
        {
            eventCameras.Remove(camera);
        }
    }

    public void DispatchTouchEvent(TouchPhase phase, PointerEventData eventData)
    {
        ClearInvalidTargets();

        if (eventCameras.Count == 0)
        {
            return;
        }

        int id = eventData.pointerId;

        // 射线没击中时hitIndex返回-1给监听者
        int invalidHitIndex = -1;

        for (int i = 0; i < eventCameras.Count; i++)
        {
            Camera eventCamera = eventCameras[i];
            if (!eventCamera.gameObject.activeInHierarchy)
            {
                continue;
            }

            List<Listener> listeners = GetListenersByCamera(eventCamera);

            switch (phase)
            {
                case TouchPhase.Began:

                    SetupListenerStates(id, listeners);

                    List<RaycastHit> hitListBegin = Raycast(eventCamera, eventData);

                    //if (hitListBegin.Count == 0)
                    //{
                    //    continue;
                    //}

                    int hitIndexBegin = 0;
                    int hitIndexEnterOnBegin = 0;
                    foreach (RaycastHit hit in hitListBegin)
                    {
                        if (!IsValidHit(hit, listeners))
                        {
                            continue;
                        }

                        Listener listener = colliderListenerMap[hit.collider];

                        DispatchBegin(listener, id, hit, eventData, ref hitIndexBegin);

                        DispatchEnter(listener, id, hit, eventData, ref hitIndexEnterOnBegin);

                        // 移除射线击中的监听者，准备分发没击中的Begin事件
                        listeners.Remove(listener);
                    }

                    foreach (Listener listener in listeners)
                    {
                        DispatchBegin(listener, id, default, eventData, ref invalidHitIndex);
                    }

                    break;

                case TouchPhase.Moved:

                    List<RaycastHit> hitListMove = Raycast(eventCamera, eventData);

                    int hitIndexMove = 0;
                    int hitIndexEnterOnMove = 0;
                    foreach (RaycastHit hit in hitListMove)
                    {
                        if (!IsValidHit(hit, listeners))
                        {
                            continue;
                        }

                        Listener listener = colliderListenerMap[hit.collider];

                        DispatchEnter(listener, id, hit, eventData, ref hitIndexEnterOnMove);

                        DispatchMove(listener, id, hit, eventData, ref hitIndexMove);

                        // 移除射线击中的监听者，准备分发没击中的Move事件
                        listeners.Remove(listener);
                    }

                    foreach (Listener listener in listeners)
                    {
                        DispatchExit(listener, id, eventData);

                        DispatchMove(listener, id, default, eventData, ref invalidHitIndex);
                    }

                    break;

                case TouchPhase.Ended:

                    List<RaycastHit> hitListEnd = Raycast(eventCamera, eventData);

                    int hitIndexEnd = 0;
                    int hitIndexClick = 0;
                    foreach (RaycastHit hit in hitListEnd)
                    {
                        if (!IsValidHit(hit, listeners))
                        {
                            continue;
                        }

                        Listener listener = colliderListenerMap[hit.collider];

                        DispatchClick(listener, id, hit, eventData, ref hitIndexClick);

                        DispatchEnd(listener, id, hit, eventData, ref hitIndexEnd);

                        RemoveListenerStates(listener, id);

                        // 移除射线击中的监听者，准备分发没击中的End事件
                        listeners.Remove(listener);
                    }

                    foreach (Listener listener in listeners)
                    {
                        DispatchEnd(listener, id, default, eventData, ref invalidHitIndex);

                        RemoveListenerStates(listener, id);
                    }

                    break;
            }
        }
    }


    private void DispatchEnter(Listener listener, int id, RaycastHit hit, PointerEventData eventData, ref int hitIndex)
    {
        if (listener.enterStates.ContainsKey(id) && !listener.enterStates[id])
        {
            listener.enterStates[id] = true;
            listener.exitStates[id] = false;
            if (listener.onEnter != null)
            {
                listener.onEnter(new TouchData3D
                {
                    receiver = listener.target,
                    hitIndex = hitIndex,
                    hitInfo = hit,
                    eventData = eventData
                });

                hitIndex++;
            }
        }
    }


    private void DispatchExit(Listener listener, int id, PointerEventData eventData)
    {
        if ((listener.enterStates.ContainsKey(id) && listener.enterStates[id]) && (listener.exitStates.ContainsKey(id) && !listener.exitStates[id]))
        {
            listener.exitStates[id] = true;
            listener.enterStates[id] = false;
            listener.onExit?.Invoke(new TouchData3D
            {
                receiver = listener.target,
                hitIndex = -1,
                eventData = eventData
            });
        }
    }


    private void DispatchBegin(Listener listener, int id, RaycastHit hit, PointerEventData eventData, ref int hitIndex)
    {
        //listener.beginStates[id] = true;

        if (listener.onBegin != null)
        {
            listener.onBegin(new TouchData3D
            {
                receiver = listener.target,
                hitIndex = hitIndex,
                hitInfo = hit,
                eventData = eventData
            });

            if (hitIndex != -1)
            {
                hitIndex++;
            }
        }
    }


    private void DispatchMove(Listener listener, int id, RaycastHit hit, PointerEventData eventData, ref int hitIndex)
    {
        if (/*listener.beginStates.ContainsKey(id) && listener.beginStates[id] && */listener.onMove != null)
        {
            listener.onMove(new TouchData3D
            {
                receiver = listener.target,
                hitIndex = hitIndex,
                hitInfo = hit,
                eventData = eventData
            });

            if (hitIndex != -1)
            {
                hitIndex++;
            }
        }
    }


    private void DispatchEnd(Listener listener, int id, RaycastHit hit, PointerEventData eventData, ref int hitIndex)
    {
        if (/*listener.beginStates.ContainsKey(id) && listener.beginStates[id] && */listener.onEnd != null)
        {
            listener.onEnd(new TouchData3D
            {
                receiver = listener.target,
                hitIndex = hitIndex,
                hitInfo = hit,
                eventData = eventData,
            });

            if (hitIndex != -1)
            {
                hitIndex++;
            }
        }
    }


    private void DispatchClick(Listener listener, int id, RaycastHit hit, PointerEventData eventData, ref int hitIndex)
    {
        if (/*listener.beginStates.ContainsKey(id) && listener.beginStates[id] && */listener.onClick != null)
        {
            listener.onClick(new TouchData3D
            {
                receiver = listener.target,
                hitIndex = hitIndex,
                hitInfo = hit,
                eventData = eventData,
            });

            hitIndex++;
        }
    }


    private bool IsValidHit(RaycastHit hit, List<Listener> listeners)
    {
        if (!colliderListenerMap.ContainsKey(hit.collider))
        {
            return false;
        }

        Listener listener = colliderListenerMap[hit.collider];
        if (!listeners.Contains(listener))
        {
            return false;
        }

        return true;
    }


    private List<Listener> GetListenersByCamera(Camera eventCamera)
    {
        List<Listener> listeners = new List<Listener>();

        foreach (var pair in colliderListenerMap)
        {
            if (pair.Value.eventCamera == eventCamera && !listeners.Contains(pair.Value))
            {
                listeners.Add(pair.Value);
            }
        }

        return listeners;
    }


    private void SetupListenerStates(int pointerId, List<Listener> listeners)
    {
        foreach (Listener listener in listeners)
        {
            //listener.beginStates[pointerId] = false;
            listener.enterStates[pointerId] = false;
            listener.exitStates[pointerId] = false;
        }
    }


    private void RemoveListenerStates(Listener listener, int id)
    {
        //listener.beginStates.Remove(id);
        listener.enterStates.Remove(id);
        listener.exitStates.Remove(id);
    }


    private List<RaycastHit> Raycast(Camera eventCamera, PointerEventData eventData)
    {
        Ray ray = eventCamera.ScreenPointToRay(eventData.position);

        RaycastHit[] hits = Physics.RaycastAll(ray, eventCamera.farClipPlane - eventCamera.nearClipPlane, eventCamera.cullingMask);

        if (hits.Length == 0)
        {
            return new List<RaycastHit>();
        }

        List<RaycastHit> list = new List<RaycastHit>(hits);

        list.Sort((a, b) => { return a.distance < b.distance ? -1 : 1; });

        return list;
    }
}