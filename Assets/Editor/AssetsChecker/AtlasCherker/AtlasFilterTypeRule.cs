// Author: futianfu
// Date: 2021/7/31 16:11:01


using System.Collections.Generic;
using System.IO;
using Unity.Utility;
using WLCore.Helper;

/// <summary>
/// 文件类型规则
/// </summary>
public class AtlasFilterTypeRule : AssetRuleBase
{
    public override string Description()
    {
        return "图片只能是png和jpg文件";
    }

    public override bool Pass(AssetInfoBase info)
    {
        AtlasAssetInfo atlasAssetInfo = info as AtlasAssetInfo;
        bool isUse = Path.GetFileName(atlasAssetInfo.foldPath).Contains("_atlas"); //只查找文件以_atlas的文件夹内部
        if (!isUse)
        {
            return true;
        }

        return _IsJpgPng(atlasAssetInfo.childFile); //判断是否jpg或png
    }

    /// <summary>
    /// 判断是否jpg或png
    /// </summary>
    /// <param name="paths"> 一个文件夹内的所有文件全路径【例： {Assets/GameAssets/Common/Module1/demo_atlas/bind_icon.png,...}】 </param>
    /// <returns> 返回是否通过测试 </returns>
    private bool _IsJpgPng(List<string> paths)
    {
        for (int j = 0; j < paths.Count; j++)
        {
            var file = paths[j];

            string extension = FileHelper.GetExtension(file); //获得后缀
            if (extension == ".jpg" || extension == ".png")
            {
                continue;
            }

            tips = "包含了其他格式资源【错误文件名:" + Path.GetFileName(file) + "】;";
            return false;
        }

        return true;
    }

    public override void Fix(AssetInfoBase info)
    {
    }

    public override bool CanFix()
    {
        return   false; //不能自动修复,隐藏修复按钮
    }
}
