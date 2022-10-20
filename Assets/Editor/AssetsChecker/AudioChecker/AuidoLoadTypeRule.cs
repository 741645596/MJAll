// AudioLoadTypeRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/09

using UnityEditor;
using UnityEngine;

/// <summary>
/// 音频加载类型规则：时长大于5秒设置为Streaming，时长小于5秒设置为DecompressOnLoad
/// 
/// DecompressOnLoad:   The audio data is decompressed when the audio clip is loaded.
/// CompressedInMemory: The audio data of the clip will be kept in memory in compressed form.
/// Streaming:          Streams audio data from disk.
/// </summary>
public class AudioLoadTypeRule : AssetRuleBase
{
    private readonly float TIME_THRESHOLD = 5f;
    public override string Description()
    {
        return $"时长大于{TIME_THRESHOLD}秒设置为Streaming，时长小于{TIME_THRESHOLD}秒设置为DecompressOnLoad";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is AudioAssetInfo == false)
        {
            tips = "检测错误";
            return false;
        }

        if (((AudioAssetInfo)info).audioLength >= TIME_THRESHOLD)
        {
            if (((AudioAssetInfo)info).loadType != AudioClipLoadType.Streaming)
            {
                tips = "加载类型需要设置为Streaming";
                return false;
            }
        }
        else
        {
            if (((AudioAssetInfo)info).loadType != AudioClipLoadType.DecompressOnLoad)
            {
                tips = "加载类型需要设置为DecompressOnLoad";
                return false;
            }
        }
        return true;
    }

    public override void Fix(AssetInfoBase info)
    {
        if (info is AudioAssetInfo == false)
        {
            return;
        }

        var importer = AssetImporter.GetAtPath(info.foldName) as AudioImporter;
        var audioInfo = info as AudioAssetInfo;
        if (importer != null)
        {
            var newSampleSettings = importer.defaultSampleSettings;
            // 设置加载类型
            if (audioInfo.audioLength > TIME_THRESHOLD)
            {
                newSampleSettings.loadType = AudioClipLoadType.Streaming;
            }
            else
            {
                newSampleSettings.loadType = AudioClipLoadType.DecompressOnLoad;
            }

            importer.defaultSampleSettings = newSampleSettings;
            importer.SaveAndReimport();
        }
    }
}