// @Author: tanjinhua
// @Date: 2021/8/12  18:15

using WLCore;

namespace WLHall
{
    public interface ISocketEvent
    {
        void OnSocketOpen();
        void OnSocketClose(int error);
        void OnSocketMessage(MsgHeader msg);
    }
}
