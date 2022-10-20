// TextureSizeRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/19

public class TextureSizeRule : AssetRuleBase
{
    private readonly int SIZE_THRESHOLD = 1024;

    public override bool CanFix()
    {
        return false;
    }

    public override string Description()
    {
        return $"图片大小不超过{SIZE_THRESHOLD}";
    }

    public override void Fix(AssetInfoBase info)
    {

    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is TextureAssetInfo == false)
        {
            return false;
        }

        var textureInfo = (TextureAssetInfo)info;
        if(textureInfo.width > SIZE_THRESHOLD || textureInfo.height > SIZE_THRESHOLD)
        {
            tips = $"尺寸[{textureInfo.width}x{textureInfo.height}]";
            return false;
        }

        return true;
    }
}
