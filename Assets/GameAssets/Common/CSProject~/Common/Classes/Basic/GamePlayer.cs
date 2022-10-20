// @Author: tanjinhua
// @Date: 2021/4/28  12:42


using WLHall.Game;

namespace Common
{
    public class GamePlayer : BaseGamePlayer
    {

        /// <summary>
        /// 托管状态
        /// </summary>
        public bool isTrust;


        /// <summary>
        /// 朋友场分数
        /// </summary>
        public long friendGameScore;


        /// <summary>
        /// 当前累计离线时间
        /// </summary>
        public int currentOfflineTime;


        /// <summary>
        /// 离线状态类型
        /// </summary>
        public int offlineState;


        /// <summary>
        /// 初始化，重置数据。注：此方法会在每局游戏开始时调用，朋友场需要每局累加的数据不要在这里初始化
        /// </summary>
        public virtual void Initialize()
        {
            isTrust = false;
            currentOfflineTime = 0;
            offlineState = 0;
        }
    }
}
