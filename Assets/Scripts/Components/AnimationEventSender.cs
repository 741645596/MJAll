// @Author: tanjinhua
// @Date: 2021/3/12  11:15


using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender : MonoBehaviour
{

    private Dictionary<string, Action> listeners;

    public void OnEvent(string eventName)
    {
        if (listeners == null || !listeners.ContainsKey(eventName))
        {
            return;
        }

        listeners[eventName].Invoke();
    }

    public void Bind(string eventName, Action eventHandler)
    {
        if (listeners == null)
        {
            listeners = new Dictionary<string, Action>();
        }

        if (eventHandler == null)
        {
            listeners.Remove(eventName);
            return;
        }

        if (listeners.ContainsKey(eventName))
        {
            Debug.LogWarning($"AnimationEventSender.Bind: 重复添加了[{eventName}]事件监听，将会覆盖旧监听方法");
        }

        listeners[eventName] = eventHandler;
    }
}