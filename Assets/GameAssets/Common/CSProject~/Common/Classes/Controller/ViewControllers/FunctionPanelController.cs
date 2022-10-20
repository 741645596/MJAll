// @Author: lili
// @Date: 2021/5/31 20:16:20

using WLHall.Game;

namespace Common
{
    /// <summary>
    /// 朋友场气泡、短语聊天、各类活动入口按钮控制器
    /// </summary>
    public abstract class FunctionPanelController : BaseGameController
    {

        private GameData _gameData;
        private FunctionPanel _panel;

        private GameMenuController _menuController;
        private FriendGameStartHandler _friendGameStartHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as GameData;
            _menuController = stage.GetController<GameMenuController>();
            _friendGameStartHandler = stage.GetController<FriendGameStartHandler>();

            _panel = OnCreateFunctionPanel();
            _panel.onClickMenuBtn = OnClickMenuButton;
        }

        /// <summary>
        /// 获取功能按钮层
        /// </summary>
        /// <returns></returns>
        public FunctionPanel GetPanel()
        {
            return _panel;
        }

        /// <summary>
        /// 创建功能按钮层，子类实现添加到场景并设置zorder
        /// </summary>
        /// <returns></returns>
        protected abstract FunctionPanel OnCreateFunctionPanel();

        /// <summary>
        /// 显示朋友场信息气泡，子类实现创建不同样式的控件
        /// </summary>
        public abstract void ShowFriendInfoBubble();


        private void OnClickMenuButton()
        {
            var menu = _menuController.GetMenu();

            if (menu.state == GameMenu.State.Show)
            {
                menu.PlayHide();
            }
            else
            {
                menu.PlayShow();
            }
        }

        #region Game Events
        public override void OnGameStart()
        {
            base.OnGameStart();

            if (_gameData.isFriendRoom)
            {
                ShowFriendInfoBubble();
            }
        }


        public override void OnFriendGameReconnect()
        {
            base.OnFriendGameReconnect();
            var inReadyState = _friendGameStartHandler.IsInReadyState();
            if (!inReadyState)
            {
                ShowFriendInfoBubble();
            }
        }
        #endregion
    }
}
