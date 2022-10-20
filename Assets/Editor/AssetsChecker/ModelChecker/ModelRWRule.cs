// ModelRWRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/09

using UnityEditor;

public class ModelRWRule : AssetRuleBase
{
    public override string Description()
    {
        return "R&W关闭(如果模型是用在粒子系统或者是使用静态核批需要开启,否则可能Crash;其他情况关闭)";
    }

    public override void Fix(AssetInfoBase info)
    {
        if (info is ModelAssetInfo == false)
        {
            return;
        }

        var model_info = (ModelAssetInfo)info;
        var importer   = AssetImporter.GetAtPath(info.foldName) as ModelImporter;

        if (importer == null)
        {
            return;
        }

        if (model_info.isReadable)
        {
            importer.isReadable   = false;
            model_info.isReadable = false;
        }

        importer.SaveAndReimport();
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is ModelAssetInfo == false)
        {
            return false;
        }

        var model_info = (ModelAssetInfo)info;

        // 粒子目录内模型需要要开启R&W
        if (model_info.isReadable)
        {
            tips = "未关闭R&W;";
            return false;
        }

        return true;
    }
}
