// @Author: lili
// @Date: 2021/5/19 19:11:48

using WLHall.Game;

namespace Common
{
    public class IdentityController : BaseGameController
    {

        private GameData _gameData;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as GameData;
        }

        /// <summary>
        /// 显示个人信息界面
        /// </summary>
        /// <param name="userId"></param>
        public void ShowIdentity(uint userId)
        {
            IdentityConfig config = GetConfig(userId, out bool isSelf);

            //IdentityLayer identity = OnCreateIdentityLayer(config);

            //if (!client.isFriendRoom && !isSelf)
            //{
            //    identity.ShowReportButton();
            //}
            //else if (client.selfIsRoomOwner && client.friendGameStartHandler.inReadyState && !isSelf)
            //{
            //    identity.ShowKickButton();
            //}
        }

        /// <summary>
        /// 获取个人信息参数配置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected IdentityConfig GetConfig(uint userId, out bool isSelf)
        {
            var player = _gameData.GetPlayerByUserId(userId);
            isSelf = player == _gameData.playerSelf;
            IdentityConfig config = new IdentityConfig
            {
                gender = player.gender,
                userId = player.userId,
                nickname = player.GetNickNameUtf16(),
                money = player.GetMoney(),
                winCount = player.GetWinCount(),
                gameCount = player.GetGameCount(),
                avatarUrl = player.GetAvatarPath()
            };
            if (config.winCount != 0 && config.gameCount != 0)
            {
                config.winRate = (float)(10000 * player.GetWinCount() / player.GetGameCount()) / 100;
            }
            return config;
        }

        /// <summary>
        /// 创建个人信息界面
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected IdentityLayer OnCreateIdentityLayer(IdentityConfig config)
        {
            IdentityLayer identity = new IdentityLayer(config);

            identity.onClickKick = OnKickPlayer;

            identity.onClickReport = OnReportPlayer;

            return identity;
        }

        /// <summary>
        /// 点击踢人按钮
        /// </summary>
        /// <param name="userId"></param>
        protected void OnKickPlayer(uint userId)
        {
            stage.Send("SendKickPlayerRequest", _gameData.GetPlayerByUserId(userId).chairId);
        }

        /// <summary>
        /// 点击举报按钮
        /// </summary>
        /// <param name="userId"></param>
        protected void OnReportPlayer(uint userId)
        {
            // TODO: 完善个人信息举报逻辑
        }
    }
}
