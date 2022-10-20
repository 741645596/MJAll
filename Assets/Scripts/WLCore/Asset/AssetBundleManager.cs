// AssetBundleManager.cs
// Author: shihongyang shihongyang@weile.com；futianfu
// Date: 2019/6/24
//Edit:


using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WLCore.Helper;

namespace Unity.Core
{
    /// <summary>
    /// AB包处理
    /// </summary>
    public class AssetBundleManager : IAssetManager
    {
        private static string s_assetsPrefix = "assets/gameassets/";

        // 存储了所有AB包的依赖的Manifest文件
        private Dictionary<string, AssetBundle>         _abs;
        private Dictionary<string, AssetBundleManifest> _manifests;
        private string                                  _downLoadPath;
        private string                                  _localPath;
        private bool                                    _cancel;

        public AssetBundleManager()
        {
            _abs          = new Dictionary<string, AssetBundle>();
            _manifests    = new Dictionary<string, AssetBundleManifest>();
            _downLoadPath = Application.persistentDataPath;  // 下载路径
            _localPath    = Application.streamingAssetsPath; // 本地路径

            _cancel = false;
        }

        /// <summary>
        /// 加载模块总依赖文件，读ab包前请先加载依赖清单文件
        /// </summary>
        /// <param name="name"> 模块名称，如common </param>
        public void LoadDependencieMF(string name)
        {
            var moduleName = name.ToLower();
            if (_manifests.ContainsKey(moduleName))
            {
                return;
            }

            //加载清单文件对应的Ab包
            var moduleAbPath = _GetModlePath(moduleName);
            AssetBundle single = AssetBundle.LoadFromFile(moduleAbPath);
            if (single == null)
            {
                WLDebug.LogWarning("加载ab清单失败，请检查资源，目录为： ", moduleAbPath);
                return;
            }

            //加载清单文件
            var singleManifest = single.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (singleManifest == null)
            {
                WLDebug.LogWarning("加载AB失败，请检查是否存在资源清单，资源目录为： " , moduleAbPath);
                return;
            }

            _manifests[moduleName] = singleManifest;
            single.Unload(false);
        }

        /// <summary>
        /// 热更新记得清除一下在加载新的依赖清单
        /// </summary>
        public void CleanDependencieMF()
        {
            _manifests.Clear();
        }

        /// <summary>
        /// 清除指定模块的依赖清单
        /// </summary>
        /// <param name="name"> 模块名，如common </param>
        public void CleanDependencieMF(string name)
        {
            var moduleName = name.ToLower();
            if (_manifests.ContainsKey(moduleName))
            {
                _manifests.Remove(moduleName);
            }
        }

        /// <summary>
        /// 加载AB包
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public AssetBundle LoadAB(string assetPath)
        {
            string lowerPath = assetPath.ToLower();
            return _GetAssetBundle(lowerPath);
        }

        /// <summary>
        ///  加载ab包内文件
        /// </summary>
        /// <param name="assetPath">ab包路径【例子：assetName="Common/Module1/prefabs"】</param>
        /// <param name="filePath">ab内部资源路径（查看manifest）【例子：filePath="TestPrefab.prefab"】</param>
        /// <returns></returns>
        public UnityEngine.Object Load(string assetPath, string filePath)
        {
            return Load<UnityEngine.Object>(assetPath, filePath);
        }

        /// <summary>
        /// 加载ab包内指定类型文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">ab包路径【例子：assetName="Common/Module1/prefabs"】</param>
        /// <param name="filePath">ab内部资源路径（查看manifest）【例子：filePath="TestPrefab.prefab"】</param>
        /// <returns></returns>
        public T Load<T>(string assetPath, string filePath) where T : UnityEngine.Object
        {
            string lower = assetPath.ToLower();
            AssetBundle ab = _GetAssetBundle(lower);
            if (ab == null)
            {
                WLDebug.Log("AssetBundle加载不到请检测ab包路径（文件夹路径+名称）!");
                return null;
            }

            var keyName = Path.Combine(s_assetsPrefix, lower, filePath);
            return ab.LoadAsset<T>(keyName);
        }

