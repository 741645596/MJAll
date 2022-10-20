// AppdomainManager.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/19

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.Mono.Cecil.Pdb;
using Unity.Utility;
using UnityEngine.Networking;
using WLCore.Coroutine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

public static class AppDomainManager
{
    public static AppDomain AppDomain { get; set; } = new AppDomain();
    private static readonly List<string> loadedDlls = new List<string>();

    public static object Invoke(string type, string method, object instance, params object[] p)
    {
        return AppDomain.Invoke(type, method, instance, p);
    }

    private static CoroutineProxy GetCoroutineProxy()
    {
        return CoroutineManager.GetDefaultCoroutineProxy();
    }

    public static void LoadAssembly(string dllfile, string pdbfile, Action<bool> callback = null, bool isReload = false)
    {
        GetCoroutineProxy().StartCoroutine(LoadAssemblyEnumerator(dllfile, pdbfile, callback, isReload));
    }

    public static void LoadAssemblies(string[] dllfiles, string[] pdbfiles, Action<bool> callback = null, bool isReload = false)
    {
        GetCoroutineProxy().StartCoroutine(LoadAssembliesEnumerator(dllfiles, pdbfiles, callback, isReload));
    }

    public static void StartDebugService()
    {
        AppDomain.DebugService.StartDebugService(56000);
    }

    public static void StoptartDebugService()
    {
        AppDomain.DebugService.StopDebugService();
    }

    public static T Instantiate<T>(string type, object[] args = null)
    {
        return AppDomain.Instantiate<T>(type, args);
    }

    private static IEnumerator LoadAssembliesEnumerator(string[] dllfiles, string[] pdbfiles, Action<bool> callcack, bool isReload)
    {
        bool result = true;
        for (int i = 0; i < dllfiles.Length; i++)
        {
            var dllfile = dllfiles[i];
            string pdbfile = null;
            if (pdbfiles != null)
            {
                pdbfile = pdbfiles[i];
            }
            // 检查是否已经加载
            if (!isReload && loadedDlls.Contains(dllfile))
            {
                continue;
            }
            yield return LoadAssemblyEnumerator(dllfile, pdbfile, (success) => {
                if (success == false)
                {
                    result = false;
                }
            }, isReload);
        }

        callcack?.Invoke(result);
    }

    private static IEnumerator LoadAssemblyEnumerator(string dllfile, string pdbfile, Action<bool> callcack = null, bool isReload = false)
    {
        // 检查是否已经加载
        if (!isReload && loadedDlls.Contains(dllfile))
        {
            WLDebug.LogWarning(dllfile + " 重复加载.");
            callcack?.Invoke(true);
            yield break;
        }

        // 读取dll
        var dllRequest = UnityWebRequest.Get(dllfile);
        yield return dllRequest.SendWebRequest();

        if (dllRequest.result == UnityWebRequest.Result.ProtocolError ||
            dllRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            WLDebug.LogWarning(dllRequest.error + " " + dllfile);
            callcack?.Invoke(false);
            yield break;
        }
        byte[] dllBytes = dllRequest.downloadHandler.data;
        if (AESEncryptTool.IsEncryptDLL(dllBytes))
        {
            dllBytes = AESEncryptTool.AESDecryptHasHead(dllBytes, AESEncryptTool.AES_KEY);
        }

        MemoryStream dllstream = new MemoryStream(dllBytes);

        // 读取pdb
        MemoryStream pdbstream = null;
        if (string.IsNullOrEmpty(pdbfile) == false)
        {
            var pdbRequest = UnityWebRequest.Get(pdbfile);
            yield return pdbRequest.SendWebRequest();
            // 源码修改，去掉警告
            if (pdbRequest.result == UnityWebRequest.Result.ProtocolError ||
                pdbRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                WLDebug.LogWarning(pdbRequest.error + " " + pdbfile);
            }
            else
            {
                byte[] pdbBytes = pdbRequest.downloadHandler.data;
                if (pdbBytes != null)
                {
                    pdbstream = new MemoryStream(pdbBytes);
                }
            }
        }
        // 加载
        loadedDlls.Add(dllfile);
        AppDomain.LoadAssembly(dllstream, pdbstream, new PdbReaderProvider());
        // 执行回调
        WLDebug.Log("已加载 " + dllfile + " 文件大小：" + dllBytes.Length);
        callcack?.Invoke(true);
    }

