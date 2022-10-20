
using WLCore;

namespace WLHall
{
    public struct MsgRoomAllocSit
    {
        public UserInfo userInfo;
        public UserGameInfo userGameInfo;

        public static MsgRoomAllocSit From(MsgHeader msg)
        {
            return new MsgRoomAllocSit
            {
                userInfo = UserInfo.From(msg),
                userGameInfo = UserGameInfo.From(msg)
            };
        }
    }
}
