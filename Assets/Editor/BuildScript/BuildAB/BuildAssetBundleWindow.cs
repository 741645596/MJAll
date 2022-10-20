using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using UnityEditor;
using UnityEngine;
using Unity.Utility;
using WLCore.Helper;


/// <summary>
/// 打ab包Editor界面
/// </summary>
public class BuildAssetBundleWindow : EditorWindow
{
    private static BuildTarget s_buildTarget = BuildTarget.Android; // 这里要使用static 窗口关闭后才会记得上一次选中哪个

    private BuildAssetBundleOptions _buildAssetBundleOptions =
        BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle; //

    private static int          s_selectModuleIndex = 0;                        // 这里要使用static 窗口关闭后才会记得上一次选中哪个
    private static List<string> _selectModuleName   = new List<string> {"全平台"}; //【例如：{"全平台"，"Common"}】

    [MenuItem("Tools/打ab包工具")]
    public static void ShowWindow()
    {
        var window = GetWindow<BuildAssetBundleWindow>();
        window.minSize = new Vector2(400, 300);
        window.maxSize = new Vector2(400, 300);
        s_buildTarget  = EditorUserBuildSettings.activeBuildTarget; // 获取当前平台
        _ScanningFile(_selectModuleName);
        window.Show();
    }

    /// <summary>
    /// GUI
    /// </summary>
    private void OnGUI()
    {
        s_buildTarget       = (BuildTarget)EditorGUILayout.EnumPopup("目标平台：", s_buildTarget); // 选择目标平台下拉窗
        s_selectModuleIndex = EditorGUILayout.Popup("选择打包模块(默认全部)：", s_selectModuleIndex, _selectModuleName.ToArray());

        _BuildAB(); // 开始打AB包逻辑

        _BuildHotUpdate(); // 点击"热更包"逻辑
    }

    /// <summary>
    /// 开始打AB包逻辑
    /// </summary>
    private void _BuildAB()
    {
        if (GUILayout.Button("开始打包"))
        {
            WLDebug.Log("开始打包");
            PackAssetBundles.Build(s_buildTarget, _buildAssetBundleOptions, _selectModuleName[s_selectModuleIndex],
                true, 1);
        }
    }

    /// <summary>
    /// 点击"热更包"逻辑
    /// </summary>
    private void _BuildHotUpdate()
    {
        // 若要开启需要unity 支持调用Dll打包功能
        if (GUILayout.Button("热更包"))
        {
            WLDebug.Log("点击 热更包 按钮");

            if (!_IsGeneratedDLL(_selectModuleName[s_selectModuleIndex])) // 是否找到当前模块需要的DLL
            {
                WLDebug.LogError(
                    "错误提示：检测Assets/StreamingAssets目录中未包含选中模块的DLL.请通过VisuolStudio生成DLL");
                return; // 找不到直接 报错 跳出
            }

            if (!_IsGeneratedCLR()) // 是否已经生成CLR绑定
            {
                WLDebug.LogError(
                    "错误提示：检测Assets/ILRuntime/Generated目录未空.请通过菜单栏：ILRuntime -> Generate CLR Binding Code生成绑定");
                return; // 没生成直接 报错 跳出
            }

            _BuildHotUpdateCore(); // 打热更包核心
        }
    }

    /// <summary>
    /// 是否找到当前模块需要的DLL，
    ///
    /// 如果没找到DLL返回false，
    /// </summary>
    /// <param name="selectModule"> 选择要打包的模块 </param>
    /// <returns> 是否生成了DLL </returns>
    private static bool _IsGeneratedDLL(string selectModule)
    {
        List<string> moduleList = new List<string>(); // 所有模块名称列表
        for (int i = 1; i < _selectModuleName.Count; i++)
        {
            moduleList.Add(_selectModuleName[i]);
        }

        // 分两种情况，所有模块和单模块
        if (_selectModuleName[s_selectModuleIndex] == "全平台") // 所有模块
        {
            if (FileHelper.GetExtensionFiles(PathHelper.s_streamingAssetsPath, "*.dll").Count !=
                moduleList.Count) // DLL数量 也就是模块数量
            {
                return false;
            }
        }
        else // 单模块
        {
            List<string> extensionFiles = FileHelper.GetExtensionFiles(PathHelper.s_streamingAssetsPath, "*.dll");
            // Dll文件名 也就是模块名

            foreach (var dllPath in extensionFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(dllPath);
                if ( fileName == selectModule)
                {
                    WLDebug.Log($"dll名{fileName} == 模块名:{selectModule}");
                    return true;
                }
            }
            WLDebug.LogError($"缺少dll名称:{selectModule}");
            return false; // 一个都找不到
        }

        return true;
    }

    /// <summary>
    /// 是否已经生成CLR绑定
    ///
    /// 先判定是否 GenAssets/ILRuntime/Generated 如果为空，
    /// 就Debug.LogError 提示....然后直接retunrn
    /// </summary>
    private static bool _IsGeneratedCLR()
    {
        // 文件夹是否为空
        if (FileHelper.IsDirectoryNull(PathHelper.s_CLRPath))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 打热更包核心
    /// </summary>
    private void _BuildHotUpdateCore()
    {
        List<string> moduleList = new List<string>(); // 所有模块名称列表
        for (int i = 1; i < _selectModuleName.Count; i++)
        {
            moduleList.Add(_selectModuleName[i]);
        }

        if (_selectModuleName[s_selectModuleIndex] == "全平台") // 所有模块打热更包
        {
            PackAssetBundles.MultiModuleBuildHotUpdate(s_buildTarget, _buildAssetBundleOptions, moduleList,
                1);
        }
        else // 模块打热更包
        {
            PackAssetBundles.BuildHotUpdate(s_buildTarget, _buildAssetBundleOptions,
                _selectModuleName[s_selectModuleIndex],
                1);
        }
    }

    /// <summary>
    /// 扫描模块个数
    /// </summary>
    /// <param name="selectModuleName"> 存储模块列表【例如：{"全平台"，"Common"}】</param>
    private static void _ScanningFile(List<string> selectModuleName)
    {
        string       assetPath = PathHelper.s_gameAssetsPath + "/";
        List<string> folders   = FileHelper.GetFolder(assetPath); // 获得所有文件和文件夹【不含子目录的文件和文件夹】
        foreach (var file in folders)
        {
            selectModuleName.Add(Path.GetFileName(file));
        }
    }
}
