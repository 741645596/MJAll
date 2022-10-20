// @Author: tanjinhua
// @Date: 2021/4/13  13:42

using WLHall;

namespace MJCommon
{
    /// z坐标 = 大厅结尾 + 间隔 * z系数；z坐标越大越靠后显示
    public class MJZorder
    {

        /// <summary>
        /// 对话框
        /// </summary>
        public const int GameDialog = HallZorder.End + HallZorder.Gap;

        /// <summary>
        /// 战绩统计
        /// </summary>
        public const int RecordStatistices = HallZorder.End + HallZorder.Gap * 2;

        /// <summary>
        /// 结算界面
        /// </summary>
        public const int SettleView = HallZorder.End + HallZorder.Gap * 3;

        /// <summary>
        /// 菜单界面
        /// </summary>
        public const int GameMenu = HallZorder.End + HallZorder.Gap * 4;

        /// <summary>
        /// 动作事件按钮层
        /// </summary>
        public const int ActionButtonPanel = HallZorder.End + HallZorder.Gap * 5;

        /// <summary>
        /// 动作事件特效层
        /// </summary>
        public const int ActionEffectPanel = HallZorder.End + HallZorder.Gap * 6;

        /// <summary>
        /// 功能按钮层
        /// </summary>
        public const int FunctionPanel = HallZorder.End + HallZorder.Gap * 7;

        /// <summary>
        /// 头像层
        /// </summary>
        public const int AvatarPanel = HallZorder.End + HallZorder.Gap * 8;

        /// <summary>
        /// 朋友场准备界面
        /// </summary>
        public const int FriendReady = HallZorder.End + HallZorder.Gap * 9;

        /// <summary>
        /// 托管层
        /// </summary>
        public const int TrustPanel = HallZorder.End + HallZorder.Gap * 10;

        /// <summary>
        /// 换牌UI
        /// </summary>
        public const int ExchangePanel = HallZorder.End + HallZorder.Gap * 11;

        /// <summary>
        /// 定缺UI
        /// </summary>
        public const int DingquePanel = HallZorder.End + HallZorder.Gap * 12;

        /// <summary>
        /// 手牌的控制层
        /// </summary>
        public const int HandCardControl = HallZorder.End + HallZorder.Gap * 13;

    }
}
