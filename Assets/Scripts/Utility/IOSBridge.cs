#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

public class IOSBridge
{
    [DllImport("__Internal")]
    public static extern void CopyToClipboard(string str);

    [DllImport("__Internal")]
    public static extern string GetClipboardContent();

    [DllImport("__Internal")]
    public static extern void VibrateFeedback(int v);

    [DllImport("__Internal")]
    public static extern void RequestReview(string appID);
}

#endif

