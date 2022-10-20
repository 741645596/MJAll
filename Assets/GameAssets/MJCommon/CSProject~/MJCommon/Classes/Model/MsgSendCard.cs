// @Author: tanjinhua
// @Date: 2021/5/4  10:17


using WLCore;

namespace MJCommon
{
    public struct MsgSendCard
    {
        public ushort msgId;
        public ushort chairId;
        public int cardValue;
        public bool isKongCard;
        public int actionCount;

        public static MsgSendCard From(MsgHeader msg)
        {
            return new MsgSendCard
            {
                msgId = msg.ReadUint16(),
                chairId = msg.ReadUint16(),
                cardValue = msg.ReadByte(),
                isKongCard = msg.ReadByte() == 1,
                actionCount = msg.ReadByte()
            };
        }
    }
}
