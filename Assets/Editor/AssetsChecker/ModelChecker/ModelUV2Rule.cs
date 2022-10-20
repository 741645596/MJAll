using System.Collections.Generic;
using UnityEditor;

public class ModelUV2Rule : AssetRuleBase
{
    private List<string> _res; // 缓存判定的结果要输出tips的内容

    public ModelUV2Rule()
    {
        _res = new List<string>();
    }

    public override string Description()
    {
        return "不包含UV2属性(一般情况不需要包含)";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is ModelAssetInfo == false)
        {
            return true;
        }

        ModelAssetInfo assetInfo = info as ModelAssetInfo;
        List<string>   check     = _Check(assetInfo); // 缓存判定的结果要输出tips的内容
        if (check.Count > 0)                          // 如果要输出的内容数量大于0，则存在错误
        {
            string str = "";
            tips = string.Join(str, check); // 合并字符串
            return false;                   // 返回错误
        }

        return true;
    }

    public override void Fix(AssetInfoBase info)
    {
        WLDebug.Log("你需要手动修改UV2问题");

        ModelCheckEditorWindow modelWindow =
            EditorWindow.GetWindow<ModelCheckEditorWindow>(true, "Send Event Window");
        modelWindow.DisplayDialog("提示", "一般情况不需要UV2,请让美术导出模型之前去掉UV2");
    }

    /// <summary>
    /// 缓存判定的结果要输出tips的内容
    /// </summary>
    /// <param name="assetInfo"> 文件的检测信息 </param>
    /// <returns> 要输出tips的内容 </returns>
    private List<string> _Check(ModelAssetInfo assetInfo)
    {
        _res.Clear();
        if (assetInfo.isImportUV2)
        {
            _res.Add("网格包含UV2;");
        }

        return  _res;
    }
}
