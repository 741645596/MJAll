/// <summary>
/// chenzhiwei@weile.com
/// 网络消息处理接口
/// <summary>

using System;
using System.Net;
using System.Net.Sockets;

namespace WLCore
{
    public class NetSocket : INetSocket
    {
        private static readonly int _maxReceiveSize = 8192;

        private Socket _socket;
        private SocketError _socketError = SocketError.Success;
        private AsyncCallback _connectCallback;
        private AsyncCallback _receiveDataCallback;
        private AsyncCallback _sendDataCallback;
        private MsgHeader _recvBuffer;

        private Action<MsgHeader, int> _readDataDelegate;
        private Action _connectSuccessDelegate;
        private Action _connectFailDelegate;
        private Action<int> _closeDelegate;

        public Action<MsgHeader> newMsgDelegate { set { } }
        public Action<MsgHeader, int> readDataDelegate { set { _readDataDelegate = value; } }
        public Action connectSuccessDelegate { set { _connectSuccessDelegate = value; } }
        public Action connectFailDelegate { set { _connectFailDelegate = value; } }
        public Action<int> closeDelegate { set { _closeDelegate = value; } }

        public NetSocket()
        {
            _connectCallback = new AsyncCallback(OnConnect);
            _receiveDataCallback = new AsyncCallback(OnReceiveData);
            _sendDataCallback = new AsyncCallback(OnSendData);
        }

