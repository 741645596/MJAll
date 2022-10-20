// @Author: tanjinhua
// @Date: 2021/5/7  15:32


using WLHall.Game;

namespace Common
{
    /// <summary>
    /// 游戏托管控制器
    /// </summary>
    public abstract class GameTrustController : BaseGameController
    {

        protected TrustPanel _panel;

        private GameAvatarController _avatarController;

        public override void OnSceneLoaded()
        {
            _avatarController = stage.GetController<GameAvatarController>();
        }

        /// <summary>
        /// 进入托管状态，显示托管UI
        /// </summary>
        public virtual void EnterTrust(bool sendMessage, GamePlayer player)
        {
            if (player == null)
            {
                return;
            }

            player.isTrust = true;

            int viewChairId = stage.ToViewChairId(player.chairId);

            var avatar = _avatarController.GetAvatar(viewChairId);
            avatar?.ShowTrust();

            if (player == stage.gameData.playerSelf)
            {
                ShowTrustPanel();

                if (sendMessage)
                {
                    stage.Send("SendTrustRequest", true);
                }
            }
        }


        /// <summary>
        /// 退出托管状态，清除托管UI
        /// </summary>
        public virtual void ExitTrust(bool sendMessage, GamePlayer player)
        {
            if (player == null)
            {
                return;
            }

            player.isTrust = false;

            int viewChairId = stage.ToViewChairId(player.chairId);

            var avatar = _avatarController.GetAvatar(viewChairId);
            avatar?.HideTrust();

            if (player == stage.gameData.playerSelf)
            {
                RemoveTrustPanel();

                if (sendMessage)
                {
                    stage.Send("SendTrustRequest", false);
                }
            }
        }


        /// <summary>
        /// 所有玩家退出托管状态，清除托管UI
        /// </summary>
        /// <param name="sendMessage"></param>
        public void ExitTrust(bool sendMessage)
        {
            stage.gameData.TraversePlayer<GamePlayer>(p => ExitTrust(sendMessage, p));
        }


        protected virtual void ShowTrustPanel()
        {
            if (_panel == null || _panel.gameObject == null)
            {
                _panel = OnCreateTrustPanel();

                _panel.onClickCancel = OnClickCancel;
            }
        }

        /// <summary>
        /// 创建托管界面，子类实现添加到场景并设置zorder
        /// </summary>
        /// <returns></returns>
        protected abstract TrustPanel OnCreateTrustPanel();


        private void RemoveTrustPanel()
        {
            if (_panel != null && _panel.gameObject != null)
            {
                _panel.RemoveFromParent();
            }

            _panel = null;
        }


        protected virtual void OnClickCancel()
        {
            GamePlayer player = stage.gameData.playerSelf as GamePlayer;

            ExitTrust(true, player);
        }


        #region Game Events
        public override void OnChangeDesk()
        {
            base.OnChangeDesk();

            GamePlayer player = stage.gameData.playerSelf as GamePlayer;

            ExitTrust(false, player);
        }


        public override void OnContinue()
        {
            base.OnContinue();

            GamePlayer player = stage.gameData.playerSelf as GamePlayer;

            ExitTrust(false, player);
        }

        public override void OnGameOver()
        {
            base.OnGameOver();

            ExitTrust(false);
        }
        #endregion
    }
}