    public static void InitializeILRuntime()
    {
        // 原生事件回调
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Byte>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Single>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.String>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Object>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.UInt32>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Int32>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.String, System.Int64>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.String>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Int64, System.Int64>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.String, System.Byte[], System.Int64>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Object, System.EventArgs>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Single, System.Single, System.Single>();
        AppDomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.KeyValuePair<System.String, System.Byte>>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Transform>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.RectTransform>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Texture2D>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object[]>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.PointerEventData>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>();
        AppDomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();

        AppDomain.DelegateManager.RegisterFunctionDelegate<System.Single>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Int32>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<System.Byte[], System.Boolean>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Boolean>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.List<System.Int32>, System.Collections.Generic.List<System.Int32>, System.Int32>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Transform, System.Boolean>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<KeyValuePair<int, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Int32>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<KeyValuePair<int, ILRuntime.Runtime.Intepreter.ILTypeInstance>, ILRuntime.Runtime.Intepreter.ILTypeInstance>();

        AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
            {
                ((Action<System.Single>)act)(arg0);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>((arg0, arg1) =>
            {
                ((Action<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>)act)(arg0, arg1);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene>((arg0) =>
            {
                ((Action<UnityEngine.SceneManagement.Scene>)act)(arg0);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
        {
            return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
            {
                return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((Action)act)();
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.String>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.String>((arg0) =>
            {
                ((Action<System.String>)act)(arg0);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler>((act) =>
        {
            return new System.EventHandler((sender, e) =>
            {
                ((Action<System.Object, System.EventArgs>)act)(sender, e);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler>((act) =>
        {
            return new System.EventHandler((sender, e) =>
            {
                ((Action<System.Object>)act)(sender);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int32>>((act) =>
        {
            return new System.Comparison<System.Int32>((x, y) =>
            {
                return ((Func<System.Int32, System.Int32, System.Int32>)act)(x, y);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<bool>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<bool>((arg0) =>
            {
                ((Action<bool>)act)(arg0);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Collections.Generic.List<System.Int32>>>((act) =>
        {
            return new System.Comparison<System.Collections.Generic.List<System.Int32>>((x, y) =>
            {
                return ((Func<System.Collections.Generic.List<System.Int32>, System.Collections.Generic.List<System.Int32>, System.Int32>)act)(x, y);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<Spine.AnimationState.TrackEntryDelegate>((act) =>
        {
            return new Spine.AnimationState.TrackEntryDelegate((trackEntry) =>
            {
                ((Action<Spine.TrackEntry>)act)(trackEntry);
            });
        });


        // 非原生回调注册
        AppDomain.DelegateManager.RegisterMethodDelegate<TouchData3D>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Networking.DownloadHandler, System.Int64>();
        AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Networking.DownloadHandler, System.String>();
        AppDomain.DelegateManager.RegisterMethodDelegate<Spine.TrackEntry>();
        AppDomain.DelegateManager.RegisterMethodDelegate<LitJson.JsonData>();
        AppDomain.DelegateManager.RegisterMethodDelegate<Unity.Widget.WLayer>();
        AppDomain.DelegateManager.RegisterMethodDelegate<Unity.Widget.WSprite>();
        AppDomain.DelegateManager.RegisterMethodDelegate<WLCore.MsgHeader>();        AppDomain.DelegateManager.RegisterMethodDelegate<WLCore.MsgHeader, System.Int32>();        AppDomain.DelegateManager.RegisterMethodDelegate<System.String, global::XZStates>();        AppDomain.DelegateManager.RegisterMethodDelegate<global::BaseEntityAdaptor.Adaptor, global::BaseEntityAdaptor.Adaptor>();        AppDomain.DelegateManager.RegisterMethodDelegate<global::BaseEntityAdaptor.Adaptor>();        AppDomain.DelegateManager.RegisterMethodDelegate<global::WNodeAdaptor.Adaptor>();        AppDomain.DelegateManager.RegisterFunctionDelegate<List<BaseEntityAdaptor.Adaptor>, List<BaseEntityAdaptor.Adaptor>, int>();        AppDomain.DelegateManager.RegisterFunctionDelegate<global::TableView, System.Int32>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<global::TableView, UnityEngine.Vector2>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<global::TableView, System.Int32, global::TableViewCell>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<global::WNodeAdaptor.Adaptor, System.Boolean>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<global::BaseEntityAdaptor.Adaptor, System.Boolean>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<global::BaseEntityAdaptor.Adaptor, global::BaseEntityAdaptor.Adaptor, System.Int32>();
        AppDomain.DelegateManager.RegisterFunctionDelegate<global::BaseEntityAdaptor.Adaptor, global::BaseEntityAdaptor.Adaptor, System.Boolean>();

        AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
        {
            return new DG.Tweening.TweenCallback(() =>
            {
                ((Action)act)();
            });
        });        AppDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<global::WNodeAdaptor.Adaptor>>((act) =>
        {
            return new System.Predicate<global::WNodeAdaptor.Adaptor>((obj) =>
            {
                return ((Func<global::WNodeAdaptor.Adaptor, System.Boolean>)act)(obj);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<global::BaseEntityAdaptor.Adaptor>>((act) =>
        {
            return new System.Predicate<global::BaseEntityAdaptor.Adaptor>((obj) =>
            {
                return ((Func<global::BaseEntityAdaptor.Adaptor, System.Boolean>)act)(obj);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<global::BaseEntityAdaptor.Adaptor>>((act) =>
        {
            return new System.Comparison<global::BaseEntityAdaptor.Adaptor>((x, y) =>
            {
                return ((Func<global::BaseEntityAdaptor.Adaptor, global::BaseEntityAdaptor.Adaptor, System.Int32>)act)(x, y);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<Comparison<List<BaseEntityAdaptor.Adaptor>>>((act) =>
        {
            return new Comparison<List<BaseEntityAdaptor.Adaptor>>((x, y) =>
            {
                return ((Func<List<BaseEntityAdaptor.Adaptor>, List<BaseEntityAdaptor.Adaptor>, int>)act)(x, y);
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Single>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<System.Single>(() =>
            {
                return ((Func<System.Single>)act)();
            });
        });
        AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Single>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<System.Single>((pNewValue) =>
            {
                ((Action<System.Single>)act)(pNewValue);
            });
        });
        
        // ======================
        LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(AppDomain);
        AppDomain.RegisterCrossBindingAdaptor(new BaseStageAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new BaseEntityAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new IComparerAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new IComparerIntAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new EventArgsAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new BaseCaseStageAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new CoroutineAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new TableViewCellAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new WNode3DAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new WNodeAdaptor());
        AppDomain.RegisterCrossBindingAdaptor(new WLayerAdaptor());

        //ILRuntime绑定
#if !UNITY_EDITOR
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(AppDomain);
#endif
    }
}
