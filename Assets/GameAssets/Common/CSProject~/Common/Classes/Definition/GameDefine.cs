// @Author: tanjinhua
// @Date: 2021/5/31  9:38


namespace Common
{

    /// <summary>
    /// 离线状态定义
    /// </summary>
    public static class OffineState
    {
        public const int None       = 0; // 没有离线，即正常状态
        public const int Playing    = 1; // 游戏中
        public const int Offline    = 2; // 离线中
        public const int Calling    = 3; // 通话中
    }

    /// <summary>
    /// 结算界面事件类型定义
    /// </summary>
    public enum SettleEvent
    {
        /// <summary>
        /// 点击继续按钮事件
        /// </summary>
        Continue,
        /// <summary>
        /// 点击换桌按钮事件
        /// </summary>
        ChangeDesk,
        /// <summary>
        /// 点击离开按钮事件
        /// </summary>
        Leave,
        /// <summary>
        /// 点击免输/再得按钮事件
        /// </summary>
        ShareAd,
        /// <summary>
        /// 倒计时结束事件
        /// </summary>
        Timeup,
        /// <summary>
        /// 点击分享按钮事件
        /// </summary>
        Share,
        /// <summary>
        /// 点击战绩统计按钮事件
        /// </summary>
        Statistics,
        /// <summary>
        /// 点击返回桌面按钮事件
        /// </summary>
        BackToDesk,
        /// <summary>
        /// 点击本具结算按钮事件
        /// </summary>
        Settlement,
        /// <summary>
        /// 播放分数动画事件
        /// </summary>
        PlayScoreAnim,
        /// <summary>
        /// 播放破产动画事件
        /// </summary>
        PlayBankruptcy,

        // 洗牌按钮事件类型
        ShareShuffle,
        FreeShuffle,
        BeanShuffle,
        DiamondShuffle
    }

    /// <summary>
    /// 洗牌按钮类型
    /// </summary>
    public enum ShuffleType
    {
        Share,
        Free,
        Bean,
        Diamond
    }
}
