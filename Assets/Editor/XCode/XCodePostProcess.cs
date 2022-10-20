#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class XCodePostProcess
{
    [PostProcessBuild(700)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }

        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);

        var framework = proj.TargetGuidByName("UnityFramework");
        proj.SetBuildProperty(framework, "ENABLE_BITCODE", "NO");
        proj.AddBuildProperty(framework, "OTHER_LDFLAGS", "-ObjC");
        proj.AddBuildProperty(framework, "OTHER_LDFLAGS", "-all_load");

        AddFramework(proj, framework);

        proj.WriteToFile(projPath);
    }


    private static void AddFramework(PBXProject project, string targetGUID)
    {
        string[] umengFrameworkArr =
        {
            "libc++.tbd",                       // 用于Bugly支持libc++编译的项目
            "libz.tbd",                         // 用于Bugly对上报数据进行压缩
            "Security.framework",               // 用于Bugly存储keychain
            "SystemConfiguration.framework",    // 用于Bugly读取异常发生时的系统信息
            "AuthenticationServices.framework", // 用于Apple ID登入
            "StoreKit.framework"                // 用于商店评分接口
        };
        foreach (string str in umengFrameworkArr)
        {
            project.AddFrameworkToProject(targetGUID, str, false);
        }
    }
}
#endif