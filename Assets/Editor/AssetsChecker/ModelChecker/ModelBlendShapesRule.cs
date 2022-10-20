using UnityEditor;

public class ModelBlendShapesRule : AssetRuleBase
{
    public override string Description()
    {
        return "Import BlendShapes等不勾选(一般情况不需Import)";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is ModelAssetInfo == false)
        {
            return false;
        }

        var model_info = (ModelAssetInfo)info;
        if (model_info.isImportBlendShapes)
        {
            tips = "网格包含BlendShapes;";
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

        if (model_info.isImportBlendShapes)
        {
            importer.importBlendShapes     = false;
            importer.importVisibility      = false;
            importer.importCameras         = false;
            importer.importLights          = false;
            model_info.isImportBlendShapes = false;
            WLDebug.Log("fix修复了BlendShapes等属性");
        }

        importer.SaveAndReimport();
    }
}
