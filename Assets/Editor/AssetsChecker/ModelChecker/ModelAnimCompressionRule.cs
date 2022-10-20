using UnityEditor;

public class ModelAnimCompressionRule : AssetRuleBase
{
    public override string Description()
    {
        return "Anim.Compression==Optimal(优化后有可能造成动作卡顿,请根据效果优化)";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is ModelAssetInfo == false)
        {
            return false;
        }

        var model_info = (ModelAssetInfo)info;
        if (model_info.isAnimCompressionOptimal)
        {
            tips = "未优化动画;";
            return false;
        }

        return true;
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

        if (model_info.isAnimCompressionOptimal)
        {
            importer.animationCompression       = ModelImporterAnimationCompression.Optimal;
            model_info.isAnimCompressionOptimal = false;
            WLDebug.Log("fix修复了animationCompression");
        }

        importer.SaveAndReimport();
    }
}
