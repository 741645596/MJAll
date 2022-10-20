using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LitJson;
using NUnit.Framework;
using UnityEditor;
using WLCore.Helper;

/// <summary>
/// 打包安卓和IOS资源包
/// </summary>
public class CommandTool : Editor
{
    // 更新资源
    public static void RefreshAssets()
    {
        AssetDatabase.Refresh();
    }

    // 打包资源
    public static void PackAssets()
    {
        //获取Python传过来的要调用Unity的类名和方法名
        string command     = "CommandTool.PackAssets";
        string platform    = BatchCommandHelp.GetArgs(command)[0]; //参数1打包平台选择，例如android
        var    buildTarget = GetBuildTarget(platform);             //解析参数参数1
        var buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression |
                                      BuildAssetBundleOptions.DeterministicAssetBundle;
        if (platform == "")
        {
            WLDebug.LogError("接收不到python传递给unity的参数");
            return;
        }

        //打资源包
        //TODO：加一个打包哪个模块判定的数组,现在默认打包全部模块
        PackAssetBundles.Build(buildTarget, buildAssetBundleOptions, "全平台", false, 1);
    }

    // 打包更新包 TODO:这样无法选择打包模块，还是需要把打包模块写到配置文件中，然后python读取后通过shell命令呼叫unity方法传参进来
    public static void PackHotUpdatePackage()
    {
        //获取Python传过来的要调用Unity的类名和方法名
        string command = "CommandTool.PackHotUpdatePackage";
        //去读外部传来的参数
        string   platform      = BatchCommandHelp.GetArgs(command)[0]; //参数1打包平台选择，例如android
        string   selectModule  = BatchCommandHelp.GetArgs(command)[1];
        string   strVersion    = BatchCommandHelp.GetArgs(command)[2];
        var      buildTarget   = GetBuildTarget(platform); // 打包在哪种平台上
        string[] selectModules = StringHelper.Split(selectModule, ",");
        int      version       = int.Parse(strVersion);
        string   dllDir        = "Assets/StreamingAssets/";
        var buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression |
                                      BuildAssetBundleOptions.DeterministicAssetBundle; //打包选项
        if (platform == "" || selectModule == "" || strVersion == "")
        {
            WLDebug.LogError("接收不到python传递给unity的参数");
            return;
        }

        //打包Ab包
        PackAssetBundles.MultiModuleBuildHotUpdate(buildTarget, buildAssetBundleOptions, selectModules.ToList(),
            version);
        // const string MODUEL_NAME = "xlhz";
        // PackAssetBundles.Build(buildTarget, buildAssetBundleOptions, MODUEL_NAME, true, isSeperateAssets, false, dllDir, true, version);
    }

    /// <summary>
    /// 获得打包平台类型
    /// </summary>
    /// <param name="platform">例:android</param>
    /// <returns></returns>
    private static BuildTarget GetBuildTarget(string platform)
    {
        platform = platform.ToLower();
        if (platform == "windows")
        {
            return BuildTarget.StandaloneWindows;
        }
        else if (platform == "osx")
        {
            return BuildTarget.StandaloneOSX;
        }
        else if (platform == "android")
        {
            return BuildTarget.Android;
        }
        else if (platform == "ios")
        {
            return BuildTarget.iOS;
        }

        return BuildTarget.Android;
    }
}
