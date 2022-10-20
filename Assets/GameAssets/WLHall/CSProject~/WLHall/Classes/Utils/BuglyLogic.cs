using System;
using UnityEngine;
using WLCore.Helper;

namespace WLHall
{
    public static class BuglyLogic
    {
        /// <summary>
        /// 开启Bugly
        /// </summary>
        public static void Init()
        {
            BuglyAgent.ConfigDebugMode(false);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                BuglyAgent.InitWithAppId("e403b36a24");
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                BuglyAgent.InitWithAppId("f7ae4110c4");
            }

            BuglyAgent.EnableExceptionHandler();
        }

        /// <summary>
        /// 真机上显示调试窗口
        /// </summary>
        public static void ShowDebugWindow()
        {
            if (PlayformHelper.IsEditor())
            {
                return;
            }

            // 这里后续还有加上，如果是测试环境才需要开启
            var obj = GameObject.Find("GameEntry");
            if (obj != null)
            {
                var com = obj.GetComponent<DebugWindow>();
                if (com == null)
                {
                    obj.AddComponent<DebugWindow>();
                }
            }
        }
    }
}
