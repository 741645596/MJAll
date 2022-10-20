// Author: futianfu
// Date: 2021/7/31 16:11:01


using System.IO;
using UnityEditor;
using UnityEngine;
using WLCore.Helper;

/// <summary>
/// PackingTag必须打上，且同一包内一致.规则
/// </summary>
public class AtlasPackingTagRule : AssetRuleBase
{
    public override string Description()
    {
        return "PackingTag必须打上，且同一包内一致.";
    }

    public override bool Pass(AssetInfoBase info)
    {
        var atlasInfo = (AtlasAssetInfo)info;

        if (!Directory.Exists(atlasInfo.foldPath))
        {
            return true;
        }

        bool isUse = atlasInfo.foldName.Contains("_atlas"); //只查找文件以_atlas的文件夹内部
        if (!isUse)
        {
            return true;
        }

        foreach (var item in atlasInfo.childFile)
        {
            string f        = PathHelper.PathFormat(item).Replace(Application.dataPath, "Assets");
            var    importer = AssetImporter.GetAtPath(f) as TextureImporter;

            if (importer == null)
            {
                continue;
            }

            if (importer.spritePackingTag == "")
            {
                tips = "PackingTag为空;";
                return false;
            }

            if (importer.spritePackingTag == atlasInfo.foldName) //spritePackingTag就是文件夹名
            {
                continue;
            }

            tips = "同一包内的PackingTag不一致;";
            return false;
        }


        return true;
    }

    public override void Fix(AssetInfoBase info)
    {
        var atlasInfo = (AtlasAssetInfo)info;

        foreach (var item in atlasInfo.childFile) // 设置文件夹内的所有packingtag 为文件夹名称
        {
            string f        = PathHelper.PathFormat(item).Replace(Application.dataPath, "Assets");
            var    importer = AssetImporter.GetAtPath(f) as TextureImporter;
            if (importer != null)
            {
                importer.spritePackingTag = atlasInfo.foldName;
            }

            AssetDatabase.Refresh();
        }
    }
}
