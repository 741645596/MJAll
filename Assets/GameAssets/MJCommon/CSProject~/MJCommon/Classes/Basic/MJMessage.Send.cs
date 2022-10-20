// @Author: tanjinhua
// @Date: 2021/4/7  11:07


using WLHall;
using System.Collections.Generic;
using WLCore;

namespace MJCommon
{
    public partial class MJMessage
    {

        protected override Dictionary<string, ushort> RegisterSendInterface()
        {
            Dictionary<string, ushort> senders = base.RegisterSendInterface();

            senders["SendTestSignal"]               = MJMsgDefine.MsgID_C2S_Test_Signal;
            senders["SendOutCard"]                  = MJMsgDefine.MsgID_C2S_OutCard_Request;
            senders["SendAction"]                   = MJMsgDefine.MsgID_C2S_Action_Request;

            return senders;
        }


        #region Send Interfaces
        public virtual void SendTestSignal(ushort serverMsgId)
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendTestSignal");

            msg.WriteUint16(serverMsgId);
            SendData(msg);
        }

        /// <summary>
        /// 发送出牌请求
        /// </summary>
        /// <param name="cardValue"></param>
        /// <param name="serverOut"></param>
        /// <param name="updateVarify">是否更新验证牌值，默认true。自动出牌传false，这时需要手牌出牌广播才会播放出牌动画更新手牌</param>
        public virtual void SendOutCard(int cardValue, bool serverOut, bool updateVarify = true)
        {
            var gameData = stage.GetGameData<MJGameData>();
            if (updateVarify)
            {
                // 保存当前打出牌牌值用于验证
                gameData.verifyingOutCardValue = cardValue;
            }

            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendOutCard");

            msg.WriteUint16(gameData.verifyingMsgId);
            msg.WriteByte((byte)cardValue);
            msg.WriteByte((byte)(serverOut ? 1 : 0));
            SendData(msg);
        }


        public virtual void SendAction(int actionType, int operateCard)
        {
            var gameData = stage.GetGameData<MJGameData>();
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendAction");

            msg.WriteUint16(gameData.verifyingMsgId);
            msg.WriteUint16((ushort)actionType);
            msg.WriteByte((byte)operateCard);
            SendData(msg);
        }


        public void SendChangeCards(List<int> cardValues)
        {
            var gameData = stage.GetGameData<MJGameData>();
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendChangeCards");
            msg.WriteUint16(gameData.verifyingMsgId);
            cardValues.ForEach(v => msg.WriteByte((byte)v));
            SendData(msg);
        }


        public virtual void SendDingque(int dingqueColorValue)
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = GetSendMsgId("SendDingque");

            var gameData = stage.GetGameData<MJGameData>();
            msg.WriteUint16(gameData.verifyingMsgId);
            msg.WriteByte((byte)dingqueColorValue);
            SendData(msg);
        }
        #endregion
    }
}
