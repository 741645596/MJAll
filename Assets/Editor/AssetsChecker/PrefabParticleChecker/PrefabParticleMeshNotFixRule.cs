// @Author: futianfu
// @Date: 2021/12/23 13:25:19


using System.Collections.Generic;

public class PrefabParticleMeshNotFixRule : AssetRuleBase
{
    private List<string> _res; // 缓存判定的结果要输出tips的内容

    public PrefabParticleMeshNotFixRule()
    {
        _res = new List<string>();
    }

    public override string Description()
    {
        string enter_space = "\r\n" + "    ";
        return "仅建议：粒子数量小于30" + enter_space +
               "仅建议：粒子类型是网格,且发射数小于5" + enter_space +
               // "粒子类型是网格,且网格的R&W是开启" + enter_space +
               "仅建议：粒子类型是网格,且网格面片小于500";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is PrefabParticleAssetInfo == false)
        {
            return true;
        }

        PrefabParticleAssetInfo assetInfo = info as PrefabParticleAssetInfo; // 文件的检测信息
        List<string>            check     = _Check(assetInfo);               // 缓存判定的结果要输出tips的内容
        if (check.Count > 0)                                                 // 如果要输出的内容数量大于0，则存在错误
        {
            string str = "";
            tips = string.Join(str, check); // 合并字符串
            return false;                   // 返回错误
        }

        return  true;
    }

    public override bool CanFix()
    {
        return false;
    }

    public override void Fix(AssetInfoBase info)
    {
        // 不修复，只提示
    }


    /// <summary>
    /// 缓存判定的结果要输出tips的内容
    /// </summary>
    /// <param name="assetInfo"> 文件的检测信息 </param>
    /// <returns> 要输出tips的内容 </returns>
    private List<string> _Check(PrefabParticleAssetInfo assetInfo)
    {
        _res.Clear();
        if (assetInfo.isOver30MaxParticles)
        {
            _res.Add("粒子数大于30;");
        }

        if (assetInfo.isOverBurstsCount)
        {
            _res.Add("网格类型粒子发射数大于5;");
        }

        // if (assetInfo.isCloseReadable)
        // {
        //     _res.Add("网格类型粒子未开启可读写;");
        // }

        if (assetInfo.isOverTrianglesCount)
        {
            _res.Add("网格类型粒子面数大于500;");
        }

        return  _res;
    }
}
