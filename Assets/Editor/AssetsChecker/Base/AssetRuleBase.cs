// AssetRuleBase.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2020/07/08

/// <summary>
///  资源规则基类
/// </summary>
public abstract class AssetRuleBase
{
    protected string tips;

    public string Info() {
        return tips;
    }

    public abstract string Description();

    public abstract bool Pass(AssetInfoBase info);

    public abstract void Fix(AssetInfoBase info);

    public virtual bool CanFix()
    {
        return true;
    }
}
