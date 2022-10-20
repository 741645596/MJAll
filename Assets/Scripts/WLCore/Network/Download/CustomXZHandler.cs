using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CustomXZHandler : DownloadHandlerScript
{
    public long totalFileLen { get; private set; }
    public long downloadedFileLen { get; private set; }
    public string fileName { get; private set; }
    public string dirPath { get; private set; }

    private string _savePath;
    private string _tempSavePath;
    private FileStream _fs;

    #region 事件
    /// <summary>
    /// 返回这条URL下需要下载的文件的总大小
    /// </summary>
    public event Action<long> eventTotalLength = null;

    /// <summary>
    /// 返回这次请求时需要下载的大小（即剩余文件大小）
    /// </summary>
    public event Action<long> eventContentLength = null;

    /// <summary>
    /// 每次下载到数据后回调进度
    /// </summary>
    public event Action<long, long> eventProgress = null;

    /// <summary>
    /// 当下载完成后回调下载的文件位置
    /// </summary>
    public event Action<string, XZStates> eventComplete = null;
    #endregion

    /// <summary>
    /// 初始化下载句柄，定义每次下载的数据上限为200kb
    /// </summary>
    /// <param name="filePath">保存到本地的文件路径</param>
    public CustomXZHandler(string filePath) : base(new byte[1024 * 200])
    {
        _savePath = filePath;
        fileName = Path.GetFileName(_savePath);
        dirPath = Path.GetDirectoryName(_savePath);
        _tempSavePath = Path.Combine(dirPath, fileName + ".temp");
        totalFileLen = -1;

        try
        {
            // 确保文件夹存在
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            _fs = new FileStream(_tempSavePath, FileMode.Append, FileAccess.Write);
            downloadedFileLen = _fs.Length;
        }
        catch (Exception ex)
        {
            WLDebug.LogWarning(ex.ToString());
        }
    }

    /// <summary>
    /// 请求下载时的第一个回调函数，会返回需要接收的文件总长度
    /// </summary>
    /// <param name="contentLength">如果是续传，则是剩下的文件大小；本地拷贝则是文件总长度</param>
    [ObsoleteAttribute]
    protected override void ReceiveContentLength(int contentLength)
    {
        if (contentLength == 0)
        {
            WLDebug.Log("【加载已经完成】");
            CompleteContent();
            return;
        }

        // 苹果手机这个回调会调用多次
        if (totalFileLen == -1)
        {
            totalFileLen = contentLength + downloadedFileLen;

            eventTotalLength?.Invoke(totalFileLen);
            eventContentLength?.Invoke(contentLength);
        }
    }

    /// <summary>
    /// 从网络获取数据时候的回调，每帧调用一次
    /// </summary>
    /// <param name="data">接收到的数据字节流，总长度为构造函数定义的200kb，并非所有的数据都是新的</param>
    /// <param name="dataLength">接收到的数据长度，表示data字节流数组中有多少数据是新接收到的，即0-dataLength之间的数据是刚接收到的</param>
    /// <returns>返回true为继续下载，返回false为中断下载</returns>
    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (data == null || data.Length == 0)
        {
            WLDebug.Log(string.Format("【加载中】<color=yellow>加载文件{0}中，没有获取到数据，加载终止</color>", fileName));
            return false;
        }
        _fs?.Write(data, 0, dataLength);
        downloadedFileLen += dataLength;

        eventProgress?.Invoke(downloadedFileLen, totalFileLen);

        return true;
    }

    /// <summary>
    /// 当接受数据完成时的回调
    /// </summary>
    protected override void CompleteContent()
    {
        if(_fs == null)
        {
            WLDebug.LogError("错误提示：文件句柄已删除");
            return;
        }

        // 文件大小不一致
        if (_fs.Length != totalFileLen)
        {
            ErrorDispose(true);
            return;
        }

        // 关闭句柄
        _CloseFileHandle();

        if (File.Exists(_tempSavePath))
        {
            if (File.Exists(_savePath))
                File.Delete(_savePath);
            File.Move(_tempSavePath, _savePath);

            eventComplete?.Invoke(_savePath, XZStates.SUCCESS);
        }
        else
        {
            WLDebug.LogWarning(string.Format("【下载失败】<color=red>下载文件{0}时失败</color>", fileName));

            eventComplete?.Invoke(_savePath, XZStates.Fail);
        }
        Dispose();
    }

    public void ErrorDispose(bool isDelete = false)
    {
        _CloseFileHandle();

        // 下载失败暂不删除缓存文件
        if (isDelete && File.Exists(_tempSavePath))
        {
            File.Delete(_tempSavePath);
        }
        eventComplete?.Invoke(_savePath, XZStates.Fail);

        Dispose();
    }

    public void CancelDispose()
    {
        _CloseFileHandle();

        eventComplete?.Invoke(_savePath, XZStates.Cancel);
        Dispose();
    }

    private void _CloseFileHandle()
    {
        if (_fs != null)
        {
            _fs.Close();
            _fs.Dispose();
            _fs = null;
        }
    }
}
