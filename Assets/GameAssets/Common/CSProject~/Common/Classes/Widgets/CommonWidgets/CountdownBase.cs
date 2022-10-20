// @Author: tanjinhua
// @Date: 2021/6/16  10:17


using UnityEngine;
using Unity.Widget;

namespace Common
{
    public abstract class CountdownBase : WNode
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public int currentTime => _time;

        private int _time;
        private Coroutine _routine;


        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time"></param>
        public virtual void StartCountdown(int time)
        {
            StopCountdown();

            _time = time;

            _routine = DelayInvokeRepeating(1f, () =>
            {
                _time--;

                OnTick(_time);

                if (_time == 0)
                {
                    StopCountdown();

                    OnTimeup();
                }
            });
        }


        /// <summary>
        /// 停止倒计时
        /// </summary>
        public virtual void StopCountdown()
        {
            if (_routine != null)
            {
                CancelInvoke(_routine);

                _routine = null;
            }

            _time = 0;
        }

        /// <summary>
        /// 倒计时更新事件，每秒执行一次
        /// </summary>
        /// <param name="time"></param>
        protected virtual void OnTick(int time)
        {
        }

        /// <summary>
        /// 倒计时结束事件
        /// </summary>
        protected virtual void OnTimeup()
        {
        }
    }
}
