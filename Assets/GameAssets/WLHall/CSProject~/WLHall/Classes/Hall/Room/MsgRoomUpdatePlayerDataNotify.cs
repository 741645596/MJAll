
using WLCore;

namespace WLHall
{
    public struct MsgRoomUpdatePlayerDataNotify
    {
        public long llMoney;
        public long llScore;
        public uint dwPrestige;
        public uint dwWinCount;
        public uint dwLostCount;
        public uint dwDrawCount;
        public uint dwFleeCount;
        public int nGameData1;
        public int nGameData2;
        public uint dwPKAwardCount;
        public uint dwPKKempCount;
        public uint dwUserID;
        public uint dwCustom;
        public ushort wReserved;
        public byte[] cbTaskData;
        public byte[] cbMatchData;
        public bool bNotifyClient;

        public static MsgRoomUpdatePlayerDataNotify From(MsgHeader msg)
        {
            var data = new MsgRoomUpdatePlayerDataNotify
            {
                llMoney = msg.ReadInt64(),
                llScore = msg.ReadInt64(),
                dwPrestige = msg.ReadUint32(),
                dwWinCount = msg.ReadUint32(),
                dwLostCount = msg.ReadUint32(),
                dwDrawCount = msg.ReadUint32(),
                dwFleeCount = msg.ReadUint32(),
                nGameData1 = msg.ReadInt32(),
                nGameData2 = msg.ReadInt32(),
                dwPKAwardCount = msg.ReadUint32(),
                dwPKKempCount = msg.ReadUint32(),
                dwUserID = msg.ReadUint32(),
                dwCustom = msg.ReadUint32(),
                wReserved = msg.ReadUint16(),
                cbTaskData = new byte[Constants.GAME_EXTEND_DATA_LEN],
                cbMatchData = new byte[Constants.GAME_EXTEND_DATA_LEN]
            };

            for (int i = 0; i < Constants.GAME_EXTEND_DATA_LEN; i++)
            {
                data.cbTaskData[i] = (byte)msg.ReadByte();
            }

            for (int i = 0; i < Constants.GAME_EXTEND_DATA_LEN; i++)
            {
                data.cbMatchData[i] = (byte)msg.ReadByte();
            }

            data.bNotifyClient = msg.ReadByte() == 1;

            return data;
        }
    }
}
