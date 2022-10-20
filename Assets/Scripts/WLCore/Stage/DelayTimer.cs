
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WLCore.Coroutine;

public static class DelayTimer
{
    /// <summary>
    /// 延迟处理回调接口
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="action"></param>
    public static void Call(float delayTime, Action action)
    {
        var c = CoroutineManager.GetCoroutineProxy("DelayTimer");
        c.StartCoroutine(IEDelayInvoke(delayTime, action));
    }

    /// <summary>
    /// 关闭所有延迟事件
    /// </summary>
    public static void Cancel()
    {
        var c = CoroutineManager.GetCoroutineProxy("DelayTimer");
        c.StopAllCoroutines();
    }

    private static IEnumerator IEDelayInvoke(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
}