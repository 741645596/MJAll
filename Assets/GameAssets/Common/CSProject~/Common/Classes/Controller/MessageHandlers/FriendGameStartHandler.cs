// @Author: tanjinhua
// @Date: 2021/5/25  10:06


using WLHall.Game;

namespace Common
{
    public class FriendGameStartHandler : BaseGameController
    {

        private GameData _gameData;
        private FriendReadyController _friendReadyController;
        private GameAvatarController _avatarController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as GameData;
            _friendReadyController = stage.GetController<FriendReadyController>();
            _avatarController = stage.GetController<GameAvatarController>();
        }

        /// <summary>
        /// 是否在准备阶段
        /// </summary>
        /// <returns></returns>
        public virtual bool IsInReadyState()
        {
            return _gameData.friendGameInfo != null && _gameData.friendGameInfo.currentGameCount == 0;
        }

        /// <summary>
        /// 是否需要显示解散按钮
        /// </summary>
        /// <returns></returns>
        public virtual bool NeedsShowDismissButton()
        {
            if (!_gameData.isQinYouQuan)
            {
                return _gameData.selfIsRoomOwner;
            }


            // TODO: 判断是否是亲友圈管理员

            return false;
        }


        /// <summary>
        /// 更新朋友场游戏信息
        /// </summary>
        /// <param name="friendGameInfo"></param>
        public virtual void UpdateFriendGameInfo(FriendGameInfo friendGameInfo)
        {
            // 保存朋友场游戏信息
            _gameData.friendGameInfo = friendGameInfo;

            if (IsInReadyState())
            {
                FriendReadyLayer.Config config = GetFriendReadyConfig(friendGameInfo);

                _friendReadyController.ShowRoomInfo(config);

                var playerSelf = _gameData.playerSelf as GamePlayer;
                if (playerSelf.IsReady())
                {
                    _friendReadyController.ShowReady();
                    var avatar = _avatarController.GetAvatar(0);
                    avatar.HideReady();
                }
                else
                {
                    _friendReadyController.ShowReadyButton();
                }

                _friendReadyController.ShowInviteButton(); // TODO: 完善邀请好友逻辑

                if (NeedsShowDismissButton())
                {
                    _friendReadyController.ShowDismissButton();
                }
                else
                {
                    _friendReadyController.ShowExitButton();
                }
            }

            // TODO: 更新GPS警告灯状态

            // TODO: 显示GPS界面

            // TODO：实时语音加入聊天房间
        }

        /// <summary>
        /// 获取朋友场准备界面参数配置
        /// </summary>
        /// <param name="friendGameInfo"></param>
        /// <returns></returns>
        protected virtual FriendReadyLayer.Config GetFriendReadyConfig(FriendGameInfo friendGameInfo)
        {
            string gameCountTitle = friendGameInfo.useCustomGameCount ? friendGameInfo.customGameCountTitle : "局数:";

            string gameCountValue = friendGameInfo.useCustomGameCount ? friendGameInfo.customGameCount : $"{friendGameInfo.totalGameCount}";

            return new FriendReadyLayer.Config
            {
                title = friendGameInfo.gameName,
                roomKey = stage.gameData.roomKey,
                gameCountTitle = gameCountTitle,
                gameCountValue = gameCountValue,
                otherInfoTitle = friendGameInfo.otherInfoTitle,
                otherInfo = friendGameInfo.otherInfo,
                deskNum = 0, // TODO: RecvInviteRuleInfo
                rules = friendGameInfo.rules,
                isOpenGPS = friendGameInfo.isOpenGPS,
                isOvertimeDisslotion = friendGameInfo.isOpenOverTimeDissolution,
                isAccumulatingOfflineTime = friendGameInfo.accumulatingOfflineTime
            };
        }
    }
}
