using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Unity.Core;

public static class SpineHelper
{
    /// <summary>
    /// 动态创建Spine Object
    /// 编辑器创建方法：右键 -> Spine -> SkeletonGraphic(UnityUI)，记得将Layer改为UI
    /// </summary>
    /// <param name="assertName"> AB包名 </param>
    /// <param name="matKey"> spine的mat文件名，包含后缀 </param>
    /// <param name="assetKey"> spine的asset文件，包含后缀 </param>
    /// <returns></returns>
    public static GameObject Create(string assertName, string matKey, string assetKey)
    {
        GameObject gObject = ObjectHelper.CreateEmptyObject();
        gObject.name = "SpineObject";

        var material = AssetsManager.Load<UnityEngine.Material>(assertName, matKey);
        var skeleton = AssetsManager.Load<SkeletonDataAsset>(assertName, assetKey);
        if (material==null || skeleton==null)
        {
            Debug.LogWarning($"错误提示：{assertName}的资源{matKey}或{assetKey}不存在，请检查资源是否正确");
            return gObject;
        }
        
        var skeletonGraphic = gObject.AddComponent<SkeletonGraphic>();
        skeletonGraphic.material = material;
        skeletonGraphic.skeletonDataAsset = skeleton;
        skeletonGraphic.Initialize(false);
        skeletonGraphic.raycastTarget = false;
        return gObject;
    }

    /// <summary>
    /// 播放spine动作
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="animationName"></param>
    /// <param name="loop"></param>
    public static void SetAnimation(GameObject obj, string animationName, bool loop)
    {
        var skeGra = obj.GetComponent<SkeletonGraphic>();
        if (skeGra == null)
        {
            Debug.LogWarning($"错误提示：SpineHelper.SetAnimation 参数obj不带有组件{animationName}，请检查代码和资源是否有误");
            return;
        }
        skeGra.AnimationState.SetAnimation(0, animationName, loop);
    }
}
