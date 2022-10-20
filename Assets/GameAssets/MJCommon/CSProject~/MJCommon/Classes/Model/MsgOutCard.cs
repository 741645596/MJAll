// @Author: tanjinhua
// @Date: 2021/5/3  19:03


using WLCore;

namespace MJCommon
{
    public struct MsgOutCard
    {
        public ushort msgId;
        public int chairId;
        public int cardValue;
        public bool serverOut;
        public int actionCount;

        public static MsgOutCard From(MsgHeader msg)
        {
            return new MsgOutCard
            {
                msgId = msg.ReadUint16(),
                chairId = msg.ReadUint16(),
                cardValue = msg.ReadByte(),
                serverOut = msg.ReadByte() == 1,
                actionCount = msg.ReadByte()
            };
        }
    }
}
