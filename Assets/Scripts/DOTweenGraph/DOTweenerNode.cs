// DOTweenerNode.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/10
using DG.Tweening;

public abstract class DOTweenerNode : DOBaseNode
{
    public float delay = 0;
    public float time;
    public Ease ease = Ease.Linear;
}
