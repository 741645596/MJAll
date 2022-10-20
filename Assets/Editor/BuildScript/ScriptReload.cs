
/// <summary>
/// unity代码重新加载之后回调方法
/// </summary>
public static class ScriptReload
{
    [UnityEditor.Callbacks.DidReloadScripts(0)]
    static void OnScriptReload()
    {
#if UNITY_EDITOR
        WLDebug.ClearLogConsole();
#endif

        WLDebug.Log("脚本重新编译完毕");
    }
}
