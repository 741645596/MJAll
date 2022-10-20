// HttpHelper.cs
// Author: shihongyang <shihongyang@weile.com>
// Date: 2020/2/14

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using WLCore.Coroutine;

namespace Unity.Network
{
    /// <summary>
    /// 用于Http请求
    /// </summary>
    public class HttpHelper
    {
        private UnityWebRequest _webRequest;
        private Coroutine _coroutine;

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="action"></param>
        /// <param name="timeout"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static HttpHelper Get(string url, Action<string, long> action)
        {
            var http = new HttpHelper();
            http.GetHttp(url, action, 10, null); 
            return http;
        }

        public static HttpHelper Get(string url, Action<string, long> action, int timeout, Dictionary<string, string> header)
        {
            var http = new HttpHelper();
            http.GetHttp(url, action, timeout, header);
            return http;
        }

        ///// <summary>
        ///// Post请求
        ///// </summary>
        ///// <param name="url">请求地址</param>
        ///// <param name="postData">请求数据json格式字符串</param>
        ///// <param name="action">回调方法</param>
        ///// <param name="timeout">超时时间</param>
        ///// <param name="header">Header</param>
        public static HttpHelper Post(string url, string postData, Action<string, byte[], long> action)
        {
            var http = new HttpHelper();
            http.PostHttp(url, postData, action, 10, null);
            return http;
        }

        public static HttpHelper Post(string url, string data, Action<string, byte[], long> action, int timeout, Dictionary<string, string> header) 
        {
            var http = new HttpHelper();
            http.PostHttp(url, data, action, timeout, header);
            return http;
        }

        public static HttpHelper Post(string url, string data, Action<string, byte[], long> action, int timeout, Dictionary<string, string> header, CertificateProxy certificateHandler)
        {
            var http = new HttpHelper();
            http.PostHttp(url, data, action, timeout, header, certificateHandler);
            return http;
        }

        public void GetHttp(string url, Action<string, long> action, int timeout, Dictionary<string, string> header)
        {
            Cancel();

            var proxy = _GetCoroutineProxy();
            _coroutine = proxy.StartCoroutine(_IEGet(url, action, timeout, header));
        }

        public void PostHttp(string url, string data, Action<string, byte[], long> action, int timeout = 10, Dictionary<string, string> header = null, CertificateProxy certificateHandler = null)
        {
            Cancel();

            var proxy = _GetCoroutineProxy();
            _coroutine = proxy.StartCoroutine(_IEPost(url, data, action, timeout, header, certificateHandler));
        }

        public void Cancel()
        {
            if (_coroutine != null)
            {
                var proxy = _GetCoroutineProxy();
                proxy.StopCoroutine(_coroutine);
                _coroutine = null;
            }

            if (_webRequest != null)
            {
                _webRequest.Abort();
                _webRequest = null;
            }
        }

        private IEnumerator _IEGet(string url, Action<string, long> action, int timeout, Dictionary<string, string> header)
        {
            if (WLDebug.LogLevel > LogLevel.Debug)
            {
                WLDebug.Log($"↑ GET [{url}]");
            }
            
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.timeout = timeout;

            if (header != null)
            {
                foreach (var itme in header)
                {
                    request.SetRequestHeader(itme.Key, itme.Value);
                }
            }

            yield return request.SendWebRequest();
            yield return null;      // 防止http回调和主线程同时读取一个比较大的ab包引起崩溃

            if (request.result != UnityWebRequest.Result.Success)
            {
                WLDebug.LogWarning($"HttpHelper.Get请求失败 url:{url} result:{request.result}");
            }

            if (WLDebug.LogLevel > LogLevel.Debug)
            {
                WLDebug.Log($"↓ code:{request.responseCode} data:{request.downloadHandler?.text}");
            }    
            action(request.downloadHandler?.text, request.responseCode);
            request.Dispose();

            _coroutine = null;
            _webRequest = null;
        }

        private IEnumerator _IEPost(string url, string postData, Action<string, byte[], long> action, int timeout, Dictionary<string, string> header, CertificateProxy certificateHandler)
        {
            if (WLDebug.LogLevel > LogLevel.Debug)
            {
                WLDebug.Log($"↑ POST [{url}] data={postData}");
            }
                
            byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);
            var request = new UnityWebRequest(url, "POST")
            {
                timeout = timeout,
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer(),
                certificateHandler = certificateHandler
            };

            if (header != null)
            {
                foreach (var itme in header)
                {
                    request.SetRequestHeader(itme.Key, itme.Value);
                }
            }

            yield return request.SendWebRequest();
            yield return null;      // 防止http回调和主线程同时读取一个比较大的ab包引起崩溃

            if (request.result != UnityWebRequest.Result.Success)
            {
                WLDebug.LogWarning($"HttpHelper.Post请求失败 url:{url} result:{request.result}");
            }

            if (WLDebug.LogLevel > LogLevel.Debug)
            {
                WLDebug.Log($"↓ code:{request.responseCode} data:{request.downloadHandler?.text}");
            }
                
            action(request.downloadHandler?.text, request.downloadHandler?.data, request.responseCode);
            request.Dispose();

            _coroutine = null;
            _webRequest = null;
        }

        private CoroutineProxy _GetCoroutineProxy()
        {
            return CoroutineManager.GetCoroutineProxy("HttpCoroutineProxy");
        }
    }
}
