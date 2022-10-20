// @Author: futianfu
// @Date: 2021/9/22 09:03:19


using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 预制网格检测逻辑
/// </summary>
public class PrefabSkinnedMeshChecker : AssetsCheckerBase<PrefabSkinnedMeshAssetInfo>
{
    private AssetsCheckEditorWindow.Settings _settings;

    public PrefabSkinnedMeshChecker(AssetsCheckEditorWindow.Settings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// 收集文件信息
    /// </summary>
    /// <returns>例：文件的项目路径，文件全路径，投射阴影是否打开，接收阴影是否打开，光照探针是否打开，反射探针是否打开</returns>
    public override List<PrefabSkinnedMeshAssetInfo> CollectAssetInfoList()
    {
        var assets_root = Path.Combine(Application.dataPath, _settings.assetsRootPath);

        var list = new List<PrefabSkinnedMeshAssetInfo>();

        var      pattern = "*.prefab";
        string[] files   = Directory.GetFiles(assets_root, pattern, SearchOption.AllDirectories); // 获取所有预制文件

        foreach (var file in files) // 遍历所有预制文件路径集合
        {
            var info = new PrefabSkinnedMeshAssetInfo(); // 信息对象初始化

            var fileInfo = new FileInfo(file);
            if (fileInfo != null)
            {
                info.filesize = fileInfo.Length; // 文件空间大小
            }

            var projectPath = file.Replace(Application.dataPath, "Assets"); // 项目路径，Asset/开头
            info.foldName = projectPath;                                    // 文件名【项目路径Asset开头】
            info.filePath = file;                                           // 文件路径

            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(projectPath).transform; // 预制的文件信息
            if (obj == null)
            {
                continue;
            }

            if (obj.GetComponentsInChildren<SkinnedMeshRenderer>().Length == 0) // 排除预制中没有蒙皮网格渲染器
                continue;

            _CheckCastShadow(obj, info);       // 检测每个节点是否 开启投射阴影
            _CheckReceiveShadows(obj, info);   // 检测每个节点是否 开启接收阴影
            _CheckLightProbes(obj, info);      // 检测每个节点是否 开启光照探针
            _CheckReflectionProbe(obj, info);  // 检测每个节点是否 开启反射探针
            _CheckDynamicOcclusion(obj, info); // 检测每个节点是否 开启动态遮挡

            list.Add(info);
        }

        list = list.OrderBy((info) => { return info.foldName; }).ToList();
        return list;
    }

    /// <summary>
    /// 检测每个节点是否 开启投射阴影
    /// </summary>
    /// <param name="obj"> 要遍历的预制父节点 </param>
    /// <param name="info"> 改预制保存的信息 </param>
    private static void _CheckCastShadow(Transform obj, PrefabSkinnedMeshAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<SkinnedMeshRenderer>()) // 获得预制下的所有子对象包括自身对象上的组件
        {
            if (child.shadowCastingMode != ShadowCastingMode.Off) // 如果投射阴影设置不为关闭状态
            {
                info.openCastShadows = true; // 就记录一下，待修复状态
            }

            if (info.openCastShadows)
                break;
        }
    }

    /// <summary>
    /// 检测每个节点是否 开启接收阴影
    /// </summary>
    /// <param name="obj"> 要遍历的预制父节点 </param>
    /// <param name="info"> 改预制保存的信息 </param>
    private static void _CheckReceiveShadows(Transform obj, PrefabSkinnedMeshAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<SkinnedMeshRenderer>()) // 获得预制下的所有子对象包括自身对象上的组件
        {
            info.openReceiveShadows = child.receiveShadows; // 如果接收阴影打开 就记录一下，待修复状态

            if (info.openReceiveShadows)
                break;
        }
    }

    /// <summary>
    /// 检测每个节点是否 开启光照探针
    /// </summary>
    /// <param name="obj"> 要遍历的预制父节点 </param>
    /// <param name="info"> 改预制保存的信息 </param>
    private static void _CheckLightProbes(Transform obj, PrefabSkinnedMeshAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<SkinnedMeshRenderer>()) // 获得预制下的所有子对象包括自身对象上的组件
        {
            if (child.lightProbeUsage != LightProbeUsage.Off) // 如果没关闭光照探针
            {
                info.openLightProbes = true; // 就记录一下，待修复状态
            }

            if (info.openLightProbes)
                break;
        }
    }

    /// <summary>
    /// 检测每个节点是否 开启反射探针
    /// </summary>
    /// <param name="obj"> 要遍历的预制父节点 </param>
    /// <param name="info"> 改预制保存的信息 </param>
    private static void _CheckReflectionProbe(Transform obj, PrefabSkinnedMeshAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<SkinnedMeshRenderer>()) // 获得预制下的所有子对象包括自身对象上的组件
        {
            if (child.reflectionProbeUsage != ReflectionProbeUsage.Off) // 如果没关闭反射探针
            {
                info.openReflectionProbes = true; // 就记录一下，待修复状态
            }

            if (info.openReflectionProbes)
                break;
        }
    }

    /// <summary>
    /// 检测每个节点是否 开启动态遮挡
    /// </summary>
    /// <param name="obj"> 要遍历的预制父节点 </param>
    /// <param name="info"> 改预制保存的信息 </param>
    private static void _CheckDynamicOcclusion(Transform obj, PrefabSkinnedMeshAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<SkinnedMeshRenderer>()) // 获得预制下的所有子对象包括自身对象上的组件
        {
            if (child.allowOcclusionWhenDynamic) // 如果没关闭反射探针
            {
                info.openDynamicOcclusion = true; //就记录一下，待修复状态
            }

            if (info.openDynamicOcclusion)
                break;
        }
    }
}
