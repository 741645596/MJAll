/// <summary>
/// chenzhiwei@weile.com
/// 缓存网络消息结构
/// <summary>

using System.Collections.Generic;
namespace WLCore
{
    public static class NetMsgFactory
    {
        private static List<MsgHeader> _msgHeaderPool = new List<MsgHeader>(50);
        private static object _lock = new object();
        private static uint _msgUsedCount = 0;                  // 记录使用个数, 方便追踪是否有漏掉cache

        public static uint GetNetUsedCount() { return _msgUsedCount; }

        private static MsgHeader GetNetMsg()
        {
            lock (_lock)
            {
                MsgHeader msg = null;
                int len = (_msgHeaderPool != null) ? _msgHeaderPool.Count : 0;
                if (len > 0)
                {
                    msg = _msgHeaderPool[len - 1];
                    _msgHeaderPool.RemoveAt(len - 1);
                }

                if (msg == null)
                    msg = new MsgHeader();

                ++_msgUsedCount;
                return msg;
            }
        }

        /// <summary>
        /// 获取网络接收消息包
        /// </summary>
        /// <returns></returns>
        public static MsgHeader GetNetMsgIn()
        {
            return GetNetMsg();
        }

        /// <summary>
        /// 获取网络发送消息包
        /// </summary>
        /// <returns></returns>
        public static MsgHeader GetNetMsgOut(long headerOffset = 8)
        {
            MsgHeader msgOut = GetNetMsg();
            msgOut.position = headerOffset;
            return msgOut;
        }

        public static void RecycleNetMsg(MsgHeader msg)
        {
            lock (_lock)
            {
                if (msg == null)
                    return;

                --_msgUsedCount;
                msg.Reset();
                if (_msgHeaderPool == null)
                {
                    _msgHeaderPool = new List<MsgHeader>(20);
                    _msgHeaderPool.Add(msg);
                }
                else
                {
                    if (_msgHeaderPool.IndexOf(msg) < 0)
                        _msgHeaderPool.Add(msg);
                }
            }
        }
    }
}
