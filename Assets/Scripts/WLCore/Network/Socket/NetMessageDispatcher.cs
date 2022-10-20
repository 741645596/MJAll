/// <summary>
/// chenzhiwei@weile.com
/// 网络消息处理接口
/// <summary>


using System;

namespace WLCore
{
    public enum eSocketType
    {
        websocket,
        socket,
    }

    /// <summary>
    /// 网络消息中转 - 把消息解包处理移到可更新模块
    /// </summary>
    public class NetMessageDispatcher
    {
        private INetSocket _socket;
        private eSocketType _socketType;

        public Action connectSuccessCallBack;
        public Action connectFailCallBack;
        public Action<int> closeCallBack;
        public Action<MsgHeader> newMsgCallBack;
        public Action<MsgHeader, int> readDataCallBack;

        public eSocketType socketType { get { return _socketType; } }

        public NetMessageDispatcher(eSocketType socketType)
        {
            _socketType = socketType;
        }

        public void Dispose()
        {
            if (_socket != null)
            {
                _socket.Dispose();
                _socket = null;
            }
        }

        public void Connect(string ip, int port, int timeout)
        {
            //WLDebugTrace.Trace("[NetMessageDispatcher] Connect ip=" + ip + ",port=" + port);

            if (_socket == null)
            {
                InitSocket();
            }

            if (_socket != null)
            {
                _socket.Connect(ip, port, timeout);
            }
        }

        private void InitSocket()
        {
            //WLDebugTrace.TraceInfo("[NetMessageDispatcher] InitSocket, socket type=" + _socketType);
            if (_socketType == eSocketType.websocket)
            {
                WLDebug.LogError("已不支持Websocket");
                //_socket = new WebSocket();
            }
            else if (_socketType == eSocketType.socket)
            {
                _socket = new NetSocket();
            }
                
            if (_socket != null)
            {
                _socket.newMsgDelegate = OnNewData;
                _socket.readDataDelegate = OnReadData;
                _socket.connectSuccessDelegate = OnConnectSuccess;
                _socket.connectFailDelegate = OnConnectFail;
                _socket.closeDelegate = OnSocketClose;
            }
        }

        public bool IsConnected()
        {
            return _socket != null && _socket.IsConnected();
        }

        private void OnConnectSuccess()
        {
            if (connectSuccessCallBack != null)
                connectSuccessCallBack();

            if (_socket != null)
                _socket.ReadMsg();
        }

        private void OnConnectFail()
        {
            if (connectFailCallBack != null)
                connectFailCallBack();
        }

        private void OnSocketClose(int errorCode)
        {
            if (closeCallBack != null)
                closeCallBack(errorCode);
        }

        public void Disconnect()
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }

        public void SendMessage(MsgHeader msg)
        {
            if (msg == null)
                return;

            if (_socket != null)
            {
                if(!_socket.IsConnected())
                {
                    //WLDebugTrace.Trace("[NetMessageDispatcher] SendMessage, socket close");
                    _socket.Close();
                    return;
                }

                //WLDebugTrace.Trace("[NetMessageDispatcher] SendMessage msg=" + msg.ToString());
                _socket.SendMsg(msg.GetSteamBuffer(), 0, msg.len);
            }
        }

        private void OnNewData(MsgHeader msg)
        {
            //WLDebugTrace.Trace("[NetMessageDispatcher] OnNewData");
            if (newMsgCallBack != null)
                newMsgCallBack(msg);
            else
                NetMsgFactory.RecycleNetMsg(msg);
        }

        private void OnReadData(MsgHeader readBuffer, int byteLen)
        {
            if (readDataCallBack != null)
                readDataCallBack(readBuffer, byteLen);
        }
    }
}
