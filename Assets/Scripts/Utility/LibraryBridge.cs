using System;
using System.Runtime.InteropServices;

public class LibraryBridge
{
    [DllImport("Assets/Plugins/libHallC.dylib")]
    public static extern IntPtr JSCryptStr_Binding(string strData, string strKey, bool bEncode, int nExpiry);

    [DllImport("StringCrypterAndroid")]
    public static extern string CryptString(string strData, string strKey, bool bEncode, int nExpiry);
}
