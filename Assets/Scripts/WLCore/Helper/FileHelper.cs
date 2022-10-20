using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using WLCore.Helper;

namespace Unity.Utility
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 获得某种后缀名的一组文件 【递归】【通用】
        /// </summary>
        /// <param name="path"> 要查找文件夹路径 </param>
        /// <param name="extensionName"> 后缀【例:"*.mat"】 </param>
        /// <returns> 返回文件路径列表 </returns>
        public  static List<string> GetExtensionFiles(string path, string extensionName)
        {
            string[] files = Directory.GetFiles(path, extensionName, SearchOption.AllDirectories);
            return files.ToList();
        }

        /// <summary>
        /// 获得某种后缀名的一组文件【通用】
        /// </summary>
        /// <param name="fileSystemInfo"> 全路径集合【例： {D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets,...}】 </param>
        /// <param name="extension"> 后缀【例：.meta】 </param>
        /// <returns> 返回一组带后缀的文件索引 </returns>
        public static List<string> GetExtensionFile(List<string> fileSystemInfo, string extension)
        {
            List<string> systemInfos = new List<string>();
            foreach (var item in fileSystemInfo)
            {
                if (IsExtension(item, extension))
                {
                    systemInfos.Add(item);
                }
            }

            return systemInfos;
        }

        /// <summary>
        /// 过滤掉文件夹，只留文件返回【通用】
        /// </summary>
        /// <param name="inFileSystemInfo"> 全路径集合【例： {D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets,...}】 </param>
        /// <returns> 只返回文件路径集合【例： {D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/....png,...}】 </returns>
        public static List<string> GetOnlyFile(List<string> inFileSystemInfo)
        {
            List<string> fileSystemInfos = new List<string>();
            foreach (var item in inFileSystemInfo)
            {
                if (IsFile(item))
                {
                    fileSystemInfos.Add(item);
                }
            }

            return fileSystemInfos;
        }

        /// <summary>
        /// 获得当前层级文件夹【不含子目录的文件夹】【通用】
        /// </summary>
        /// <param name="path"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        /// <returns> 返回当前层级内的文件夹路径【例： {D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets,...}】 </returns>
        public static List<string> GetFolder(string path)
        {
            List<string> list = new List<string>();

            DirectoryInfo dir = new DirectoryInfo(path);
            //获取到指定目录下的所有文件以及子文件夹
            DirectoryInfo[] files = dir.GetDirectories();

            foreach (var item in files)
            {
                list.Add(item.FullName);
            }

            return list;
        }

        /// <summary>
        /// 获得当前层级文件【不含子目录文件】【通用】
        /// </summary>
        /// <param name="path"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        /// <returns> 返回当前层级内的文件路径集合【例： {D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/....png,...}】 </returns>
        public static List<string> GetFile(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                FileInfo[]    files         = directoryInfo.GetFiles();
                List<string>  list          = new List<string>();
                foreach (var item in files)
                {
                    list.Add(item.FullName);
                }

                return list;
            }

            return null;
        }

        /// <summary>
        /// 获得路径下的所有文件【包含子目录的文件】【通用】
        /// </summary>
        /// <param name="path"> 从这级路径开始遍历【例如:】 </param>
        /// <param name="pattern"> 获得哪种后缀类型的文件【例如jpg,】 </param>
        /// <returns>  </returns>
        public static List<string> GetAllFile(string path, string pattern)
        {
            if (pattern.Length == 0)
            {
                pattern = "*";
            }
            else
            {
                pattern = $"*.{pattern}";
            }

            string[] files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
            return  files.ToList();
        }

        /// <summary>
        ///【包含子目录的文件】【递归查找】【返回指定类型文件】【通用】
        /// </summary>
        /// <param name="path"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        /// <param name="onlyType"> 只取某种类型文件，如果取全部类型用"*"【例："*"或"*.jpg"】 </param>
        /// <returns> 返回完整路径 </returns>
        public static List<string> GetAllFileAndFolder(string path, string onlyType)
        {
            string[]     allFile = Directory.GetFiles(path, onlyType, SearchOption.AllDirectories);
            List<string> list    = new List<string>(); //返回完整路径
            foreach (var item in allFile)
            {
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// 【包含子目录的文件和文件夹】【递归查找】【通用】
        /// </summary>
        /// <param name="path"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        /// <returns> 返回所有文件和文件夹路径集合【例： {D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/....png,...}】 </returns>
        public static List<string> GetAllFileAndFolder(string path)
        {
            //递归查找

            List<FileSystemInfo> childFiles = new List<FileSystemInfo>(); //这个必须写在方法外，
            childFiles.Clear();
            //子模块内的模块路径【例如：folder = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】
            GetAllFileAndFolder(path, ref childFiles);

            List<string> list = new List<string>();
            list.Clear();
            foreach (var item in childFiles)
            {
                list.Add(item.FullName);
            }

            childFiles.Clear();
            return list;
        }

        /// <summary>
        /// 文件的后缀名是否包含指定字符串【通用】
        /// </summary>
        /// <param name="name"> 文件名 【例如:test.png】 </param>
        /// <param name="extension"> 后缀名 【例如:".meta"】 </param>
        /// <returns> true表示是包含后缀 </returns>
        public static bool IsExtension(string path, string extension)
        {
            return path.EndsWith(extension, System.StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 获得文件后缀【通用】
        /// </summary>
        /// <param name="filePath"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        /// <returns> 返回后缀名带"." 例如：.png </returns>
        public static string GetExtension(string filePath)
        {
            return Path.GetExtension(PathHelper.PathFormat(filePath));
        }

        /// <summary>
        /// 是否是文件【通用】
        /// </summary>
        /// <param name="fileSystemInfo"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        /// <returns> true表示是文件 </returns>
        public static bool IsFile(string fileSystemInfo)
        {
            return File.Exists(fileSystemInfo);
        }

        /// <summary>
        /// 文件夹是否存在【通用】
        /// </summary>
        /// <param name="path"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        /// <returns> true表示存在 </returns>
        public static bool IsDirectory(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// 创建文件夹【通用】
        /// </summary>
        /// <param name="path"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        public static void CreateDirectory(string path)
        {
            if (IsDirectory(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 先删除再创建文件夹【通用】
        /// </summary>
        /// <param name="path"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        public static void DelCreateDirectory(string path)
        {
            if (IsDirectory(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 拷贝【通用】
        /// </summary>
        /// <param name="currentPath"> 当前路径 </param>
        /// <param name="targetPath"> 目标路径 </param>
        public static void Copy(string currentPath, string targetPath)
        {
            File.Copy(currentPath, targetPath, true);
        }

        /// <summary>
        /// 删除文件【通用】
        /// </summary>
        /// <param name="path"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 删除文件夹【通用】
        /// </summary>
        /// <param name="path"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        public static void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                directoryInfo.Delete(true);
            }
        }

        /// <summary>
        /// 清除文件和文件夹【通用】
        /// </summary>
        /// <param name="dir"> 全路径【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
        public static void CleanDirectory(string dir)
        {
            foreach (string subdir in Directory.GetDirectories(dir))
            {
                Directory.Delete(subdir, true);
            }

            foreach (string subFile in Directory.GetFiles(dir))
            {
                File.Delete(subFile);
            }
        }

        /// <summary>
        /// 拷贝【通用】
        /// </summary>
        /// <param name="srcDir"> 当前路径 </param>
        /// <param name="tgtDir"> 目标路径 </param>
        public static void CopyDirectory(string srcDir, string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();
            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
            }
        }

        /// <summary>
        /// 查找路径下的文件夹名否在存是小写【通用】
        /// </summary>
        /// <param name="path"> 要查询文件夹的路径【例如:D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common/Module1/】 </param>
        /// <returns> true表示存在小写 </returns>
        public static bool IsDirLower(string path)
        {
            //获取路径下的文件夹的路径
            //子模块内的模块路径【例：item = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common/Module1/demo_atlas】
            List<string> cModelFold = GetFolder(path);
            //遍历文件内的路径
            foreach (var item in cModelFold)
            {
                //每个路径的文件夹名进行对比自身小写文件夹名，如果相同返回true
                if (Path.GetFileName(item) != Path.GetFileName(item).ToLower())
                {
                    Debug.LogError("存在Ab包名称为大写的文件夹，请改成小写:" + item);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 重命名文件【只支持Unity工程内改名】【通用】
        /// </summary>
        /// <param name="projectPath"> 请传Unity工程路径 【例如：Assets/StreamingAssets/Common/Module1/demo_atlas_ab.ab】</param>
        /// <param name="newName"> 新名字 </param>
        public static void ReName(string projectPath, string newName)
        {
            // 新文件名
            string newStr = Path.GetDirectoryName(projectPath) + newName;

            // 改名方法
            FileInfo fi = new FileInfo(projectPath);
            fi.MoveTo(Path.Combine(newStr));
        }

        /// <summary>
        /// 可以用于，移动文件【通用】
        /// </summary>
        /// <param name="projectPath"> 请传Unity工程路径 【例如：Assets/StreamingAssets/Common/Module1/demo_atlas_ab.ab】</param>
        /// <param name="newPath"> 新名字 </param>
        public static void MoveFile(string projectPath, string newPath)
        {
            FileInfo fi = new FileInfo(projectPath);
            fi.MoveTo(newPath);
        }

        /// <summary>
        /// 读取文件【读取 TXT】【通用】
        /// </summary>
        /// <param name="filepathIncludingFileName"> 要读取文件路径【例如：Assets/StreamingAssets/Common/Module1/demo_atlas_ab.txt】 </param>
        /// <returns> 返回读取的内容集合 </returns>
        public static List<string> ReadFile(string filepathIncludingFileName)
        {
            var          sr    = File.OpenText(filepathIncludingFileName);
            List<string> list  = new List<string>();
            var          input = "";

            while (true)
            {
                input = sr.ReadLine();

                if (input == null)
                {
                    break;
                }

                list.Add(input);
                Debug.Log("line=" + input);
            }

            sr.Close();
            return list;
        }

        /// <summary>
        /// 写入文件【例如写入TxT内容】【通用】
        /// </summary>
        /// <param name="filepathIncludingFileName"> 要写入文件路径 【例如：Assets/StreamingAssets/Common/Module1/demo_atlas_ab.txt】 </param>
        /// <param name="content"> 写入的内容 </param>
        public static void WriteFile(string filepathIncludingFileName, List<string> content)
        {
            StreamWriter fileWriter = File.CreateText(filepathIncludingFileName);

            foreach (var item in content)
            {
                fileWriter.WriteLine(item);
            }

            fileWriter.Close();
        }

        /// <summary>
        /// 是否忽略以下文件".meta;.DS_Store;.git;.gitignore;CSProject~;"【非通用】
        /// </summary>
        /// <param name="childFiles"> 全路径集合【例： {D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets,...}】 </param>
        /// <returns> 返回过滤后的文件路径集合 </returns>
        public static List<string> GetIgnore(List<string> childFiles)
        {
            List<string> filteredFiles = new List<string>();
            filteredFiles.Clear();
            foreach (var file in childFiles)
            {
                bool isGitignoreFile = !IsExtension(file, ".meta")
                                       && !IsExtension(file, ".DS_Store")
                                       && !IsExtension(file, ".git")
                                       && !IsExtension(file, ".gitignore")
                                       && !file.Contains("README")
                                       && !file.Contains("readme")
                                       && !file.Contains("CSProject~")
                                       && Path.GetFileName(file) != ("CSProject~");
                if (isGitignoreFile)
                {
                    filteredFiles.Add(file);
                }
            }

            return filteredFiles;
        }

        /// <summary>
        /// 文件夹内 是否不为空
        /// </summary>
        /// <param name="path"> 文件夹路径 </param>
        /// <returns> 是否不为空 </returns>
        public static bool IsDirectoryNull(string path)
        {
            if (!Directory.Exists(path))
            {
                return true; // 找不着路径也为空
            }

            if (Directory.GetFileSystemEntries(path).Length == 0) // 返回当前目录子文件和子目录的数量
            {
                return true; // 为空
            }

            return false; // 不为空
        }

        //私有函数 /********************** 华丽分割线 **************************/

        /// <summary>
        /// 获得所有文件和文件夹【包含子目录的文件和文件夹】【递归查找】
        /// </summary>
        /// <param name="path"> 【例： D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】  </param>
        /// <param name="fileSystemInfos"> 需要外部new一个索引 </param>
        /// <returns> fileSystemInfos = 返回获得的文件和文件夹索引 </returns>
        private static void GetAllFileAndFolder(string path, ref List<FileSystemInfo> fileSystemInfos)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                fileSystemInfos.Add(fi); //添加获取文件名
            }

            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                fileSystemInfos.Add(d);                               //添加获取文件
                GetAllFileAndFolder(d.FullName, ref fileSystemInfos); //递归获取子文件等
            }
        }
    }
}
