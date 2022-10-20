// @Author: tanjinhua
// @Date: 2021/5/6  14:45


using System.Collections.Generic;
using WLCore;

namespace MJCommon
{
    public class MsgHuInfo
    {
        public MJHintsData.Type type;
        public int count;
        public Dictionary<int, MJHintsData> hintsDataMap;


        public static MsgHuInfo From(MsgHeader msg, MJHintsData.Type type)
        {
            MsgHuInfo data = new MsgHuInfo
            {
                type = type,
                hintsDataMap = new Dictionary<int, MJHintsData>()
            };

            int count = msg.ReadByte();

            for (int i = 0; i < count; i++)
            {
                int infoCount = msg.ReadByte();

                int selectCardValue = msg.ReadByte();

                data.hintsDataMap[selectCardValue] = MJHintsData.From(msg, type, infoCount - 1);
            }

            return data;
        }
    }
}
