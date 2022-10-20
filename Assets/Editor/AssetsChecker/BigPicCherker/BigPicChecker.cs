// @Author: futianfu
// @Date: 2021/9/17 10:49:33

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 大图检测逻辑
/// </summary>
public class BigPicChecker : AssetsCheckerBase<BigPicAssetInfo>
{
    private AssetsCheckEditorWindow.Settings _settings;

    public BigPicChecker(AssetsCheckEditorWindow.Settings settings)
    {
        this._settings = settings;
    }

    /// <summary>
    /// 收集文件信息
    /// </summary>
    /// <returns>例：图片宽高，文件的项目路径，文件全路径，是否被修复，压缩格式</returns>
    public override List<BigPicAssetInfo> CollectAssetInfoList()
    {
        var assets_root = Path.Combine(Application.dataPath, _settings.assetsRootPath);
        var textureType = _settings.textureType.Split(',');

        var list = new List<BigPicAssetInfo>();

        for (int i = 0; i < textureType.Length; i++)
        {
            var      pattern = $"*.{textureType[i]}";
            string[] files   = Directory.GetFiles(assets_root, pattern, SearchOption.AllDirectories); //获取所有贴图文件

            foreach (var file in files)
            {
                var info     = new BigPicAssetInfo();                         //创建个贴图的信息对象
                var f        = file.Replace(Application.dataPath, "Assets");  //项目路径：Assets/......
                var importer = AssetImporter.GetAtPath(f) as TextureImporter; //贴图配置信息
                if (importer == null)
                {
                    continue;
                }

                var fileInfo = new FileInfo(file);
                if (fileInfo != null)
                {
                    info.filesize = fileInfo.Length; //文件空间大小
                }

                var obj = AssetDatabase.LoadAssetAtPath<Texture>(f); //贴图的文件信息
                if (obj != null)
                {
                    info.width  = obj.width;  //图片宽
                    info.height = obj.height; //图片高
                }

                info.foldName = f;     //文件名【项目路径Asset开头】
                info.filePath = file;  //文件路径
                info.isFixed  = false; //是否被修复过
                var android_setting = importer.GetPlatformTextureSettings("Android");
                var ios_setting     = importer.GetPlatformTextureSettings("iPhone");
                info.androidFormat = android_setting.format; //安卓压缩格式
                info.iosFormat     = ios_setting.format;     //ios压缩格式

                list.Add(info);
            }
        }

        list = list.OrderBy((info) => { return info.foldName; }).ToList();
        return list;
    }
}
