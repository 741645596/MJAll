/// <summary>
/// chenzhiwei@weile.com
/// 调试日志管理对象
/// <summary>

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace WLCore
{
    // 需要的话放到命名空间下
}

public enum eDebugTraceLevel
{
    debug       = 0,
    info        = 1,
    warning     = 2,
    error       = 3,
    system      = 10,
    close       = 100           // 完全关闭
}

public static class WLDebugTrace
{
    public static bool traceEnable = false;                      // 是否开启DebugTrace的日志功能
    public static bool errorToConsole = false;                  // 是否将错误日志也输出到 Editor console
    public static eDebugTraceLevel level = eDebugTraceLevel.debug;
    public static Action<string> sendToServerCallback;          // 后续有必要的话将错误日志上发到服务器, 方便定位问题
    public static Action<string> NSLogOnIOSCallback;            // IOS平台下，支持将日志直接写入到系统日志中

    public static string _logPath;                              // 日志路径 dir/xxx.txt
    private static readonly int _maxFileSize = 1024 * 1024;     // 控制单个日志文件大小
    private static bool _bCheckLogSize = true;                  // 是否将过大日志分成小文件
    private static bool _bDispose = true;
    private static Dictionary<string, bool> _dictErrorLog = new Dictionary<string, bool>();     // 控制错误日志的输出频率, 错误日志只记录一次

    // 线程相关
    private static string _writeToLocalStr = "";
    private static Thread _debugTraceThread;
    private static object _threadLock = new object();
    private static AutoResetEvent reset = new AutoResetEvent(false);

    /// <summary>
    /// 日志初始化
    /// </summary>
    public static void Init()
    {
        _bDispose = false;

        WLDebugTrace.errorToConsole = true;
        string logDir = Application.persistentDataPath;
#if UNITY_EDITOR
        logDir = System.Environment.CurrentDirectory + "/Logs";
#endif
        logDir += "/WLLog.txt";
        WLDebugTrace.SetLogPath(logDir);

        // 记录游戏开始日志
        WLDebugTrace.TraceSystem("------------------- GameStart " + System.DateTime.Now.ToString("f") + " -------------------");
        WLDebugTrace.TraceSystem("Application.persistentDataPath = " + Application.persistentDataPath);
        WLDebugTrace.TraceSystem("Application.streamingAssetsPath = " + Application.streamingAssetsPath);
    }

    /// <summary>
    /// GameEntry destroy 时调用, 一定要退出所有自建线程
    /// </summary>
    public static void Dispose()
    {
        try
        {
            _bDispose = true;
            if (_debugTraceThread != null)
                _debugTraceThread.Abort();
            _debugTraceThread = null;
        }
        catch {}
    }

    public static void SetLogPath(string logPath)
    {
        _logPath = logPath.Replace("\\", "/");
        _logPath = logPath.Replace("//", "/");
    }

    public static void TraceInfo(string msg)
    {
        Trace(msg, eDebugTraceLevel.info, false);
    }

    public static void TraceError(string msg)
    {
        Trace(msg, eDebugTraceLevel.error, true);
    }

    public static void TraceSystem(string msg)
    {
        Trace(msg, eDebugTraceLevel.system, false);
    }

    public static void Trace(string msg, eDebugTraceLevel logLevel = eDebugTraceLevel.debug, bool errorLog = false)
    {
        if (_bDispose)
            return;

        if (logLevel < level)
            return;

        if (string.IsNullOrEmpty(msg))
            return;

        try
        {
            if (errorLog)
            {
                if (_dictErrorLog.ContainsKey(msg))   // 错误日志只记录一遍
                    return;

                _dictErrorLog.Add(msg, true);

                if (errorToConsole)
                    Debug.LogError(msg);

                if (sendToServerCallback != null)
                    sendToServerCallback(msg);
            }

            // 异步写入文件
            writeToLocalAsync(msg);
            if (NSLogOnIOSCallback != null)
                NSLogOnIOSCallback(msg);
        }
        catch {}
    }

    /// <summary>
    /// 日志写入到本地采用线程处理，避免主线程卡顿
    /// </summary>
    /// <param name="msg"></param>
    private static void writeToLocalAsync(string msg)
    {
        if (_debugTraceThread == null)
        {
            _debugTraceThread = new Thread(new ThreadStart(writeLogThread));
            _debugTraceThread.Start();
        }

        lock (_threadLock)
        {
            _writeToLocalStr += msg + "\r\n";   // 添加到local写入
            reset.Set();
        }
    }
    
    private static void writeLogThread()
    {
        while (true)
        {
            reset.WaitOne();
            lock (_threadLock)
            {
                if (!string.IsNullOrEmpty(_writeToLocalStr))
                {
                    writeToLocalImpl(_writeToLocalStr);
                    _writeToLocalStr = "";
                }
            }
        }
    }

    private static void writeToLocalImpl(string msg)
    {
        if (string.IsNullOrEmpty(_logPath))
            return;

        DateTime dateTime = DateTime.Now;
        msg = "[" + dateTime.ToString() + "]" + msg;

        StreamWriter w;
        if (_bCheckLogSize)
            checkLogSize();

        if (!File.Exists(_logPath))
        {
            w = File.CreateText(_logPath);
            w.Close();
        }

        File.AppendAllText(_logPath, msg);
    }
    
    private static void checkLogSize()
    {
        try
        {
            if (!File.Exists(_logPath))
                return;

            FileStream file = File.Open(_logPath, FileMode.Open);
            if (file.Length < _maxFileSize)
            {
                file.Close();
                return;
            }

            file.Close();

            int s = _logPath.LastIndexOf('/') + 1;
            int e = _logPath.LastIndexOf('.');
            if (e > s)
            {
                string dir = _logPath.Substring(0, s);
                string backupPath = _logPath.Substring(s, e - s);
                backupPath = dir + backupPath + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                File.Copy(_logPath, backupPath, true);
                File.Delete(_logPath);
            }
        }
        catch {}
    }
}
