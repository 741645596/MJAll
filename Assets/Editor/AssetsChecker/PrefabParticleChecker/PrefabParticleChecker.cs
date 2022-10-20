// @Author: futianfu
// @Date: 2021/9/22 13:33:19


using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// 预制粒子检测逻辑
/// </summary>
public class PrefabParticleChecker : AssetsCheckerBase<PrefabParticleAssetInfo>
{
    private AssetsCheckEditorWindow.Settings _settings;

    public PrefabParticleChecker(AssetsCheckEditorWindow.Settings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// 收集文件信息
    /// </summary>
    /// <returns>例：文件的项目路径，文件全路径，投射阴影是否打开，接收阴影是否打开，光照探针是否打开，反射探针是否打开，最大粒子数判定</returns>
    public override List<PrefabParticleAssetInfo> CollectAssetInfoList()
    {
        var assets_root = Path.Combine(Application.dataPath, _settings.assetsRootPath);

        var list = new List<PrefabParticleAssetInfo>();

        var      pattern = "*.prefab";
        string[] files   = Directory.GetFiles(assets_root, pattern, SearchOption.AllDirectories); // 获取所有预制文件

        foreach (var file in files) // 遍历所有预制文件路径集合
        {
            var info = new PrefabParticleAssetInfo(); // 信息对象初始化

            var fileInfo = new FileInfo(file);
            if (fileInfo != null)
            {
                info.filesize = fileInfo.Length; // 文件空间大小
            }

            var projectPath = file.Replace(Application.dataPath, "Assets"); // 项目路径，Asset/开头
            info.foldName = projectPath;                                    // 文件名【项目路径Asset开头】
            info.filePath = file;                                           // 文件路径

            info.openCastShadows       = false; //是否打开投射阴影
            info.openReceiveShadows    = false; //是否打开接收阴影
            info.openLightProbes       = false; //是否打开光照探针
            info.openReflectionProbes  = false; //是否打开反射探针
            info.isDefaultMaxParticles = false; //是否修复默认1000个最大粒子数
            info.isCloseRenderer       = false; //是否关闭了Renderer
            info.isOpenPrewarm         = false; //是否关闭了Prewarm
            info.isOpenCollision       = false; //是否关闭了Collision
            info.isOpenTrigger         = false; //是否关闭了Trigger
            info.isOver30MaxParticles  = false; //是否超过30个最大粒子数
            info.isOverBurstsCount     = false; //是否超过5个粒子发射数
            info.isCloseReadable       = false; //是否可读写
            info.isOverTrianglesCount  = false; //是否超过500面数

            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(projectPath).transform; // 预制的文件信息
            if (obj == null)
            {
                continue;
            }

            if (obj.GetComponentsInChildren<ParticleSystem>().Length == 0) // 排除非粒子预制
            {
                continue;
            }

            _CheckDefaultMaxParticles(obj, info); // 检测每个节点是否 优化最大粒子数
            _CheckOver30MaxParticles(obj, info);  // 检测每个节点是否 MaxParticles超过30个
            _CheckPrewarm(obj, info);             // 检测每个节点是否 打开Prewarm
            _CheckCollision(obj, info);           // 检测每个节点是否 打开碰撞器
            _CheckTrigger(obj, info);             // 检测每个节点是否 打开触发器
            _CheckRenderer(obj, info);            // 检测每个节点是否 打开渲染
            _CheckCastShadows(obj, info);         // 检测每个节点是否 打开投射阴影
            _CheckReceiveShadows(obj, info);      // 检测每个节点是否 打开接收阴影
            _CheckLightProbes(obj, info);         // 检测每个节点是否 打开光照探针
            _CheckReflectionProbes(obj, info);    // 检测每个节点是否 打开反射探针
            _CheckTrianglesCount(obj, info);      // 检测每个节点 模型面数
            _CheckReadable(obj, info);            // 检测每个节点是否 可读写
            _CheckBurstsCount(obj, info);         // 检测每个节点 发射粒子数

            list.Add(info);
        }

        list = list.OrderBy((info) => { return info.foldName; }).ToList();
        return list;
    }

    private static void _CheckDefaultMaxParticles(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            info.isDefaultMaxParticles = child.main.maxParticles == 1000; // 如果MaxParticles默认是1000,就记录一下，待修复状态
            if (info.isDefaultMaxParticles)
                break;
        }
    }

