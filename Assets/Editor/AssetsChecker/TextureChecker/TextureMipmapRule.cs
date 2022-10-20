// TextureMipmapRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/19
using System;
using UnityEditor;

public class TextureMipmapRule: AssetRuleBase
{
    public TextureMipmapRule()
    {
    }

    public override string Description()
    {
        return "图片设置需要关闭Mipmap";
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

        if (texture_info.isMiamap)
        {
            importer.mipmapEnabled = false;
            texture_info.isMiamap = false;
        }
        importer.SaveAndReimport();
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is TextureAssetInfo == false)
        {
            return false;
        }

        var isMipmap = ((TextureAssetInfo)info).isMiamap;
        if(isMipmap)
        {
            tips = "未关闭Mipmap";
            return false;
        }
        return true;
    }
}