        public void Connect(string ip, int port, int timeout)
        {
            if (IsConnected())
            {
                //WLDebugTrace.TraceInfo("[NetSocket] Connect, socket already connected");
                return;
            }

            CloseSocket();

            IPAddress ipv6Address = null;   // 测试一下服务器, 服务端已支持ipv6
            IPAddress ipAddress = null;
            bool success = IPAddress.TryParse(ip, out ipAddress);
            if (!success)
            {
                IPHostEntry hostInfo = Dns.GetHostEntry(ip);
                int len = (hostInfo.AddressList != null) ? hostInfo.AddressList.Length : 0;
                for (int i = 0; i < len; ++i)
                {
                    IPAddress curr = hostInfo.AddressList[i];
                    if (curr == null) continue;
                    if (ipv6Address == null && curr.AddressFamily == AddressFamily.InterNetworkV6)
                        ipv6Address = curr;
                    if (ipAddress == null)
                        ipAddress = curr;
                }
                if (ipv6Address != null && Socket.OSSupportsIPv6)
                    ipAddress = ipv6Address;
            }

            AddressFamily connectFamily = (ipAddress.AddressFamily == AddressFamily.InterNetworkV6) ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;
            _socket = new Socket(connectFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.ReceiveBufferSize = ushort.MaxValue;
            _socket.SendBufferSize = ushort.MaxValue;
            _socket.ReceiveTimeout = 30000;
            _socket.SendTimeout = 30000;

            IAsyncResult result = _socket.BeginConnect(ipAddress, port, _connectCallback, _socket);
            result.AsyncWaitHandle.WaitOne(timeout, true);
            if (!_socket.Connected)
            {
                OnConnectFail();
            }
        }

        private void OnConnect(System.IAsyncResult ar)
        {
            try
            {
                Socket s = ar.AsyncState as Socket;
                if (s != null && (int)s.Handle > 0)
                {
                    s.EndConnect(ar);
                }
                    
                if (s == null || !s.Connected)
                {
                    OnConnectFail();
                }
                else
                {
                    if (_recvBuffer == null)
                    {
                        _recvBuffer = NetMsgFactory.GetNetMsgIn();
                        _recvBuffer.stream.SetLength(_maxReceiveSize);
                    }

                    OnConnectSuccess();

                    if (_readDataDelegate != null)
                    {
                        BeginReceive(s);
                    }
                }
            }
            catch
            {
                OnConnectFail();
            }
        }

        public void SendMsg(byte[] data, int offset, int len)
        {
            if (!IsConnected())
            {
                CloseSocket();
                return;
            }

            try
            {
                //_socket.BeginSend(data, offset, len, SocketFlags.None, out _socketError, _sendDataCallback, _socket);
                _socket.Send(data, offset, len, SocketFlags.None, out _socketError);
            }
            catch
            {
                WLDebugTrace.TraceError("[NetSocket] SendMsg error");
                Close();
            }
        }

        private void OnSendData(System.IAsyncResult ar)
        {
            try
            {
                Socket worker = ar.AsyncState as Socket;
                if (worker != null && (int)worker.Handle > 0)
                {
                    int byteLen = worker.EndSend(ar, out _socketError);
                    //if(WLDebugTrace.traceEnable)
                    //    WLDebugTrace.Trace("[NetSocket] OnSendData, byte len=" + byteLen);
                }
                else
                {
                    WLDebugTrace.Trace("[NetSocket] OnSendData worker invalid");
                }
            }
            catch (Exception e)
            {
                WLDebugTrace.Trace("[NetSocket] OnSendData error=" + e.Message);
            }
        }

        private void BeginReceive(Socket s)
        {
            try
            {
                if (IsConnected())
                {
                    int offset = (int)_recvBuffer.position;
                    s.BeginReceive(_recvBuffer.GetSteamBuffer(), offset, _maxReceiveSize - offset, SocketFlags.None, out _socketError, _receiveDataCallback, s);
                }
                else
                {
                    WLDebugTrace.TraceError("[NetSocket] BeginReceive socket is closed");
                    Close();
                }
            }
            catch (Exception e)
            {
                WLDebugTrace.TraceError("[NetSocket] BeginReceive error=" + e.Message);
                Close();
            }
        }

        public void ReadMsg()
        {
            // do nothing
        }

        private void OnReceiveData(System.IAsyncResult ar)
        {
            Socket s = ar.AsyncState as Socket;
            if (s == null || (int)s.Handle <= 0)
                return;

            int byteRead = -1;
            try
            {
                if (s != null && (int)s.Handle > 0)
                {
                    byteRead = s.EndReceive(ar, out _socketError);
                    //if (WLDebugTrace.traceEnable)
                    //    WLDebugTrace.Trace("[NetSocket] OnReceiveData, byte len=" + byteRead);
                }
            }
            catch (Exception e)
            {
                WLDebugTrace.Trace("[NetSocket] OnReceiveData error=" + e.Message);
            }

            if (byteRead < 0)
            {
                WLDebug.Log("[NetSocket] OnReceiveData byteRead=0", byteRead);
                Close();
                return;
            }

            if (_readDataDelegate != null)
            {
                _readDataDelegate(_recvBuffer, byteRead);
            }

            BeginReceive(s);
        }

        private void CloseSocket()
        {
            if (_recvBuffer != null)
            {
                NetMsgFactory.RecycleNetMsg(_recvBuffer);
                _recvBuffer = null;
            }

            if (_socket != null)
            {
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }

                _socket.Dispose();
                _socket = null;
            }
        }

        public void Close()
        {
            CloseSocket();
            OnClose();
        }

        public void Dispose()
        {
            CloseSocket();

            _connectSuccessDelegate = null;
            _connectFailDelegate = null;
            _closeDelegate = null;
        }

        public bool IsConnected()
        {
            return _socket != null && _socket.Connected && ((int)_socket.Handle > 0);
        }

        public void OnConnectSuccess()
        {
            //WLDebugTrace.Trace("[NetSocket] OnConnectSuccess");
            if (_connectSuccessDelegate != null)
            {
                _connectSuccessDelegate();
            }
        }

        public void OnConnectFail()
        {
            //WLDebugTrace.Trace("[NetSocket] OnConnectFail");

            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }

            if (_connectFailDelegate != null)
            {
                _connectFailDelegate();
            }
        }

        public void OnClose()
        {
            if (_closeDelegate != null)
            {
                _closeDelegate((int)_socketError);
            }
        }
    }
}
