// DORootNode.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/10
using XNode;

public class DORootNode : Node
{
	[Output(backingValue = ShowBackingValue.Never)] public string next;
}