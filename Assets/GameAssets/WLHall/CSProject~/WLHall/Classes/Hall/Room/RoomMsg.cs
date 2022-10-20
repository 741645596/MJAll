using WLCore;

namespace WLHall
{
    public struct RoomMsg
    {
        public uint dwID;
        public uint dwObjectType;
        public uint dwObjectID;
        public uint dwTimeInterval;
        public uint dwSendCount;
        public string szBuff;
        public long dwLastShow;

        public void From(MsgHeader msg)
        {
            dwID = msg.ReadUint32();
            dwObjectType = msg.ReadUint32();
            dwObjectID = msg.ReadUint32();
            dwTimeInterval = msg.ReadUint32();
            dwSendCount = msg.ReadUint32();
            szBuff = msg.ReadTCharString(Constants.ROOM_MSG_LEN);
            // todo xzh
            dwLastShow = 1;
            //dwLastShow = DateEx.GetCurDateSeconds();
        }
    }
}
