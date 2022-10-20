// AudioChecker.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/08

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 音频资源检测器
/// </summary>
public class AudioChecker : AssetsCheckerBase<AudioAssetInfo>
{
    private AssetsCheckEditorWindow.Settings settings;

    public AudioChecker(AssetsCheckEditorWindow.Settings settings)
    {
        this.settings = settings;
    }

    public override List<AudioAssetInfo> CollectAssetInfoList()
    {
        var assets_root = Path.Combine(Application.dataPath, settings.assetsRootPath);
        var audio_types = settings.audioType.Split(',');

        var list = new List<AudioAssetInfo>();

        for (int i = 0; i < audio_types.Length; i++)
        {
            var      pattern = $"*.{audio_types[i]}";
            string[] files   = Directory.GetFiles(assets_root, pattern, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var info     = new AudioAssetInfo();
                var f        = file.Replace(Application.dataPath, "Assets");
                var importer = AssetImporter.GetAtPath(f) as AudioImporter;

                if (importer == null)
                {
                    continue;
                }

                if (fileInfo != null)
                {
                    info.filesize = fileInfo.Length;
                }

                info.foldName = f;
                info.isMono   = importer.forceToMono;
                info.loadType = importer.defaultSampleSettings.loadType;
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(f);
                if (clip != null)
                {
                    info.audioLength = clip.length;
                }

                list.Add(info);
            }
        }

        return list;
    }
}
