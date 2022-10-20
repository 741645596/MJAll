using UnityEngine;
namespace WeiLe.Core
{
    /// <summary>
    /// 如果项目默认是30帧，某些动作需要在60帧表现才会流畅，可以使用下面接口暂时调高帧率，待动画完在设置回原来帧率
    /// </summary>
    public static class FPSManager
    {
        private const int Const_Default_FPS = 30;//默认帧率

        private static int _currentFPS;//当前帧率
        private static int _totalCnt;

        /// <summary>
        /// 增加帧率，与ReleaseFPS成对
        /// </summary>
        /// <param name="FPS"></param>
        public static void RetainFPS(int FPS)
        {
            _totalCnt++;
            if (_currentFPS != FPS)
            {
                _currentFPS = FPS;
                Application.targetFrameRate = _currentFPS;
            }
        }

        /// <summary>
        /// 释放帧率
        /// </summary>
        public static void ReleaseFPS()
        {
            if (_totalCnt > 0)
            {
                _totalCnt--;
            }

            if (_totalCnt == 0)
            {
                ResetFPS();
            }
        }

        /// <summary>
        /// 重置帧率
        /// </summary>
        public static void ResetFPS()
        {
            _totalCnt = 0;
            if (_currentFPS != Const_Default_FPS)
            {
                _currentFPS = Const_Default_FPS;
                Application.targetFrameRate = Const_Default_FPS;
            }
        }
    }
}
