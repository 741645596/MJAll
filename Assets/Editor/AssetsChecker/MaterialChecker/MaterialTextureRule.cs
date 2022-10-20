// MaterialTextureRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/20

public class MaterialTextureRule : AssetRuleBase
{
    public override string Description()
    {
        return "检查包含空贴图的材质";
    }

    public override void Fix(AssetInfoBase info)
    {
        
    }

    public override bool CanFix()
    {
        return false;
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is MaterialAssetInfo == false)
        {
            return false;
        }

        var material_info = (MaterialAssetInfo)info;

        if(material_info.hasEmptyTexture)
        {
            tips = "包含空纹理采样";
            return false;
        }
        return true;
    }
}
