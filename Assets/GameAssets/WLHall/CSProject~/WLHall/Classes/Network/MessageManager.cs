// @Author: tanjinhua
// @Date: 2021/3/23  11:04


using System;
using System.Collections.Generic;
using WLCore;

namespace WLHall
{
    public abstract class MessageManager : ISocketEvent, IDisposable
    {
        private string _name;
        private Dictionary<ushort, Action<MsgHeader>> _processFuncs;
        private Queue<MsgHeader> _defaultQueue;    // 进队后马上执行
        private Queue<MsgHeader> _waitingQueue;    // 进队后等待，调用StartProcessWaitingQueue后开始执行
        private bool _processWaitingQueue = false;


        protected ClientSocket _cs;


        public MessageManager()
        {
            _name = GetType().Name;
            _processFuncs = new Dictionary<ushort, Action<MsgHeader>>();
            _defaultQueue = new Queue<MsgHeader>();
            _waitingQueue = new Queue<MsgHeader>();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Dispose()
        {
            UnregisterMessageProcessors();

            ClearNetMsgQueue();

            _defaultQueue = null;

            _waitingQueue = null;

            _processFuncs = null;

            _cs?.Dispose();
            _cs = null;
        }

        /// <summary>
        /// 注册消息处理方法
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="processor"></param>
        public void RegisterMessageProcessor(ushort msgId, Action<MsgHeader> processor)
        {
            _processFuncs[msgId] = processor;
        }


        /// <summary>
        /// 取消消息处理方法
        /// </summary>
        /// <param name="msgId"></param>
        public void UnregisterMessageProcessor(ushort msgId)
        {
            _processFuncs.Remove(msgId);
        }


        /// <summary>
        /// 清除所有消息处理方法
        /// </summary>
        public void UnregisterMessageProcessors()
        {
            _processFuncs.Clear();
        }


        /// <summary>
        /// 获取消息处理方法
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public Action<MsgHeader> GetMessageProcessor(ushort msgId)
        {
            if (_processFuncs.ContainsKey(msgId))
            {
                return _processFuncs[msgId];
            }
            return null;
        }


        /// <summary>
        /// 执行消息处理方法
        /// </summary>
        /// <param name="msg"></param>
        public virtual void OnProcessMessage(MsgHeader msg)
        {
            WLDebugTrace.Trace("[MessageManager] OnProcessMessage, msgid=" + msg.messageID);
            ushort msgId = msg.messageID;

            Action<MsgHeader> processor = GetMessageProcessor(msgId);

            if (processor != null)
            {
                processor.Invoke(msg);
                NetMsgFactory.RecycleNetMsg(msg);
                return;
            }

            WLDebug.LogWarning($"{_name} 未找到消息处理方法, messageId: {msgId}");
        }


        /// <summary>
        /// 开始执行等待队列
        /// </summary>
        public void StartProcessWaitingQueue()
        {
            _processWaitingQueue = true;
        }


        /// <summary>
        /// 是否需要进入等待队列
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected abstract bool NeedsWaiting(MsgHeader msg);


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public virtual void SendData(MsgHeader msg, byte flag = MsgHeader.Flag.None)
        {
            _cs?.SendMessage(msg, flag);
        }


        #region Socke Events
        public virtual void OnSocketClose(int errorCode)
        {
            WLDebug.Log($"{_name} socket close. code = {errorCode}");
        }

        public virtual void OnSocketError()
        {
            WLDebug.LogWarning($"{_name} OnSocketError");
        }

        public virtual void OnSocketMessage(MsgHeader msg)
        {
            if (NeedsWaiting(msg))
            {
                _waitingQueue.Enqueue(msg);
            }
            else
            {
                _defaultQueue.Enqueue(msg);
            }
        }

        public virtual void OnSocketOpen()
        {
            WLDebug.Log($"{_name} Socket Open");
        }
        #endregion


        /// <summary>
        /// 由使用者调用
        /// </summary>
        public virtual void Update()
        {
            _cs?.Update();

            while (_defaultQueue != null && _defaultQueue.Count > 0)
            {
                MsgHeader msg = _defaultQueue.Dequeue();
                if (msg != null)
                {
                    OnProcessMessage(msg);
                }
            }

            while (_waitingQueue != null && _waitingQueue.Count > 0 && _processWaitingQueue)
            {
                MsgHeader msg = _waitingQueue.Dequeue();
                if (msg != null)
                {
                    OnProcessMessage(msg);
                }
            }
        }

        protected void ClearNetMsgQueue()
        {
            if (_defaultQueue != null)
            {
                while (_defaultQueue.Count > 0)
                {
                    MsgHeader msg = _defaultQueue.Dequeue();
                    NetMsgFactory.RecycleNetMsg(msg);
                }
            }

            if (_waitingQueue != null)
            {
                while (_waitingQueue.Count > 0)
                {
                    MsgHeader msg = _waitingQueue.Dequeue();
                    NetMsgFactory.RecycleNetMsg(msg);
                }
            }
        }
    }
}
