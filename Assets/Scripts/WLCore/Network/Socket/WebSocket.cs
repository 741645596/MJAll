///// <summary>
///// chenzhiwei@weile.com
///// 网络消息处理
///// <summary>

//using System;
//using System.Net.WebSockets;
//using System.Threading;

//namespace WLCore
//{
//    public class WebSocket : INetSocket
//    {
//        private ClientWebSocket _ws;
//        private byte[] _recvBuffer = new byte[100 * 1024];

//        private Action<MsgHeader> _newMsgDelegate;
//        private Action _connectSuccessDelegate;
//        private Action _connectFailDelegate;
//        private Action _closeDelegate;

//        public Action<MsgHeader> newMsgDelegate { set { _newMsgDelegate = value; } }
//        public Action<MsgHeader, int> readDataDelegate { set { } }
//        public Action connectSuccessDelegate { set { _connectSuccessDelegate = value; } }
//        public Action connectFailDelegate { set { _connectFailDelegate = value; } }
//        public Action closeDelegate { set { _closeDelegate = value; } }

//        public async void Connect(string ip, int port, int timeout)
//        {
//            WLDebugTrace.Trace("[WebSocket] Connect ip=" + ip + ",port=" + port);
            
//            if (_ws != null)
//            {
//                WebSocketState curState = _ws.State;
//                if (curState == WebSocketState.Open)
//                {
//                    // 追踪多次调用源
//                    WLDebugTrace.Trace("[WebSocket] Connect, already connected");
//                    return;
//                }

//                CloseWebSocket();
//            }

//            _ws = new ClientWebSocket();
//            //_ws.Options.SetRequestHeader("datatype", "1");        // websocket不启用压缩

//            string url = ip;
//            if(port > 0) url = $"ws://{ip}:{port}";
//            Uri uri = new Uri(url);

//            CancellationTokenSource cts = new CancellationTokenSource(timeout);
//            await _ws.ConnectAsync(uri, cts.Token);
//            WebSocketState state = _ws.State;
//            if (state == WebSocketState.Open)
//                OnConnectSuccess();
//            else
//                OnConnectFail();
//        }

//        public async void SendMsg(byte[] data, int offset, int len)
//        {
//            if (_ws == null || _ws.State != WebSocketState.Open)
//            {
//                WLDebugTrace.Trace("[WebSocket] SendMsg, socket is not connect");
//                Close();
//                return;
//            }

//            try
//            {
//                await _ws.SendAsync(new ArraySegment<byte>(data, offset, len), WebSocketMessageType.Binary, true, CancellationToken.None);
//            }
//            catch
//            {
//                WLDebugTrace.Trace("[WebSocket] SendMsg error, state=" + _ws.State);
//                Close();
//            }
//        }

//        public async void ReadMsg()
//        {
//            while (_ws != null)
//            {
//                if (_ws.State != WebSocketState.Open)
//                {
//                    Close();
//                    return;
//                }

//                MsgHeader msg = null;
//                try
//                {
//                    if (WLDebugTrace.traceEnable)
//                        WLDebugTrace.Trace("[WebSocket] ReadMsg start" + ",threadid=" + Thread.CurrentThread.ManagedThreadId.ToString());

//                    msg = NetMsgFactory.GetNetMsgIn();
//                    WebSocketReceiveResult recvResult = null;
//                    Array.Clear(_recvBuffer, 0, _recvBuffer.Length);
//                    ArraySegment<byte> arrData = new ArraySegment<byte>(_recvBuffer);

//                    do
//                    {
//                        recvResult = await _ws.ReceiveAsync(arrData, CancellationToken.None);
//                        if (recvResult == null) break;
//                        if (_ws.State == WebSocketState.CloseReceived && recvResult.MessageType == WebSocketMessageType.Close)
//                        {
//                            //WLDebugTrace.Trace("[WebSocket] ReadMsg, close start");
//                            await _ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
//                            //WLDebugTrace.Trace("[WebSocket] ReadMessage, close end");
//                            break;
//                        }

//                        if (recvResult.Count > 0)
//                        {
//                            if (WLDebugTrace.traceEnable)
//                                WLDebugTrace.Trace("[WebSocket] ReadMsg, count=" + recvResult.Count);
//                            msg.WriteBytes(arrData.Array, 0, recvResult.Count);
//                        }

//                        if (recvResult.EndOfMessage)
//                        {
//                            if (_newMsgDelegate != null)
//                                _newMsgDelegate(msg);
//                            else
//                                NetMsgFactory.RecycleNetMsg(msg);

//                            break;
//                        }
//                    } while (true);
//                }
//                catch (Exception e)
//                {
//                    WLDebugTrace.Trace("[WebSocket] ReadMsg, e=" + e.Message);
//                    Close();
//                }
//                finally
//                {
//                    if (msg != null)
//                        NetMsgFactory.RecycleNetMsg(msg);
//                }
//            }
//        }

//        public void Dispose()
//        {
//            WLDebugTrace.Trace("[WebSocket] Dispose");
//            CloseWebSocket();

//            _newMsgDelegate = null;
//            _connectSuccessDelegate = null;
//            _connectFailDelegate = null;
//            _closeDelegate = null;
//        }

//        public void Close()
//        {
//            WLDebugTrace.Trace("[WebSocket] Close");
//            CloseWebSocket();
//            OnClose();
//        }

//        public bool IsConnected()
//        {
//            return _ws != null && _ws.State == WebSocketState.Open;
//        }

//        public void OnConnectSuccess()
//        {
//            WLDebugTrace.Trace("[WebSocket] OnConnectSuccess");
//            if (_connectSuccessDelegate != null)
//                _connectSuccessDelegate();
//        }

//        public void OnConnectFail()
//        {
//            WLDebugTrace.Trace("[WebSocket] OnConnectFail state=" + _ws.State);

//            CloseWebSocket();
//            if (_connectFailDelegate != null)
//                _connectFailDelegate();
//        }

//        public void OnClose()
//        {
//            if (_closeDelegate != null)
//                _closeDelegate();
//        }

//        private void CloseWebSocket()
//        {
//            WLDebugTrace.Trace("[WebSocket] CloseWebSocket");
//            if (_ws != null)
//            {
//                _ws.Abort();
//                _ws.Dispose();
//                _ws = null;
//            }
//        }
//    }
//}
