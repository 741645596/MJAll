using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using WLCore.Coroutine;

namespace Unity.Utility
{

    public static class Utilities
    {
        /// <summary>
        /// 截屏/截图
        /// </summary>
        /// <param name="screenRect"></param>
        /// <param name="savePath"></param>
        /// <param name="finishCallback"></param>
        public static void CaptureScreen(Rect screenRect, string savePath = null, Action<string> finishCallback = null)
        {
            var proxy = CoroutineManager.GetDefaultCoroutineProxy();
            proxy.StartCoroutine(StartCapture(screenRect, savePath, finishCallback));
        }

        private static IEnumerator StartCapture(Rect screenRect, string savePath = null, Action<string> finishCallback = null)
        {
            yield return new WaitForEndOfFrame();

            Texture2D t = new Texture2D(Mathf.FloorToInt(screenRect.width), Mathf.FloorToInt(screenRect.height), TextureFormat.RGB24, false);
            t.ReadPixels(screenRect, 0, 0, false);
            t.Apply();

            string path = savePath ?? Application.dataPath + "/" + "screenshottmp.jpg";
            byte[] data;
            if (path.EndsWith(".png", StringComparison.Ordinal))
            {
                data = t.EncodeToPNG();
            }
            else
            {
                data = t.EncodeToJPG();
            }
            File.WriteAllBytes(path, data);
            finishCallback?.Invoke(path);
        }

        /// <summary>
        /// 随机数
        /// </summary>
        private static System.Random _random = new System.Random();
        public static double Random()
        {
            return _random.NextDouble();
        }

        public static double Random(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public static string GetStackTrace()
        {
            return new StackTrace().ToString();
        }
    }


}
