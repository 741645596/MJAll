// DOBaseNode.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/20
using DG.Tweening;
using UnityEngine;
using XNode;

public abstract class DOBaseNode : Node
{
    [Output(backingValue = ShowBackingValue.Never)] public string next;
    [Input(backingValue = ShowBackingValue.Never)] public string prev;


    public DOBaseNode GetNextNode()
    {
        var port = GetOutputPort("next");
        if (port == null || port.Connection == null)
        {
            return null;
        }
        return port.Connection.node as DOBaseNode;
    }

    public virtual Tween GenerateTween(Transform transform)
    {
        return null;
    }

    /// <summary>
    /// 回滚之前状态，为了方便在预设阶段重置状态
    /// </summary>
    /// <param name="transform"></param>
    public virtual void Rollback(Transform transform)
    {

    }
}
