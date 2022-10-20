// ModelMeshTangentRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/20

using UnityEditor;

public class ModelMeshTangentRule : AssetRuleBase
{
    public override string Description()
    {
        return "不包含Tangent属性(一般情况不需要包含)";
    }

    public override void Fix(AssetInfoBase info)
    {
        if (info is ModelAssetInfo == false)
        {
            return;
        }

        var model_info = (ModelAssetInfo)info;
        var importer = AssetImporter.GetAtPath(info.foldName) as ModelImporter;

        if (importer == null)
        {
            return;
        }

        if (model_info.isImportTangents != ModelImporterTangents.None)
        {
            importer.importTangents = ModelImporterTangents.None;
            model_info.isImportTangents = importer.importTangents;
        }

        importer.SaveAndReimport();
    }

    public override bool Pass(AssetInfoBase info)
    {
        if(info is ModelAssetInfo == false)
        {
            return false;
        }

        var model_info = (ModelAssetInfo)info;
        if(model_info.isImportTangents != ModelImporterTangents.None)
        {
            tips = "网格包含Tangent属性;";
            return false;
        }
        return true;
    }
}
