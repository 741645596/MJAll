// @Author: futianfu
// @Date: 2021/8/2 10:49:33


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 图集专有属性
/// </summary>
public class AtlasAssetInfo : AssetInfoBase
{
    public string       foldPath;  //当前目录的全路径
    public List<string> childFile; //当前目录下的所有子文件


    public AtlasAssetInfo()
    {
    }
}
