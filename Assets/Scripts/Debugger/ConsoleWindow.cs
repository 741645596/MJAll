// ConsoleWindow.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/12
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class ConsoleWindow
{
    private readonly Queue<LogInfo> logInfoQueue = new Queue<LogInfo>();
    private LogInfo selectedNodeInfo = null;
    private bool isLockScroll = true;

    private bool infoFilter = true;
    private bool warningFilter = true;
    private bool errorFilter = true;
    private Vector2 logScrollPosition = Vector2.zero;
    private Vector2 stackScrollPosition = Vector2.zero;

    private readonly DebugWindow debugger;

    public int MaxLine { get; set; } = 1000;

    public ConsoleWindow(DebugWindow debugger)
    {
        this.debugger = debugger;
    }

    public void OnLogMessageReceived(string logMessage, string stackTrace, LogType logType)
    {
        if (logType == LogType.Assert)
        {
            logType = LogType.Error;
        }

        var node = new LogInfo(logType, logMessage, stackTrace);
        logInfoQueue.Enqueue(node);

        while (logInfoQueue.Count > MaxLine)
        {
            logInfoQueue.Dequeue();
        }
    }

    private void Clear()
    {
        logInfoQueue.Clear();
    }

    public void Draw()
    {
        GUI.DragWindow(new Rect(0, 0, float.MaxValue, 0));
        GUI.skin.verticalScrollbar.fixedWidth = 50;
        GUI.skin.verticalScrollbarThumb.fixedWidth = 50;
        GUI.skin.horizontalScrollbar.fixedHeight = 50;
        GUI.skin.horizontalScrollbarThumb.fixedHeight = 50;
        GUI.skin.toggle.fixedHeight = 50;
        GUI.skin.toggle.fixedWidth = 50;
        DrawMenu();
        DrawLog();
        DrawStack();
    }

    private void DrawStack()
    {
        GUILayout.BeginVertical("box");
        {
            stackScrollPosition = GUILayout.BeginScrollView(stackScrollPosition, GUILayout.Height(150));
            {
                if (selectedNodeInfo != null)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.fontSize = 23;
                    GUILayout.BeginHorizontal();
                    Color32 color = Color.black;
                    GUILayout.Label(selectedNodeInfo.LogMessage, style);
                    if (GUILayout.Button("复制", GUILayout.Width(60f), GUILayout.Height(30f)))
                    {
                        TextEditor textEditor = new TextEditor
                        {
                            text = selectedNodeInfo.LogMessage + "\n\n" + selectedNodeInfo.StackTrack
                        };
                        textEditor.OnFocus();
                        textEditor.Copy();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Label(selectedNodeInfo.StackTrack, style);
                }
                GUILayout.EndScrollView();
            }
        }
        GUILayout.EndVertical();
    }

    private void DrawLog()
    {
        GUILayout.BeginVertical("box");
        {
            if (isLockScroll)
            {
                logScrollPosition.y = float.MaxValue;
            }

            logScrollPosition = GUILayout.BeginScrollView(logScrollPosition);
            {
                bool selected = false;
                foreach (LogInfo logNode in logInfoQueue)
                {
                    switch (logNode.LogType)
                    {
                        case LogType.Log:
                            if (!infoFilter)
                            {
                                continue;
                            }
                            break;

                        case LogType.Warning:
                            if (!warningFilter)
                            {
                                continue;
                            }
                            break;

                        case LogType.Error:
                            if (!errorFilter)
                            {
                                continue;
                            }
                            break;
                    }
                    GUIStyle style = new GUIStyle();
                    style.fontSize = 23;
                    if (GUILayout.Toggle(selectedNodeInfo == logNode, GetLogString(logNode), style))
                    {
                        selected = true;
                        if (selectedNodeInfo != logNode)
                        {
                            selectedNodeInfo = logNode;
                            stackScrollPosition = Vector2.zero;
                        }
                    }
                }
                if (!selected)
                {
                    selectedNodeInfo = null;
                }
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
    }

    private void DrawMenu()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("关闭", GUILayout.Width(100f), GUILayout.Height(100f)))
            {
                debugger.CloseConsoleWindow();
            }

            if (GUILayout.Button("清空日志", GUILayout.Width(100f), GUILayout.Height(100f)))
            {
                Clear();
            }

            isLockScroll = GUILayout.Toggle(isLockScroll, "滚动", GUILayout.Width(100f), GUILayout.Height(100f));
            GUILayout.FlexibleSpace();
            infoFilter = GUILayout.Toggle(infoFilter, "信息", GUILayout.Width(100f), GUILayout.Height(100f));
            warningFilter = GUILayout.Toggle(warningFilter, "警告", GUILayout.Width(100f), GUILayout.Height(100f));
            errorFilter = GUILayout.Toggle(errorFilter, "错误", GUILayout.Width(100f), GUILayout.Height(100f));
        }
        GUILayout.EndHorizontal();
    }


    private string GetLogString(LogInfo logNode)
    {
        Color32 color = GetLogStringColor(logNode.LogType);
        var sb = new StringBuilder(1024);
        sb.AppendFormat("<color=#{0}{1}{2}{3}>{4}{5}</color>",
            color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"),
            logNode.LogTime.ToString("[HH:mm:ss.fff] "), logNode.LogMessage);
        return sb.ToString();
    }

    private Color32 GetLogStringColor(LogType logType)
    {
        Color32 color = Color.white;
        switch (logType)
        {
            case LogType.Log:
                color = Color.white;
                break;

            case LogType.Warning:
                color = Color.yellow;
                break;

            case LogType.Error:
                color = Color.red;
                break;
        }

        return color;
    }
}