using System;
using UnityEngine;


namespace WLCore.Helper
{
    /// <summary>
    /// 平台相关通用逻辑接口，请勿放非平台相关接口
    /// </summary>
    public static class PlayformHelper
    {
        public const int iPhoneX_Version = 39;

        // 是否为编辑器平台
        public static bool IsEditor()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                return true;
            }

            return false;
        }

        // 是否为IOS平台
        public static bool IsIOS()
        {
            return Application.platform == RuntimePlatform.IPhonePlayer;
        }

        // 是否为安卓平台
        public static bool IsAndod()
        {
            return Application.platform == RuntimePlatform.Android;
        }

        public static bool IsWindows()
        {
            return Application.platform == RuntimePlatform.WindowsPlayer;
        }

        // 是否为WebGL
        public static bool IsWebGL()
        {
            return Application.platform == RuntimePlatform.WebGLPlayer;
        }

        //是否是移动平台
        public static bool IsNative()
        {
            return IsIOS() || IsAndod();
        }

        // 获取Log输出等级
        public static LogLevel GetLogLevel()
        {
            // 本地测试输出所有信息
            if (IsEditor() /*|| GlobalConfig.IS_BENDI_CeShI*/)
            {
                return LogLevel.Debug;
            }
            return IsNative() || IsWebGL() ? LogLevel.Warn : LogLevel.Debug;
        }

        /// <summary>
        /// ios版本，39是iPhoneX
        /// </summary>
        /// <returns></returns>
        public static int GetIosGeneration()
        {
#if UNITY_IOS || UNITY_IPHONE
        return (int)UnityEngine.iOS.Device.generation;
#else
            return 0;
#endif
        }
    }
}
