// @Author: futianfu
// @Date: 2021/9/22 13:33:19


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 预制粒子检测规则
/// </summary>
public class PrefabParticleRule : AssetRuleBase
{
    private List<string> _res; // 缓存判定的结果要输出tips的内容

    public PrefabParticleRule()
    {
        _res = new List<string>();
    }

    public override string Description()
    {
        return "Particle建议关闭阴影和光照探针";
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

    public override void Fix(AssetInfoBase info)
    {
        PrefabParticleAssetInfo assetInfo = info as PrefabParticleAssetInfo;
        if (assetInfo == null)
        {
            return;
        }

        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(info.foldName).transform; // 加载预制
        if (obj == null)
        {
            return;
        }

        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) // 获得预制下的所有子对象包括自身对象的组件
        {
            Renderer renderComp = child.GetComponent<Renderer>();
            renderComp.shadowCastingMode    = ShadowCastingMode.Off;    // 关闭投射阴影
            renderComp.receiveShadows       = false;                    // 关闭接收阴影打开
            renderComp.lightProbeUsage      = LightProbeUsage.Off;      // 关闭光照探针
            renderComp.reflectionProbeUsage = ReflectionProbeUsage.Off; // 关闭反射探针
            ParticleSystem.MainModule mainModule = child.main;



            EditorUtility.SetDirty(child); //【保存】覆盖预制状态
        }

        assetInfo.openCastShadows       = false; // 修复状态
        assetInfo.openReceiveShadows    = false; // 修复状态
        assetInfo.openLightProbes       = false; // 修复状态
        assetInfo.openReflectionProbes  = false; // 修复状态


        EditorUtility.SetDirty(obj); //【保存】覆盖预制状态
        PrefabUtility.SavePrefabAsset(obj.gameObject);
        AssetDatabase.Refresh(); // 刷新界面
    }


    /// <summary>
    /// 缓存判定的结果要输出tips的内容
    /// </summary>
    /// <param name="assetInfo"> 文件的检测信息 </param>
    /// <returns> 要输出tips的内容 </returns>
    private List<string> _Check(PrefabParticleAssetInfo assetInfo)
    {
        _res.Clear();
        if (assetInfo.openReceiveShadows || assetInfo.openCastShadows)
        {
            _res.Add("未关闭阴影;");
        }

        if (assetInfo.openLightProbes || assetInfo.openReflectionProbes)
        {
            _res.Add("未关光照探针;");
        }

        return  _res;
    }
}
