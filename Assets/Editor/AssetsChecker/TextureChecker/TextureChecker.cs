// TextureChecker.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/19

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TextureChecker : AssetsCheckerBase<TextureAssetInfo>
{
    private AssetsCheckEditorWindow.Settings settings;

    public TextureChecker(AssetsCheckEditorWindow.Settings settings)
    {
        this.settings = settings;
    }

    public override List<TextureAssetInfo> CollectAssetInfoList()
    {
        var assets_root = Path.Combine(Application.dataPath, settings.assetsRootPath);
        var textureType = settings.textureType.Split(',');

        var list = new List<TextureAssetInfo>();

        for (int i = 0; i < textureType.Length; i++)
        {
            var      pattern = $"*.{textureType[i]}";
            string[] files   = Directory.GetFiles(assets_root, pattern, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                var info = new TextureAssetInfo();
                var f    = file.Replace(Application.dataPath, "Assets");

                var importer = AssetImporter.GetAtPath(f) as TextureImporter;

                if (importer == null)
                {
                    continue;
                }

                if (fileInfo != null)
                {
                    info.filesize = fileInfo.Length;
                }

                info.foldName   = f;
                info.isReadable = importer.isReadable;

                var s = new FileStream(file, FileMode.Open);

                var obj = AssetDatabase.LoadAssetAtPath<Texture>(f);
                if (obj != null)
                {
                    info.width  = obj.width;
                    info.height = obj.height;
                }

                info.isMiamap   = importer.mipmapEnabled;
                info.filterMode = importer.filterMode;
                var android_setting = importer.GetPlatformTextureSettings("Android");
                info.androidTextureImportFormat = android_setting.format;
                var ios_setting = importer.GetPlatformTextureSettings("iPhone");
                info.iosTextureImportFormat = ios_setting.format;
                list.Add(info);

                s.Close();
                s.Dispose();
            }
        }

        list = list.OrderBy((info) => { return info.foldName; }).ToList();
        return list;
    }
}
