using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unity.Core
{
    public class LoadDll
    {
        /// <summary>
        /// 加载DLL
        /// </summary>
        /// <param name="modules">dll文件名数组</param>
        /// <param name="callback">完成回调</param>
        /// <param name="isReload">是否重写加载</param>
        public static void Load(List<string> modules, Action<bool> callback, bool isReload = false)
        {
#if UNITY_EDITOR
            LoadFromEditor(modules, callback, isReload);
            AssetsManager.LoadDependencieMFs(modules);
#else
            LoadFromApp(modules, callback, isReload);
            AssetsManager.LoadDependencieMFs(modules);
#endif
        }

        /// <summary>
        /// 重新加载dll，一般用在热更新的dll后重新加载
        /// </summary>
        /// <param name="module"></param>
        /// <param name="callback"> 加载成功回调，可以为null </param>
        public static void Reload(string module, Action<bool> callback)
        {
            Load(new List<string>() { module}, callback, true);
        }

        private static void LoadFromEditor(List<string> modules, Action<bool> callback, bool isReload)
        {
            // 编辑器模式下，只识别本地路径，并且加载pdb用于调试
            var count = modules.Count;
            string[] dllfiles = new string[count];
            string[] pdbfiles = new string[count];
            for (int i = 0; i < count; i++)
            {
                var moduleName = modules[i].ToLower(); // 文件名好dll名统一小写
                var root = Path.Combine(Application.streamingAssetsPath, moduleName);
                var dllPath = Path.Combine(root, $"{moduleName}.dll");
                var pbdPath = Path.Combine(root, $"{moduleName}.pdb");

                var dllUri = new Uri(dllPath);
                dllfiles[i] = dllUri.AbsoluteUri;

                if (File.Exists(pbdPath))
                {
                    var pbdUri = new Uri(pbdPath);
                    pdbfiles[i] = pbdUri.AbsoluteUri;
                    continue;
                }
                pdbfiles[i] = null;
            }
            AppDomainManager.LoadAssemblies(dllfiles, pdbfiles, callback, isReload);
        }

        private static void LoadFromApp(List<string> modules, Action<bool> callback, bool isReload)
        {
            // App模式先读下载路径，然后在读本地路径
            var count = modules.Count;
            string[] dllfiles = new string[count];
            for (int i = 0; i < count; i++)
            {
                var moduleName = modules[i].ToLower(); // 文件名好dll名统一小写
                var root = Path.Combine(Application.persistentDataPath, moduleName);
                var dllPath = Path.Combine(root, $"{moduleName}.dll");
                if (!File.Exists(dllPath))
                {
                    var downPath = Path.Combine(Application.streamingAssetsPath, moduleName);
                    dllPath = Path.Combine(downPath, $"{moduleName}.dll");
                }

                var dllUri = new Uri(dllPath);
                dllfiles[i] = dllUri.AbsoluteUri;
            }
            AppDomainManager.LoadAssemblies(dllfiles, null, callback, isReload);
        }
    }

    
}
