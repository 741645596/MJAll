// ReloadHelper.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/30

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 用于热更工程dll运行时重载，如在运行时，测试工程代码修改编译完成后
/// 将自动热重启无需再点击播放器重新播放
/// 非测试用例由于关联场景数据较多占时不支持热重启
/// </summary>
[InitializeOnLoad]
public static class ReloadHelper
{
    private static FileSystemWatcher s_watcher;
    private static bool s_isReloading = false;
    private static readonly ConcurrentQueue<Action> s_queue = new ConcurrentQueue<Action>();

    static ReloadHelper()
    {
        EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
        EditorApplication.update += OnEditorUpdate;
    }

    /// <summary>
    /// Update事件
    /// </summary>
    private static void OnEditorUpdate()
    {
        if (s_queue.IsEmpty)
        {
            return;
        }

        if (s_queue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// 播放模式改变时调用
    /// </summary>
    /// <param name="playingState"></param>
    private static void OnEditorPlayModeStateChanged(PlayModeStateChange playingState)
    {
        switch (playingState)
        {
            // 进入播放模式
            case PlayModeStateChange.EnteredPlayMode:
                WatcherStart();
                SetAutoRefresh(false);
                break;

            // 退出播放模式
            case PlayModeStateChange.EnteredEditMode:
                SetAutoRefresh(true);
                WatchStop();
                break;

            case PlayModeStateChange.ExitingPlayMode:
            case PlayModeStateChange.ExitingEditMode:
            default:
                break;
        }
    }

    /// <summary>
    /// 设置编辑器是否自动编译
    /// </summary>
    /// <param name="b"></param>
    private static void SetAutoRefresh(bool b)
    {
        if (EditorPrefs.HasKey("kAutoRefresh"))
        {
            EditorPrefs.SetBool("kAutoRefresh", b);
        }
    }

    /// <summary>
    /// 初始化监听
    /// </summary>
    private static void WatcherStart()
    {
        WLDebug.Log("本地热更开启监听：WatcherStart");

        s_watcher = new FileSystemWatcher();
        s_watcher.BeginInit();
        s_watcher.Filter = "*.dll";
        s_watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.Size;
        s_watcher.Path = Application.streamingAssetsPath + "/";
        s_watcher.IncludeSubdirectories = true;
        s_watcher.Changed += WatchChanged;
        s_watcher.Created += WatchChanged;
        s_watcher.EnableRaisingEvents = true;
        s_watcher.EndInit();
    }

    /// <summary>
    /// 停止监听
    /// </summary>
    private static void WatchStop()
    {
        WLDebug.Log("本地热更停止监听：WatchStop");
        if (s_watcher != null)
        {
            s_watcher.EnableRaisingEvents = false;
            s_watcher.Changed -= WatchChanged;
            s_watcher.Dispose();
            s_watcher = null;
        }
    }

    /// <summary>
    /// 文件变化事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void WatchChanged(object sender, FileSystemEventArgs e)
    {
        s_queue.Enqueue(Reload);
    }

    /// <summary>
    /// 重载
    /// </summary>
    private static void Reload()
    {
        AsyncReload();
    }

    /// <summary>
    /// 延时0.1s调用，防止多次Restart
    /// </summary>
    private static async void AsyncReload()
    {
        if (s_isReloading)
        {
            return;
        }

        WLDebug.Log("本地热更重新加载：Reload");
        var gameEntry = _GetGameEntry();
        if (gameEntry != null)
        {
            gameEntry.Restart();
        }
        s_isReloading = true;

        // 防止调用多次
        await Task.Delay(TimeSpan.FromSeconds(0.5f));
        s_isReloading = false;
    }

    /// <summary>
    /// 查找场景里的 GameEntry 游戏入口
    /// </summary>
    private static GameEntry _GetGameEntry()
    {
        var go = GameObject.Find("GameEntry");
        if (go == null)
        {
            return null;
        }

        return go.GetComponent<GameEntry>();
    }
}