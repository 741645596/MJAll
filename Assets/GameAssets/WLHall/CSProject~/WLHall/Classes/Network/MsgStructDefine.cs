/// <summary>
/// chenzhiwei@weile.com
/// 网络消息相关结构定义
/// <summary>

using WLCore;

namespace WLHall
{
    public struct stGameMsgExtData
    {
        public uint dwServerID;                 //!<目标服务器ID
        public uint dwFromServer;               //!<发送服务器ID
        public uint dwPlayer;                   //!<用户ID
        public uint dwRoomID;					//!<房间ID(可选)
    };

    public struct stGUID
    {
        public uint Data1;
        public ushort Data2;
        public ushort Data3;
        public byte[] Data4;
    };

    public struct stMsgPacket
    {
        public stGUID guidPacket;           //!<包标识ID
        public uint dwPacketSize;           //!<包总长度,单位：字节
        public uint dwRealSize;             //!<实际大小,单位：字节
        public uint dwIndex;		        //!<包索引
    };

    public static class MsgUtils
    {
        public static void WriteGameMsgExtData(MsgHeader buffer, ref stGameMsgExtData data)
        {
            buffer.WriteUint32(data.dwServerID);
            buffer.WriteUint32(data.dwFromServer);
            buffer.WriteUint32(data.dwPlayer);
            buffer.WriteUint32(data.dwRoomID);
        }

        public static void ReadGameMsgExtData(MsgHeader buffer, ref stGameMsgExtData data)
        {
            data.dwServerID = buffer.ReadUint32();
            data.dwFromServer = buffer.ReadUint32();
            data.dwPlayer = buffer.ReadUint32();
            data.dwRoomID = buffer.ReadUint32();
        }

        public static void ReadGUID(MsgHeader buffer, ref stGUID data)
        {
            if (data.Data4 == null)
                data.Data4 = new byte[8];

            data.Data1 = buffer.ReadUint32();
            data.Data2 = buffer.ReadUint16();
            data.Data3 = buffer.ReadUint16();
            buffer.stream.Read(data.Data4, 0, 8);
        }

        public static void ReadMsgPacket(MsgHeader buffer, ref stMsgPacket data)
        {
            ReadGUID(buffer, ref data.guidPacket);
            data.dwPacketSize = buffer.ReadUint32();
            data.dwRealSize = buffer.ReadUint32();
            data.dwIndex = buffer.ReadUint32();
        }
    }
}