    private static void _CheckOver30MaxParticles(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            info.isOver30MaxParticles = child.main.maxParticles > 30; // 如果MaxParticles超过30个,就记录一下，待修复状态
            if (info.isOver30MaxParticles)
                break;
        }
    }

    private static void _CheckPrewarm(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            info.isOpenPrewarm = child.main.prewarm; // 如果Prewarm是打开的,就记录一下，待修复状态
            if (info.isOpenPrewarm)
                break;
        }
    }

    private static void _CheckCollision(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            info.isOpenCollision = child.collision.enabled; // 如果Collision是打开的,就记录一下，待修复状态
            if (info.isOpenCollision)
                break;
        }
    }

    private static void _CheckTrigger(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            info.isOpenTrigger = child.trigger.enabled; // 如果Trigger是打开的,就记录一下，待修复状态
            if (info.isOpenTrigger)
                break;
        }
    }

    private static void _CheckRenderer(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            ParticleSystemRenderer renderComp = child.GetComponent<ParticleSystemRenderer>();
            info.isCloseRenderer = renderComp.enabled == false &&
                                   renderComp.renderMode !=
                                   ParticleSystemRenderMode.None; // 如果Renderer组件关闭，则为需设置RenderMode为空,就记录一下，待修复状态
            if (info.isCloseRenderer)
                break;
        }
    }

    private static void _CheckCastShadows(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            ParticleSystemRenderer renderComp = child.GetComponent<ParticleSystemRenderer>();

            info.openCastShadows =
                renderComp.shadowCastingMode != ShadowCastingMode.Off; // 如果投射阴影设置不为关闭状态,就记录一下，待修复状态
            if (info.openCastShadows)
                break;
        }
    }

    private static void _CheckReceiveShadows(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            ParticleSystemRenderer renderComp = child.GetComponent<ParticleSystemRenderer>();
            info.openReceiveShadows = renderComp.receiveShadows; // 如果投射阴影设置不为关闭状态,就记录一下，待修复状态
            if (info.openReceiveShadows)
                break;
        }
    }

    private static void _CheckLightProbes(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            ParticleSystemRenderer renderComp = child.GetComponent<ParticleSystemRenderer>();
            info.openLightProbes = renderComp.lightProbeUsage != LightProbeUsage.Off; // 如果投射阴影设置不为关闭状态,就记录一下，待修复状态
            if (info.openLightProbes)
                break;
        }
    }

    private static void _CheckReflectionProbes(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            ParticleSystemRenderer renderComp = child.GetComponent<ParticleSystemRenderer>();
            info.openReflectionProbes =
                renderComp.reflectionProbeUsage != ReflectionProbeUsage.Off; // 如果投射阴影设置不为关闭状态,就记录一下，待修复状态
            if (info.openReflectionProbes)
                break;
        }
    }

    private static void _CheckTrianglesCount(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            ParticleSystemRenderer renderComp = child.GetComponent<ParticleSystemRenderer>();
            if (!renderComp.enabled || renderComp.renderMode != ParticleSystemRenderMode.Mesh) // 如果渲染模式是网格类型
                continue;
            if (renderComp.mesh != null) ; // 如果有设置模型才继续
            {
                string assetPath     = AssetDatabase.GetAssetPath(renderComp.mesh);
                var    loadAssetPath = assetPath.Replace(Application.dataPath, "Assets");
                var    gameObj       = AssetDatabase.LoadAssetAtPath<GameObject>(loadAssetPath);
                if (gameObj == null)
                    continue;
                var mf = gameObj.GetComponent<MeshFilter>(); // 用这种方式才能获得正确的面数
                if (mf == null)
                    continue;
                if (mf.sharedMesh == null)
                    continue;
                if (mf.sharedMesh.triangles.Length > 500) // 如果面数大于500
                    info.isOverTrianglesCount = true;     // 就记录一下，待修复状态
            }
            if (info.isOverTrianglesCount)
                break;
        }
    }

    private static void _CheckReadable(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            ParticleSystemRenderer renderComp = child.GetComponent<ParticleSystemRenderer>();
            if (!renderComp.enabled || renderComp.renderMode != ParticleSystemRenderMode.Mesh) // 如果渲染模式是网格类型
                continue;
            if (renderComp.mesh != null) ; // 如果有设置模型才继续
            {
                string assetPath     = AssetDatabase.GetAssetPath(renderComp.mesh);
                var    loadAssetPath = assetPath.Replace(Application.dataPath, "Assets");
                var    gameObj       = AssetDatabase.LoadAssetAtPath<GameObject>(loadAssetPath);
                if (gameObj == null)
                    continue;
                var mf = gameObj.GetComponent<MeshFilter>(); // 用这种方式才能获得正确的面数
                if (mf == null)
                    continue;

                ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (modelImporter == null)
                    continue;

                info.isCloseReadable = !modelImporter.isReadable; // 就记录一下，待修复状态
            }

            if (info.isCloseReadable)
                break;
        }
    }

    private static void _CheckBurstsCount(Transform obj, PrefabParticleAssetInfo info)
    {
        foreach (var child in obj.GetComponentsInChildren<ParticleSystem>()) //获得预制下的所有子对象包括自身对象上的组件
        {
            if (!child.emission.enabled) // 如果没开启发射器，就跳过
                continue;
            int burstsCount = 0; // 发射器粒子数统计
            for (int i = 0; i < child.emission.burstCount; i++)
            {
                ParticleSystem.Burst bursts = child.emission.GetBurst(i);
                burstsCount += (int) bursts.count.constant;
            }

            info.isOverBurstsCount = burstsCount > 5; // 粒子发射数量如果大于5,就记录一下，待修复状态
            if (info.isOverBurstsCount)
                break;
        }
    }
}
