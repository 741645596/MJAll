using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Unity.Utility;
using WLCore.Helper;


public static class PackAssetBundles
{
    private static int s_buildTime; //打包时间

    /// <summary>
    /// 多模块打热更包
    /// </summary>
    /// <param name="buildTarget"> 运行平台 【例如：android】</param>
    /// <param name="buildAssetBundleOptions"> ab包选项 </param>
    /// <param name="selectModels"> 要打包的模块【例如：{"全平台"，"Common"}】 </param>
    /// <param name="dllDir"></param>
    /// <param name="version"></param>
    public  static void MultiModuleBuildHotUpdate(BuildTarget buildTarget,
        BuildAssetBundleOptions                               buildAssetBundleOptions,
        List<string>                                          selectModels,
        int                                                   version)
    {
        foreach (var model in selectModels) //逐个模块打热更包
        {
            BuildHotUpdate(buildTarget, buildAssetBundleOptions, model, version);
        }
    }


    /// <summary>
    /// 单模块打热更包
    /// </summary>
    /// <param name="buildTarget"> 运行平台 【例如：android】</param>
    /// <param name="buildAssetBundleOptions"> ab包选项 </param>
    /// <param name="selectModel"> 要打包的模块【例如：{"全平台"，"Common"}】 </param>
    /// <param name="dllDir"></param>
    /// <param name="version"></param>
    public  static void BuildHotUpdate(BuildTarget buildTarget, BuildAssetBundleOptions buildAssetBundleOptions,
        string                                     selectModel, int                     version)
    {
        _SwitchPlatform(buildTarget); // 自动切换平台

        selectModel.ToLower();
        bool hotUpdate = true;

        // 构建时间 TODO:
        CalcBuildTime(version, hotUpdate);

        //查找每个模块之间Ab包是否重名
        if (_IsAbFoldRepeat(PathHelper.s_gameAssetsPath))
            return;

        // if (_IsAbFoldLower(FileHelper.s_gameAssetsPath))//查找AB文件名，Ab包所对应的文件夹名是否存在小写
        // return;

        // 打包资源
        _BuildAssetsBundle(buildTarget, buildAssetBundleOptions, selectModel);

        string outpath = string.Format("AssetBundles/{0}", selectModel);

        //1、判断是否打包出内容,selectModel是个
        if (!Directory.Exists(outpath))
        {
            Debug.LogError($"请先打包资源 {outpath}");
            return;
        }

        //2、拷贝一份到缓存文件夹 TODO:
        string tempPath = string.Format("AssetBundles/{0}_{1}_temp", buildTarget, selectModel); // 主包路径
        FileHelper. DelCreateDirectory(tempPath);
        FileHelper.CopyDirectory(outpath, tempPath);

        //删除.manifest文件【.manifest实际无用，留一份在AssetBundles文件夹内可以阅读】
        _DestroyManifest();

        // //3、拷贝dll到包里 TODO:
        string dllDir =
            Path.Combine(PathHelper.s_streamingAssetsPath,
                selectModel); //dll路径，例如"/Users/futianfu/UnityRuntime/Assets/tempStreamingAssets/common"
        if (dllDir != "")
        {
            CopyDLL(dllDir, tempPath);
        }

        //4、 分包路径 删除重创
        string remotePath = string.Format("AssetBundles/{0}_{1}_remote", buildTarget, selectModel);
        FileHelper. DelCreateDirectory(remotePath);

        // 5、主包资源,从tempPaht拷贝到remotePaht TODO:
        CopyMainPackage(tempPath, remotePath, selectModel);

        // 6、生成更新包资源的mainifest文件,这里面有md5值和版本号【这个版本号是用来检测是否热更的】【roomres.json】 TODO:
        CreateManifest(remotePath, ResModule.FILE_NAME, selectModel);

        // 8、复制完后删除缓存文件
        Directory.Delete(tempPath, true); //删除缓存文件夹
        Directory.Delete(outpath, true);  //删除缓存文件夹

        // 复制到StreamingAssets目录
        // _CopyToStreamingAssets("AssetBundles");

        WLDebug.Log($"热更包打包完成，输出路径{outpath}");
    }

