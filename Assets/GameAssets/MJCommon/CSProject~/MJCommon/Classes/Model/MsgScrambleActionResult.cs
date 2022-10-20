// @Author: tanjinhua
// @Date: 2021/5/6  9:02


using System.Collections.Generic;
using WLCore;

namespace MJCommon
{
    public class MsgScrambleActionResult : MsgActionResult
    {
        public int recoverChairId;
        public int recoverFuziIndex;
        public FuziData recoverFuziData;

        public new static MsgScrambleActionResult From(MsgHeader msg)
        {
            MsgScrambleActionResult data = new MsgScrambleActionResult
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
                    data.toRemoveCardValues.Add(value);
                }
            }
            data.fuziIndex = msg.ReadByte();
            data.fuziData = FuziData.From(msg);

            data.recoverChairId = msg.ReadByte();
            data.recoverFuziIndex = msg.ReadByte();
            data.recoverFuziData = FuziData.From(msg);

            data.actionCount = msg.ReadByte();

            return data;
        }
    }
}
