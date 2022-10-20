

using WLCore;

namespace WLHall
{
    public class UserGameInfo
    {
        public long llScore;
        public uint dwWinCount;
        public uint dwLostCount;
        public uint dwDrawCount;
        public uint dwFleeCount;
        public int nGameData1;
        public int nGameData2;
        public uint dwPKKempCount;
        public uint dwPKAwardCount;
        public uint dwPKCount;
        public uint dwPKWinCount;
        public uint dwPKEliminate;
        public ushort wDeskID;
        public ushort wChairID;
        public MsgSystemTime tmLastSave;
        public byte[] cbTaskData;
        public byte[] cbMatchData;
        public byte byReady;
        public byte byLookOn;
        public ushort wReserved;
        public uint dwCustom;

        public static UserGameInfo From(MsgHeader msg)
        {
            UserGameInfo info = new UserGameInfo
            {
                llScore = msg.ReadInt64(),
                dwWinCount = msg.ReadUint32(),
                dwLostCount = msg.ReadUint32(),
                dwDrawCount = msg.ReadUint32(),
                dwFleeCount = msg.ReadUint32(),
                nGameData1 = msg.ReadInt32(),
                nGameData2 = msg.ReadInt32(),
                dwPKKempCount = msg.ReadUint32(),
                dwPKAwardCount = msg.ReadUint32(),
                dwPKCount = msg.ReadUint32(),
                dwPKWinCount = msg.ReadUint32(),
                dwPKEliminate = msg.ReadUint32(),
                wDeskID = msg.ReadUint16(),
                wChairID = msg.ReadUint16(),
                tmLastSave = MsgSystemTime.From(msg)
            };

            info.cbTaskData = new byte[Constants.GAME_EXTEND_DATA_LEN];
            for (int i = 0; i < Constants.GAME_EXTEND_DATA_LEN; i++)
            {
                info.cbTaskData[i] = (byte)msg.ReadByte();
            }
            info.cbMatchData = new byte[Constants.GAME_EXTEND_DATA_LEN];
            for (int i = 0; i < Constants.GAME_EXTEND_DATA_LEN; i++)
            {
                info.cbMatchData[i] = (byte)msg.ReadByte();
            }
            info.byReady = (byte)msg.ReadByte();
            info.byLookOn = (byte)msg.ReadByte();
            info.wReserved = msg.ReadUint16();
            info.dwCustom = msg.ReadUint32();

            return info;
        }
    }
}
