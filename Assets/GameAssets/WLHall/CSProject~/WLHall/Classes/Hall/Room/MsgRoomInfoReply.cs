
using WLCore;

namespace WLHall
{
    public class MsgRoomInfoReply
    {
        public uint dwPropCount;
        public uint dwSysMsgCount;
        public ushort wPlayerCount;
        public uint[] vSupportProp;
        public RoomMsg[] vRoomMsgs;
        public BasePlayer player;
        public string strRoomKey;
        public uint dwOwnerID;
        public byte bGaming;
        public byte gpsenable;
        public byte isCreateByHall;

        public MsgRoomInfoReply(MsgHeader msg)
        {
            msg.ReadHeader();

            dwPropCount = msg.ReadUint32();
            dwSysMsgCount = msg.ReadUint32();
            wPlayerCount = msg.ReadUint16();
            vSupportProp = new uint[dwPropCount];
            for (int i = 0; i < dwPropCount; i++)
            {
                vSupportProp[i] = msg.ReadUint32();
            }

            vRoomMsgs = new RoomMsg[dwSysMsgCount];
            for (int i = 0; i < dwSysMsgCount; i++)
            {
                RoomMsg roomMsg = new RoomMsg();
                roomMsg.From(msg);
                vRoomMsgs[i] = roomMsg;
            }

            player = new BasePlayer
            {
                userInfo = UserInfo.From(msg),
                userGameInfo = UserGameInfo.From(msg)
            };
        }
    }
}
