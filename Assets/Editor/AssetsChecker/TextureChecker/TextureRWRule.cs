// TextureRWRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/19

using UnityEditor;

/// <summary>
/// 贴图资源检测器
/// </summary>
public class TextureRWRule : AssetRuleBase
{
    public override string Description()
    {
        return "图片设置需要关闭R&W选项";
    }

    public override void Fix(AssetInfoBase info)
    {
        if (info is TextureAssetInfo == false)
        {
            return;
        }

        var texture_info = (TextureAssetInfo)info;
        var importer = AssetImporter.GetAtPath(info.foldName) as TextureImporter;

        if (importer == null)
        {
            return;
        }

        if (texture_info.isReadable)
        {
            importer.isReadable = false;
            texture_info.isReadable = false;
        }

        importer.SaveAndReimport();
    }

    public override bool Pass(AssetInfoBase info)
    {
        if(info is TextureAssetInfo == false)
        {
            return false;
        }

        var isReadable = ((TextureAssetInfo)info).isReadable;
        if(isReadable)
        {
            tips = "未关闭R&W选项";
        }
        return isReadable == false;
    }
}
