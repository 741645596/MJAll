// AssetInfoBase.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/08

using System.Collections.Generic;

/// <summary>
/// 资源信息基类
/// </summary>
public class AssetInfoBase : IComparer<AssetInfoBase>
{
    public string                     foldName; //称项目内的路径，Asset开头
    public string                     filePath; //文件路径
    public long                       filesize; //
    public Dictionary<string, string> ruleAndErrorDescriptions;

    public int Compare(AssetInfoBase x, AssetInfoBase y)
    {
        return 0;
    }
}
