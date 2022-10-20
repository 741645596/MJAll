// @Author: tanjinhua
// @Date: 2021/4/7  10:57



using System.Collections.Generic;
using WLCore;
using WLHall;

namespace Common
{
    public abstract partial class GameMessage
    {
        protected override Dictionary<string, ushort> RegisterSendInterface()
        {
            Dictionary<string, ushort> senders = new Dictionary<string, ushort>();

            senders["SendChatText"]                     = GameMsgDefine.MsgID_C2S_GameChat_Request;
            senders["SendBiaoQing"]                     = GameMsgDefine.MsgID_C2S_BiaoQing_Request;
            senders["SendDelayReadyTime"]               = GameMsgDefine.MsgID_C2S_Delay_Ready_Time;
            senders["SendDissolutionApply"]             = GameMsgDefine.MsgID_C2S_FRIEND_TEAM_DissolutionApply_Request;
            senders["SendFriendDissolutionVote"]        = GameMsgDefine.MsgID_C2S_FRIEND_TEAM_DissolutionVote_Request;
            senders["SendTrustRequest"]                 = GameMsgDefine.MsgID_C2S_AutoPlay_Request;
            senders["SendReadyRequest"]                 = GameMsgDefine.MsgID_C2S_Ready_Request;
            senders["SendGPS"]                          = GameMsgDefine.MsgID_C2S_GPS;
            senders["SendVoiceChat"]                    = GameMsgDefine.MsgID_C2S_Record;
            senders["SendKickPlayerRequest"]            = GameMsgDefine.MsgID_C2S_KickPlayer;
            senders["SendFriendLiXianState"]            = GameMsgDefine.MsgID_C2S_FRIEND_LiXian;
            senders["SendRankingMatchInfo"]             = GameMsgDefine.MsgID_C2S_Ranking_Info;
            senders["SendQifuConfigRequest"]            = GameMsgDefine.MsgID_C2S_FriendCfg_ZYQS;
            senders["SendQifu"]                         = GameMsgDefine.MsgID_C2S_Friend_ZYQS;
            senders["SendCoinWinTaskRewardRequest"]     = GameMsgDefine.MsgID_C2S_CoinMarket_Task_Receive;
            senders["SendCommonPriceListRequest"]       = GameMsgDefine.MsgID_C2S_CommonPriceList_Request;
            senders["SendUseMagicPropRequest"]          = GameMsgDefine.MsgID_C2S_UseMagicProp_Request;
            senders["SendUseShuffleRequest"]            = GameMsgDefine.MsgID_C2S_UseShuffle_Request;
            senders["SendNewClient"]                    = GameMsgDefine.MsgID_C2S_NewClient_Request;

            return senders;
        }



        #region Send Interfaces
        /// <summary>
        /// 发送短语聊天
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        public virtual void SendChatText(int index, string text = "")
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendChatText");
            msg.WriteByte((byte)index);
            long pos = msg.position;
            msg.WriteStringW(text);
            msg.position = pos + 1024 * 2;
            SendData(msg);
        }


        public virtual void SendBiaoQing(int index)
        {
            WLDebug.Info("SendBiaoQing");
            ushort id = GetSendMsgId("SendBiaoQing");
        }


        public virtual void SendDelayReadyTime(int delay)
        {
            WLDebug.Info("SendDelayReadyTime");
            ushort id = GetSendMsgId("SendDelayReadyTime");
        }


        public virtual void SendDissolutionApply()
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendDissolutionApply");
            SendData(msg);
        }


        public virtual void SendFriendDissolutionVote(byte code)
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendFriendDissolutionVote");
            msg.WriteByte(code);

            SendData(msg);
        }


        public virtual void SendTrustRequest(bool isTrust)
        {
            var gameData = stage.GetGameData<GameData>();

            // 如果标志禁止托管则屏蔽
            if (gameData.trustForbidden)
            {
                return;
            }

            (gameData.playerSelf as GamePlayer).isTrust = isTrust;

            int code = isTrust ? 1 : 0;

            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendTrustRequest");
            msg.WriteByte((byte)code);

            SendData(msg);
        }


        public virtual void SendReadyRequest()
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendReadyRequest");
            SendData(msg);
        }


        public virtual void SendGPS(double lantitude, double longitude)
        {
            WLDebug.Info("SendGPS");
            ushort id = GetSendMsgId("SendGPS");
        }


        public virtual void SendVoiceChat(string url, int recordTime)
        {
            WLDebug.Info("SendVoiceChat");
            ushort id = GetSendMsgId("SendVoiceChat");
        }


        public virtual void SendKickPlayerRequest(int chairId)
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendKickPlayerRequest");
            msg.WriteUint16((ushort)chairId);

            SendData(msg);
        }


        public virtual void SendFriendLiXianState(int state)
        {
            WLDebug.Info("SendFriendLiXianState");
            ushort id = GetSendMsgId("SendFriendLiXianState");
        }


        public virtual void SendRankingMatchInfo()
        {
            WLDebug.Info("SendRankingMatchInfo");
            ushort id = GetSendMsgId("SendRankingMatchInfo");
        }


        public virtual void SendQifuConfigRequest(int type)
        {
            WLDebug.Info("SendQifuConfigRequest");
            ushort id = GetSendMsgId("SendQifuConfigRequest");
        }


        public virtual void SendQifu(int index, bool isFree)
        {
            WLDebug.Info("SendQifu");
            ushort id = GetSendMsgId("SendQifu");
        }


        public virtual void SendCoinWinTaskRewardRequest(int type)
        {
            WLDebug.Info("SendCoinWinTaskRewardRequest");
            ushort id = GetSendMsgId("SendCoinWinTaskRewardRequest");
        }


        public virtual void SendCommonPriceListRequest(int type)
        {
            WLDebug.Info("SendCommonPriceListRequest");
            ushort id = GetSendMsgId("SendCommonPriceListRequest");
        }


        public virtual void SendUseMagicPropRequest(int type, int targetChairId, uint propId, int count, int freeCount)
        {
            WLDebug.Info("SendUseMagicPropRequest");
            ushort id = GetSendMsgId("SendUseMagicPropRequest");
        }


        public virtual void SendUseShuffleRequest(int type, bool isFree)
        {
            WLDebug.Info("SendUseShuffleRequest");
            ushort id = GetSendMsgId("SendUseShuffleRequest");
        }


        public virtual void SendNewClient()
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendNewClient");
            SendData(msg);
        }
        #endregion
    }
}
