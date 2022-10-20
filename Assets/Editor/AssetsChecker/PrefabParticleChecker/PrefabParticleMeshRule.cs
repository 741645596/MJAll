using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class PrefabParticleMeshRule : AssetRuleBase
{
    private readonly List<string> _res; // 缓存判定的结果要输出tips的内容

    public PrefabParticleMeshRule()
    {
        _res = new List<string>();
    }

    public override string Description()
    {
        return "粒子类型是网格,则网格的R&W必须开启";
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
        PrefabParticleAssetInfo assetInfo = info as PrefabParticleAssetInfo; // 文件的检测信息

        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(info.foldName).transform; // 加载预制
        if (obj == null)
        {
            return;
        }

        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) // 获得预制下的所有子对象包括自身对象的组件
        {
            ParticleSystemRenderer renderComp = child.GetComponent<ParticleSystemRenderer>();
            if (renderComp.mesh != null) ; // 如果有设置模型才继续
            {
                string        assetPath     = AssetDatabase.GetAssetPath(renderComp.mesh);
                ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                modelImporter.isReadable = true;
                EditorUtility.SetDirty(child); //【保存】覆盖预制状态
            }
        }

        if (assetInfo != null)
        {
            assetInfo.isCloseReadable = false; // 修复状态
        }

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
        if (assetInfo.isCloseReadable)
        {
            _res.Add("网格类型粒子未开启可读写;");
        }

        return  _res;
    }
}
