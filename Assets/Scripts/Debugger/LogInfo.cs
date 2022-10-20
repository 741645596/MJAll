// LogInfo.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/12
using System;
using UnityEngine;

public class LogInfo
{
    public DateTime LogTime;
    public LogType LogType;
    public string LogMessage;
    public string StackTrack;

    public LogInfo(LogType logType, string logMessage, string stackTrack)
    {
        LogTime = DateTime.Now;
        LogType = logType;
        LogMessage = logMessage;
        StackTrack = stackTrack;
    }
}
