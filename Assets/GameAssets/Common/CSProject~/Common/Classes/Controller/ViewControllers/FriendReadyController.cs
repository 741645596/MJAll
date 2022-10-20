// @Author: lili
// @Date: 2021/5/20 20:17:22

using WLHall.Game;

namespace Common
{
    public abstract class FriendReadyController : BaseGameController
    {

        private FriendReadyLayer _readyLayer;

        private FriendDissolutionHandler _dissolutionHandler;

        public override void OnSceneLoaded()
        {
            _dissolutionHandler = stage.GetController<FriendDissolutionHandler>();
        }

        /// <summary>
        /// 获取界面实例，如果不存在则创建新的
        /// </summary>
        /// <returns></returns>
        public FriendReadyLayer GetFriendReadyLayer()
        {
            if (_readyLayer == null || _readyLayer.gameObject == null)
            {
                _readyLayer = OnCreateFriendReadyLayer();
                _readyLayer.onClickDissolution = OnClickDissolution;
                _readyLayer.onClickExit = OnClickExit;
                _readyLayer.onClickReady = OnClickReady;
                _readyLayer.onClickInvite = OnClickInvite;
            }

            return _readyLayer;
        }

        /// <summary>
        /// 创建朋友场准备界面，子类实现添加到场景并设置zorder
        /// </summary>
        /// <returns></returns>
        protected abstract FriendReadyLayer OnCreateFriendReadyLayer();

        /// <summary>
        /// 显示中间的房间信息（房号，局数，规则等）
        /// </summary>
        /// <param name="config"></param>
        public void ShowRoomInfo(FriendReadyLayer.Config config)
        {
            GetFriendReadyLayer().SetConfig(config);
        }

        /// <summary>
        /// 显示准备按钮
        /// </summary>
        /// <param name="isReady"></param>
        public void ShowReadyButton()
        {
            GetFriendReadyLayer().ShowReadyButton();
        }

        /// <summary>
        /// 显示自己准备状态
        /// </summary>
        public void ShowReady()
        {
            GetFriendReadyLayer().ShowReady();
        }

        /// <summary>
        /// 显示邀请按钮
        /// </summary>
        public void ShowInviteButton()
        {
            GetFriendReadyLayer().ShowInviteButton();
        }

        /// <summary>
        /// 显示解散按钮
        /// </summary>
        public void ShowDismissButton()
        {
            GetFriendReadyLayer().ShowDismissButton();
        }

        /// <summary>
        /// 显示退出按钮
        /// </summary>
        public void ShowExitButton()
        {
            GetFriendReadyLayer().ShowExitButton();
        }

        /// <summary>
        /// 删除朋友场准备界面
        /// </summary>
        public void Clear()
        {
            if (_readyLayer != null && _readyLayer.gameObject != null)
            {
                _readyLayer.RemoveFromParent();
            }

            _readyLayer = null;
        }

        /// <summary>
        /// 点击解散按钮
        /// </summary>
        protected virtual void OnClickDissolution()
        {
            _dissolutionHandler.ApplyDissolution();
        }

        /// <summary>
        /// 点击退出按钮
        /// </summary>
        protected virtual void OnClickExit()
        {
            stage.ExitWithDialog();
        }

        /// <summary>
        /// 点击准备按钮
        /// </summary>
        protected virtual void OnClickReady()
        {
            stage.Send("SendReadyRequest");
        }

        /// <summary>
        /// 点击邀请按钮
        /// </summary>
        protected virtual void OnClickInvite()
        {

        }

        #region Game Event
        public override void OnGameStart()
        {
            base.OnGameStart();

            Clear();
        }
        #endregion
    }
}
