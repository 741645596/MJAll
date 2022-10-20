using System.IO;
using Unity.Utility;
using UnityEngine;

namespace WLCore.Helper
{
    /// <summary>
    /// 路径帮助类
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// GameAssets 用于存放ab包的根目录路径
        /// </summary>
        public static readonly string s_gameAssetsPath = Application.dataPath + "/GameAssets";

        /// <summary>
        /// streamingAssetsPath 的路径
        /// </summary>
        public static readonly string s_streamingAssetsPath = Application.dataPath + "/StreamingAssets";

        /// <summary>
        /// ILRuntime的 CLR Binding 生成路径
        /// </summary>
        public static readonly string s_CLRPath = Application.dataPath + "/ILRuntime/Generated";

        /// <summary>
        /// 获得第一个分隔符“/”后的路径
        /// </summary>
        /// <param name="path"> 【例：Assets/Editor/BuildScript/ScriptReload.cs】 </param>
        /// <returns> 【例：Editor/BuildScript/ScriptReload.cs】 </returns>
        public static string GetSecondPath(string path)
        {
            var index = path.IndexOf('/');
            return path.Substring(index + 1);
        }

        /// <summary>
        /// 获得第一个分隔符“/”前的路径
        /// </summary>
        /// <param name="path"> 原路径【例：Assets/GameAssets/Common】 </param>
        /// <returns> 【例：Assets】 </returns>
        public static string GetFirstPath(string path)
        {
            var index = path.IndexOf('/');
            return path.Substring(0, index);
        }

        /// <summary>
        /// 路径格式化 【"\\"或"//"转成 "/"】
        /// </summary>
        /// <param name="path"> 任意样式原路径【例：\\Users\\futianfu\\UnityRuntime//Assets】 </param>
        /// <returns> 返回路径只包含"/"【例：/Users/futianfu/UnityRuntime/Assets】 </returns>
        public static string PathFormat(string path)
        {
            if (path.Contains("\\"))
            {
                return path.Replace("\\", "/");
            }
            else if (path.Contains("//"))
            {
                return path.Replace("//", "/");
            }
            return path;
        }

        /// <summary>
        /// AssetBundles 的路径【项目之外缓存ab的路径】
        /// </summary>
        /// <returns> 【例如：/Users/futianfu/UnityRuntime/AssetBundles】 </returns>
        public static string AssetBundlesPath()
        {
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(Application.dataPath);
            DirectoryInfo projectDirectoryInfo = new DirectoryInfo(rootDirectoryInfo.Parent.FullName);
            return projectDirectoryInfo + "/AssetBundles";
        }

        /// <summary>
        /// Assets的上一层文件夹路径
        /// </summary>
        /// <returns> 【例如：/Users/futianfu/UnityRuntime】 </returns>
        public static string AssetsParentPath()
        {
            DirectoryInfo rootDirectoryInfo    = new DirectoryInfo(Application.dataPath);
            DirectoryInfo projectDirectoryInfo = new DirectoryInfo(rootDirectoryInfo.Parent.FullName);
            return projectDirectoryInfo.ToString();
        }

        /// <summary>
        /// 获得模块路径
        /// </summary>
        /// <param name="str"> 全路径 【例如：/Users/futianfu/UnityRuntime/Assets/GameAssets/Common/Module1】 </param>
        /// <returns> 【例如：/Common/Module1】 </returns>
        public static string GetModelPath(string str)
        {
            bool b = str.Length < Application.dataPath.Length + "/GameAssets".Length;
            if (b)
            {
                WLDebug.LogError("GetModelPath(:", str, ")", "路径不在GameAssets内或为空");
                return null;
            }

            str = PathHelper.PathFormat(str);
            return str.Substring(Application.dataPath.Length + "/GameAssets".Length);
        }

        /// <summary>
        /// 获得SteamingAsset下的相对路径
        /// </summary>
        /// <param name="str"> 全路径 【例如：/Users/futianfu/UnityRuntime/Assets/StreamingAssets/common/module1】 </param>
        /// <returns> 【例如：common/module1】 </returns>
        public static string InSteamingAssetPath(string str)
        {
            bool b = str.Length < Application.streamingAssetsPath.Length;
            if (b)
            {
                WLDebug.LogError("InSteamingAssetPath(:", str, ")", "路径不在StreamingAssets内或为空");
                return null;
            }

            str = PathHelper.PathFormat(str);
            return str.Substring(Application.streamingAssetsPath.Length + "/".Length);
        }

        /// <summary>
        /// 获得项目路径
        /// </summary>
        /// <param name="str"> 全路径 【例如：/Users/futianfu/UnityRuntime/Assets/GameAssets】 </param>
        /// <returns> 【例如：/Assets/GameAssets】 </returns>
        public static string GetProjectPath(string str)
        {
            bool b = str.Length < Application.dataPath.Length;
            if (b)
            {
                WLDebug.LogError("GetProjectPath(:", str, ")", "路径不在Assets内或为空");
                return null;
            }

            return "Assets" + str.Substring(Application.dataPath.Length);
        }

        /// <summary>
        /// 获得硬盘路径，不同设备路径不同
        /// </summary>
        /// <param name="str"> 全路径 </param>
        /// <returns> 【例如：/Users/futianfu/Library/Application Support/DefaultCompany/UnityRuntime】 </returns>
        public static string GetHardDiskPath(string str)
        {
            return "";
        }
    }
}
