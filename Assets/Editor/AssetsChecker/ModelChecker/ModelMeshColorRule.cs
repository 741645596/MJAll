// ModelMeshColorRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/20

using UnityEditor;

public class ModelMeshColorRule : AssetRuleBase
{
    // public override bool CanFix()
    // {
    //     return false;
    // }

    public override string Description()
    {
        return "不包含Color属性(一般情况不需要包含)";
    }

    public override void Fix(AssetInfoBase info)
    {
        WLDebug.Log("你需要手动修复Color属性");
        ModelCheckEditorWindow modelWindow =
            EditorWindow.GetWindow<ModelCheckEditorWindow>(true, "Send Event Window");
        modelWindow.DisplayDialog("提示", "一般情况不需要Color属性,请让美术导出模型之前去掉Color属性");
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is ModelAssetInfo == false)
        {
            return false;
        }

        var model_info = (ModelAssetInfo)info;
        if (model_info.meshColorCount > 0)
        {
            tips = "网格包含Color属性;";
            return false;
        }

        return true;
    }
}
