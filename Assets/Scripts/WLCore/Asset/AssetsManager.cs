// AssetsManager.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/20

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Network;
using UnityEngine;
using Unity.Utility;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using WLCore.Helper;
using Object = UnityEngine.Object;

namespace Unity.Core
{
    /// <summary>
    /// 总的资源管理，包括判断是本地模式还是ab包模式
    /// </summary>
    public static class AssetsManager
    {
        public enum LoadType
        {
            Local,       // 读取本地资源
            AssetBundle, // 读取AB包资源
        }

        private static LoadType      _loadType          = LoadType.AssetBundle;     //读取本地资源
        private static IAssetManager _devAssetManager   = new AssetBundleManager(); //开发模式

        /// <summary>
        /// 是直接加载本地资源，还是加载Ab包
        /// </summary>
        /// <param name="t"></param>
        public static void SetLoadType(LoadType t)
        {
            _devAssetManager.UnloadAll(true);

            #if UNITY_EDITOR
            // 编辑器模式可以选择用哪种模式
            if (t == LoadType.Local)
            {
                _loadType        = t;
                _devAssetManager = new DevAssetManager();
            }
            #endif
            WLDebug.Log("资源加载方式：" + _loadType);
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Object Load(string path, string key)
        {
            Object res = _devAssetManager.Load(path, key);
            if (res == null)
            {
                WLDebug.LogWarning("读取资源失败：" + path + " " + key);
            }

            return res;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void LoadAsync<T>(string assetName, string key, Action<T> action) where T : Object
        {
            if (string.IsNullOrEmpty(assetName) || string.IsNullOrEmpty(key))
            {
                WLDebug.LogWarning("读取资源失败：" + assetName + " " + key + " " + typeof(T));
                action?.Invoke(null);
            }

            _devAssetManager.LoadAsync<T>(assetName, key, action);
        }

        /// <summary>
        /// 取消异步
        /// </summary>
        public static void CancelLoadAsync()
        {
            _devAssetManager.CancelLoadAsync();
        }

        /// <summary>
        /// 异步加载所有
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="action"></param>
        public static void LoadAllAsync(string assetName, Action<Object[]> action)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                WLDebug.LogWarning("读取资源失败：" + assetName);
                action?.Invoke(null);
            }

            _devAssetManager.LoadAllAsync(assetName, action);
        }

        /// <summary>
        /// 加载资源【泛型返回对应资源】
        /// </summary>
        /// <param name="assetName">资源文件夹路径</param>
        /// <param name="key">文件路径</param>
        /// <param name="isCleanOnSwichScene">切换场景是否清除，默认清除</param>
        /// <returns></returns>
        public static T Load<T>(string assetName, string key, bool isCleanOnSwichScene = true) where T : Object
        {
            if (string.IsNullOrEmpty(key))
            {
                WLDebug.LogWarning("读取资源失败：" + " " + key + " " + typeof(T));
                return null;
            }

            T res = _devAssetManager.Load<T>(assetName, key);
            if (res == null)
            {
                WLDebug.LogWarning("读取资源失败：" + " " + key);
                return null;
            }

            return res;
        }

        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="isForce"> 是否强力卸载，true表示正在使用的也会卸载掉，默认false </param>
        public static void Unload(string assetName, bool isForce = false)
        {
            _devAssetManager.Unload(assetName, isForce);
        }

        /// <summary>
        /// 卸载全部
        /// </summary>
        /// <param name="unload"></param>
        public static void UnloadAll(bool unload = false)
        {
            _devAssetManager.UnloadAll(unload);
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 查找资源是否加载存在
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contains(string assetName, string key)
        {
            return _devAssetManager.Contains(assetName, key);
        }

        /// <summary>
        /// 加载模块的依赖清单，一定要在Load之前加载依赖关系，否则资源可能显示为白图
        /// </summary>
        /// <param name="names"></param>
        /// <param name="isReload"></param>
        public static void LoadDependencieMFs(List<string> names)
        {
            foreach (var item in names)
            {
                _devAssetManager.LoadDependencieMF(item);
            }
        }

        /// <summary>
        /// 加载模块的清单文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isReload"></param>
        public static void LoadDependencieMF(string name)
        {
            _devAssetManager.LoadDependencieMF(name);
        }

        /// <summary>
        /// 清除所有依赖清单，热更新资源之后需要清除下依赖关系
        /// </summary>
        public static void CleanDependencieMF()
        {
            _devAssetManager.CleanDependencieMF();
        }

        /// <summary>
        /// 在关闭的时候调用【卸载】
        /// </summary>
        public static void OnShutdown()
        {
            _devAssetManager.UnloadAll(true);
        }
    }
}
