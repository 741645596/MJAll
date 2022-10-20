// @Author: tanjinhua
// @Date: 2021/4/13  14:02


using WLCore;

namespace MJCommon
{

    /// <summary>
    /// 动作事件数据
    /// </summary>
    public struct MJActionData
    {
        public int actionType;
        public int showType;
        public int operateCard;
        public int showCount;
        public int[] cardValues;


        public static MJActionData From(MsgHeader msg)
        {
            MJActionData data = new MJActionData
            {
                actionType = msg.ReadByte(),
                showType = msg.ReadByte(),
                operateCard = msg.ReadByte(),
                showCount = msg.ReadByte(),
            };

            data.cardValues = new int[data.showCount];

            for (int i = 0; i < data.showCount; i++)
            {
                data.cardValues[i] = msg.ReadByte();
            }

            return data;
        }
    }
}
