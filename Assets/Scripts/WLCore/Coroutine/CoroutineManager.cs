// CoroutineManager.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/07/28

using System.Collections.Generic;
using UnityEngine;

namespace WLCore.Coroutine
{
    /// <summary>
    /// 协程代理管理类
    /// </summary>
    public class CoroutineManager
    {
        private static readonly string CoroutineManager_Key = "CoroutineManager";
        private static Dictionary<string, CoroutineProxy> proxies = new Dictionary<string, CoroutineProxy>();
        private static CoroutineManager s_instance;

        /// <summary>
        /// 注册并返回一个协程代理
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static CoroutineProxy RegisterCoroutineProxy(string key)
        {
            _Init();

            if (proxies.ContainsKey(key))
            {
                WLDebug.LogWarning($"CoroutineManager: {key} 已经注册");
                return proxies[key];
            }
            var obj = new GameObject(key);
            obj.transform.SetParent(proxies[CoroutineManager_Key].transform);

            var proxy = obj.AddComponent<CoroutineProxy>();
            proxies.Add(key, proxy);

            return proxy;
        }

        /// <summary>
        /// 注销并清理一个协程代理
        /// </summary>
        /// <param name="key"></param>
        public static void UnregisterCoroutineProxy(string key)
        {
            if (proxies.ContainsKey(key) == false)
            {
                WLDebug.LogWarning($"CoroutineManager: {key} 未注册");
                return;
            }

            var proxy = proxies[key];
            if (proxy == null)
            {
                return;
            }
            var obj = proxy.gameObject;
            if (obj != null)
            {
                proxy.StopAllCoroutines();
                UnityEngine.Object.DestroyImmediate(obj);
            }

            proxies.Remove(key);
        }

        /// <summary>
        /// 返回一个携程代理，如果该key值没有注册则注册一个返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static CoroutineProxy GetCoroutineProxy(string key)
        {
            if(proxies.ContainsKey(key))
            {
                return proxies[key];
            }

            return RegisterCoroutineProxy(key);
        }

        /// <summary>
        /// 如果不需要手动关闭协程，播放即删的可以使用该接口
        /// </summary>
        /// <returns></returns>
        public static CoroutineProxy GetDefaultCoroutineProxy()
        {
            _Init();

            return proxies[CoroutineManager_Key];
        }

        private static CoroutineManager _Init()
        {
            if (s_instance == null)
            {
                var obj = new GameObject(CoroutineManager_Key);
                UnityEngine.Object.DontDestroyOnLoad(obj);

                var proxy = obj.AddComponent<CoroutineProxy>();
                proxies.Add(CoroutineManager_Key, proxy);

                s_instance = new CoroutineManager();
            }
            return s_instance;
        }
    }
}