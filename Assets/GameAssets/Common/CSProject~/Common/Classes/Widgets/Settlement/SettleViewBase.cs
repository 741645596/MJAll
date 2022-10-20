// @Author: tanjinhua
// @Date: 2021/6/8  17:23


using System;
using Unity.Widget;

namespace Common
{
    public abstract class SettleViewBase : WLayer
    {
        public Action<SettleEvent> onEvent;


        public abstract void Countdown(int time);


        public abstract void ShowFriendGameOverState();
    }
}