    /// <summary>
    /// 自动切换平台
    /// </summary>
    /// <param name="buildTarget"></param>
    private static void _SwitchPlatform(BuildTarget buildTarget)
    {
        if (EditorUserBuildSettings.activeBuildTarget != buildTarget)
        {
            #pragma warning disable 618
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget); // 自动切换平台
            #pragma warning restore 618
        }
    }


    /// <summary>
    /// 打ab包
    /// </summary>
    /// <param name="buildTarget"> 运行平台 【例如：android】</param>
    /// <param name="buildAssetBundleOptions"> ab包选项 </param>
    /// <param name="selectModel"> 要打包的模块【例如：{"全平台"，"Common"}】 </param>
    public static void Build(BuildTarget buildTarget, BuildAssetBundleOptions buildAssetBundleOptions,
        string                           selectModel, bool                    hotUpdate , int version)
    {
        _SwitchPlatform(buildTarget); // 自动切换平台

        // 构建时间 TODO:
        CalcBuildTime(version, hotUpdate);

        //查找每个模块之间Ab包是否重名
        if (_IsAbFoldRepeat(PathHelper.s_gameAssetsPath))
            return;

        // if (_IsAbFoldLower(FileHelper.s_gameAssetsPath))//查找AB文件名，Ab包所对应的文件夹名是否存在小写
        // return;

        // 打包资源
        _BuildAssetsBundle(buildTarget, buildAssetBundleOptions, selectModel);

        //删除.manifest文件【.manifest实际无用，留一份在AssetBundles文件夹内可以阅读】
        _DestroyManifest();

        // 复制到StreamingAssets目录
        _CopyToStreamingAssets("AssetBundles");

        WLDebug.Log("资源包打包完成");
    }

    //3.2拷贝dll
    static void CopyDLL(string dllDir, string dstDir)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(dllDir);
        FileInfo[]    files         = directoryInfo.GetFiles("*.dll", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            Debug.Log($"拷贝DLL {dstDir + "/" + files[i].Name}");
            CopyFile(files[i].FullName, dstDir + "/" + files[i].Name);
        }
    }

    //3.3拷贝
    static void CopyFile(string src, string dst)
    {
        File.Copy(src, dst, true);
    }

    // 5.2、拷贝主包资源
    static void CopyMainPackage(string srcPath, string remotePath, string subName)
    {
        string dir = remotePath + "/" + subName;
        CreateDir(dir);
        FileHelper.CopyDirectory(srcPath, dir);
    }

    // 5.3
    static void CreateDir(string dir)
    {
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }

        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
    }


    /// <summary>
    /// 删除.manifest文件
    /// </summary>
    private static void _DestroyManifest()
    {
        List<string> allFileAndFolder = FileHelper.GetAllFileAndFolder(PathHelper.AssetBundlesPath());
        List<string> onlyFiles        = FileHelper.GetOnlyFile(allFileAndFolder);
        List<string> manifestFiles    = FileHelper.GetExtensionFile(onlyFiles, ".manifest");
        foreach (var item in manifestFiles)
        {
            FileHelper.DeleteFile(item);
        }
    }

    #region 初始化删除旧文件操作【删除AssetBundles 和 StreamingAssets】

    /// <summary>
    ///  删除 AssetBundles
    /// </summary>
    private static void _ClearOld(string common = "")
    {
        // _DeleteStreamingAssets(common);
        _DeleteAssetBundles(common);
    }

    /// <summary>
    /// 删除 AssetBundles
    /// </summary>
    private static void _DeleteStreamingAssets(string common)
    {
        FileHelper.DeleteFolder(PathHelper.s_streamingAssetsPath + common);
    }

    /// <summary>
    /// 删除 StreamingAssets
    /// </summary>
    private static void _DeleteAssetBundles(string common)
    {
        FileHelper.DeleteFolder(PathHelper.AssetBundlesPath() + common);
    }

    #endregion

    #region 打包逻辑

    /// <summary>
    /// 获得用户选择的模块对应的路径
    /// </summary>
    /// <param name="buildTarget"> 运行平台 【例如：android】</param>
    /// <param name="buildAssetBundleOptions"> ab包选项 </param>
    /// <param name="selectModel"> 要打包的模块【例如：{"全平台"，"Common"}】 </param>
    private static void _BuildAssetsBundle(BuildTarget buildTarget, BuildAssetBundleOptions buildAssetBundleOptions,
        string                                         selectModel)
    {
        //打包模块判断
        if (selectModel == "全平台")
        {
            // _ClearOld();                                                  // 删除AssetBundles 和 StreamingAssets
            _AllPlatformsRecursive(buildTarget, buildAssetBundleOptions); //【全平台打包】循环执行多次单模块打包
            BuildHelper.SetAllMoudleAbName(PathHelper.s_gameAssetsPath);  //一键打标签,由于按模块打包会清除其他模块标签，需要再打一次全平台
        }
        else
        {
            // _ClearOld("/" + selectModel); // 删除AssetBundles

            string assetPath      = Path.Combine(PathHelper.AssetBundlesPath() , selectModel.ToLower());
            string gameAssetsPath = Path.Combine(PathHelper.s_gameAssetsPath , selectModel);
            _ModelRecursive(buildTarget, buildAssetBundleOptions, assetPath, gameAssetsPath); //【单模块打包】逐层查找到最内层文件

            BuildHelper.SetAllMoudleAbName(PathHelper.s_gameAssetsPath); //一键打标签,由于按模块打包会清除其他模块标签，需要再打一次全平台
        }
    }

    /// <summary>
    /// 【全平台打包】循环执行多次单模块打包
    /// </summary>
    /// <param name="buildTarget"> 运行平台 【例如：android】</param>
    /// <param name="buildAssetBundleOptions"> ab包选项 </param>
    private static void _AllPlatformsRecursive(BuildTarget buildTarget, BuildAssetBundleOptions buildAssetBundleOptions)
    {
        List<string> dirInfos = FileHelper.GetFolder(PathHelper.s_gameAssetsPath);
        foreach (var item in dirInfos)
        {
            string assetPath      = PathHelper.AssetBundlesPath() + "/" + Path.GetFileName(item).ToLower();
            string gameAssetsPath = PathHelper.s_gameAssetsPath + "/" + Path.GetFileName(item);
            _ModelRecursive(buildTarget, buildAssetBundleOptions, assetPath, gameAssetsPath); //【单模块打包】逐层查找到最内层文件
        }
    }


    /// <summary>
    /// 【单模块打包】逐层查找到最内层文件
    /// </summary>
    /// <param name="buildTarget"> 运行平台 【例如：android】</param>
    /// <param name="buildAssetBundleOptions"> ab包选项 </param>
    /// <param name="assetPath"> 要打包的模块路径【例如：D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common/】</param>
    /// <param name="gameAssetsPath"> 要打包的模块路径【例如：D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common】 </param>
    private static void _ModelRecursive(BuildTarget buildTarget, BuildAssetBundleOptions buildAssetBundleOptions,
        string                                      assetPath,   string                  gameAssetsPath)
    {
        BuildHelper.SetMoudleAbName(gameAssetsPath);                          //一键打标签
        FileHelper.CreateDirectory(assetPath);                                //创建文件夹
        BuildHelper.BuildAB(buildTarget, buildAssetBundleOptions, assetPath); //打ab包
    }

    #endregion

    #region 规范检测

    /// <summary>
    /// 查找每个模块之间Ab包是否重名
    /// </summary>
    /// <param name="path"> 资源文件夹路径【例如：D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets】 </param>
    /// <returns></returns>
    private static bool _IsAbFoldRepeat(string path)
    {
        //用于存储所有模块的ab包名
        Dictionary<string, string> AbFolds = new Dictionary<string, string>();

        //子模块内的模块路径【例如：item = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common】
        List<string> gameFolds = FileHelper.GetFolder(path);
        foreach (var item in gameFolds)
        {
            //子模块内的模块路径【例如：cItem = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common/Module1】
            List<string> modelFold = FileHelper.GetFolder(item);
            List<string> ignoreDir = FileHelper.GetIgnore(modelFold); //忽略CSProject~
            foreach (var cItem in ignoreDir)
            {
                //查找文件夹内重名文件夹
                if (_IsDirRepeat(cItem, AbFolds))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 查找文件夹内重名文件夹
    /// </summary>
    /// <param name="path"> 要查找的文件夹路径 </param>
    /// <param name="abFolds"> new一个字典用于比较 </param>
    /// <returns></returns>
    private static bool _IsDirRepeat(string path, Dictionary<string, string> abFolds)
    {
        //子模块内的模块路径【例如：ccItem = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common/Module1/demo_atlas】
        List<string> cModelFold = FileHelper.GetFolder(path);
        foreach (var ccItem in cModelFold)
        {
            if (abFolds.ContainsKey(Path.GetFileName(ccItem)))
            {
                Debug.LogError("存在重名: " + ccItem + " 和 " + abFolds[Path.GetFileName(ccItem)] + " 请重命名");
                return true;
            }
            else
            {
                abFolds.Add(Path.GetFileName(ccItem), ccItem);
            }
        }

        return false;
    }

    /// <summary>
    /// 检测Ab包文件夹名称是否有大写【规范必须小写】
    /// </summary>
    /// <param name="path"> 要查找重名的路径 </param>
    private static bool _IsAbFoldLower(string path)
    {
        //子模块内的模块路径【例如：item = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common】
        List<string> gameFolds = FileHelper.GetFolder(path);
        foreach (var item in gameFolds)
        {
            //子模块内的模块路径【例如：cItem = D:/Users/admin/Desktop/JiaXiangGit2/UnityRuntime/Assets/GameAssets/Common/Module1】
            List<string> modelFold = FileHelper.GetFolder(item);
            List<string> ignoreDir = FileHelper.GetIgnore(modelFold); //忽略CSProject~
            foreach (var cItem in ignoreDir)
            {
                if (FileHelper.IsDirLower(cItem))
                    return true;
            }
        }

        return false;
    }

    #endregion

    #region  创建 资源MD5和热更版本号 TXT

    /// <summary>
    /// 构建时间
    /// </summary>
    /// <param name="version"></param>
    /// <param name="hotUpdate"></param>
    public static void CalcBuildTime(int version, bool hotUpdate)
    {
        //如果版本号 version=0，读取 ResVersion.txt 内的版本号
        if (version == 0)
        {
            string path    = Application.dataPath + "/ResVersion.txt";
            string content = File.ReadAllText(path);
            version = int.Parse(content);
        }

        //构建码从 BuildNo.txt 内读取
        int    buildNo     = 0;
        string buildNoPath = Application.dataPath + "/../../BuildNo.txt";
        if (File.Exists(buildNoPath))
        {
            string content = File.ReadAllText(buildNoPath);
            buildNo = int.Parse(content);
        }
        else
        {
            buildNo = 0;
        }

        buildNo++; //每次构建号累+1
        if (buildNo >= 10000)
        {
            buildNo = 0;
        }

        File.WriteAllText(buildNoPath, buildNo.ToString());   //重写构建号到 BuildNo.txt
        s_buildTime = 2000000000 + version * 10000 + buildNo; //buildNo用时间代替，去知道构建顺序
    }

    //创建分包配置文件【roomres.json】
    static void CreateManifest(string dir, string manifestFilename, string moduleName)
    {
        string relativePath = Path.GetFullPath(dir);
        relativePath = PathHelper.PathFormat(relativePath) + "/";

        Dictionary<string, Dictionary<string, SingleNewResInfo>> resDictionary =
            new Dictionary<string, Dictionary<string, SingleNewResInfo>>();
        DirectoryInfo moduleDirectoryInfo = new DirectoryInfo(dir + "/" + moduleName);
        List<string>  subModules          = FileHelper.GetFolder(moduleDirectoryInfo.FullName);
        foreach (var subModule in subModules)
        {
            //subModule = "/Users/futianfu/UnityRuntime/AssetBundles/Android_common_remote/common/game"
            Dictionary<string, SingleNewResInfo> dic      = new Dictionary<string, SingleNewResInfo>();
            DirectoryInfo subDirectoryInfo = new DirectoryInfo(subModule);
            FileInfo[]    abs              = subDirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            for (int i = 0; i < abs.Length; i++)
            {
                SingleNewResInfo patch    = new SingleNewResInfo();
                string           fullName = PathHelper.PathFormat(abs[i].FullName);
                string           path     = fullName.Replace(relativePath, "");
                patch.name   = abs[i].Name;                                        //名字
                patch.path   = path;                                               //路径
                patch.module = moduleName == "" ? path.Split('/')[0] : moduleName; //模块

                byte[] data = File.ReadAllBytes(fullName);
                patch.md5       = MD5Helper.CalcMD5(data); //md5值
                patch.size      = data.Length;             //文件大小
                patch.buildtime = s_buildTime;             //构建时间
                dic[patch.name] = patch;
            }

            string subModuleName = Path.GetFileName(subModule);
            resDictionary.Add(subModuleName, dic);
        }

        // __main__dll和清单文件json生成
        CreateOtherManifest(moduleName, moduleDirectoryInfo, dir, resDictionary);

        System.TimeSpan ts      = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        int             Version = System.Convert.ToInt32(ts.TotalMilliseconds / 1000.0);

        var manifest = new NewResDictionary();
        manifest.resdictionary = resDictionary;
        manifest.version       = Version;

        // string p = dir + "/" + manifestFilename;
        string p = dir + "/" + moduleName + manifestFilename;
        manifest.Serialize(manifest, p);
    }

    /// <summary>
    /// __main__dll和清单文件json生成
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="moduleDirectoryInfo"></param>
    /// <param name="dir"></param>
    /// <param name="resDictionary"></param>
    private static void CreateOtherManifest(string moduleName, DirectoryInfo moduleDirectoryInfo, string dir,
        Dictionary<string, Dictionary<string, SingleNewResInfo>> resDictionary)
    {
        string relativePath = Path.GetFullPath(dir);
        relativePath = PathHelper.PathFormat(relativePath) + "/";

        Dictionary<string, SingleNewResInfo> otherDic     = new Dictionary<string, SingleNewResInfo>();
        List<string>                         otherModules = FileHelper.GetFile(moduleDirectoryInfo.FullName);
        relativePath = PathHelper.PathFormat(relativePath) + "/";
        for (int i = 0; i < otherModules.Count; i++)
        {
            SingleNewResInfo patch    = new SingleNewResInfo();
            string           fullName = PathHelper.PathFormat(otherModules[i]);
            string           path     = fullName.Replace(relativePath, "");
            string           s        = StringHelper.Split(path, "_remote/")[1];
            patch.name   = Path.GetFileName(otherModules[i]);                  //名字
            patch.path   = s;                                                  //路径
            patch.module = moduleName == "" ? path.Split('/')[0] : moduleName; //模块

            byte[] data = File.ReadAllBytes(fullName);
            patch.md5            = MD5Helper.CalcMD5(data); //md5值
            patch.size           = data.Length;             //文件大小
            patch.buildtime      = s_buildTime;             //构建时间
            otherDic[patch.name] = patch;
        }

        resDictionary.Add("__main__", otherDic);
    }

    #endregion

    /// <summary>
    /// 拷贝到StreamingAssets
    /// </summary>
    /// <param name="srcDir"> 要拷贝的文件路径 </param>
    private static void _CopyToStreamingAssets(string srcDir)
    {
        FileHelper.CopyDirectory(srcDir, PathHelper.s_streamingAssetsPath);
    }
}
