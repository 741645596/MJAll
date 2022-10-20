// @Author: tanjinhua
// @Date: 2021/5/28  15:38


using Common;
using Unity.Widget;

namespace MJCommon
{
    public class MJFriendGameOverHandler : FriendGameOverHandler
    {

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();
        }

        protected override FriendRecordStatistics OnCreateRecordStatistics()
        {
            var view = new MJFriendRecordStatistics();

            view.AddTo(WDirector.GetRootLayer(), MJZorder.RecordStatistices);

            return view;
        }
    }
}
