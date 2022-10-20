// AudioAssetInfo.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/09

using UnityEngine;

/// <summary>
/// 音频资源信息
/// </summary>
public class AudioAssetInfo : AssetInfoBase
{
    public bool isMono;
    public float audioLength;
    public AudioClipLoadType loadType;
}
