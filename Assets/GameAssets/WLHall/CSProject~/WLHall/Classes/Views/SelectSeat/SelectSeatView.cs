// @Author: tanjinhua
// @Date: 2021/9/15  20:03


using System.Collections.Generic;
using Unity.Widget;

namespace WLHall
{
    public class SelectSeatView : WNode
    {
        public struct PlayerInfo
        {
            public ushort chairId;
            public uint userId;
            public int gender;
            public string nickname;
            public uint avatarId;
            public string avatarUrl;
        }


        public static void Display(List<PlayerInfo> playerInfos, int playerCount, string roomKey, bool isPuker)
        {
            var view = new SelectSeatView(playerInfos, playerCount, roomKey, isPuker);
            //Director.GetRootLayer().AddChild(view, 10, TagDefine.SELECT_SEAT_VIEW);
            view.AddTo(WDirector.GetRootLayer());
        }

        public SelectSeatView(List<PlayerInfo> playerInfos, int playerCount, string roomKey, bool isPuker)
        {
        }

        public void UpdateSeats(List<PlayerInfo> playerInfos)
        {

        }
    }
}
