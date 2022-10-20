// AudioMonoRule.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/09

using UnityEditor;

/// <summary>
/// 音频格式规则：音频设置需要设置为单声道
/// </summary>
public class AudioMonoRule : AssetRuleBase
{
    public override string Description()
    {
        return "音频设置需要设置为单声道";
    }

    public override bool Pass(AssetInfoBase info)
    {
        if (info is AudioAssetInfo == false)
        {
            tips = "检测错误";
            return false;
        }

        if (((AudioAssetInfo)info).isMono == true)
        {
            return true;
        }
        else
        {
            tips = "音频资源须设置为单声道";
            return false;
        }
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
            // 设置单声道
            if (audioInfo.isMono == false)
            {
                importer.forceToMono = true;
            }

            var newSampleSettings = importer.defaultSampleSettings;

            SerializedObject serializedObject = new SerializedObject(importer);
            SerializedProperty normalize = serializedObject.FindProperty("m_Normalize");
            if (normalize.boolValue == true)
            {
                normalize.boolValue = false;
                serializedObject.ApplyModifiedProperties();
            }

            importer.defaultSampleSettings = newSampleSettings;
            importer.SaveAndReimport();
        }
    }
}