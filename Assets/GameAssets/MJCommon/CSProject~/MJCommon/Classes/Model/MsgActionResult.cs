// @Author: tanjinhua
// @Date: 2021/5/5  10:49


using System.Collections.Generic;
using WLCore;

namespace MJCommon
{
    public class MsgActionResult
    {
        public ushort msgId;
        public int chairId;
        public int showType;
        public List<int> toRemoveCardValues;
        public int fuziIndex;
        public FuziData fuziData;
        public int actionCount;

        public static MsgActionResult From(MsgHeader msg)
        {
            MsgActionResult date = new MsgActionResult
            {
                msgId = msg.ReadUint16(),
                chairId = msg.ReadByte(),
                showType = msg.ReadByte(),
                toRemoveCardValues = new List<int>()
            };

            for (int i = 0; i < 4; i++)
            {
                int value = msg.ReadByte();
                if (value != Card.Invalid)
                {
                    date.toRemoveCardValues.Add(value);
                }
            }

            date.fuziIndex = msg.ReadByte();
            date.fuziData = FuziData.From(msg);
            date.actionCount = msg.ReadByte();

            return date;
        }
    }
}
