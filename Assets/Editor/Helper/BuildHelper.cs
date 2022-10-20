using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Unity.Utility;
using WLCore.Helper;

public static class BuildHelper
{
    #region 一键打标签【非通用，含有业务逻辑，如果要更换Ab包位置需要维护】

    /// <summary>
    /// 设置所有模块的标签
    /// </summary>
    /// <param name="path"> 从哪个文件夹开始打标签 【例如：FileHelper.gameAssetsPath 就是全部模块打标签】 </param>
    public static void SetAllMoudleAbName(string path)
    {
        //清除AB包名
        ClearAssetBundlesName();

        //全平台打标签
        //子模块内的模块路径【例如：folder = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common】
        List<string> folders       = FileHelper.GetFolder(path);
        List<string> filteredFiles = FileHelper.GetIgnore(folders); //忽略一些文件
        foreach (var folder in filteredFiles)                       //遍历一级模块
        {
            _ForeachSecondModel(folder); //遍历二级模块
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 设置单个模块的标签
    /// </summary>
    /// <param name="path"> 从哪个文件夹开始打标签 【例如：FileHelper.gameAssetsPath 就是全部模块打标签】 </param>
    public static void SetMoudleAbName(string path)
    {
        ClearAssetBundlesName();   //清除AB包名
        _ForeachSecondModel(path); //遍历二级模块
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 遍历二级模块
    /// </summary>
    /// <param name="path"> 子模块内的模块路径 </param>
    private static void _ForeachSecondModel(string path)
    {
        //子模块内的模块路径【例如：folder = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common】
        List<string> folders       = FileHelper.GetFolder(path);
        List<string> filteredFiles = FileHelper.GetIgnore(folders); //忽略一些文件
        foreach (var folder in filteredFiles)
        {
            _ForeachABFile(folder); //遍历子模块内的ab包文件夹，并打AB包
        }
    }

    /// <summary>
    /// 遍历子模块内的ab包文件夹，并打AB包
    /// </summary>
    /// <param name="foldPath"> 子模块内的模块路径 </param>
    private static void _ForeachABFile(string foldPath)
    {
        //子模块内的模块路径【例如：childFolders[i] = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common/Module1/_atlas】
        List<string> childFolders = FileHelper.GetFolder(foldPath);
        foreach (var cFolder in childFolders) //cFolder.Name = ab包名
        {
            _SetFoldAbName(Path.GetFileName(cFolder), cFolder); //设置文件夹内的文件标签名
        }
    }

    /// <summary>
    /// 设置文件夹内的文件标签名
    /// </summary>
    /// <param name="foldName"> ab名 </param>
    /// <param name="foldPath"> ab包路径  </param>
    private static void _SetFoldAbName(string foldName, string foldPath)
    {
        List<string> childFiles    = FileHelper.GetAllFileAndFolder(foldPath); //获得所有文件和文件夹【包含子目录的文件和文件夹】
        List<string> filteredFiles = FileHelper.GetIgnore(childFiles);         //忽略一些文件
        foreach (var item in filteredFiles)                                    //遍历每个ab包文件
        {
            //设置单个AssetBundle的Name
            string modelPath = PathHelper.GetModelPath(foldPath);
            string substring = modelPath.Substring(1);
            string abPath    = PathHelper.GetSecondPath(substring);
            string assetPath = PathHelper.PathFormat(item);
            SetAbName(assetPath, abPath);

            //设置合图
            _SetSpritePackge(assetPath, foldName, "_atlas");
        }
    }

    #endregion

    /// <summary>
    /// 清除之前设置过的AssetBundleName【通用】
    /// </summary>
    public static void ClearAssetBundlesName()
    {
        //清除未使用的ab包名
        AssetDatabase.RemoveUnusedAssetBundleNames();

        //有坑,如果打开会出现把所有标签的ab包打到一个文件夹情况
        //打包之前删除所有ab包名
        foreach (string item in AssetDatabase.GetAllAssetBundleNames())
        {
            AssetDatabase.RemoveAssetBundleName(item, true);
        }
    }

    /// <summary>
    /// 设置单个AssetBundle的Name【通用】
    /// </summary>
    /// <param name="assetPath">【例如 D:\Users\admin\Desktop\JiaXiangGit2\UnityRuntime\Assets\GameAssets\Common\Module1\demo_atlas\bind_icon.png】 </param>
    /// <param name="abPath"> ab包名 【例如 cFolder = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common/Module1/demo_atlas】 </param>
    public static void SetAbName(string assetPath, string abPath)
    {
        string pcDir = PathHelper.PathFormat(assetPath);
        //路径2：Assets\GameAssets\Common\Module1\demo_atlas\bind_icon.png
        string        importerPath  = PathHelper.GetProjectPath(pcDir);      //这个路径必须是以Assets开始的路径
        AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath); //得到Asset
        if (null == assetImporter)
        {
            return;
        }

        assetImporter.assetBundleName    = abPath;
        assetImporter.assetBundleVariant = "";

        //pcDir = D:\Users\admin\Desktop\JiaXiangGit2\UnityRuntime\Assets\GameAssets\Common\Module1\demo_atlas\bind_icon.png
        // WLDebug.Log("打标签命名文件路径:" + pcDir);
    }

    /// <summary>
    /// 设置合图【通用】
    /// </summary>
    /// <param name="assetPath"> 要设置合图的文件路径 </param>
    /// <param name="packgeName"> 要设置的合图名称 </param>
    /// <param name="include"> 要设置合图的文件夹是否包含的字符串，如果不需要请设置成"" </param>
    private static void _SetSpritePackge(string assetPath, string packgeName, string include)
    {
        string pcDir = PathHelper.PathFormat(assetPath);
        //路径2：Assets\GameAssets\Common\Module1\demo_atlas\bind_icon.png
        string        importerPath  = PathHelper.GetProjectPath(pcDir);      //这个路径必须是以Assets开始的路径
        AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath); //得到Asset
        if (null == assetImporter)
        {
            return;
        }

        TextureImporter textureImporter = assetImporter as TextureImporter;
        if (textureImporter == null)
        {
            return;
        }

        //这里要判断一下，是否文件夹带_atlas后缀
        if (packgeName.Contains(include))
        {
            // 打图集
            textureImporter.spritePackingTag = packgeName;
        }
        else
        {
            //清除其他不用打图集的标签名
            textureImporter.spritePackingTag = "";
        }
    }

    /// <summary>
    /// 开始资源打包【通用】
    /// </summary>
    /// <param name="buildTarget"> 运行平台 【例如：android】</param>
    /// <param name="buildAssetBundleOptions"> ab包选项 </param>
    /// <param name="assetPath"> 要打包的模块路径【例如：D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common/】</param>
    public static void BuildAB(BuildTarget buildTarget, BuildAssetBundleOptions buildAssetBundleOptions,
        string                             assetPath)
    {
        WLDebug.Log("开始资源打包");

        BuildPipeline.BuildAssetBundles(assetPath, buildAssetBundleOptions, buildTarget);

        WLDebug.Log("AB包生成路径：", assetPath);
        WLDebug.Log("完成资源打包");
    }
}
