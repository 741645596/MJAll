using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using WLCore.Coroutine;
/// <summary>
/// Download task.
/// </summary>
public class XZTask
{
    public event Action<string, XZStates> OnCompleted;
    public event Action<long, long> OnProcesses;

    private const string DownloadCoroutineProxy_Key = "DownloadCoroutineProxy";
    private Coroutine _coroutine;
    private UnityWebRequest _webRequest;
    private CustomXZHandler _fileHandler;
    private string _url;
    private string _filePath;

    public void DownloadFile(string url, string filePath)
    {
        _url = url;
        _filePath = filePath;
        var coroutineProxy = CoroutineManager.GetCoroutineProxy(DownloadCoroutineProxy_Key);
        _coroutine = coroutineProxy.StartCoroutine(_DownloadFile(url, filePath));
    }

    /// <summary>
    /// 获取存储的完整路径，如/xx/xx/xx.ab
    /// </summary>
    /// <returns></returns>
    public string GetFilePath()
    {
        return _filePath;
    }

    /// <summary>
    /// 获取下载的地址
    /// </summary>
    /// <returns></returns>
    public string GetUrl()
    {
        return _url;
    }

    /// <summary>
    /// Cancels the download.
    /// </summary>
    public void CancelDownload()
    {
        if (_webRequest != null)
        {
            _webRequest.Abort();
            _webRequest = null;
        }

        if (_coroutine != null)
        {
            var coroutineProxy = CoroutineManager.GetCoroutineProxy(DownloadCoroutineProxy_Key);
            coroutineProxy.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (_fileHandler != null)
        {
            _fileHandler.CancelDispose();
            _fileHandler = null;
        }
    }

    /// <summary>
    /// Downloads the file.
    /// </summary>
    /// <returns>The file.</returns>
    /// <param name="url">URL.</param>
    /// <param name="filePath">File path.</param>
    public IEnumerator _DownloadFile(string url, string filePath)
    {
        _webRequest = UnityWebRequest.Get(url);
        _webRequest.disposeDownloadHandlerOnDispose = true;

        _fileHandler = new CustomXZHandler(filePath);
        _fileHandler.eventProgress += OnProcesses;
        _fileHandler.eventComplete += OnCompleted;

        _webRequest.SetRequestHeader("Range", "bytes=" + _fileHandler.downloadedFileLen + "-");
        _webRequest.downloadHandler = _fileHandler;

        yield return _webRequest.SendWebRequest();

        var result = _webRequest.result;
        if (result == UnityWebRequest.Result.ConnectionError
            || result == UnityWebRequest.Result.ProtocolError
            || result == UnityWebRequest.Result.DataProcessingError)
        {
            WLDebug.Log(string.Format("【提示】加载文件{0}失败，失败原因：{1}", _fileHandler.fileName, _webRequest.error));
            _fileHandler.ErrorDispose();
        }

        _webRequest = null;
        _coroutine = null;
        _fileHandler = null;
    }
}
