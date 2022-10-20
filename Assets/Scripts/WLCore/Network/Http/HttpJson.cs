// HttpHelper.cs
// Author: shihongyang <shihongyang@weile.com>
// Date: 2020/2/14

using System;
using LitJson;

namespace Unity.Network
{
    /// <summary>
    /// 用于Http请求返回是json格式的数据，这是对HttpHelper的二次封装
    /// </summary>
    public static class HttpJson
    {
        /// <summary>
        /// 请求json格式，如果返回数据非json格式或者请求超时，则callback(null)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void Get(string url, Action<JsonData> callback)
        {
            HttpHelper.Get(url, (txt, code) =>
            {
                if (txt != null && (code == 200 || code == 0))
                {
                    var jsonData = JsonMapper.ToObject(txt);
                    callback?.Invoke(jsonData);
                }
                else
                {
                    callback?.Invoke(null);
                }
            });
        }

        /// <summary>
        /// 请求json格式，如果返回数据非json格式或者请求超时，则callback(null)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void Post(string url, Action<JsonData> callback)
        {
            HttpHelper.Post(url, "", (txt, _, code) =>
            {
                if (txt != null && (code == 200 || code == 0))
                {
                    var jsonData = JsonMapper.ToObject(txt);
                    callback?.Invoke(jsonData);
                }
                else
                {
                    callback?.Invoke(null);
                }
            });
        }

        /// <summary>
        /// 同上，新增postData
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"> 指定获得数据 </param>
        /// <param name="callback"></param>
        public static void Post(string url, string postData, Action<JsonData> callback)
        {
            HttpHelper.Post(url, postData, (txt, _, code) =>
            {
                if (txt != null && (code == 200 || code == 0))
                {
                    var jsonData = JsonMapper.ToObject(txt);
                    callback?.Invoke(jsonData);
                }
                else
                {
                    callback?.Invoke(null);
                }
            });
        }
    }
}
