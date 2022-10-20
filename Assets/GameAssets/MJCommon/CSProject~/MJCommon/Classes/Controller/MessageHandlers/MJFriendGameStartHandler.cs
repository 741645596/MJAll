// @Author: tanjinhua
// @Date: 2021/5/25  16:44


using Common;

namespace MJCommon
{
    public class MJFriendGameStartHandler : FriendGameStartHandler
    {
        private MJGameData _gameData;

        private MJTableController _tableController;

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            _gameData = stage.gameData as MJGameData;

            _tableController = stage.GetController<MJTableController>();
        }

        public override void UpdateFriendGameInfo(FriendGameInfo friendGameInfo)
        {
            base.UpdateFriendGameInfo(friendGameInfo);

            string gameCountText = $"{friendGameInfo.currentGameCount}/{friendGameInfo.totalGameCount}局";

            if (friendGameInfo.useCustomGameCount)
            {
                gameCountText = friendGameInfo.customGameCountTitle + friendGameInfo.customGameCount;
            }

            // 显示游戏局数
            _tableController.SetFriendGameCount(gameCountText);

            _tableController.SetRemainCardCount(_gameData.remainCardCount);

            // 显示朋友场游戏名称
            _tableController.SetFriendGameName(friendGameInfo.gameName);
        }
    }
}
