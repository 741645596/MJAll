// @Author: tanjinhua
// @Date: 2021/4/21  13:49


using System.Collections.Generic;
using WLHall.Game;

namespace Common
{
    /// <summary>
    /// 游戏菜单控制器
    /// </summary>
    public abstract class GameMenuController : BaseGameController
    {
        private GameData _gameData;
        private GameMenu _menu;

        private FriendGameOverHandler _friendGameOverHandler;
        private FriendGameStartHandler _friendGameStartHandler;
        private FriendDissolutionHandler _friendDissolutionHandler;
        private GameTrustController _trustController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as GameData;

            _friendGameOverHandler = stage.GetController<FriendGameOverHandler>();
            _friendGameStartHandler = stage.GetController<FriendGameStartHandler>();
            _friendDissolutionHandler = stage.GetController<FriendDissolutionHandler>();
            _trustController = stage.GetController<GameTrustController>();

            _menu = OnCreateGameMenu();

            List<MenuButton.Config> configs = GetConfigs();

            _menu.Initialize(configs);

            UpdateDismissButtonState();
        }

        /// <summary>
        /// 获取菜单节点
        /// </summary>
        /// <returns></returns>
        public GameMenu GetMenu()
        {
            return _menu;
        }

        /// <summary>
        /// 创建菜单界面，子类实现添加到场景并设置zorder
        /// </summary>
        /// <returns></returns>
        protected abstract GameMenu OnCreateGameMenu();

        /// <summary>
        /// 获取菜单配置
        /// </summary>
        /// <returns></returns>
        protected virtual List<MenuButton.Config> GetConfigs()
        {
            List<MenuButton.Config> configs = new List<MenuButton.Config>();

            var asset = "Common/Game/game_ui_atlas";
            configs.Add(new MenuButton.Config
            {
                assetName = asset,
                imagePath = "game_menu/btn_setting.png",
                name = "setting",
                onClick = OnClickMenuButton
            });

            if (!_gameData.isFriendRoom)
            {
                configs.Add(new MenuButton.Config
                {
                    assetName = asset,
                    imagePath = "game_menu/btn_auto.png",
                    name = "trust",
                    onClick = OnClickMenuButton
                });

                configs.Add(new MenuButton.Config
                {
                    assetName = asset,
                    imagePath = "game_menu/btn_rule.png",
                    name = "rule",
                    onClick = OnClickMenuButton
                });
            }

            configs.Add(new MenuButton.Config
            {
                assetName = asset,
                imagePath = "game_menu/btn_exit.png",
                name = "exit",
                onClick = OnClickMenuButton
            });

            if (_gameData.isFriendRoom)
            {
                configs.Add(new MenuButton.Config
                {
                    assetName = asset,
                    imagePath = "game_menu/btn_dismiss.png",
                    name = "dismiss",
                    onClick = OnClickMenuButton
                });
            }

            return configs;
        }

        /// <summary>
        /// 更新解散按钮状态
        /// </summary>
        public void UpdateDismissButtonState()
        {
            bool dismissVisible = NeedsShowDismissButton();

            _menu.SetMenuButtonVisible("dismiss", dismissVisible);
        }

        /// <summary>
        /// 是否需要显示解散按钮
        /// </summary>
        /// <returns></returns>
        protected virtual bool NeedsShowDismissButton()
        {
            if (!_gameData.isFriendRoom)
            {
                return false;
            }

            var isFriendGameOver = _friendGameOverHandler.IsFriendGameOver();
            if (isFriendGameOver)
            {
                return false;
            }

            var inReadyState = _friendGameStartHandler.IsInReadyState();
            if (inReadyState)
            {
                return _friendGameStartHandler.NeedsShowDismissButton();
            }

            return true;
        }

        /// <summary>
        /// 菜单按钮点击事件
        /// </summary>
        /// <param name="name"></param>
        protected virtual void OnClickMenuButton(string name)
        {
            switch (name)
            {
                case "trust":
                    OnClickTrustButton();
                    break;

                case "exit":
                    stage.ExitWithDialog();
                    break;

                case "dismiss":
                    _friendDissolutionHandler.ApplyDissolution();
                    break;
            }
        }

        /// <summary>
        /// 点击托管
        /// </summary>
        protected virtual void OnClickTrustButton()
        {
            if (_gameData.trustForbidden)
            {
                return;
            }

            if (_gameData.isFriendRoom)
            {
                return;
            }

            GamePlayer player = _gameData.playerSelf as GamePlayer;

            _trustController.EnterTrust(true, player);
        }


        #region Game Events
        public override void OnGameStart()
        {
            base.OnGameStart();

            UpdateDismissButtonState();
        }

        public override void OnFriendGameStart()
        {
            base.OnFriendGameStart();

            // 更新菜单解散按钮状态
            UpdateDismissButtonState();

            _menu.state = GameMenu.State.Hide;
        }
        #endregion
    }
}
