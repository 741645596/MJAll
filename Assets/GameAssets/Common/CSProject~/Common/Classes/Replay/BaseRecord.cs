// @Author: tanjinhua
// @Date: 2021/12/14  19:31


using WLCore;
using WLCore.Coroutine;
using WLHall.Game;
using System.Collections;
using UnityEngine;

namespace Common
{
    public abstract class BaseRecord
    {

        public BaseGameStage stage { get; internal set; }

        internal int index;
        internal float interval;
        private string _proxyKey => GetType().Name;
        private CoroutineProxy _proxy;
        private CoroutineProxy proxy
        {
            get
            {
                if (_proxy == null)
                {
                    _proxy = CoroutineManager.RegisterCoroutineProxy(_proxyKey);
                }
                return _proxy;
            }
        }

        /// <summary>
        /// 实例后触发
        /// </summary>
        public virtual void OnInitialize()
        {
        }


        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="msg"></param>
        public abstract void Read(MsgHeader msg);


        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回动画时间(s)</returns>
        public abstract float Execute();


        /// <summary>
        /// 撤销
        /// </summary>
        public abstract void Undo();


        /// <summary>
        /// 中断
        /// </summary>
        public abstract void Abort();

        /// <summary>
        /// 重播一次
        /// </summary>
        public virtual void Replay()
        {
            Undo();

            interval = Execute();
        }


        /// <summary>
        /// 弃用，切换局数或者退出回放时调用
        /// </summary>
        public virtual void Depose()
        {
            stage = null;

            if (_proxy != null)
            {
                CoroutineManager.UnregisterCoroutineProxy(_proxyKey);
                _proxy = null;
            }
        }

        /// <summary>
        /// 开始协程
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return proxy.StartCoroutine(enumerator);
        }

        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="coroutine"></param>
        public void StopCoroutine(Coroutine coroutine)
        {
            _proxy?.StopCoroutine(coroutine);
        }

        /// <summary>
        /// 停止全部协程
        /// </summary>
        public void StopCoroutine()
        {
            _proxy?.StopAllCoroutines();
        }
    }
}
