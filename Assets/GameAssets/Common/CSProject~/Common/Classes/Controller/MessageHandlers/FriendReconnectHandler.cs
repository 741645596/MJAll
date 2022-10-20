// @Author: tanjinhua
// @Date: 2021/5/26  15:43


using System.Collections.Generic;
using WLHall.Game;

namespace Common
{
    public class FriendReconnectHandler : BaseGameController
    {

        private GameData _gameData;
        private GameAvatarController _avatarController;
        private FriendDissolutionHandler _dissolutionHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as GameData;
            _avatarController = stage.GetController<GameAvatarController>();
            _dissolutionHandler = stage.GetController<FriendDissolutionHandler>();
        }

        /// <summary>
        /// 更新玩家分数，刷新头像显示
        /// </summary>
        /// <param name="scores"></param>
        public virtual void RefreshPlayerScore(List<long> scores)
        {
            // 更新玩家数据
            _gameData.TraversePlayer(p =>
            {
                GamePlayer player = p as GamePlayer;

                player.friendGameScore = scores[player.chairId];

                _avatarController.UpdateScore(stage.ToViewChairId(player.chairId));
            });
        }

        /// <summary>
        /// 刷新解散对话框
        /// </summary>
        /// <param name="applyChairId"></param>
        /// <param name="votes"></param>
        public virtual void RefreshDissolutionDialog(int applyChairId, List<int> votes)
        {
            _dissolutionHandler.RemoveDialog();

            if (applyChairId != 0xFF && votes.Contains(1))
            {
                _dissolutionHandler.RecvDissolutionReconnect(applyChairId, votes);
            }
        }
    }
}
