using UnityEngine;
using System.Runtime.InteropServices;

public class Helper
{
    public static string JSCryptStr(string strData, string strKey, bool bEncode, int nExpiry)
    {
        //if (Application.isEditor)
        //{
        //    return Marshal.PtrToStringAuto(LibraryBridge.JSCryptStr_Binding(strData, strKey, bEncode, nExpiry));
        //}

        //if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    return IOSBridge.CryptString(strData, strKey, bEncode, nExpiry);
        //}

        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    return LibraryBridge.CryptString(strData, strKey, bEncode, nExpiry);
        //}

        return "";
    }
}
