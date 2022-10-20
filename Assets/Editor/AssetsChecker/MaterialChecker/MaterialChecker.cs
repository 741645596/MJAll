// MaterialChecker.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/20

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class MaterialChecker : AssetsCheckerBase<MaterialAssetInfo>
{
    private AssetsCheckEditorWindow.Settings settings;

    public MaterialChecker(AssetsCheckEditorWindow.Settings settings)
    {
        this.settings = settings;
    }

    public override List<MaterialAssetInfo> CollectAssetInfoList()
    {
        var assets_root = Path.Combine(Application.dataPath, settings.assetsRootPath);

        string[] files = Directory.GetFiles(assets_root, "*.mat", SearchOption.AllDirectories);
        var      list  = new List<MaterialAssetInfo>();
        foreach (var file in files)
        {
            var f        = file.Replace(Application.dataPath, "Assets");
            var material = AssetDatabase.LoadAssetAtPath<Material>(f);

            var info = new MaterialAssetInfo();
            if (material != null)
            {
                var shader = material.shader;
                var count  = shader.GetPropertyCount();
                for (var i = 0; i < count; i++)
                {
                    if (shader.GetPropertyType(i) == ShaderPropertyType.Texture)
                    {
                        var name    = shader.GetPropertyName(i);
                        var texture = material.GetTexture(name);

                        if (texture == null)
                        {
                            info.hasEmptyTexture = true;
                            Debug.Log($"{f} {name} is null");
                            break;
                        }
                    }
                }
            }

            info.foldName = f;
            var fileInfo = new FileInfo(file);
            if (fileInfo != null)
            {
                info.filesize = fileInfo.Length;
            }

            list.Add(info);
        }

        list = list.OrderBy((info) => { return info.foldName; }).ToList();

        return list;
    }
}
