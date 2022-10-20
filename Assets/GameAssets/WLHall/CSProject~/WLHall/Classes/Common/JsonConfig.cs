// FHConfig.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/11/4
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Unity.Core;

namespace WLCommon
{
    /// <summary>
    /// 配置表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonConfig<T> where T : class
    {
        private Dictionary<int, T> configs = new Dictionary<int, T>();

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="assetName">资源包名</param>
        /// <param name="filename">资源key</param>
        /// <returns></returns>
        public bool Load(string assetName, string filename)
        {
            try
            {
                var asset = AssetsManager.Load<TextAsset>(assetName, filename);
                if (asset == null)
                {
                    WLDebug.LogWarning($"找不到配置文件{filename}.");
                    return false;
                }

                var valueArr = JsonMapper.ToObject<List<T>>(asset.text);
                for (int i = 0; i < valueArr.Count; i++)
                {
                    var value = valueArr[i];
                    var t = value.GetType();
                    var ps = t.GetField("id");
                    if (ps == null)
                    {
                        configs.Add(i, value);
                    }
                    else
                    {
                        var k = (int)ps.GetValue(value);
                        configs.Add(k, value);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                WLDebug.LogWarning("JSON解析失败:" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 通过ID获取配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(int id)
        {
            if(configs.ContainsKey(id))
            {
                return configs[id];
            }
            return null;
        }

        public List<T> GetAll()
        {
            var result = new List<T>();
            foreach (T item in configs.Values)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// 清理配置
        /// </summary>
        public void Clear()
        {
            configs.Clear();
        }
    }
}
