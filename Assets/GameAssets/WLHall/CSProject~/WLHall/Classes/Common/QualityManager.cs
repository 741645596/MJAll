using Unity.Widget;
using Unity.Utility;
using UnityEngine;
using UnityEngine.iOS;
using WLCore.Helper;

namespace WLHall
{ 
    public static class QualityManager
    {
        /// <summary>
        /// 画面质量等级
        /// </summary>
        public enum QualityLevel
        {
            Low,
            Medium,
            High
        }

        public static bool isOpenShadow = true;
        public static bool isOpenBloom = true;
        public static float pixelRatio = 1.0f;    // 屏幕像素比
        public static bool isOpenHDR = true;
        public static bool isOpenMSAA = true;

        /// <summary>
        /// 初始设备配置信息和设置画面质量
        /// </summary>
        public static void InitQualityInfo()
        {
            // 以下参数根据自己项目配置
            var quality = GetDeviceLevel();
            QualitySettings.SetQualityLevel((int)quality);

            if (quality == QualityLevel.High)
            {
                // 高端机 - 全开
                Application.targetFrameRate = 60;
                isOpenShadow = true;
                isOpenBloom = true;
                isOpenHDR = true;
                isOpenMSAA = true;
                pixelRatio = 1.0f;
            }
            else if (quality == QualityLevel.Medium)
            {
                // 中端机 - 适量开启
                Application.targetFrameRate = 60;
                isOpenShadow = true;
                isOpenBloom = true;
                isOpenHDR = false;
                isOpenMSAA = false;
                pixelRatio = 0.8f;
            }
            else
            {
                // 低端机 - 全关
                Application.targetFrameRate = 45;
                isOpenShadow = false;
                isOpenBloom = false;
                isOpenHDR = false;
                isOpenMSAA = false;
                pixelRatio = 0.6f;
            }
        }

        /// <summary>
        /// 获取设备配置级别，返回值参考Quality设置面板
        /// </summary>
        /// <returns></returns>
        public static QualityLevel GetDeviceLevel()
        {
            // 苹果手机根据产品型号判断
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (PlayformHelper.GetIosGeneration() > PlayformHelper.iPhoneX_Version)
                {
                    return QualityLevel.High;
                }

                if(PlayformHelper.GetIosGeneration() < 0x1F)
                {
                    return QualityLevel.Low;
                }

                return QualityLevel.Medium;
            }

            WLDebug.Log("处理器：" + SystemInfo.processorFrequency + "内存：" + SystemInfo.systemMemorySize);

            // 高端配置
            if (SystemInfo.processorFrequency > 2500 && SystemInfo.systemMemorySize >= 6000)
            {
                return QualityLevel.High;
            }
            // 低端配置
            else if (SystemInfo.systemMemorySize < 4096 || SystemInfo.processorFrequency < 1900)
            {
                return QualityLevel.Low;
            }
            // 中端配置
            return QualityLevel.Medium;
        }


        /// <summary>
        /// 根据InitQualityInfo初始数据配置数据，
        /// 备注：切换场景Scene，因为摄像机变更需要重新设置一遍，或根据情况针对性处理，比如后处理
        /// </summary>
        public static void InitDefaultConfig()
        {
            SetBloomEnable(isOpenBloom);

            CameraUtil.SetPostProcessingEnable(isOpenBloom);

            DesignResolution.SetScreenResolution(pixelRatio);

            Camera.main.allowHDR = isOpenHDR;

            Camera.main.allowMSAA = isOpenMSAA;
        }

        /// <summary>
        /// 开关Bloom
        /// </summary>
        public static void SetBloomEnable(bool enable)
        {
            // 如果Bloom组件名称不是Global Volume那需要自己管理了
            var obj = GameObject.Find("Global Volume");
            if (obj != null)
            {
                obj.SetActive(enable);
            }
        }

        /// <summary>
        /// 根据
        /// </summary>
        /// <returns></returns>
        public static string GetQualityDes()
        {
            var level = GetDeviceLevel();
            if (level == QualityLevel.Low)
                return "Low";
            else if (level == QualityLevel.Medium)
                return "Medium";
            return "High";
        }

        /// <summary>
        /// 用来测试最新打的包是不是最新代码，请忽略
        /// </summary>
        /// <returns></returns>
        public static string CheckVerValue()
        {
            return "2";
        }

    }

}