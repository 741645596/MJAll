// @Author: tanjinhua
// @Date: 2021/10/22  9:36


using Common;
using Unity.Widget;

namespace MJCommon
{
    public class MJFriendReadyController : FriendReadyController
    {
        protected override FriendReadyLayer OnCreateFriendReadyLayer()
        {
            var layer = new FriendReadyLayer();

            layer.AddTo(WDirector.GetRootLayer(), MJZorder.FriendReady);

            return layer;
        }
    }
}
