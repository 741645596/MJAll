// @Author: tanjinhua
// @Date: 2021/8/18  18:20


using WLCore;
using WLHall;
using System.Collections.Generic;

namespace NS_XLHZ
{
    public partial class ClientXLHZ
    {
        protected override Dictionary<string, ushort> RegisterSendInterface()
        {
            var senders = base.RegisterSendInterface();

            // 消息ID冲突，重新绑定
            senders["SendReadyRequest"] = XLHZMessages.MSG_C2S_READY_REQUEST;
            senders["SendTrustRequest"] = XLHZMessages.MSG_C2S_AUTOPLAY_REQUESET;
            senders["SendChatText"] = XLHZMessages.MSG_C2S_GAME_CHAT;
            senders["SendNewClient"] = XLHZMessages.MSG_C2S_NewClient;
            senders["SendBiaoQing"] = XLHZMessages.MSG_C2S_BIAOQING;
            senders["SendDissolutionApply"] = XLHZMessages.MSG_C2S_FRIENDROOM_APPLY_DISSOLUTION;
            senders["SendFriendDissolutionVote"] = XLHZMessages.MSG_C2S_FRIENDROOM_DISSOLUTION;
            senders["SendGPS"] = XLHZMessages.MSG_C2S_CeJu;
            senders["SendKickPlayerRequest"] = XLHZMessages.MsgID_C2S_KickPlayer;
            senders["SendVoiceChat"] = XLHZMessages.MSG_C2S_Record;
            senders["SendFriendLiXianState"] = XLHZMessages.MsgID_C2S_FRIEND_LiXian;
            senders["SendDingque"] = XLHZMessages.MSG_C2S_DINGQUE_REQUST;
            senders["SendOutCard"] = XLHZMessages.MSG_C2S_OUTCARD_REQUST;
            senders["SendAction"] = XLHZMessages.MSG_C2S_ACTION_REQUST;
            senders["SendDissolutionApply"] = XLHZMessages.MSG_C2S_FRIENDROOM_APPLY_DISSOLUTION;
            senders["SendChangeCards"] = XLHZMessages.MSG_C2S_CHANGECARD_REQUEST;

            return senders;
        }


        public void SendTopupFail()
        {
            // TODO:
        }
    }
}
