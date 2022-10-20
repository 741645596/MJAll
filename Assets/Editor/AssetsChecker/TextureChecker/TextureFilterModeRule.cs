// TextureFilterModeRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/19

using UnityEditor;
using UnityEngine;

public class TextureFilterModeRule : AssetRuleBase
{
    public override string Description()
    {
        return "图片设置FilterMode需要为Bilinear";
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

        if (texture_info.filterMode != FilterMode.Bilinear)
        {
            importer.filterMode = FilterMode.Bilinear;
            texture_info.filterMode = FilterMode.Bilinear;
        }
        importer.SaveAndReimport();
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is TextureAssetInfo == false)
        {
            return false;
        }

        var filterMode = ((TextureAssetInfo)info).filterMode;
        // 没设置的默认值是-1 同时也是Bilinear
        if ((int)filterMode == -1)
        {
            return true;
        }

        if (filterMode != FilterMode.Bilinear)
        {
            tips = "FilterMode不是Bilinear";
            return false;
        }
        return true;
    }
}
