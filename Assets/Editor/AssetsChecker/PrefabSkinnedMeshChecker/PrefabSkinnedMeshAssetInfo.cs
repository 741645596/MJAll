// @Author: futianfu
// @Date: 2021/9/22 09:03:19


/// <summary>
/// 预制蒙皮网格专有属性
/// </summary>
public class PrefabSkinnedMeshAssetInfo : AssetInfoBase
{
    public string foldPath;             // 当前目录的全路径
    public bool   openCastShadows;      // 是否打开投射阴影
    public bool   openReceiveShadows;   // 是否打开接收阴影
    public bool   openLightProbes;      // 是否打开光照探针
    public bool   openReflectionProbes; // 是否打开反射探针
    public bool   openDynamicOcclusion; // 动态遮挡

    public PrefabSkinnedMeshAssetInfo()
    {
    }
}
