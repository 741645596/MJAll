/// <summary>
/// chenzhiwei@weile.com
/// 网络消息分包处理
/// <summary>


using System;
using UnityEngine;
using Unity.Utility;
using System.Threading;
using WLCore;
using System.Collections.Generic;
using System.Text;

namespace WLHall
{
    public enum ePacketMsgAddState
    {
        PMAS_FAILED = -1,       //!<失败,不是一个合法分包
        PMAS_OK = 0,            //!<成功，包添加成功，继续等待其他分包
        PMAS_FINISHED = 1,      //!<完成，包接收完毕，可以处理
    };

    public class MsgPacket
    {
        private MsgHeader _msg;
        private int _packetCount;           //!<剩余分包数量

        public MsgPacket()
        {
        }

        public void Reset()
        {
            _packetCount = 0;
            RecycleMsgHeader();
        }

        public void Dispose()
        {
            RecycleMsgHeader();
        }

        private void RecycleMsgHeader()
        {
            if (_msg != null)
            {
                NetMsgFactory.RecycleNetMsg(_msg);
                _msg = null;
            }
        }

        public MsgHeader GetBuffer(bool bRemoveRef)
        {
            MsgHeader retMsg = _msg;
            if(bRemoveRef) _msg = null;
            return retMsg;
        }

        public ePacketMsgAddState AddPacketMsg(MsgHeader msg)
        {
            WLDebugTrace.Trace("[MsgPacket] AddPacketMsg");
            if ((msg.byFlag & MsgHeader.Flag.Packet) == 0 || msg.len < (MsgHeader.headerSize + Constants.PACKET_STRUCT_SIZE))
                return ePacketMsgAddState.PMAS_FAILED;

            stMsgPacket pkg = new stMsgPacket();
            msg.position = msg.len - Constants.PACKET_STRUCT_SIZE;
            MsgUtils.ReadMsgPacket(msg, ref pkg);
            WLDebugTrace.Trace("[MsgPacket] AddPacketMsg, idx=" + pkg.dwIndex + ",pkg size=" + pkg.dwPacketSize + ",real size=" + pkg.dwRealSize);

            if (_msg == null)
                _msg = NetMsgFactory.GetNetMsgIn();

            if (pkg.dwIndex == 0)
            {
                _msg.WriteBytes(msg.GetSteamBuffer(), 0, msg.byHeaderOffset);
                _msg.ReadHeader();

                _packetCount = (int)(pkg.dwPacketSize - msg.byHeaderOffset + Constants.MSG_PACKET_SIZE - 1) / Constants.MSG_PACKET_SIZE;
                WLDebugTrace.Trace("[MsgPacket] AddPacketMsg, init pkg count=" + _packetCount);
            }

            int dstOffset = msg.byHeaderOffset + Constants.MSG_PACKET_SIZE * (int)pkg.dwIndex;
            int dataLen = msg.len - Constants.PACKET_STRUCT_SIZE - msg.byHeaderOffset;
            WLDebugTrace.Trace("[MsgPacket] AddPacketMsg, dataLen=" + dataLen);
            // 因为都走小端, 这里直接拷贝字节流
            _msg.stream.SetLength(dstOffset + dataLen);
            Array.ConstrainedCopy(msg.GetSteamBuffer(), msg.byHeaderOffset, _msg.GetSteamBuffer(), dstOffset, dataLen);

            --_packetCount;
            return (_packetCount > 0) ? ePacketMsgAddState.PMAS_OK : ePacketMsgAddState.PMAS_FINISHED;
        }

        public bool RecvFinished()
        {
            WLDebugTrace.Trace("[MsgPacket] RecvFinished, msg stream len=" + _msg.streamLength);
            if (_msg == null || _msg.streamLength < MsgHeader.headerSize)
                return false;

            byte byMask = _msg.byMask;
            byte byFlag = _msg.byFlag;
            if ((byFlag & MsgHeader.Flag.Filled) == 0)
                return true;

            int len = (int)_msg.streamLength - _msg.byHeaderOffset;
            if(len > 0)
            {
                byte[] buffer = _msg.GetSteamBuffer();
                int idx = _msg.byHeaderOffset;
                if ((byFlag & MsgHeader.Flag.Mask) != 0)
                {
                    for (int i = len - 1; i >= 0; --i)
                    {
                        byMask ^= (byte)i;
                        buffer[idx + i] ^= byMask;
                    }
                }

                if ((byFlag & MsgHeader.Flag.Encode) != 0)
                {
                    for (int i = 0; i < len; ++i)
                        buffer[idx + i] = (byte)~ClientSocket.g_Decode_Table[buffer[idx + i]];
                }

                if ((byFlag & MsgHeader.Flag.Compress) != 0)
                {
                    byte[] uncompressData = null;
                    int outLen = 0;
                    ZipHelper.Decompress(buffer, _msg.byHeaderOffset, len, ref uncompressData, ref outLen);
                    WLDebugTrace.Trace("[MsgPacket] RecvFinished, Decompress size=" + outLen);
                    if (uncompressData != null && outLen > 0)
                    {
                        _msg.position = _msg.byHeaderOffset;
                        _msg.WriteBytes(uncompressData, 0, outLen);
                        WLDebugTrace.Trace("[MsgPacket] RecvFinished, total size=" + _msg.streamLength);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            byte flag = MsgHeader.Flag.Filled;
            _msg.byFlag &= (byte)~flag;
            _msg.len = 0;
            return true;
        }

    }
}
