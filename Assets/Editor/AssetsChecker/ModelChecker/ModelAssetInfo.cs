// ModelAssetInfo.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/09

using UnityEditor;

public class ModelAssetInfo : AssetInfoBase
{
    public bool                  isReadable;               // 是否可读写
    public int                   meshColorCount;           // 导入颜色
    public ModelImporterNormals  isImportNormals;          // 导入法线
    public ModelImporterTangents isImportTangents;         // 导入切线
    public bool                  isImportUV2;              // 导入UV2
    public bool                  isImportBlendShapes;      // 导入混合形状
    public bool                  isOptimizeGameObjects;    // 是否优化游戏对象
    public bool                  isAnimCompressionOptimal; // 是否优化动画
}
