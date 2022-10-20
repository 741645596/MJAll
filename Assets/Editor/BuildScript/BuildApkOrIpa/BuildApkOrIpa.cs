using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using WLCore.Helper;

/// <summary>
/// 构建APK
/// </summary>
public class BuildApkOrIpa : Editor
{
    public static void BuildForAndroid()
    {
        //获取Python传过来的要调用Unity的类名和方法名
        string   command  = "BuildApkOrIpa.BuildForAndroid";
        string   apkPath  = BatchCommandHelp.GetArgs(command)[0];         //参数1打包平台选择，例如android
        DateTime dt       = DateTime.Now;                                 //获取当前时间
        string   dateTime = string.Format("{0:yyyyMMdd-HHmmssffff}", dt); //20170122-1603353262
        string   allPath  = apkPath + "UnityRuntime" + dateTime + ".apk";
        if (apkPath == "")
        {
            WLDebug.LogError("接收不到python传递给unity的参数");
            return;
        }

        BuildPipeline.BuildPlayer(GetBuildScenes(), allPath, BuildTarget.Android, BuildOptions.None);
    }

    /// <summary>
    /// 在这里找出你当前工程所有的场景文件，
    /// 假设你只想把部分的scene文件打包
    /// 那么这里可以写你的条件判断 总之返回一个字符串数组。
    /// </summary>
    /// <returns></returns>
    private static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }

        return names.ToArray();
    }
}