        /// <summary>
        /// 异步加载ab包
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">ab包路径【例子：assetName="Common/Module1/prefabs"】</param>
        /// <param name="filePath">ab内部资源路径（查看manifest）【例子：filePath="TestPrefab.prefab"】</param>
        /// <param name="action">加载成功回调</param>
        public void LoadAsync<T>(string assetPath, string filePath, Action<T> action) where T : UnityEngine.Object
        {
            string lower  = assetPath.ToLower();
            AssetBundle ab = _GetAssetBundle(lower);
            if (ab == null)
            {
                action?.Invoke(null);
                return;
            }

            _cancel = false;
            var keyName = Path.Combine(s_assetsPrefix, lower, filePath);
            var req     = ab.LoadAssetAsync(keyName);
            req.completed += (o) =>
            {
                if (_cancel)
                {
                    return;
                }

                action?.Invoke(((AssetBundleRequest)o).asset as T);
            };
        }

        /// <summary>
        /// 异步加载ab包
        /// </summary>
        /// <param name="assetPath">ab包路径【例子：assetName="Common/Module1/prefabs"】</param>
        /// <param name="filePath">ab内部资源路径（查看manifest）【例子：filePath="TestPrefab.prefab"】</param>
        /// <param name="action">加载成功回调</param>
        public void LoadAsync(string assetPath, string filePath, Action<UnityEngine.Object> action)
        {
            LoadAsync<UnityEngine.Object>(assetPath, filePath, action);
        }

        /// <summary>
        /// 异步加载AssetBundle中所有的Assets资源
        /// </summary>
        /// <param name="assetPath"> ab路径 【例如：Common/Module1/prefabs】</param>
        /// <param name="action"> 加载成功回调 </param>
        public void LoadAllAsync(string assetPath, Action<UnityEngine.Object[]> action)
        {
            string lower = assetPath.ToLower();
            AssetBundle ab = _GetAssetBundle(lower);
            if (ab == null)
            {
                action?.Invoke(null);
                return;
            }

            _cancel = false;
            var req = ab.LoadAllAssetsAsync();
            req.completed += (o) =>
            {
                if (_cancel)
                {
                    return;
                }

                action?.Invoke(((AssetBundleRequest)o).allAssets);
            };
        }

        /// <summary>
        /// 同步加载AssetBundle中所有的Assets资源
        /// </summary>
        /// <param name="assetPath"> ab路径 【例如：Common/Module1/prefabs】 </param>
        /// <returns></returns>
        public UnityEngine.Object[] LoadAll(string assetPath)
        {
            string lower = assetPath.ToLower();
            AssetBundle ab = _GetAssetBundle(lower);
            if (ab == null)
            {
                return null;
            }

            return ab.LoadAllAssets();
        }

        /// <summary>
        /// 卸载ab
        /// </summary>
        /// <param name="path">ab包路径</param>
        /// <param name="unload">是否卸载实例化的资源</param>
        public void Unload(string path, bool unload = false)
        {
            if (_abs.ContainsKey(path))
            {
                _abs[path].Unload(unload);
                _abs.Remove(path);
            }
        }

        /// <summary>
        /// 卸载所以的ab包
        /// </summary>
        /// <param name="unload">true时场景中已经实例化的资源也会被卸载</param>
        public void UnloadAll(bool unload = false)
        {
            _abs.Clear();
            AssetBundle.UnloadAllAssetBundles(unload);

            //WLDebug.Log("AssetBundleManager UnloadAll !!!! ", unload);
        }

        /// <summary>
        /// 取消异步加载ab包
        /// </summary>
        public void CancelLoadAsync()
        {
            _cancel = true;
        }

        /// <summary>
        /// 是否存在ab
        /// </summary>
        /// <param name="assetName">ab路径</param>
        /// <param name="filePath">ab内部资源路径</param>
        /// <returns></returns>
        public bool Contains(string assetName, string filePath)
        {
            string lower = assetName.ToLower();
            AssetBundle ab = _GetAssetBundle(lower);
            if (ab == null)
            {
                return false;
            }

            string assetKey = Path.Combine(s_assetsPrefix, lower, filePath);
            return ab.Contains(assetKey);
        }

        private string _GetModlePath(string modleName)
        {
            // 优先读下载路径
            string downLoadModuleAbPath = $"{_downLoadPath}/{modleName}/{modleName}";
            if (File.Exists(downLoadModuleAbPath))
            {
                return downLoadModuleAbPath;
            }

            return $"{_localPath}/{modleName}/{modleName}";
        }

