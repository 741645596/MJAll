// @Author: futianfu
// @Date: 2021/9/22 13:33:19


using UnityEditor;
using UnityEngine;

/// <summary>
/// 预制只用来观看的粒子检测规则
/// </summary>
public class PrefabParticleEmissionRule : AssetRuleBase
{
    public override string Description()
    {
        string enter_space = "\r\n" + "    ";
        return "如果MaxParticles默认是1000，且Emission关闭，设置最大粒子数为0" + enter_space +
               "如果MaxParticles默认是1000，且Emission打开，Bursts为空,需要设置最大粒子数为1" + enter_space +
               "如果MaxParticles默认是1000，且Emission打开，且Bursts不为空，需要设置最大粒子数为Bursts";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is PrefabParticleAssetInfo == false)
        {
            return true;
        }

        PrefabParticleAssetInfo assetInfo = info as PrefabParticleAssetInfo;

        if (assetInfo.isDefaultMaxParticles)
        {
            tips = "未优化默认最大粒子数;";
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

        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(info.foldName).transform; // 加载预制
        if (obj == null)
        {
            return;
        }

        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) // 获得预制下的所有子对象包括自身对象的组件
        {
            if (child.main.maxParticles != 1000) // 如果没有修改过默认最大粒子数1000的情况才继续
                continue;

            ParticleSystem.MainModule mainModule = child.main;
            if (child.emission.enabled) // 是否开启发射器
            {
                if ( child.emission.burstCount == 0) // 如果没设置burst数量
                    mainModule.maxParticles = 1;     // 就给最大粒子1个
                else                                 // 如果有设置burst数量
                {
                    int burstsCount = 0; // 发射器粒子数统计
                    for (int i = 0; i < child.emission.burstCount; i++)
                    {
                        ParticleSystem.Burst bursts = child.emission.GetBurst(i);
                        burstsCount += (int)bursts.count.constant;
                    }

                    mainModule.maxParticles = burstsCount; // 就给最大粒子=burst数量
                }
            }
            else
            {
                mainModule.maxParticles = 0; // 没开发射器，最大粒子数为0
            }

            EditorUtility.SetDirty(child); //【保存】覆盖预制状态
        }

        assetInfo.isDefaultMaxParticles = false; // 修复状态

        EditorUtility.SetDirty(obj); //【保存】覆盖预制状态
        PrefabUtility.SavePrefabAsset(obj.gameObject);
        AssetDatabase.Refresh(); // 刷新界面
    }
}
