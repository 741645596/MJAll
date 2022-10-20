// @Author: futianfu
// @Date: 2021/9/22 09:03:19


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 预制蒙皮网格检测规则
/// </summary>
public class PrefabSkinnedMeshRule : AssetRuleBase
{
    private List<string> _res; // 缓存判定的结果要输出tips的内容

    public PrefabSkinnedMeshRule()
    {
        _res = new List<string>();
    }

    public override string Description()
    {
        string enter_space = "\r\n" + "    ";
        return "不产生阴影且不接受阴影" + enter_space +
               "关闭光照探针和反射探针" + enter_space +
               "关闭动态遮挡";
    }


    public override bool Pass(AssetInfoBase info)
    {
        if (info is PrefabSkinnedMeshAssetInfo == false)
        {
            return true;
        }

        PrefabSkinnedMeshAssetInfo assetInfo = info as PrefabSkinnedMeshAssetInfo; // 文件的检测信息
        List<string>               check     = _Check(assetInfo);                  // 缓存判定的结果要输出tips的内容
        if (check.Count > 0)                                                       // 如果要输出的内容数量大于0，则存在错误
        {
            string str = "";
            tips = string.Join(str, check); // 合并字符串
            return false;                   // 返回错误
        }

        return true;
    }

    /// <summary>
    /// 缓存判定的结果要输出tips的内容
    /// </summary>
    /// <param name="assetInfo"> 文件的检测信息 </param>
    /// <returns> 要输出tips的内容 </returns>
    private List<string> _Check(PrefabSkinnedMeshAssetInfo assetInfo)
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

        if (assetInfo.openDynamicOcclusion)
        {
            _res.Add("未关动态遮挡;");
        }

        return  _res;
    }

    public override void Fix(AssetInfoBase info)
    {
        PrefabSkinnedMeshAssetInfo assetInfo = info as PrefabSkinnedMeshAssetInfo;
        if (assetInfo == null)
        {
            return;
        }

        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(info.foldName).transform; // 加载预制
        if (obj == null)
        {
            return;
        }

        foreach (var child in obj.GetComponentsInChildren<SkinnedMeshRenderer>()) // 获得预制下的所有子对象包括自身对象的组件
        {
            child.shadowCastingMode         = ShadowCastingMode.Off;    // 关闭投射阴影
            child.receiveShadows            = false;                    // 关闭接收阴影打开
            child.lightProbeUsage           = LightProbeUsage.Off;      // 关闭光照探针
            child.reflectionProbeUsage      = ReflectionProbeUsage.Off; // 关闭反射探针
            child.allowOcclusionWhenDynamic = false;                    // 关闭动态遮挡

            EditorUtility.SetDirty(child); //【保存】覆盖预制状态
        }

        assetInfo.openCastShadows      = false; // 修复状态
        assetInfo.openReceiveShadows   = false; // 修复状态
        assetInfo.openLightProbes      = false; // 修复状态
        assetInfo.openReflectionProbes = false; // 修复状态
        assetInfo.openDynamicOcclusion = false; // 修复状态

        EditorUtility.SetDirty(obj); //【保存】覆盖预制状态
        PrefabUtility.SavePrefabAsset(obj.gameObject);
        AssetDatabase.Refresh(); // 刷新界面
    }
}
