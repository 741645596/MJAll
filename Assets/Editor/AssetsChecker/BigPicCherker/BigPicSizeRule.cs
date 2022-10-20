// @Author: futianfu
// @Date: 2021/9/17 10:49:33


using UnityEditor;
using UnityEngine;
using WLCore.Helper;


/// <summary>
/// 查找图片尺寸超过范围的资源
/// </summary>
public class BigPicSizeRule : AssetRuleBase
{
    public int _customSize; //用户自定义尺寸

    private BigPicCheckEditorWindow _tab; //点击修复后创建一个小窗选择压缩格式

    public BigPicSizeRule(int customSize)
    {
        _customSize = customSize; //外部传入一个默认的用户初始化尺寸
        _tab        = EditorWindow.GetWindow<BigPicCheckEditorWindow>(typeof(BigPicCheckEditorWindow));
    }

    /// <summary>
    /// 规则描述
    /// </summary>
    /// <returns> 返回字符被UI调用 </returns>
    public override string Description()
    {
        return "贴图尺寸宽高任意一个大于指定范围即显示";
    }

    /// <summary>
    /// 检测是否通过
    /// </summary>
    /// <param name="info"> 要检测的对象信息 </param>
    /// <returns> 是否通过检测，没通过UI条目就变黄 </returns>
    public override bool Pass(AssetInfoBase info)
    {
        if (info is BigPicAssetInfo == false)
        {
            return true;
        }

        BigPicAssetInfo assetInfo = info as BigPicAssetInfo;
        if (assetInfo.height > _customSize || assetInfo.width > _customSize)
        {
            tips = "高   " + assetInfo.height + "   宽   " + assetInfo.width + " ; 安卓压缩格式   " + assetInfo.androidFormat +
                   "   IOS压缩格式   " + assetInfo.iosFormat + " ;";
            return false;
        }

        return true;
    }

    public override void Fix(AssetInfoBase info)
    {
        _tab.SetASTC(this, info); //点击修复后创建一个小窗选择压缩格式
    }

    /// <summary>
    /// 设置ASTC_4x4纹理压缩格式
    /// </summary>
    /// <param name="info"> 要设置的对象信息 </param>
    public virtual void Set_ASTC_4x4(AssetInfoBase info)
    {
        _Set_ASTC(info, TextureImporterFormat.ASTC_4x4); //设置统一的纹理压缩格式
    }

    /// <summary>
    /// 设置ASTC_5x5纹理压缩格式
    /// </summary>
    /// <param name="info"> 要设置的对象信息 </param>
    public virtual void Set_ASTC_5x5(AssetInfoBase info)
    {
        _Set_ASTC(info, TextureImporterFormat.ASTC_5x5); //设置统一的纹理压缩格式
    }

    /// <summary>
    /// 设置ASTC_6x6纹理压缩格式
    /// </summary>
    /// <param name="info"> 要设置的对象信息 </param>
    public virtual void Set_ASTC_6x6(AssetInfoBase info)
    {
        _Set_ASTC(info, TextureImporterFormat.ASTC_6x6); //设置统一的纹理压缩格式
    }

    /// <summary>
    /// 设置统一的纹理压缩格式
    /// </summary>
    /// <param name="info"> 要设置的对象信息 </param>
    /// <param name="tpFormat"> 要设置成哪种压缩格式 </param>
    private void _Set_ASTC(AssetInfoBase info, TextureImporterFormat tpFormat)
    {
        if (info is BigPicAssetInfo == false)
        {
            return;
        }

        //找到对应的贴图文件
        BigPicAssetInfo assetInfo = info as BigPicAssetInfo;
        string          f         = PathHelper.PathFormat(assetInfo.filePath).Replace(Application.dataPath, "Assets");
        var             importer  = AssetImporter.GetAtPath(f) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        //安卓设置
        TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
        androidSettings.overridden = true;
        androidSettings.format     = tpFormat;
        importer.SetPlatformTextureSettings(androidSettings);
        importer.SaveAndReimport();

        //IOS设置
        TextureImporterPlatformSettings iosSettings = importer.GetPlatformTextureSettings("iPhone");
        iosSettings.overridden = true;
        iosSettings.format     = tpFormat;
        importer.SetPlatformTextureSettings(iosSettings);
        importer.SaveAndReimport();

        assetInfo.isFixed = true; //标记被修复过
    }
}
