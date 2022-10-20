// TextureFormatRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/19

using UnityEditor;

public class TextureFormatRule : AssetRuleBase
{
    public override string Description()
    {
        return "图片设置格式需要为ASTC";
    }

    public override void Fix(AssetInfoBase info)
    {
        if (info is TextureAssetInfo == false)
        {
            return;
        }
        var importer = AssetImporter.GetAtPath(info.foldName) as TextureImporter;

        if (importer == null)
        {
            return;
        }

        var texture_info = (TextureAssetInfo)info;

        var android_setting = importer.GetPlatformTextureSettings("Android");
        //Debug.Log($"{android_setting.name} {android_setting.format} {android_setting.overridden}");
        android_setting.overridden = true;
        android_setting.format = TextureImporterFormat.ASTC_4x4;
        texture_info.androidTextureImportFormat = android_setting.format;
        importer.SetPlatformTextureSettings(android_setting);

        var ios_setting = importer.GetPlatformTextureSettings("iPhone");
        //Debug.Log($"{ios_setting.name} {ios_setting.format} {ios_setting.overridden}");
        ios_setting.overridden = true;
        ios_setting.format = TextureImporterFormat.ASTC_4x4;
        texture_info.iosTextureImportFormat = ios_setting.format;
        importer.SetPlatformTextureSettings(ios_setting);

        importer.SaveAndReimport();
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is TextureAssetInfo == false)
        {
            return false;
        }

        var texture_info = (TextureAssetInfo)info;
        var pass = true;
        tips = "";
        if(CheckFormat(texture_info.androidTextureImportFormat) == false)
        {
            tips = "Android平台不是ASTC格式";
            pass = false;
        }
        if (CheckFormat(texture_info.iosTextureImportFormat) == false)
        {
            tips += " iOS平台不是ASTC格式";
            pass = false;
        }
        return pass;
    }

    private bool CheckFormat(TextureImporterFormat format)
    {
        if (format == TextureImporterFormat.ASTC_4x4
            || format == TextureImporterFormat.ASTC_5x5
            || format == TextureImporterFormat.ASTC_6x6
            || format == TextureImporterFormat.ASTC_8x8
            || format == TextureImporterFormat.ASTC_10x10
            || format == TextureImporterFormat.ASTC_12x12)
        {
            return true;
        }

        return false;
    }
}
