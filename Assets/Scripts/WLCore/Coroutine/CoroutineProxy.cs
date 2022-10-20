// CoroutineProxy.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/07/28

namespace WLCore.Coroutine
{
    using System.Collections;
    using UnityEngine;
    using System;

    public class CoroutineProxy : MonoBehaviour
    {

        private struct RepeatInvokeRequest
        {
            public float delay;
            public Action action;
            public float escaped;
        }

        /// <summary>
        /// 延迟调用
        /// </summary>
        /// <param name="time">延迟时间</param>
        /// <param name="action">回调方法</param>
        /// <returns></returns>
        public Coroutine DelayInvoke(float time, Action action)
        {
            return StartCoroutine(IEDelayInvoke(time, action));
        }

        /// <summary>
        /// 重复延时调用
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="initialTime"></param>
        /// <returns></returns>
        public Coroutine DelayInvokeRepeating(float time, Action action, float initialTime = 0)
        {
            var request = new RepeatInvokeRequest
            {
                delay = time,
                action = action,
                escaped = initialTime
            };
            return StartCoroutine(IEInvokeRepeating(request));
        }

        /// <summary>
        /// 取消延迟调用
        /// </summary>
        /// <param name="coroutine"></param>
        public void CancelInvoke(Coroutine coroutine)
        {
            if (coroutine == null)
            {
                return;
            }
            StopCoroutine(coroutine);
        }

        /// <summary>
        /// 延迟调用的IEnumerator
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private IEnumerator IEDelayInvoke(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }

        private IEnumerator IEInvokeRepeating(RepeatInvokeRequest request)
        {
            while(true)
            {
                request.escaped += Time.deltaTime;
                if (request.escaped > request.delay)
                {
                    request.escaped -= request.delay;
                    request.action.Invoke();
                }

                yield return null;
            }
        }
    }
}