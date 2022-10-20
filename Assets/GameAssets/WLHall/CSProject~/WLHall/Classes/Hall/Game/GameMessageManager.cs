// @Author: tanjinhua
// @Date: 2021/8/16  15:48

using WLCore;

namespace WLHall
{
    /// <summary>
    /// 游戏消息管理器
    /// </summary>
    public class GameMessageManager : MessageManager
    {
        private BaseRoomManager _roomManager;

        public GameMessageManager(BaseRoomManager roomManager)
        {
            _roomManager = roomManager;
        }

        public override void Dispose()
        {
            _roomManager = null;

            base.Dispose();
        }

        public override void SendData(MsgHeader msg, byte flag)
        {
            _roomManager?.SendData(msg, flag);
        }

        protected override bool NeedsWaiting(MsgHeader msg)
        {
            return false;
        }
    }
}
