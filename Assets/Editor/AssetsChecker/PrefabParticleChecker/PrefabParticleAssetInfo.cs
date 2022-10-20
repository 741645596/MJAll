// @Author: futianfu
// @Date: 2021/9/22 13:33:19


/// <summary>
/// 预制粒子专有属性
/// </summary>
public class PrefabParticleAssetInfo : AssetInfoBase
{
    public bool openCastShadows;       //是否打开投射阴影
    public bool openReceiveShadows;    //是否打开接收阴影
    public bool openLightProbes;       //是否打开光照探针
    public bool openReflectionProbes;  //是否打开反射探针
    public bool isDefaultMaxParticles; //是否修复默认1000个最大粒子数
    public bool isCloseRenderer;       //是否关闭了Renderer
    public bool isOpenPrewarm;         //是否关闭了Prewarm
    public bool isOpenCollision;       //是否关闭了Collision
    public bool isOpenTrigger;         //是否关闭了Trigger
    public bool isOver30MaxParticles;  //是否超过30个最大粒子数
    public bool isOverBurstsCount;     //是否超过5个粒子发射数
    public bool isCloseReadable;       //是否可读写
    public bool isOverTrianglesCount;  //是否超过500面数

    public PrefabParticleAssetInfo()
    {
    }
}
