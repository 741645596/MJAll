// @Author: tanjinhua
// @Date: 2021/8/12  15:12


using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Core;
using Unity.Widget;
using UnityEngine;

public static class TransformExtension
{
    /// <summary>
    /// 获取第一层的所有节点
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static List<Transform> GetChildren(this Transform transform)
    {
        var result = new List<Transform>();
        var count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            result.Add(transform.GetChild(i));
        }

        return result;
    }

    /// <summary>
    /// 获取所有的子节点
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static List<Transform> GetAllChildren(this Transform transform)
    {
        var result = new List<Transform>();

        _AddChildren(result, transform);

        return result;
    }

    /// <summary>
    /// 移除所有子节点
    /// </summary>
    /// <param name="transform"></param>
    public static void RemoveAllChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 查找sunTransform在父节点下的索引值，找不到返回-1
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="sunTransform"></param>
    /// <returns></returns>
    public static int FindChildIndex(this Transform transform, Transform sunTransform)
    {
        var count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            if (transform.GetChild(i) == sunTransform)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 将该节点移到兄弟节点borther的上面
    /// </summary>
    /// <param name="borther"></param>
    /// <returns></returns>
    public static void MoveToAbove(this Transform transform, Transform borther)
    {
        var parent = transform.parent;
        if (parent == null)
        {
            WLDebug.LogWarning("错误提示：MoveToAbove父节点不存在，请检查代码");
            return;
        }

        var index = parent.FindChildIndex(borther);
        if (index == -1)
        {
            return;
        }

        // 自己如果已经在兄弟节点前面需要 - 1
        var selfIndex = parent.FindChildIndex(transform);
        var res = index < selfIndex ? index : index - 1;
        transform.SetSiblingIndex(res);
    }

    /// <summary>
    /// 将该节点移到兄弟节点borther的下面
    /// </summary>
    /// <param name="borther"></param>
    /// <returns></returns>
    public static void MoveToBelow(this Transform transform, Transform borther)
    {
        var parent = transform.parent;
        if (parent == null)
        {
            WLDebug.LogWarning("错误提示：MoveToBelow父节点不存在，请检查代码");
            return;
        }

        var index = parent.FindChildIndex(borther);
        if (index == -1)
        {
            return;
        }

        // 自己如果已经在兄弟节点前面需要 - 1
        var selfIndex = parent.FindChildIndex(transform);
        var res = index < selfIndex ? index + 1 : index;
        transform.SetSiblingIndex(res);
    }

    /// <summary>
    /// 将该节点移到第1个位置
    /// </summary>
    /// <param name="transform"></param>
    public static void MoveToTop(this Transform transform)
    {
        transform.SetAsFirstSibling();
    }

    /// <summary>
    /// 将该节点移到最后一个位置
    /// </summary>
    /// <param name="transform"></param>
    public static void MoveToBottom(this Transform transform)
    {
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// 建议使用该接口，只查找第一层子节点，不会递归子节点查询，childName可以包含子节点，如："FirstNodeName/SubNodeName"
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static Transform FindInChildren(this Transform transform, string childName)
    {
        return transform.Find(childName);
    }

    /// <summary>
    /// 建议使用该接口，只查找第一层子节点，不会递归子节点查询，childName可以包含子节点，如："FirstNodeName/SubNodeName"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static T FindInChildren<T>(this Transform transform, string childName) where T : Component
    {
        var child = transform.Find(childName);
        if (child != null)
        {
            return child.GetComponent<T>();
        }

        return default;
    }

    /// <summary>
    /// 建议使用该接口，只查找第一层子节点，不会递归子节点查询，childName可以包含子节点，如："FirstNodeName/SubNodeName"
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static Transform FindInChildren(this Transform transform, Func<Transform, bool> condition)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (condition(child))
            {
                return child;
            }
        }
        return null;
    }

    /// <summary>
    /// 建议使用该接口，只查找第一层子节点，不会递归子节点查询，childName可以包含子节点，如："FirstNodeName/SubNodeName"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static T FindInChildren<T>(this Transform transform, Func<Transform, bool> condition) where T : Component
    {
        var child = transform.FindInChildren(condition);
        if (child != null)
        {
            return child.GetComponent<T>();
        }

        return default;
    }

    /// <summary>
    /// 递归查找子节点，优先建议使用FindInChildren
    /// </summary>
    /// <param name="transform">当前变换组件</param>
    /// <param name="childName">子节点物体的名称</param>
    /// <returns></returns>
    public static Transform FindReference(this Transform transform, string childName)
    {
        //1.通过子物体名称在子物体中查找变换组件
        Transform child = transform.Find(childName);
        if (child != null)
        {
            return child;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            //2.将任务交给子物体,子物体又查找子物体的子物体
            var child2 = FindReference(transform.GetChild(i), childName);
            if (child2 != null)
            {
                return child2;
            }
        }
        return null;
    }

    /// <summary>
    /// 根据自定义条件超找目标，优先建议使用FindInChildren
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static Transform FindReference(this Transform transform, Func<Transform, bool> condition)
    {
        List<Transform> children = transform.GetAllChildren();
        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (condition(child))
            {
                return child;
            }
        }
        return null;
    }

    /// <summary>
    /// 根据名称获取子节点的组件，优先建议使用FindInChildren
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindReference<T>(this Transform transform, string name) where T : Component
    {
        var child = transform.FindReference(name);
        if (child != null)
        {
            return child.GetComponent<T>();
        }

        return default;
    }

    /// <summary>
    /// 根据自定义条件获取子节点的组件，优先建议使用FindInChildren
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static T FindReference<T>(this Transform transform, Func<Transform, bool> condition) where T : Component
    {
        var child = transform.FindReference(condition);
        if (child != null)
        {
            return child.GetComponent<T>();
        }

        return default;
    }

    /// <summary>
    /// 如果GameObject是通过WNode3D或继承WNode3D创建的，可以通过该方法反向找到继承的类
    /// 通过Find的方法多少有点性能问题，请不要在Update这类热点函数内调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static T FindNode<T>(this Transform transform, string childName) where T : WNode3D
    {
        var tran = transform.FindInChildren(childName);
        if (tran == null)
        {
            return default;
        }

        var com = tran.GetComponent<WMonoEvent>();
        if (com != null)
        {
            return com.self as T;
        }
        return default;
    }

    /// <summary>
    /// 播放TweenGraph动画
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="asset"></param>
    /// <param name="key"></param>
    public static Sequence RunTweenGraph(this Transform transform, string asset, string key)
    {
        var root = _CreateRootNode(asset, key);
        if (root == null)
        {
            return null;
        }

        var seq = transform.CreateTweenGraphSequence(root);
        seq.Play();
        return seq;
    }

    /// <summary>
    /// 播放TweenGraph动画，带动画结束回调
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="asset"></param>
    /// <param name="key"></param>
    /// <param name="finish"> 动画结束回调 </param>
    public static Sequence RunTweenGraph(this Transform transform, string asset, string key, Action<Transform> finish)
    {
        Debug.Assert(finish != null, "finish参数不能为空");

        var root = _CreateRootNode(asset, key);
        if (root == null)
        {
            return null;
        }

        var seq = transform.CreateTweenGraphSequence(root);
        seq.onComplete += () =>
        {
            try
            {
                finish?.Invoke(transform);
            }
            catch (Exception ex)
            {
                WLDebug.LogError(ex.ToString());
            }
        };
        seq.Play();

        return seq;
    }

    /// <summary>
    /// 播放dotween graph序列动画，指定动作事件回调
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="asset"></param>
    /// <param name="key"></param>
    /// <param name="events"> Do Event事件回调字典key:事件名称 value:回调方法 </param>
    public static Sequence RunTweenGraph(this Transform transform, string asset, string key, Dictionary<string, Action> events)
    {
        Debug.Assert(events != null, "events参数不能为空");

        var graph = AssetsManager.Load<DOTweenGraph>(asset, key);
        if(graph == null)
        {
            Debug.LogWarning($"graph is null, asset:{asset} key:{key}");
            return null;
        }

        var root = graph.GetRootNode();
        if(root == null)
        {
            Debug.LogWarning($"graph root is null, asset:{asset} key:{key}");
            return null;
        }

        foreach (var item in events)
        {
            graph.SetEvent(item.Key, item.Value);
        }

        var seq = transform.CreateTweenGraphSequence(root);
        seq.Play();
        return seq;
    }

    /// <summary>
    /// 内部自己用，一般用不上
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public static Sequence CreateTweenGraphSequence(this Transform transform, DORootNode root)
    {
        var sequence = DOTween.Sequence();
        var tweenNode = root.GetOutputPort("next").Connection.node as DOBaseNode;
        while (tweenNode != null)
        {
            var tween = tweenNode.GenerateTween(transform);
            if (tween != null)
            {
                sequence.Append(tween);
            }
            tweenNode = tweenNode.GetNextNode();
        }
        sequence.SetTarget(transform.gameObject);
        sequence.SetLink(transform.gameObject);
        sequence.SetAutoKill(true);
        return sequence;
    }

    private static DORootNode _CreateRootNode(string asset, string key)
    {
        var graph = AssetsManager.Load<DOTweenGraph>(asset, key);
        if (graph == null)
        {
            Debug.LogWarning($"graph is null, asset:{asset} key:{key}");
            return null;
        }

        var root = graph.GetRootNode();
        if (root == null)
        {
            Debug.LogWarning($"graph root is null, asset:{asset} key:{key}");
            return null;
        }
        return root;
    }

    private static void _AddChildren(List<Transform> resultTrs, Transform curTrs)
    {
        var count = curTrs.childCount;
        for (int i = 0; i < count; i++)
        {
            var t = curTrs.GetChild(i);
            resultTrs.Add(t);

            _AddChildren(resultTrs, t);
        }
    }
}
