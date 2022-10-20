// @Author: futianfu
// @Date: 2021/8/2 11:01:35


using System.IO;
using UnityEditor;
using UnityEngine;
using WLCore.Helper;

/// <summary>
/// 所有贴图的压缩格式必须一致，且是ASTC格式
/// </summary>
public class AtlasFormatRule : AssetRuleBase
{
    private AtlasCheckEditorWindow _atlasTab; //点击修复后创建一个小窗选择压缩格式

    public AtlasFormatRule()
    {
        _atlasTab = EditorWindow.GetWindow<AtlasCheckEditorWindow>(typeof(AtlasCheckEditorWindow));
    }

    public override string Description()
    {
        return "所有贴图压缩格式必须一致,且是ASTC格式";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is AtlasAssetInfo == false)
        {
            return false;
        }

        var atlasInfo = (AtlasAssetInfo)info;

        //只查找文件以_atlas的文件夹内部
        bool isUse = Path.GetFileName(atlasInfo.foldPath).Contains("_atlas");
        if (!isUse)
        {
            return true;
        }

        //是否是ASTC格式
        if (!_IsASTC(atlasInfo))
        {
            return false;
        }

        //是否格式一致
        if (!_IsSame(atlasInfo))
        {
            return false;
        }

        return true;
    }

    public override void Fix(AssetInfoBase info)
    {
        _atlasTab.SetASTC(this, info); //点击修复后创建一个小窗选择压缩格式
    }

    public virtual void Set_ASTC_4x4(AssetInfoBase info)
    {
        _Set_ASTC(info, TextureImporterFormat.ASTC_4x4); //设置统一的纹理压缩格式
    }

    public virtual void Set_ASTC_5x5(AssetInfoBase info)
    {
        _Set_ASTC(info, TextureImporterFormat.ASTC_5x5); //设置统一的纹理压缩格式
    }

    public virtual void Set_ASTC_6x6(AssetInfoBase info)
    {
        _Set_ASTC(info, TextureImporterFormat.ASTC_6x6); //设置统一的纹理压缩格式
    }

    /// <summary>
    /// 设置统一的纹理压缩格式
    /// </summary>
    /// <param name="info">资源信息</param>
    /// <param name="tpFormat">要设置成哪种压缩格式</param>
    private void _Set_ASTC(AssetInfoBase info, TextureImporterFormat tpFormat)
    {
        if (info is AtlasAssetInfo == false)
        {
            return;
        }

        var atlasInfo = (AtlasAssetInfo)info;

        for (int j = 0; j < atlasInfo.childFile.Count; j++)
        {
            var file = atlasInfo.childFile[j];

            string f        = PathHelper.PathFormat(file).Replace(Application.dataPath, "Assets");
            var    importer = AssetImporter.GetAtPath(f) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            var androidSetting = importer.GetPlatformTextureSettings("Android");
            androidSetting.overridden = true;
            androidSetting.format     = tpFormat;
            importer.SetPlatformTextureSettings(androidSetting);
            importer.SaveAndReimport();

            var iosSetting = importer.GetPlatformTextureSettings("iPhone");
            iosSetting.overridden = true;
            iosSetting.format     = tpFormat;
            importer.SetPlatformTextureSettings(iosSetting);
            importer.SaveAndReimport();
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 是否格式一致
    /// </summary>
    /// <param name="atlasInfo"> 图集信息 </param>
    /// <returns> 如果是一致则继续下面的逻辑，如果不一致则提示资源问题 </returns>
    private bool _IsSame(AtlasAssetInfo atlasInfo)
    {
        TextureImporterFormat format = TextureImporterFormat.Alpha8;
        foreach (var item in atlasInfo.childFile)
        {
            string f        = PathHelper.PathFormat(item).Replace(Application.dataPath, "Assets");
            var    importer = AssetImporter.GetAtPath(f) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            var androidFormat = importer.GetPlatformTextureSettings("Android").format; //安卓格式
            var iosFormat     = importer.GetPlatformTextureSettings("iPhone").format;  //IOS格式

            //给个参照物初始化
            if (format == TextureImporterFormat.Alpha8)
            {
                format = androidFormat;
            }

            //如果不正确就提示
            if (androidFormat != format || iosFormat != format)
            {
                tips = "安卓和苹果平台所有贴图压缩格式必须一致;";
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 是否是ASTC格式
    /// </summary>
    /// <param name="atlasInfo"> 图集信息 </param>
    /// <returns> 如果是ASTC则继续下面的逻辑，如果不是ASTC提示资源问题 </returns>
    private bool _IsASTC(AtlasAssetInfo atlasInfo)
    {
        foreach (var item in atlasInfo.childFile)
        {
            string f        = PathHelper.PathFormat(item).Replace(Application.dataPath, "Assets");
            var    importer = AssetImporter.GetAtPath(f) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            var  androidFormat  = importer.GetPlatformTextureSettings("Android").format; //安卓格式
            bool andoridIsRight = _IsRightFormat(androidFormat);                         //安卓格式是否正确
            var  iosFormat      = importer.GetPlatformTextureSettings("iPhone").format;  //IOS格式
            bool iosIsRight     = _IsRightFormat(iosFormat);                             //IOS格式是否正确
            //如果不正确就提示
            if (!andoridIsRight || !iosIsRight)
            {
                tips = "安卓和苹果平台必须压缩格式是ASTC;";
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 检测格式是否为ASTC
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    private bool _IsRightFormat(TextureImporterFormat format)
    {
        return format == TextureImporterFormat.ASTC_4x4
               || format == TextureImporterFormat.ASTC_5x5
               || format == TextureImporterFormat.ASTC_6x6;
    }
}