        /// <summary>
        /// 获得ab包
        /// </summary>
        /// <param name="assetname"> ab包路径【例子：assetName="Common/Module1/prefabs"】 </param>
        /// <param name="path"> streamingAssetsPath路径【例如：/Users/futianfu/UnityRuntime/Assets/StreamingAssets/Common/Module1/prefabs】 </param>
        /// <returns></returns>
        private AssetBundle _GetAssetBundle(string abPath)
        {
            // 特殊处理：由于依赖清单的依赖关系不包含最外层模块名，因此将 common/module1/demo_atlas 转成 abName = "module1/demo_atlas
            string abName = PathHelper.GetSecondPath(abPath);
            if (_abs.ContainsKey(abName))
            {
                return _abs[abName];
            }

            return _LoadAssetbundle(abPath, abName);
        }

        /// <summary>
        /// 加载Ab包，同时加载依赖Ab包
        /// </summary>
        /// <param name="moduleName"> ab包路径【例子：moduleName="Common"】 </param>
        /// <param name="abName"> ab包路径【例子：abName="Common/Module1/prefabs"】 </param>
        /// <returns></returns>
        private AssetBundle _LoadAssetbundle(string abPath, string abName)
        {
            string moduleName = PathHelper.GetFirstPath(abPath);
            _LoadDependencies(moduleName, abName);

            Unload(abName); // 确保asset首次加载

            string abFullPath = _FindAssetFullPath(abPath);
            AssetBundle asset = AssetBundle.LoadFromFile(abFullPath);
            if (asset != null)
            {
                _abs.Add(abName, asset);
            }

            return asset;
        }

        //查找完依赖后，把依赖的所有ab包都存着
        /// <summary>
        /// 加载依赖Ab包
        /// </summary>
        /// <param name="moduleName"> ab包路径【例子：moduleName="Common"】 </param>
        /// <param name="abName"> ab包路径【例子：abName="Common/Module1/prefabs"】 </param>
        private void _LoadDependencies(string moduleName, string abName)
        {
            // 收集依赖信息
            var deps = _CollectDependencies(moduleName, abName);
            foreach (var item in deps)
            {
                string name = item.Key;
                if (_abs.ContainsKey(name))
                {
                    continue;
                }

                string path = item.Value;
                //WLDebug.Log("=========== path = ", path);
                AssetBundle asset = AssetBundle.LoadFromFile(path);
                if (asset != null)
                {
                    _abs.Add(name, asset);
                }
            }
        }

        /// <summary>
        /// 收集多个manifest内的所有依赖Ab包路径
        /// </summary>
        /// <param name="moduleName"> ab包路径【例子：moduleName="Common"】 </param>
        /// <param name="abName"> ab包路径【例子：abName="Common/Module1/prefabs"】 </param>
        /// <returns> depdic[key = "module1/demo_atlas"] = "/Users/futianfu/UnityRuntime/Assets/StreamingAssets/module1/demo_atlas" </returns>
        private Dictionary<string, string> _CollectDependencies(string moduleName, string abName)
        {
            var depdic = new Dictionary<string, string>();
            foreach (var item in _manifests)
            {
                //加载一个manifest内的所有依赖Ab包
                _GetInManifestAllAb(moduleName, abName, item, depdic);
            }

            return depdic;
        }

        /// <summary>
        /// 收集一个manifest内的所有依赖Ab包路径
        /// </summary>
        /// <param name="moduleName"> ab包路径【例子：moduleName="Common"】 </param>
        /// <param name="abName"> ab包路径【例子：abName="Common/Module1/prefabs"】 </param>
        /// <param name="item"> 通过这个manifest字典查找依赖 </param>
        /// <param name="depdic"> 返回那些依赖Ab包名和路径
        /// depdic[depName = "module2/test_images"] = "/Users/futianfu/Library/Application Support/DefaultCompany/UnityRuntime/common/module2/test_images" </param>
        private void _GetInManifestAllAb(string moduleName,
            string abName,
            KeyValuePair<string, AssetBundleManifest> item,
            Dictionary<string, string> depdic)
        {
            //循环加载依赖项
            string[] depNames = item.Value.GetAllDependencies(abName);
            foreach (var depName in depNames)
            {
                if (depdic.ContainsKey(depName))
                {
                    continue;
                }

                string combine = Path.Combine(moduleName, depName);
                string depPath = _FindAssetFullPath(combine);
                depdic.Add(depName, depPath);
            }
        }

        /// <summary>
        /// 检测本地是否有这个文件
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private string _FindAssetFullPath(string res)
        {
            string p = Path.Combine(_downLoadPath, res);
            if (File.Exists(p))
            {
                return p;
            }

            return Path.Combine(_localPath, res);
        }
    }
}
