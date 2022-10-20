// @Author: tanjinhua
// @Date: 2021/4/12  10:58


using WLHall;
using WLHall.Game;

namespace Common
{
    public abstract class GameStage : BaseGameStage
    {
        private GameAvatarController _avatarController;
        private FriendGameStartHandler _friendGameStartHandler;


        public GameStage(BaseRoomManager roomManager) : base(roomManager)
        {
        }


        public override void OnInitialize()
        {
            base.OnInitialize();

            LoadScene();

            // view controllers
            AddController<IdentityController>();
            AddController<GameSettingController>();

            // message handlers
            AddController<FriendGameStartHandler>();
            AddController<FriendDissolutionHandler>();
            AddController<FriendReconnectHandler>();
        }


        protected override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            _avatarController = GetController<GameAvatarController>();
            _friendGameStartHandler = GetController<FriendGameStartHandler>();
        }


        /// <summary>
        /// 换桌操作
        /// </summary>
        public override void ChangeDesk()
        {
            if (gameData.isFriendRoom)
            {
                return;
            }

            int flag = GetContinueFlag();

            if (flag != 0)
            {
                // TODO：显示弹窗提示

                WLDebug.LogWarning($"GameStage.ChangeDesk: 换桌失败，金币不足或超过上限。flag = {flag}");

                return;
            }

            base.ChangeDesk();
        }


        /// <summary>
        /// 继续操作
        /// </summary>
        public override void Continue()
        {
            if (!gameData.isFriendRoom && gameData.anyPlayerLeft)
            {
                ChangeDesk();

                return;
            }


            int flag = GetContinueFlag();

            if (flag != 0)
            {
                // TODO：显示弹窗提示

                WLDebug.LogWarning($"GameStage.Continue: 继续失败，金币不足或超过上限。flag = {flag}");

                return;
            }

            Send("SendReadyRequest");

            base.Continue();
        }


        /// <summary>
        /// 退出游戏，返回大厅场景。 支持自定义弹窗提示
        /// </summary>
        /// <param name="dialogText"></param>
        /// <param name="dialogTips"></param>
        public override void ExitWithDialog(string dialogText = null, string dialogTips = null)
        {
            Exit();

            // TODO: 待弹窗功能完善

            // 默认退出提示逻辑
            //if (string.IsNullOrEmpty(dialogText))
            //{
            //    if (!gameData.showDialogOnExit)
            //    {
            //        Exit();

            //        return;
            //    }

            //    dialogText = "确定要离开游戏吗？";

            //    if (!isFriendRoom)
            //    {
            //        dialogTips = "（如果退出系统将为您代打到本局结束!）";
            //    }

            //    if (selfIsRoomOwner && friendGameStartHandler.inReadyState)
            //    {
            //        bool isReviewMode = false; // TODO: 完善审核模式条件获取

            //        dialogText = "离开后房间继续为您保留";

            //        dialogTips = isReviewMode ? "" : "（快去微信邀请好友加入）";
            //    }
            //}

            //// 显示退出提示弹窗
            //GameDialog.CreateWithOKCancel(dialogText, index =>
            //{
            //    if (index == GameDialog.ClickType.OK)
            //    {
            //        Exit();
            //    }
            //}, dialogTips);
        }


        #region Notices
        /// <summary>
        /// 玩家加入通知
        /// </summary>
        /// <param name="player"></param>
        public override void OnPlayerJoin(BaseGamePlayer player, bool isSelf)
        {
            int viewChairId = ToViewChairId(player.chairId);

            _avatarController.Append(viewChairId, player.gender, player.GetAvatarPath(), player.userId);

            _avatarController.UpdateScore(viewChairId);

            var inReadyState = _friendGameStartHandler.IsInReadyState();
            if (player.IsReady() && (!gameData.isFriendRoom || !isSelf || !inReadyState))
            {
                var avatar = _avatarController.GetAvatar(viewChairId);
                avatar.ShowReady();
            }
        }

        /// <summary>
        /// 玩家离开通知
        /// </summary>
        /// <param name="player"></param>
        public override void OnPlayerLeave(BaseGamePlayer player)
        {
            _avatarController.Remove(player.chairId);
        }

        /// <summary>
        /// 使用道具成功通知
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="propId"></param>
        /// <param name="usedCount"></param>
        public override void OnUsePropSuccessed(uint fromUserId, uint toUserId, uint propId, int usedCount)
        {
            WLDebug.Info("GameStage.OnUsePropSuccessed");
        }

        /// <summary>
        /// 使用道具失败通知
        /// </summary>
        /// <param name="errMsg"></param>
        public override void OnUsePropFailed(string errMsg)
        {
            WLDebug.Info("GameStage.OnUsePropFailed");
        }

        /// <summary>
        /// 玩家豆豆等信息发生改变通知
        /// </summary>
        /// <param name="player"></param>
        public override void OnUpdatePlayerInfo(BaseGamePlayer player)
        {
            if (!(gameData as GameData).autoUpdatePlayerInfo)
            {
                return;
            }

            _avatarController.UpdateScore(ToViewChairId(player.chairId));
        }
        #endregion


        /// <summary>
        /// 获取能否继续(换桌)的标志
        /// </summary>
        /// <returns>0 表示可以，1 表示金币超过上线，-1 表示金币低于准入数</returns>
        protected virtual int GetContinueFlag()
        {
            if (gameData.isFriendRoom)
            {
                return 0;
            }

            if (gameData.playerSelf.GetMoney() < gameData.roomInfo.minMoney)
            {
                return -1;
            }

            if (gameData.playerSelf.GetMoney() > gameData.roomInfo.maxMoney)
            {
                return 1;
            }

            return 0;
        }
    }
}
