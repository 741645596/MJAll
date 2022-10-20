using UnityEditor;

public class ModelOptimizeGameObjectRule : AssetRuleBase
{
    public override string Description()
    {
        return "Optimize Game Object需要开启(建议勾选且优化节点,需要手动处理)";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is ModelAssetInfo == false)
        {
            return false;
        }

        var model_info = (ModelAssetInfo)info;
        if (model_info.isOptimizeGameObjects)
        {
            tips = "未优化游戏对象;";
            return false;
        }

        return true;
    }

    public override void Fix(AssetInfoBase info)
    {
        WLDebug.Log("你需要手动修复,指定优化的节点");

        ModelCheckEditorWindow modelWindow =
            EditorWindow.GetWindow<ModelCheckEditorWindow>(true, "Send Event Window");
        modelWindow.DisplayDialog("提示", "你需要手动修复,指定优化的节点");
    }
}
