// DevAssetManager.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/24

#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

namespace Unity.Core
{
    /// <summary>
    /// 开发模式，直接读取资源
    /// </summary>
    public class DevAssetManager : IAssetManager
    {
        public string RootPath { get; set; } = "Assets/GameAssets";
        //TODO: 这里的缓存删掉，然后使用ab自己的缓存【只要不unload，就能加载到，缺点同名ab包加载前一个】
        private Dictionary<string, Object> assetsMap { get; set; }

        public DevAssetManager()
        {
            assetsMap = new Dictionary<string, Object>();
        }

        public T Load<T>(string res, string key) where T : Object
        {
            var assetPath = Path.Combine(RootPath, res, key);
            assetPath.Replace("\\", "/");
            if (assetsMap.ContainsKey(assetPath))
            {
                return assetsMap[assetPath] as T;
            }

            T obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            assetsMap[assetPath] = obj;
            return obj;
        }

        public T Load<T>( string key) where T : Object
        {
            var assetPath = Path.Combine(RootPath,key);
            assetPath.Replace("\\", "/");
            if (assetsMap.ContainsKey(assetPath))
            {
                return assetsMap[assetPath] as T;
            }

            T obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            assetsMap[assetPath] = obj;
            return obj;
        }


        public Object Load(string res, string key)
        {
            var assetPath = Path.Combine(RootPath, res, key);
            assetPath.Replace("\\", "/");
            if (assetsMap.ContainsKey(assetPath))
            {
                return assetsMap[assetPath];
            }

            Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            assetsMap[assetPath] = obj;
            return obj;
        }

        public void Unload(string res, bool unload = false)
        {
        }

        public void UnloadAll(bool unload = false)
        {
            assetsMap.Clear();
            Resources.UnloadUnusedAssets();
        }

        public bool Contains(string res, string key)
        {
            var assetPath = Path.Combine(RootPath, res, key);
            assetPath.Replace("\\", "/");
            if (assetsMap.ContainsKey(assetPath))
            {
                return true;
            }

            var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            if (obj == null)
            {
                return false;
            }
            return true;
        }

        public void LoadAsync<T>(string res, string key, Action<T> action) where T : Object
        {
            var obj = Load<T>(res, key);
            action?.Invoke(obj);
        }

        public void LoadAsync(string res, string key, Action<Object> action)
        {
            var obj = Load(res, key);
            action?.Invoke(obj);
        }

        public void LoadAllAsync(string res, Action<Object[]> action)
        {
            action(null);
        }

        public Object[] LoadAll(string res)
        {
            return null;
        }

        public void CancelLoadAsync()
        {
            
        }

        public void LoadDependencieMF(string name)
        {

        }

        public void CleanDependencieMF()
        {
        }
    }
}
#endif