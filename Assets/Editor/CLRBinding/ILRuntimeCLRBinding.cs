#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
    private static ILRuntime.Runtime.Enviorment.AppDomain domain;

    [MenuItem("ILRuntime/Generate CLR Binding Code")]
    public static void GenerateCLRBindingByAnalysis()
    {
        var setting_path = "Assets/Editor/CLRBinding/CLR Binding Setting.asset";
        var settings = AssetDatabase.LoadAssetAtPath<CLRBindingSettingSO>(setting_path);

        if(settings == null)
        {
            Debug.LogWarning($"CLRBinding Fail, Can't find setting setting_path={setting_path}");
            return;
        }
        List<string> dllNames = new List<string>(settings.DllList);
        Debug.Log("GenerateCLRBindingByAnalysis Start.");

        InitILRuntime();
        for (int i = 0; i < dllNames.Count; i++)
        {
            var dll_file = Path.Combine(Application.streamingAssetsPath, dllNames[i]);
            if (!File.Exists(dll_file))
            {
                Debug.LogWarning($"CLRBinding Fail, Path={dll_file}");
                continue;
            }

            GenerateCLRBindingByAnalysis(dll_file);
        }
        Debug.Log("GenerateCLRBindingByAnalysis Over.");
    }

    private static void GenerateCLRBindingByAnalysis(string dll_file)
    {
        FileStream fs = new FileStream(dll_file, FileMode.Open, FileAccess.Read);
        {
            domain.LoadAssembly(fs);
        }

        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/ILRuntime/Generated");
    }

    static void InitILRuntime()
    {
        domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        domain.RegisterCrossBindingAdaptor(new BaseStageAdaptor());
        domain.RegisterCrossBindingAdaptor(new BaseEntityAdaptor());
        domain.RegisterCrossBindingAdaptor(new IComparerAdaptor());
        domain.RegisterCrossBindingAdaptor(new IComparerIntAdaptor());
        domain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());
        domain.RegisterCrossBindingAdaptor(new EventArgsAdaptor());
        domain.RegisterCrossBindingAdaptor(new BaseCaseStageAdaptor());
        domain.RegisterCrossBindingAdaptor(new CoroutineAdaptor());
        domain.RegisterCrossBindingAdaptor(new TableViewCellAdaptor());
        domain.RegisterCrossBindingAdaptor(new WNode3DAdaptor());
        domain.RegisterCrossBindingAdaptor(new WNodeAdaptor());
        domain.RegisterCrossBindingAdaptor(new WLayerAdaptor());
    }
}
#endif
