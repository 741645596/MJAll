
using System.Collections.Generic;
using Unity.Core;
using UnityEngine;
using WLCore.Stage;

/// <summary>
/// 游戏启动脚本入口，只能存在一个，启动多个会报错。所以测试用例之间不能互相切换
/// </summary>
public class GameEntry : MonoBehaviour
{
    public string[] modules;        // 编辑器配置加载dll
    public string stageName;        // 编辑器配置加载入口Stage
    public bool isPersistentStage = false;
    private bool isLoaded = false;

    /// <summary>
    /// MonoBehaviour的启动函数
    /// </summary>
    private void Start()
    {
        // 旧逻辑，需要移到相应的Stage
        //UIParticles.UIParticles.USE_POOL = true;
        //DG.Tweening.DOTween.SetTweensCapacity(400, 200);
        //DeleteOldRes();

        // 初始默认参数
        cc.Init();

        // 本地热更会判断GameEntry是否存在然后重置游戏，所以需要
        DontDestroyOnLoad(this);

        // 资源读取方式
        AssetsManager.SetLoadType(AssetsManager.LoadType.Local);

        // 加载DLL
        LoadDll.Load(new List<string>(modules), (success) =>
        {
            if (success == false)
            {
                WLDebug.LogWarning("错误提示：加载dll失败，请查看上面错误日志输出");
                return;
            }

            // 启动调试 端口56000
            if (Application.isEditor)
            {
                AppDomainManager.StartDebugService();
            }
            AppDomainManager.InitializeILRuntime();
            StageManager.Init();

            StartGame(success);
        });
    }

    private void StartGame(bool success)
    {
        if (isPersistentStage)
        {
            StageManager.RunPersistentStage(stageName);
        }
        else
        {
            StageManager.RunStage(stageName);
        }
        isLoaded = true;
    }

    /// <summary>
    /// 重启游戏
    /// </summary>
    public void Restart()
    {
        WLDebug.ClearLogConsole();

        StageManager.Clean();
        isLoaded = false;
        LoadDll.Load(new List<string>(modules), StartGame, true);
    }

    private void Update()
    {
        if (isLoaded == false)
        {
            return;
        }

        StageManager.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        StageManager.Clean();

        //WLDebugTrace.Dispose();
    }

    private void OnApplicationPause(bool bPause)
    {
        if (isLoaded)
        {
            StageManager.OnApplicationPause(bPause);
        }
    }

    private void OnApplicationFocus(bool bFocus)
    {
        if (isLoaded)
        {
            StageManager.OnApplicationFocus(bFocus);
        }
    }

    #region 大版本更新
    //NewResDictionary DeserializeResDictionary(string str)
    //{
    //    var dic = NewResDictionary.Deserialize(str);
    //    return dic;
    //}

    //private NewResDictionary GetPackageResDictionay()
    //{
    //    NewResDictionary ret = new NewResDictionary();
    //    string packPath = Path.Combine(Application.streamingAssetsPath, ResModule.FILE_NAME);
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        try
    //        {
    //            var uri = new System.Uri(packPath);
    //            var uwr = UnityEngine.Networking.UnityWebRequest.Get(uri.AbsoluteUri);
    //            uwr.SendWebRequest();
    //            while (!uwr.isDone) { }
    //            if (uwr.downloadHandler == null || uwr.error != null)
    //            {
    //                uwr.Dispose();
    //            }
    //            else
    //            {
    //                ret = DeserializeResDictionary(uwr.downloadHandler.text);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            WLDebug.LogError(e);
    //        }
    //    }
    //    else
    //    {
    //        if (File.Exists(packPath))
    //        {
    //            ret = DeserializeResDictionary(File.ReadAllText(packPath));
    //        }
    //    }
    //    return ret;
    //}

    //private string GetLocalResDicPath()
    //{
    //    string path = Path.Combine(Application.persistentDataPath, ResModule.FILE_NAME);
    //    return path;
    //}

    //private bool DeleteOldRes()
    //{
    //    var packageDic = GetPackageResDictionay();
    //    string path = GetLocalResDicPath();
    //    if (!File.Exists(path))
    //    {
    //        return false;
    //    }
    //    var localDic = DeserializeResDictionary(File.ReadAllText(path));
    //    if (packageDic.Version > localDic.Version)
    //    {
    //        foreach (var key in packageDic.ResDictionary)
    //        {
    //            var info = key.Value;
    //            string oldPath = Path.Combine(Application.persistentDataPath, info.Name);
    //            if (File.Exists(oldPath))
    //            {
    //                File.Delete(oldPath);
    //                WLDebug.Log($"DeleteOldRes {oldPath}");
    //            }
    //            if (localDic.ResDictionary.ContainsKey(key.Key))
    //            {
    //                localDic.ResDictionary.Remove(key.Key);
    //            }
    //        }

    //        localDic.Version = packageDic.Version;
    //        SaveLocal(localDic);
    //        WLDebug.Log("DeleteOldRes Save Local Success");
    //        return true;
    //    }
    //    return false;
    //}

    //private void SaveLocal(NewResDictionary dic)
    //{
    //    if (dic == null)
    //    {
    //        return;
    //    }
    //    try
    //    {
    //        string path = GetLocalResDicPath();
    //        NewResDictionary.Serialize(dic, path);
    //    }
    //    catch (Exception e)
    //    {
    //        WLDebug.LogError(e);
    //    }
    //}
    #endregion
}
