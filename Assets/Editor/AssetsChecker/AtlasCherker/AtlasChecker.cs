// @Author: futianfu
// @Date: 2021/8/2 10:53:47


using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Utility;
using WLCore.Helper;

/// <summary>
/// 图集检测逻辑
/// </summary>
public class AtlasChecker : AssetsCheckerBase<AtlasAssetInfo>
{
    private AssetsCheckEditorWindow.Settings _settings;

    public AtlasChecker(AssetsCheckEditorWindow.Settings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// 收集合图文件夹路径等信息
    /// </summary>
    /// <returns> 返回收集好的对象集合 </returns>
    public override List<AtlasAssetInfo> CollectAssetInfoList()
    {
        var list = new List<AtlasAssetInfo>();

        // assetsRoot = /Users/futianfu/UnityRuntime/Assets/GameAssets
        string       assetsRoot = Path.Combine(Application.dataPath, _settings.assetsRootPath);
        List<string> modulePath = FileHelper.GetFolder(assetsRoot);
        // item = /Users/futianfu/UnityRuntime/Assets/GameAssets/Common
        foreach (var item in modulePath)
        {
            // childModulePath = /Users/futianfu/UnityRuntime/Assets/GameAssets/Common/Module1
            List<string> childModulePath = FileHelper.GetFolder(item);
            List<string> ignorePath      = FileHelper.GetIgnore(childModulePath); //忽略一些文件
            foreach (var cItem in ignorePath)
            {
                // abItem = /Users/futianfu/UnityRuntime/Assets/GameAssets/Common/Module1/demo_atlas
                List<string> abPath = FileHelper.GetFolder(cItem);
                foreach (var abItem in abPath)
                {
                    //只查找文件以_atlas的文件夹内部
                    bool isUse = Path.GetFileName(abItem).Contains("_atlas");
                    if (!isUse)
                    {
                        continue;
                    }

                    string type = "*";
                    List<string>
                        files = FileHelper.GetAllFileAndFolder(abItem, type); //获取到指定目录下的所有子文件
                    List<string> ignore = FileHelper.GetIgnore(files);        //忽略一些文件

                    string projectPath = abItem.Replace(Application.dataPath, "Assets");

                    var info = new AtlasAssetInfo();
                    info.foldName  = Path.GetFileName(abItem); //文件夹名
                    info.foldPath  = abItem;                           //全路径
                    info.filePath  = projectPath;                      //项目内的路径，Asset开头
                    info.childFile = ignore;                           //在这里查找完内部子文件，然后存在字典里面

                    list.Add(info);
                }
            }
        }

        list = list.OrderBy((info) => info.foldName).ToList();
        return list;
    }
}
