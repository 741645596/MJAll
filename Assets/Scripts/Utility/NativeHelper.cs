using UnityEngine;

public static class NativeHelper
{
    /// <summary>
    /// 获取剪切板信息
    /// </summary>
    /// <returns></returns>
    public static string GetClipboardContent()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return IOSBridge.GetClipboardContent();
#else
        return "";
#endif
    }

    /// <summary>
    /// 复制内容到剪切板
    /// </summary>
    /// <param name="str">内容</param>
    public static void CopyToClipboard(string str)
    {
#if UNITY_IOS && !UNITY_EDITOR
        IOSBridge.CopyToClipboard(str);
#else

#endif
    }

    /// <summary>
    /// 震动反馈
    /// </summary>
    /// <param name="v">
    /// 震动反馈强度
    /// 0: Light
    /// 1: Medium
    /// 2: Heavy (其他值默认选项)
    /// </param>
    public static void VibrateFeedback(int v)
    {
#if UNITY_IOS && !UNITY_EDITOR
        IOSBridge.VibrateFeedback(v);
#else

#endif
    }

    /// <summary>
    /// 请求AppStore评分
    /// </summary>
    /// <param name="appID">应用ID</param>
    public static void RequestReview(string appID)
    {
#if UNITY_IOS && !UNITY_EDITOR
        IOSBridge.RequestReview(appID);
#else

#endif
    }
}
