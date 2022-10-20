/// <summary>
/// chenzhiwei@weile.com
/// 网络消息处理接口
/// <summary>

using System;
namespace WLCore
{
    public interface INetSocket : IDisposable
    {
        void Connect(string ip, int port, int timeout);
        void SendMsg(byte[] data, int offset, int len);
        void ReadMsg();
        void Close();
        bool IsConnected();

        Action<MsgHeader> newMsgDelegate { set; }
        Action<MsgHeader, int> readDataDelegate { set; }
        Action connectSuccessDelegate { set; }
        Action connectFailDelegate { set; }
        Action<int> closeDelegate { set; }

        void OnConnectSuccess();
        void OnConnectFail();
        void OnClose();
    }
}
