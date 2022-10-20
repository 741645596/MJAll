// @Author: tanjinhua
// @Date: 2021/8/20  17:22


using System.Collections.Generic;

namespace MJCommon
{
    public class MJUtils
    {
        /// <summary>
        /// 虚拟手牌数据
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<int> FakeHandCardValues(int count)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < count; i++)
            {
                result.Add(Card.Rear);
            }

            return result;
        }

    }
}
