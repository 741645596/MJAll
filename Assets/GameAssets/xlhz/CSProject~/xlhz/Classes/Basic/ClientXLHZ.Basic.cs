// @Author: tanjinhua
// @Date: 2021/8/18  18:17


using Common;
using MJCommon;
using WLHall;

namespace NS_XLHZ
{
    public partial class ClientXLHZ : MJClient
    {
        /// <summary>
        /// 胡牌消息业务
        /// </summary>
        public XLHZWinHandler winHandler;


        public ClientXLHZ(BaseRoomManager roomManager) : base(roomManager)
        {
            // 重写基类模块
            handController = CreateController<XLHZHandController>();
            outCardHandler = CreateController<XLHZOutCardHandler>();
            dingqueHandler = CreateController<XLHZDingqueHandler>();
            sendCardHandler = CreateController<XLHZSendCardHandler>();

            // 自定义模块
            winHandler = CreateController<XLHZWinHandler>();
        }

        protected override void OnSceneLoaded() => base.OnSceneLoaded();


        protected override GameData OnCreateGameData() => new XLHZGameData();


        public override BasePlayer OnCreatePlayer() => new XLHZGamePlayer();
    }
}
