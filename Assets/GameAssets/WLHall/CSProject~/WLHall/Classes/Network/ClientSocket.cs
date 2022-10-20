// @Author: tanjinhua
// @Date: 2021/8/12  18:06


using System;
using UnityEngine;
using Unity.Utility;
using System.Threading;
using WLCore;
using System.Collections.Generic;
using System.Text;

namespace WLHall
{
    public class ClientSocket
    {
        private static float _heartBeatInterval = 5f;           // 心跳间隔
        private static byte[] _byteCache = new byte[1024];      // 预生成字节
        private static uint _maxByte = 1024 * 1024;             // 超出则提示网络包过大

        private NetMessageDispatcher _msgDispatcher;
        private ISocketEvent _eventHandler;
        private Dictionary<string, MsgPacket> _dicPacket;
        private List<MsgPacket> _lsPacketCache;

        private int _buffUsedSize;                              // socket用, 移到这里方便后期更新

        private bool _bKeepAlive = false;
        private float _heartBeatElapsed = 0f;
        private uint _ping;

        public ClientSocket(ISocketEvent eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public void Dispose()
        {
            if (_msgDispatcher != null)
            {
                _msgDispatcher.Dispose();
                _msgDispatcher = null;
            }

            if(_dicPacket != null)
            {
                foreach (MsgPacket v in _dicPacket.Values)
                {
                    if (v != null)
                    {
                        v.Dispose();
                    }
                    else
                    {
                        WLDebug.LogWarning("错误提示：_dicPacket.Values is null value");
                    }
                }

                _dicPacket.Clear();
                _dicPacket = null;
            }

            if (_lsPacketCache != null)
            {
                for (int i = 0; i < _lsPacketCache.Count; ++i)
                {
                    MsgPacket pkg = _lsPacketCache[i];
                    if (pkg != null)
                    {
                        pkg.Dispose();
                    }
                    else
                    {
                        WLDebug.LogWarning("错误提示：_lsPacketCache[i] is null value, count = ", _lsPacketCache.Count);
                    }
                }

                _lsPacketCache.Clear();
                _lsPacketCache = null;
            }
        }

        public void Connect(string ip, int port, int timeout = 5000)
        {
            WLDebugTrace.Trace("[ClientSocket] Connect ip=" + ip + ",port=" + port);

            if (_msgDispatcher == null)
            {
                _msgDispatcher = new NetMessageDispatcher(QuickConfig.socketType);
                _msgDispatcher.connectSuccessCallBack = OnConnectSuccess;
                _msgDispatcher.connectFailCallBack = OnConnectFail;
                _msgDispatcher.closeCallBack = OnSocketClose;
                _msgDispatcher.newMsgCallBack = OnNewMessage;
                _msgDispatcher.readDataCallBack = OnReadData;
            }

            _msgDispatcher.Connect(ip, port, timeout);
        }

        public void StartKeepAlive()
        {
            _bKeepAlive = true;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            _buffUsedSize = 0;
            if (_msgDispatcher != null)
            {
                _msgDispatcher.Disconnect();
                _msgDispatcher = null;
            }
        }

        public void OnConnectSuccess()
        {
            if (_eventHandler != null)
            {
                _eventHandler.OnSocketOpen();
            }
        }

        public void OnConnectFail()
        {

        }

        protected void OnSocketClose(int errorCode)
        {
            _buffUsedSize = 0;
            if (_eventHandler != null)
            {
                _eventHandler.OnSocketClose(errorCode);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="flag"></param>
        public void SendMessage(MsgHeader msg, byte flag = MsgHeader.Flag.None)
        {
            if (msg == null || _msgDispatcher == null)
                return;

            try
            {
                if (WLDebugTrace.traceEnable && msg.messageID != 0xFFFF)
                    WLDebugTrace.Trace("[ClientSocket] SendMessage msgid=" + msg.messageID);

                if ((msg.byFlag & MsgHeader.Flag.Filled) == 0)
                    msg.byFlag = flag;

                if(_msgDispatcher.socketType == eSocketType.socket)
                {
                    if ((flag & MsgHeader.Flag.Filled) == 0)
                        msg.byFlag = flag;
                }

                EscapeMessage(msg);
                msg.WriteHeader();
                if (_msgDispatcher != null)
                    _msgDispatcher.SendMessage(msg);
            }
            catch { }
            finally { NetMsgFactory.RecycleNetMsg(msg); }
        }

        public void OnNewMessage(MsgHeader msg)
        {
            msg.ReadHeader();
            bool bUnEscapeMsg = UnEscapeMessage(msg);
            if (WLDebugTrace.traceEnable)
                WLDebugTrace.Trace("[ClientSocket] OnNewData UnEscapeMessage, msgid=" + msg.messageID + ",bytelen=" + msg.len);
            if (!bUnEscapeMsg)
            {
                WLDebug.LogWarning("ClientSocket.OnNewData: 消息数据解压失败");
                NetMsgFactory.RecycleNetMsg(msg);
                return;
            }

            // 心跳返回
            if (msg.messageID == 0xFFFF)
            {
                uint timestamp = msg.ReadUint32();
                _ping = (uint)DateTime.Now.Millisecond - timestamp;
                NetMsgFactory.RecycleNetMsg(msg);
                return;
            }

            if (_eventHandler != null)
                _eventHandler.OnSocketMessage(msg);
        }

        private void OnReadData(MsgHeader recvBuffer, int byteLen)
        {
            _buffUsedSize += byteLen;
            while (_buffUsedSize > 0)
            {
                recvBuffer.position = 0;
                int len = recvBuffer.ReadUint16();
                if (WLDebugTrace.traceEnable)
                    WLDebugTrace.Trace("[ClientSocket] ReadData, msg len=" + len);
                if (_buffUsedSize >= MsgHeader.headerSize && len <= _buffUsedSize)
                {
                    if (recvBuffer.len > Constants.MSG_MAX_SIZE)
                    {
                        WLDebugTrace.TraceError("[ClientSocket] ReadData error over max size, msgid=" + recvBuffer.messageID + ",len=" + recvBuffer.len);
                        Disconnect();
                        return;
                    }

                    MsgHeader msg = NetMsgFactory.GetNetMsgIn();
                    msg.WriteBytes(recvBuffer.GetSteamBuffer(), 0, len);
                    msg.ReadHeader();

                    _buffUsedSize -= len;
                    if (_buffUsedSize > 0)
                    {
                        recvBuffer.position = 0;
                        recvBuffer.WriteBytes(recvBuffer.GetSteamBuffer(), len, _buffUsedSize);
                    }

                    if ((msg.byFlag & MsgHeader.Flag.Packet) != 0)
                    {
                        if (WLDebugTrace.traceEnable)
                            WLDebugTrace.Trace("[ClientSocket] ReadData, is packet");
                        OnHallMsg(msg);
                    }
                    else
                    {
                        bool bUnEscapeMsg = UnEscapeMessage(msg);
                        if (WLDebugTrace.traceEnable)
                            WLDebugTrace.Trace("[ClientSocket] ReadData UnEscapeMessage, msgid=" + msg.messageID + ",bytelen=" + msg.len);
                        if (!bUnEscapeMsg)
                        {
                            WLDebugTrace.TraceError("[ClientSocket] ReadData msg UnEscapeMessage fail");
                            NetMsgFactory.RecycleNetMsg(msg);
                            Disconnect();
                            return;
                        }

                        // 心跳返回
                        if (msg.messageID == 0xFFFF)
                        {
                            uint timestamp = msg.ReadUint32();
                            _ping = (uint)DateTime.Now.Millisecond - timestamp;
                            NetMsgFactory.RecycleNetMsg(msg);
                        }
                        else
                        {
                            OnHallMsg(msg);
                        }
                    }
                }
                else break;
            }

            recvBuffer.position = _buffUsedSize;
        }

        private bool OnHallMsg(MsgHeader msg)
        {
            int wMsgOff = Constants.GetMsgOffset(msg.messageID);
            if(msg.messageID == Constants.MSG_KEEPALIVE || wMsgOff == Constants.MSG_OFFSET_LOBBY_TO_HALLSERVER) // 转发给房间 
            {
                if ((msg.byFlag & MsgHeader.Flag.Packet) != 0)
                {
                    MsgHeader newMsg = AddPacketMsg(msg);
                    NetMsgFactory.RecycleNetMsg(msg);

                    if (newMsg == null)
                        return true;

                    byte flag = MsgHeader.Flag.Packet;
                    newMsg.byFlag &= (byte)~flag;
                    OnHandlerProcessMsg(newMsg);
                    return true;
                }
            }

            // 游戏分包
            if (ResetPacketMsg(msg))
                return true;

            OnHandlerProcessMsg(msg);
            return false;
        }

        private bool ResetPacketMsg(MsgHeader msg)
        {
            if ((msg.byFlag & MsgHeader.Flag.Packet) != 0)
            {
                stGameMsgExtData extDat = new stGameMsgExtData();
                msg.position = MsgHeader.headerSize;
                MsgUtils.ReadGameMsgExtData(msg, ref extDat);

                byte[] buffer = msg.GetSteamBuffer();
                Array.ConstrainedCopy(buffer, Constants.GAME_MSG_HEADER_SIZE, buffer, MsgHeader.headerSize, (int)msg.streamLength - Constants.GAME_EXT_DATA_SIZE);
                msg.len -= Constants.GAME_EXT_DATA_SIZE;
                msg.byHeaderOffset -= Constants.GAME_EXT_DATA_SIZE;

                MsgHeader newMsg = AddPacketMsg(msg);
                NetMsgFactory.RecycleNetMsg(msg);

                if (newMsg == null)
                    return true;

                int len = (int)newMsg.streamLength;
                newMsg.stream.SetLength(len + Constants.GAME_EXT_DATA_SIZE);
                byte[] newBuffer = newMsg.GetSteamBuffer();
                Array.ConstrainedCopy(newBuffer, MsgHeader.headerSize, newBuffer, Constants.GAME_MSG_HEADER_SIZE, len - MsgHeader.headerSize);

                byte flag = MsgHeader.Flag.Packet;
                newMsg.byFlag &= (byte)~flag;
                newMsg.byHeaderOffset = Constants.GAME_MSG_HEADER_SIZE;
                newMsg.len = (ushort)newMsg.streamLength;
                newMsg.position = MsgHeader.headerSize;
                MsgUtils.WriteGameMsgExtData(newMsg, ref extDat);

                OnHandlerProcessMsg(newMsg);
                return true;
            }

            return false;
        }

        private void OnHandlerProcessMsg(MsgHeader msg)
        {
            msg.position = msg.byHeaderOffset;
            if (_eventHandler != null)
                _eventHandler.OnSocketMessage(msg);
            else
                NetMsgFactory.RecycleNetMsg(msg);
        }

        public void Update()
        {
            if(_bKeepAlive && _msgDispatcher != null && _msgDispatcher.IsConnected())
                KeepAlive();
        }

        protected void KeepAlive()
        {
            _heartBeatElapsed += Time.deltaTime;
            if (_heartBeatElapsed > _heartBeatInterval)
            {
                _heartBeatElapsed = 0;
                MsgHeader msg = NetMsgFactory.GetNetMsgOut();
                msg.messageID = 0xFFFF;

                msg.WriteUint32((uint)DateTime.Now.Millisecond);
                msg.WriteUint32(_ping);
                SendMessage(msg);

                if (WLDebugTrace.traceEnable)
                {
                    uint count = NetMsgFactory.GetNetUsedCount();
                    if (count > 0 && count % 10 == 0)
                        WLDebugTrace.Trace("[ClientSocket] Update, NetMsgFactory.GetNetUsedCount=" + count);
                }
            }
        }

        private bool UnEscapeMessage(MsgHeader msg)
        {
            byte byMask = msg.byMask;
            byte byFlag = msg.byFlag;
            if ((byFlag & MsgHeader.Flag.Filled) == 0)
                return true;

            int byteLen = (int)msg.streamLength;
            int wLen = byteLen - msg.byHeaderOffset;
            if (wLen > 0)
            {
                byte[] buffer = msg.GetSteamBuffer();
                int idx = msg.byHeaderOffset;
                if ((byFlag & MsgHeader.Flag.Mask) != 0)
                {
                    for (int i = wLen - 1; i >= 0; i--)
                    {
                        byMask ^= (byte)i;
                        buffer[idx + i] ^= byMask;
                    }
                }

                if ((byFlag & MsgHeader.Flag.Encode) != 0)
                {
                    if (_msgDispatcher.socketType == eSocketType.websocket)
                    {
                        if (WLDebugTrace.traceEnable)
                            WLDebugTrace.Trace("[ClientSocket] UnEscapeMessage, Encode");
                        return false;
                    }
                    else if(_msgDispatcher.socketType == eSocketType.socket)
                    {
                        for (int i = 0; i < wLen; ++i)
                            buffer[idx + i] = (byte)~g_Decode_Table[buffer[idx + i]];
                    }
                }

                if ((byFlag & MsgHeader.Flag.Compress) != 0)
                {
                    byte[] uncompressData = null;
                    int outLen = 0;
                    ZipHelper.Decompress(buffer, msg.byHeaderOffset, wLen, ref uncompressData, ref outLen);
                    if (WLDebugTrace.traceEnable)
                        WLDebugTrace.Trace("[ClientSocket] UnEscapeMessage, Decompress size=" + outLen);
                    if (uncompressData != null && outLen > 0)
                    {
                        msg.position = msg.byHeaderOffset;
                        msg.WriteBytes(uncompressData, 0, outLen);
                        msg.len = (ushort)(outLen + msg.byHeaderOffset);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (WLDebugTrace.traceEnable)
                        WLDebugTrace.Trace("[ClientSocket] UnEscapeMessage, no compress");
                }
            }

            byte flag = MsgHeader.Flag.Filled;
            msg.byFlag &= (byte)~flag;
            return true;
        }

        private void EscapeMessage(MsgHeader msg)
        {
            byte byFlag = msg.byFlag;
            if ((byFlag & MsgHeader.Flag.Filled) != 0)
                return;

            if ((byFlag & MsgHeader.Flag.Offset) == 0)
                msg.byHeaderOffset = (byte)MsgHeader.headerSize;

            long byteLen = msg.streamLength;
            long len = byteLen - msg.byHeaderOffset;
            if (len > 0)
            {
                byte[] data = GetByteCache((int)len);
                byte[] srcData = msg.GetSteamBuffer();
                Array.ConstrainedCopy(srcData, msg.byHeaderOffset, data, 0, (int)len);

                if ((byFlag & MsgHeader.Flag.Compress) != 0)
                {
                    byte[] compressData = null;
                    int outLen = 0;
                    ZipHelper.Compress(data, (int)len, ref compressData, ref outLen);
                    if (compressData != null && outLen > 0 && outLen < len)
                    {
                        msg.position = msg.byHeaderOffset;
                        msg.WriteBytes(compressData, 0, outLen);

                        len = outLen;
                        msg.len = (ushort)(outLen + msg.byHeaderOffset);
                    }
                    else
                    {
                        msg.byFlag ^= MsgHeader.Flag.Compress;
                    }
                }

                if ((byFlag & MsgHeader.Flag.Encode) != 0)
                {
                    if (_msgDispatcher.socketType == eSocketType.websocket)
                    {
                        msg.byFlag ^= MsgHeader.Flag.Encode;
                    }
                    else
                    {
                        byte[] buffer = msg.GetSteamBuffer();
                        int idx = msg.byHeaderOffset;
                        for (int i = 0; i < len; ++i)
                            buffer[idx+i] = g_Encode_Table[buffer[idx+i]];
                    }
                }

                if ((byFlag & MsgHeader.Flag.Mask) != 0)
                {
                    byte byMask = (byte)Utilities.Random(256);
                    byte[] buffer = msg.GetSteamBuffer();
                    int idx = msg.byHeaderOffset;
                    for (int i = 0; i < len; i++)
                    {
                        buffer[idx+i] ^= byMask;
                        byMask ^= (byte)i;
                    }
                    msg.byMask = byMask;
                }
            }

            msg.byFlag |= MsgHeader.Flag.Filled;
        }

        private MsgHeader AddPacketMsg(MsgHeader msg)
        {
            if (WLDebugTrace.traceEnable)
                WLDebugTrace.Trace("[ClientSocket] AddPacketMsg, msgid=" + msg.messageID + ",len=" + msg.len);
            if (msg.len < MsgHeader.headerSize + Constants.PACKET_STRUCT_SIZE)
                return null;

            int offset = msg.len - Constants.PACKET_STRUCT_SIZE;
            string strGuid = Encoding.UTF8.GetString(msg.GetSteamBuffer(), offset, Constants.GUID_LEN);
            if (_dicPacket == null)
                _dicPacket = new Dictionary<string, MsgPacket>();

            MsgPacket pkg = null;
            _dicPacket.TryGetValue(strGuid, out pkg);
            if (pkg == null)
            {
                pkg = GetMsgPkg();
                _dicPacket.Add(strGuid, pkg);
            }

            ePacketMsgAddState state = pkg.AddPacketMsg(msg);
            if (state == ePacketMsgAddState.PMAS_FAILED)
            {
                _dicPacket.Remove(strGuid);
                RecycleMsgPkg(pkg);
            }
            else if (state == ePacketMsgAddState.PMAS_FINISHED)
            {
                _dicPacket.Remove(strGuid);
                if (!pkg.RecvFinished())
                {
                    RecycleMsgPkg(pkg);
                    return null;
                }

                return pkg.GetBuffer(true);
            }
            
            return null;
        }

        private MsgPacket GetMsgPkg()
        {
            MsgPacket pkg = null;
            int len = (_lsPacketCache != null) ? _lsPacketCache.Count : 0;
            if (len > 0)
            {
                pkg = _lsPacketCache[len - 1];
                _lsPacketCache.RemoveAt(len - 1);
            }

            if (pkg == null)
                pkg = new MsgPacket();

            return pkg;
        }

        private void RecycleMsgPkg(MsgPacket pkg)
        {
            if (pkg == null)
                return;

            pkg.Reset();
            if (_lsPacketCache == null)
            {
                _lsPacketCache = new List<MsgPacket>(20);
                _lsPacketCache.Add(pkg);
            }
            else
            {
                if (_lsPacketCache.IndexOf(pkg) < 0)
                    _lsPacketCache.Add(pkg);
            }
        }

        private byte[] GetByteCache(int byteLen)
        {
            if (byteLen > _maxByte)
                WLDebug.LogWarning("[ClientSocket] GetByteCache warning, byteLen=" + byteLen);

            // 所需字节数超出则进行扩容
            if (byteLen >= _byteCache.Length)
            {
                WLDebugTrace.Trace("[ClientSocket] GetByteCache, byteLen=" + byteLen);
                int capacity = byteLen + 512;
                _byteCache = new byte[capacity];
            }

            return _byteCache;
        }

        public static byte[] g_Encode_Table = new byte[256]
        {
            205,240,162,226,237,136,227,152,    188,102,193,124,244,219, 71,150,
            246,108, 94, 17,175, 47, 64, 66,     65,  7,222, 76,138, 99, 77, 81,
            192,155, 56, 39, 25,  3,151,101,     61, 68,172,167, 24,160, 97, 19,
            179,180,198, 33, 21,224,197, 15,    120,196,239, 44, 83, 38, 46,103,
            84, 95,213,200,170, 23, 70,149,     163,148,251,186,211,189,100, 42,
            191, 52, 72, 53, 67,215,245,207,    144,146, 45,181, 93,147,153, 80,
            116,114, 49,  4, 88, 16, 90,127,    255,202, 85, 55,178,221,229, 10,
            13,105,177, 58,  0, 60,234, 34,      50,141,242,156,134, 28,176,118,
            48,  1,210,  6,187,119,243,128,     232,166,  5,236,137, 73,253,217,
            214,212, 69,111, 79,184, 51, 87,    216,135,168,158,249, 92, 35,182,
            107,235,126, 31,  2,254,133,233,     18,201,174,  8,159, 82, 37,113,
            9, 63, 41,104, 59, 26,231,145,       89,122,110,142, 86,164, 29, 30,
            169,154,223,112,140, 78, 75,220,    238, 54,143,195,131,130,228,139,
            121,109, 12,161,125, 57, 74,190,    250,171,209,199, 40,123,204,247,
            183,173, 98,185,208,248,241, 14,     27,203,218,230, 43, 96, 22,194,
            129, 11,115,165, 32,132, 91, 36,    157,117,225,206, 20,106, 62,252,
        };

        public static byte[] g_Decode_Table = new byte[256]
        {
            139,126, 91,218,156,117,124,230,     84, 79,144, 14, 45,143, 24,200,
            154,236, 87,208,  3,203, 17,186,    211,219, 74, 23,130, 65, 64, 92,
            11,204,136, 97,  8, 81,194,220,      35, 77,176, 19,196,165,193,234,
            127,157,135,105,174,172, 54,148,    221, 42,140, 75,138,215,  1, 78,
            233,231,232,171,214,109,185,241,    173,114, 41, 57,228,225, 58,107,
            160,224, 82,195,191,149, 67,104,    155, 71,153,  9, 98,163,237,190,
            18,209, 29,226,177,216,246,192,      76,142,  2, 95,238, 46, 69,108,
            60, 80,158, 13,159,  6,128,122,     199, 47, 70, 34,244, 43, 93,152,
            120, 15, 50, 51, 10, 89,131,102,    250,115,227, 48, 59,134, 68, 53,
            167, 72,166,162,182,184,240,217,    248,161, 62,222,132,  7,100, 83,
            210, 44,253,183, 66, 12,118,212,    101, 63,187, 38,213, 30, 85,235,
            129,141,147,207,206,164, 96, 31,    106, 28,180,123,247,178, 40,175,
            223,245, 16, 52,198,201,205, 36,    188, 86,150, 22, 33,255,  4,168,
            27, 37,125,179,110,189,111,170,     103,112, 21,242, 56,146,229, 61,
            202,  5,252,249, 49,145, 20, 73,    119, 88,137, 94,116,251, 55,197,
            254, 25,133,121,243,169,239, 32,     26, 99, 39,181,  0,113, 90,151,
        };
    }
}
