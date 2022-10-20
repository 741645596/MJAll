// @Author: tanjinhua
// @Date: 2021/8/18  18:23


using System.Collections.Generic;
using MJCommon;

namespace NS_XLHZ
{
    public class XLHZGameData : MJGameData
    {
        /// <summary>
        /// 一炮多响信息
        /// </summary>
        public struct MultiWinInfo
        {
            public int cardValue;
            public int winPlayerCount;
        }

        /// <summary>
        /// 牌墙配置
        /// </summary>
        public override List<int> wallStacks
        {
            get
            {
                if (hongZhongNum == 6)
                {
                    return fangShu == 3 ? new List<int> { 15, 14, 14, 14} : new List<int> { 13, 13, 13, 0 };
                }
                else if (hongZhongNum == 4)
                {
                    return fangShu == 3 ? Wall.Stacks112 : new List<int> { 13, 12, 13, 0 };
                }
                else
                {
                    return Wall.Stacks144;
                }
            }
        }

        public override DiceUsage diceUsage => DiceUsage.Two;

        /// <summary>
        /// 是否换三张
        /// </summary>
        public bool changeCard = true;

        /// <summary>
        /// 是否定缺
        /// </summary>
        public bool dingque = true;

        /// <summary>
        /// 呼叫转移
        /// </summary>
        public bool huJiaoZhuanYi = false;

        /// <summary>
        /// 是否显示流水
        /// </summary>
        public bool showLiuShui = true;

        /// <summary>
        /// 红中数量
        /// </summary>
        public int hongZhongNum = 4;

        /// <summary>
        /// 房数
        /// </summary>
        public int fangShu = 3;

        /// <summary>
        /// 封顶
        /// </summary>
        public int fengDing = 0;

        /// <summary>
        /// 一炮多响记录
        /// </summary>
        public List<MultiWinInfo> multiWinInfos = new List<MultiWinInfo>();


        public override void Initialize()
        {
            base.Initialize();

            multiWinInfos = new List<MultiWinInfo>();
        }
    }
}
