// @Author: futianfu
// @Date: 2021/9/17 10:49:33


using UnityEditor;

/// <summary>
/// 大图检测用到的数据
/// </summary>
public class BigPicAssetInfo : AssetInfoBase
{
    public int                   width;         //图片宽
    public int                   height;        //图片高
    public bool                  isFixed;       //是否被修复过
    public TextureImporterFormat androidFormat; //安卓压缩格式
    public TextureImporterFormat iosFormat;     //ios压缩格式
}
