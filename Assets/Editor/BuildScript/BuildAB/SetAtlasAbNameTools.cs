using UnityEditor;
using Unity.Utility;
using WLCore.Helper;

/// <summary>
/// 设置AB包名和打图集工具
/// </summary>
public class SetAtlasAbNameTools
{
    [MenuItem("Tools/设置AB包名和打图集工具")]
    public static void ShowWindow()
    {
        //调用设置
        // BuildHelper.SetAllAbName(true, FileHelper.s_gameAssetsPath);
        BuildHelper.SetAllMoudleAbName(PathHelper.s_gameAssetsPath);
    }
}
