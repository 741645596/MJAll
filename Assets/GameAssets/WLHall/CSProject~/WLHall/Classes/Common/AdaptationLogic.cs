// FHAdaptationLogic.cs
// Author: weile 
// Date: 2021/6/23
using Unity.Widget;
using UnityEngine;
using WeiLe.Notification;
using WLCore.Helper;

namespace WLCommon
{
    // 
    /// <summary>
    /// UI适配逻辑统一处理文件，现在适配方案：ios刘海屏左右适配。安卓全部不做偏移。如果后续有出特殊屏幕可以在该文件热更
    /// </summary>
    public class AdaptationLogic
    {
        // 默认偏移量
        private const int C_Default_Offset = 56;
        private static ScreenOrientation orientation = Screen.orientation;
        private static float orientation_timer = 0;
        // 初始注册回调事件
        public static void Init()
        {
            _ResetUIAdapt();
        }

        // 初始UI适配逻辑
        private static void _ResetUIAdapt()
        {
            var offset = _GetOffect();
            DesignResolution.SetScreenOffset(offset.x, offset.y);
        }

        private static Vector2 _GetOffect()
        {
            if (PlayformHelper.IsIOS())
            {
                return _GetIosOffset();
            }
            else if (PlayformHelper.IsAndod())
            {
                return _GetAndroidOffset();
            }

            // 默认
            return _GetDefaultOffset();
        }

        private static Vector2 _GetIosOffset()
        {
            // iPhone X 以上
            if (PlayformHelper.GetIosGeneration() >= PlayformHelper.iPhoneX_Version)
            {
                var ori = ScreenOrientation.LandscapeLeft;
                if (Screen.orientation == ori)
                {
                    return new Vector2(C_Default_Offset, 0);
                }
                return new Vector2(0, C_Default_Offset);
            }

            return _GetDefaultOffset();
        }

        private static Vector2 _GetAndroidOffset()
        {
            // 安卓暂不偏移，后续有特殊手机可以在这里统一处理
            return Vector2.zero;
        }

        private static Vector2 _GetDefaultOffset()
        {
            // 默认不偏移
            return Vector2.zero;
        }

        /// <summary>
        /// 每隔1s检测是否转换了屏幕，安卓监听屏幕旋转比较麻烦，所以开定时器检查
        /// </summary>
        /// <param name="deltaTime"></param>
        public static void __OnUpdate(float deltaTime)
        {
            orientation_timer += deltaTime;
            if (orientation_timer > 1f)
            {
                orientation_timer = 0;
                if (Screen.orientation != orientation)
                {
                    orientation = Screen.orientation;

                    _ResetUIAdapt();

                    // 重置cocos ui适配
                    NotificationCenter.PostNotification("OnDeviceOrientationDidChange");
                }
            }
        }
    }
}
