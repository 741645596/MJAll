// @Author: tanjinhua
// @Date: 2021/4/12  10:49


using Common;
using WLHall;
using WLHall.Game;

namespace MJCommon
{
    public partial class MJStage : GameStage
    {
        public MJStage(BaseRoomManager roomManager) : base(roomManager)
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            // view controllers
            AddController<MJFriendReadyController>();
            AddController<MJFunctionPanelController>();
            AddController<MJMenuController>();
            AddController<MJSpaceController>();
            AddController<MJEnvController>();
            AddController<MJTrustController>();
            AddController<MJTableController>();
            AddController<MJInputController>();
            AddController<MJHandController>();
            AddController<MJMeldController>();
            AddController<MJDeskCardController>();
            AddController<MJWallController>();
            AddController<MJWinCardController>();
            AddController<MJActionButtonController>();
            AddController<MJActionEffectController>();
            AddController<MJHintsController>();
            AddController<MJCardTintController>();
            AddController<MJAvatarController>();
            AddController<MJAudioController>();
            AddController<MJSettleController>();

            // message handlers
            AddController<MJGameStartHandler>();
            AddController<MJReconnectHandler>();
            AddController<MJOutCardHandler>();
            AddController<MJSendCardHandler>();
            AddController<MJActionResultHandler>();
            AddController<MJHuInfoHandler>();
            AddController<MJTingInfoHandler>();
            AddController<MJGameOverHandler>();
            AddController<MJExchangeHandler>();
            AddController<MJDingqueHandler>();
            AddController<MJFriendGameStartHandler>();
            AddController<MJFriendGameOverHandler>();
        }

        public override void OnShutdown()
        {
            base.OnShutdown();


            MJCardPool.Clear();
        }

        /// <summary>
        /// 返回BuildSetting中场景名称
        /// </summary>
        /// <returns></returns>
        protected override string GetBuildSceneName() => "MJScene";


        /// <summary>
        /// 创建游戏数据
        /// </summary>
        /// <returns></returns>
        protected override BaseGameData OnCreateGameData() => new MJGameData();


        /// <summary>
        /// 创建消息模块
        /// </summary>
        /// <returns></returns>
        protected override BaseGameMessage OnCreateGameMessage() => new MJMessage();


        #region Utils
        /// <summary>
        /// 服务器座位号转换为以relativeChairId视角为准的视图座位号
        /// </summary>
        /// <param name="serverChairId"></param>
        /// <param name="relativeChairId"></param>
        /// <returns></returns>
        public override int ToViewChairIdRelative(int serverChairId, int relativeChairId)
        {
            int basicViewChairId = base.ToViewChairIdRelative(serverChairId, relativeChairId);

            return MJChairConverter.ToView(gameData.maxPlayerCount, basicViewChairId);
        }


        /// <summary>
        /// 视图座位号转换为服务器座位号
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public override int ToServerChairId(int viewChairId)
        {
            int basicViewChairId = MJChairConverter.ToViewBasic(gameData.maxPlayerCount, viewChairId);

            return base.ToServerChairId(basicViewChairId);
        }


        /// <summary>
        /// 获取下家视图座位号
        /// </summary>
        /// <param name="currentViewChairId"></param>
        /// <returns></returns>
        public override int NextViewChairId(int currentViewChairId)
        {
            return MJChairConverter.ToNextView(gameData.maxPlayerCount, currentViewChairId);
        }
        #endregion

    }
}