using System;
using System.IO;
using UnityEngine;

namespace WLCore.Helper
{
    public static class CacheHelper
    {
        /// <summary>
        /// 获取缓存文件夹路径，文件夹不存在会自动创建
        /// </summary>
        /// <returns></returns>
        public static string GetCacheDirectory()
        {
            string path = _GetCacheDir();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// 缓存路径下fileName文件是否存在
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileExist(string fileName)
        {
            return File.Exists(GetFullPath(fileName));
        }

        /// <summary>
        /// 获取缓存文件夹下的fileName文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFullPath(string fileName)
        {
            var path = Path.Combine(GetCacheDirectory(), fileName);
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// 删除缓存文件夹
        /// </summary>
        public static void Clear()
        {
            try
            {
                var p = _GetCacheDir();
                if (Directory.Exists(p))
                {
                    Directory.Delete(p, true);
                    Directory.CreateDirectory(p);
                }
            }
            catch(Exception e)
            {
                WLDebug.LogWarning(string.Format("删除临时文件失败，异常：{0}", e.Message.ToString()));
            }
        }

        private static string _GetCacheDir()
        {
            return Path.Combine(Application.persistentDataPath, "__cache");
        }
    }

}