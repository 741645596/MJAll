// TextureAssetInfo.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/19

using UnityEditor;
using UnityEngine;

public class TextureAssetInfo : AssetInfoBase
{
    public bool isReadable;
    public int width;
    public int height;
    public bool isMiamap;
    public FilterMode filterMode;
    public TextureImporterFormat iosTextureImportFormat;
    public TextureImporterFormat androidTextureImportFormat;

    public TextureAssetInfo()
    {
    }
}
