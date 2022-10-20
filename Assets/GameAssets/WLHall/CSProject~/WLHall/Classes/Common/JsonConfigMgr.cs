// FHConfigManager.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/11/4
using System;
using System.Collections.Generic;

namespace WLCommon
{
    /// <summary>
    /// 读json文件管理类
    /// 1. 使用工具可以把Excel转为json和C#文件
    /// 2. Excel第一列最好是id，方便索引
    /// 3. 所有json文件不能重名
    /// </summary>
    public static class JsonConfigMgr
    {
        private static Dictionary<Type, object> configs = new Dictionary<Type, object>();

        /// <summary>
        /// 根据ID获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">配置ID</param>
        /// <returns></returns>
        public static T Get<T>(int id, string aseetName, string filename) where T : class
        {
            var type = typeof(T);
            if(configs.ContainsKey(type) == false)
            {
                var cfg = Load<T>(aseetName, filename);

                if (cfg == null)
                {
                    WLDebug.LogWarning($"配置文件 {aseetName}/{filename} 加载失败，请检查格式是否正确");
                    return null;
                }
            }

            var config = configs[type] as JsonConfig<T>;
            return config.Get(id);
        }

        /// <summary>
        /// 获取T类型的所有配置，返回List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetAll<T>(string aseetName, string filename) where T : class
        {
            var type = typeof(T);
            if (configs.ContainsKey(type) == false)
            {
                var cfg = Load<T>(aseetName, filename);
                if (cfg == null)
                {
                    WLDebug.LogWarning($"配置文件 {aseetName}/{filename} 加载失败，请检查格式是否正确");
                    return null;
                }
            }
            var config = configs[type] as JsonConfig<T>;
            return config.GetAll();
        }

        /// <summary>
        /// 清空所有配置
        /// </summary>
        public static void Clear()
        {
            configs.Clear();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName">资源包名</param>
        /// <param name="filename">资源key</param>
        /// <returns></returns>
        private static JsonConfig<T> Load<T>(string assetName, string filename) where T : class
        {
            var config = new JsonConfig<T>();
            var type = typeof(T);
            if (configs.ContainsKey(type))
            {
                WLDebug.LogWarning($"{filename} 多次加载");
                return configs[type] as JsonConfig<T>;
            }
            var success = config.Load(assetName, filename);
            if (success)
            {
                configs.Add(type, config);
                return config;
            }
            return null;
        }
    }
}
