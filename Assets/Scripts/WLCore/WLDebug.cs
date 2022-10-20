// Log.cs
// Author: shihongyang <shihongyang@weile.com>
// Data: 2019/5/15

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using System;

/// <summary>
/// 大于等于当前等级的则显示
/// </summary>
public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error,
    None,
}

public enum LogColor
{
    green,
    black,
    blue,
    cyan,
    white,
    yellow,
    red
}

namespace Unity.Utility
{
    // 方便调用，不再使用命名空间
    // 留着兼容外部引用不报错
}

public static class WLDebug
{
    public static LogLevel LogLevel = LogLevel.Debug;

    /// <summary>
    /// 是否输出log信息，正式版本建议关闭，默认可以
    /// </summary>
    /// <param name="b"></param>
    public static void LogEnable(bool b)
    {
        UnityEngine.Debug.unityLogger.logEnabled = b;
        LogLevel = b ? LogLevel.Debug : LogLevel.None;
    }

    public static void Log(params object[] args)
    {
        if (LogLevel > LogLevel.Debug)
        {
            return;
        }

        string info = FormatString(args);
        UnityEngine.Debug.Log(info);
    }

    public static void Info(params object[] args)
    {
        if (LogLevel > LogLevel.Info)
        {
            return;
        }

        string info = FormatString(args);
        UnityEngine.Debug.Log(info);
    }

    public static void LogWarning(params object[] args)
    {
        if (LogLevel > LogLevel.Warn)
        {
            return;
        }

        string info = FormatString(args);
        UnityEngine.Debug.LogWarning("警告：" + info);
    }

    public static void LogError(params object[] args)
    {
        if (LogLevel > LogLevel.Error)
        {
            return;
        }

        // LogError在真机会崩溃，所以用LogWarning
        string info = FormatString(args);
        UnityEngine.Debug.LogWarning(info);
    }

    public static void StackTrace()
    {
        StackTrace st = new StackTrace(true);
        Log(st.ToString());
    }

    /// <summary>
    /// 输出对象（自定义的）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="color"></param>
    public static void LogObject<T>(T content, LogColor color = LogColor.black)
    {
        if (LogLevel > LogLevel.Debug)
        {
            return;
        }
        UnityEngine.Debug.Log(Message(typeof(T).Name, color, true) + "\n" + ObjectMessage(content));
    }

    /// <summary>
    /// 输出List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    public static void LogList<T>(List<T> content, LogColor color = LogColor.black)
    {
        if (LogLevel > LogLevel.Debug || content == null || content.Count <= 0)
        {
            return;
        }

        var result = Title(typeof(T).Name + " List", 0, color) + "\n\n";
        for (int i = 0; i < content.Count; i++)
        {
            result += Title("index: " + i, 1, color) + "\n";
            result += content[i] + "\n";
        }
        result += Title("End", 0, color);

        UnityEngine.Debug.Log(result);
    }

    /// <summary>
    /// 输出数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    public static void LogArray<T>(T[] content, LogColor color = LogColor.black)
    {
        if (LogLevel > LogLevel.Debug || content == null || content.Length <= 0)
        {
            return;
        }

        var result = Title(typeof(T).Name + " List", 0, color) + "\n";
        for (int i = 0; i < content.Length; i++)
        {
            result += Title("index: " + i, 1, color) + "\n";
            result += content[i] + "\n";
        }
        result += Title("End", 0, color);

        UnityEngine.Debug.Log(result);
    }

    /// <summary>
    /// 输出Dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    public static void LogDictionary<T, Y>(Dictionary<T, Y> content)
    {
        if (LogLevel > LogLevel.Debug || content == null || content.Count <= 0)
        {
            return;
        }

        var result = Title(typeof(T).Name + " Dictionary", 0, LogColor.green) + "\n\n";
        foreach (var key in content.Keys)
        {
            result += Title("key: " + key, 1, LogColor.green) + "\n";
            result += content[key] + "\n";
        }
        result += Title("End", 0, LogColor.green);
        UnityEngine.Debug.Log(result);
    }

    /// <summary>
    /// 清除unity控制台log信息
    /// </summary>
    public static void ClearLogConsole()
    {
#if UNITY_EDITOR
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        var method = logEntries.GetMethod("Clear");
        method?.Invoke(new object(), null);
#endif
    }

    private static string FormatString(params object[] args)
    {
        string info = "";
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == null)
            {
                info += "null";
            }
            else
            {
                info += args[i].ToString();
            }
            info += "\t";
        }
        return info;
    }

    private static string Message(object message, LogColor color = LogColor.black, bool bold = false, bool italic = false)
    {
        var content = string.Format("<color={0}>{1}</color>", color.ToString(), message);
        content = bold ? string.Format("<b>{0}</b>", content) : content;
        content = italic ? string.Format("<i>{0}</i>", content) : content;

        return content;
    }

    private static string Title(object title, int level, LogColor color)
    {
        return Message(string.Format("{0}{1}{2}{1}", Space(level * 2), Character(16 - level * 2), title), color);
    }

    private static object Character(int number)
    {
        var result = "";
        for (int i = 0; i < number; i++)
        {
            result += "=";
        }
        return result;
    }

    private static string ObjectMessage<T>(T content)
    {
        var result = "";

        var fields = typeof(T).GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            result += Space(4) + Message(fields[i].Name + " : ", LogColor.green, true) + fields[i].GetValue(content) + "\n";
        }

        var properties = typeof(T).GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            result += Space(4) + Message(properties[i].Name + " : ", LogColor.green, true) + properties[i].GetValue(content, null) + "\n";
        }

        return result;
    }

    private static string Space(int number)
    {
        var space = "";
        for (int i = 0; i < number; i++)
        {
            space += " ";
        }
        return space;
    }

}
