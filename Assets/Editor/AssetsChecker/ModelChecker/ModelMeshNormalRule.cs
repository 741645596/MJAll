// ModelMeshNormalRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/20

using UnityEditor;

public class ModelMeshNormalRule : AssetRuleBase
{
    public override string Description()
    {
        return "不包含Normal属性(根据具体业务开关)";
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

        if (model_info.isImportNormals != ModelImporterNormals.None)
        {
            importer.importNormals = ModelImporterNormals.None;
            model_info.isImportNormals = importer.importNormals;
            WLDebug.Log("fix修复了Normals");
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
        if(model_info.isImportNormals != ModelImporterNormals.None)
        {
            tips = "网格包含Normal属性;";
            return false;
        }
        return true;
    }
}
