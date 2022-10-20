// @Author: futianfu
// @Date: 2021/9/22 13:33:19


using UnityEditor;
using UnityEngine;

/// <summary>
/// 预制空粒子检测规则
/// </summary>
public class PrefabParticleRendererRule : AssetRuleBase
{
    public override string Description()
    {
        return "Renderer关闭时检查Material是否为空";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is PrefabParticleAssetInfo == false)
        {
            return true;
        }

        PrefabParticleAssetInfo assetInfo = info as PrefabParticleAssetInfo;

        if (assetInfo.isCloseRenderer)
        {
            tips = "Renderer关闭Material必须为空;";
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
            ParticleSystemRenderer renderComp = child.GetComponent<ParticleSystemRenderer>();
            if (renderComp.enabled == false &&
                renderComp.renderMode != ParticleSystemRenderMode.None) //如果Renderer组件关闭，则为需设置RenderMode为空
            {
                renderComp.renderMode = ParticleSystemRenderMode.None;

                EditorUtility.SetDirty(child); //【保存】覆盖预制状态
            }
        }

        assetInfo.isCloseRenderer = false; //修复状态

        EditorUtility.SetDirty(obj); //【保存】覆盖预制状态
        PrefabUtility.SavePrefabAsset(obj.gameObject);
        AssetDatabase.Refresh();     //刷新界面
    }
}
