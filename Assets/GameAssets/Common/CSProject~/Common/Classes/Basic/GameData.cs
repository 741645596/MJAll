// @Author: tanjinhua
// @Date: 2021/4/12  9:33


using WLHall.Game;
using System.Collections.Generic;

namespace Common
{
    public class GameData : BaseGameData
    {

        #region Configs
        /// <summary>
        /// 人物方言音效路径列表，优先级按索引顺序由高到低，即第0个最优先
        /// </summary>
        public virtual List<string> dialectVoiceDirectories => null;
        #endregion


        #region Dynamic datas
        /// <summary>
        /// 出牌、动作事件等用于验证的消息ID
        /// </summary>
        public ushort verifyingMsgId = 0;

        /// <summary>
        /// 点击退出时是否要显示提示弹窗
        /// </summary>
        public bool showDialogOnExit = false;

        /// <summary>
        /// 收到玩家信息更新通知时是否马上自动更细UI
        /// </summary>
        public bool autoUpdatePlayerInfo = true;

        /// <summary>
        /// 禁止进入托管状态
        /// </summary>
        public bool trustForbidden = false;

        /// <summary>
        /// 玩家结算分数标志
        /// </summary>
        public List<int> playerSettleScoreFlags = null;

        /// <summary>
        /// 朋友场游戏信息
        /// </summary>
        public FriendGameInfo friendGameInfo = null;

        /// <summary>
        /// 申请解散玩家座位号
        /// </summary>
        public int applyDissolutionChairId = 0xFFFF;

        /// <summary>
        /// 超时解散玩家座位号
        /// </summary>
        public int overtimeDissolutionChairId = 0xFFFF;

        /// <summary>
        /// 是否时断线重连结算
        /// </summary>
        public bool isReconnectGameEnd = false;
        #endregion



        /// <summary>
        /// 初始化动态数据。注：此方法会在每局游戏开始时调用，朋友场需要每局累加的数据不要在这里初始化
        /// </summary>
        public virtual void Initialize()
        {
            verifyingMsgId = 0;
            autoUpdatePlayerInfo = true;
            trustForbidden = false;
            playerSettleScoreFlags = null;
            applyDissolutionChairId = 0xFFFF;
            overtimeDissolutionChairId = 0xFFFF;
            isReconnectGameEnd = false;
        }
    }
}
