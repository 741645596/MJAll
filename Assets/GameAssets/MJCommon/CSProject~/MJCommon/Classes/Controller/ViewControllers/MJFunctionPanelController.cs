// @Author: lili
// @Date: 2021/5/31 20:43:43

using Common;
using Unity.Widget;
using WLHall;

namespace MJCommon
{
    /// <summary>
    /// 游戏内各种入口控制器
    /// </summary>
    public class MJFunctionPanelController : FunctionPanelController
    {
        private FriendRoomInfoBubble _friendInfoBubble;
        private MJGameData _gameData;

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            _gameData = stage.gameData as MJGameData;
        }

        /// <summary>
        /// 显示朋友场信息气泡
        /// </summary>
        public override void ShowFriendInfoBubble()
        {
            //if (_friendInfoBubble == null || _friendInfoBubble.gameObject == null)
            //{
            //    _friendInfoBubble = OnCreateFriendInfoBubble();
            //}

            //CustomRoomManager roomManager = client.roomManager as CustomRoomManager;

            //_friendInfoBubble.UpdateRoomKey("房号" + roomManager.roomKey);
        }

        protected virtual FriendRoomInfoBubble OnCreateFriendInfoBubble()
        {
            _friendInfoBubble = new FriendRoomInfoBubble();
            //_friendInfoBubble.zorder = GameZorder.GameEntry;
            //_friendInfoBubble.rectTransform.pivot = Layouts.leftCenter;
            //_friendInfoBubble.LayoutWithParent(Layouts.leftTop, new Vector2(112, -37), true);
            // TODO
            _friendInfoBubble.onClick = OnClickFriendInfoBubble;

            return _friendInfoBubble;
        }

        protected virtual void OnClickFriendInfoBubble()
        {
            FriendGameInfo info = _gameData.friendGameInfo;
            FriendInfoPopNode popNode = new FriendInfoPopNode(info, _gameData.roomKey);
            //popNode.zorder = GameZorder.GameEntry;
            //popNode.rectTransform.pivot = Layouts.leftTop;
            //popNode.LayoutWithParent(Layouts.leftTop, new Vector2(_friendInfoBubble.position.x + _friendInfoBubble.size.x, 0));
            // TODO
        }

        protected override FunctionPanel OnCreateFunctionPanel()
        {
            var panel = new FunctionPanel();

            panel.AddTo(WDirector.GetRootLayer(), MJZorder.FunctionPanel);

            return panel;
        }
    }
}
