using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Unity.Utility;
using WLCore.Coroutine;
using WLCore.Helper;

public enum XZStates
{
    SUCCESS,
    Fail,
    Cancel
}

/// <summary>
/// 添加了缓存机制的下载工具类
/// </summary>
public static class XZUtils
{
    private static Dictionary<string, XZTask> processingTasks = new Dictionary<string, XZTask>();

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="originUrl"></param>
    /// <param name="finishCallback"></param>
    /// <returns></returns>
    public static XZTask DownloadFile(string originUrl,
        Action<string, XZStates>             finishCallback)
    {
        return DownloadFile(originUrl, finishCallback, null);
    }

    /// <summary>
    /// 下载文件，新增进度回调
    /// </summary>
    /// <param name="originUrl"></param>
    /// <param name="finishCallback"></param>
    /// <param name="progressCallback"> 回调参数意思：（ 当前下载大小（单位B）, 总的文件大小 ） </param>
    /// <returns></returns>
    public static XZTask DownloadFile(string originUrl,
        Action<string, XZStates> finishCallback,
        Action<long, long> progressCallback)
    {
        // 本地已存在，直接返回
        string fileName = MD5Helper.GetMD5(originUrl);
        string filePath = CacheHelper.GetFullPath(fileName);
        if (CacheHelper.IsFileExist(fileName))
        {
            finishCallback.Invoke(filePath, XZStates.SUCCESS);
            return null;
        }

        // 正在下载中...
        if (processingTasks.ContainsKey(filePath))
        {
            WLDebug.LogWarning("提示：已经在下载了，请检查是否重复调用下载...url=", originUrl);
            return null;
        }

        XZTask downloadTask = new XZTask();
        processingTasks.Add(filePath, downloadTask);
        downloadTask.OnCompleted += finishCallback;
        downloadTask.OnCompleted += OnCompleted;
        if (progressCallback != null)
        {
            downloadTask.OnProcesses += progressCallback;
        }
        downloadTask.DownloadFile(originUrl, filePath);
        return downloadTask;
    }

    /// <summary>
    /// 指定文件下载路径，本地存在也会下载
    /// </summary>
    /// <param name="originUrl"> 请求下载的路径 </param>
    /// <param name="filePath"> 要保存的完整路径 </param>
    /// <param name="finishCallback"> 下载结束的回调函数 </param>
    /// <param name="progressCallback"> 回调参数意思：（ 当前进度（0.0-1.0）, 当前下载大小（单位B）, 总的文件大小 ） </param>
    /// <returns></returns>
    public static XZTask DownloadFile(string originUrl,
        string filePath,
        Action<string, XZStates> finishCallback,
        Action<long, long> progressCallback)
    {
        // 正在下载中...
        filePath = filePath.Replace('\\', '/');
        if (processingTasks.ContainsKey(filePath))
        {
            WLDebug.LogWarning("提示：已经在下载了，请检查是否重复调用下载...url=", originUrl);
            return null;
        }

        XZTask downloadTask = new XZTask();
        processingTasks.Add(filePath, downloadTask);
        downloadTask.OnCompleted += finishCallback;
        downloadTask.OnCompleted += OnCompleted;
        if (progressCallback != null)
        {
            downloadTask.OnProcesses += progressCallback;
        }
        downloadTask.DownloadFile(originUrl, filePath);
        return downloadTask;
    }

    /// <summary>
    /// 取消下载，参数为上面函数的返回值
    /// </summary>
    /// <param name="downloadTask"></param>
    public static void CancelDownload(XZTask downloadTask)
    {
        string filePath = downloadTask.GetFilePath();
        CancelDownload(filePath);
    }

    /// <summary>
    /// 通过下载的文件路径取消下载
    /// </summary>
    /// <param name="filePath"></param>
    public static void CancelDownload(string filePath)
    {
        if (processingTasks.ContainsKey(filePath))
        {
            var task = processingTasks[filePath];
            task.OnCompleted -= OnCompleted;
            task.CancelDownload();
            processingTasks.Remove(filePath);
        }
    }

    private static void OnCompleted(string filePath, XZStates states)
    {
        if (processingTasks.ContainsKey(filePath))
        {
            processingTasks[filePath].OnCompleted -= OnCompleted;
            processingTasks.Remove(filePath);
        }
    }
}
