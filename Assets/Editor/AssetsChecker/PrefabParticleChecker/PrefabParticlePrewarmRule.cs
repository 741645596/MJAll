// @Author: futianfu
// @Date: 2021/12/22 17:45:19

using UnityEditor;
using UnityEngine;

/// <summary>
/// 预制粒子检测规则
/// </summary>
public class PrefabParticlePrewarmRule : AssetRuleBase
{
    public override string Description()
    {
        return "prewarm建议关闭";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is PrefabParticleAssetInfo == false)
        {
            return true;
        }

        PrefabParticleAssetInfo assetInfo = info as PrefabParticleAssetInfo;

        if (assetInfo.isOpenPrewarm)
        {
            tips = "未关闭Prewarm;";
            return false;
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

        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(info.foldName).transform; //加载预制
        if (obj == null)
        {
            return;
        }

        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象的组件
        {
            ParticleSystem.MainModule mainModule = child.main;
            mainModule.prewarm = false;
            EditorUtility.SetDirty(child); //【保存】覆盖预制状态
        }

        assetInfo.isOpenPrewarm = false; //修复状态

        EditorUtility.SetDirty(obj); //【保存】覆盖预制状态
        PrefabUtility.SavePrefabAsset(obj.gameObject);
        AssetDatabase.Refresh(); //刷新界面
    }
}
