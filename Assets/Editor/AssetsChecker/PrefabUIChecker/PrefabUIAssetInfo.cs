// @Author: futianfu
// @Date: 2021/9/22 17:03:19


/// <summary>
/// 预制UI专有属性
/// </summary>
public class PrefabUIAssetInfo : AssetInfoBase
{
    public string foldPath;     //当前目录的全路径
    public bool   tipCloseRay;  //提示是否需要关闭射线
    public bool   needCloseRay; //是否需要关闭射线
    public bool   btnOpenRay;   //按钮需要打开射线

    public PrefabUIAssetInfo()
    {
    }
}
