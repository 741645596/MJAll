// @Author: tanjinhua
// @Date: 2021/4/24  23:07


using System.IO;
using Unity.Utility;
using UnityEngine;

[AddComponentMenu("UI/Remote Image")]
public class RemoteImage : SKImage
{
    [SerializeField]
    private string _url;
    public string url
    {
        get => _url;
        set
        {
            if (_url == value)
            {
                return;
            }

            _url = value;

            Download();
        }
    }

    [SerializeField]
    public bool autoUpdateSize = false;
    
    private XZTask _downloadTask;
    private bool _isFinished = true;

    protected override void Start()
    {
        base.Start();

        Download();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (!_isFinished)
        {
            CancelDownload();
        }
    }

    private void Download()
    {
        CancelDownload();

        if (string.IsNullOrEmpty(_url))
        {
            return;
        }

        _isFinished = false;
        _downloadTask = XZUtils.DownloadFile(_url, OnDownload);
    }

    private void OnDownload(string filePath, XZStates states)
    {
        if (states != XZStates.SUCCESS)
        {
            WLDebug.Log("【提示】：头像不存在url=", _url);
            _isFinished = true;
            _downloadTask = null;
            return;
        }

        try
        {
            byte[] data = File.ReadAllBytes(filePath);
            var format = SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_4x4) ? TextureFormat.ASTC_4x4 : TextureFormat.RGBA32;
            Texture2D texture = new Texture2D(2, 2, format, false, false);
            texture.LoadImage(data);
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            sprite = Sprite.Create(texture, rect, (transform as RectTransform).pivot);

            if (autoUpdateSize)
            {
                SetNativeSize();
            }

            _isFinished = true;
            _downloadTask = null;
        }
        catch
        {
            WLDebug.LogWarning("异常错误：RemoteImage.OnDownload: 文件读取失败");
        }
    }

    private void CancelDownload()
    {
        XZUtils.CancelDownload(_downloadTask);
        _downloadTask = null;
        _isFinished = true;
    }
}
