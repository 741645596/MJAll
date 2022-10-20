// @Author: tanjinhua
// @Date: 2021/4/23  17:14


using System.Collections.Generic;

namespace MJCommon
{
    public class MJSettleData
    {
        /// <summary>
        /// 玩家id
        /// </summary>
        public uint userId;
        /// <summary>
        /// 玩家昵称
        /// </summary>
        public string nickname = "";
        /// <summary>
        /// 玩家头像
        /// </summary>
        public string avatarUrl = "";
        /// <summary>
        /// 玩家性别
        /// </summary>
        public int gender;
        /// <summary>
        /// 视图座位号
        /// </summary>
        public int viewChairId;
        /// <summary>
        /// 是否破产
        /// </summary>
        public bool bankruptcy;
        /// <summary>
        /// 是否是超时解散玩家
        /// </summary>
        public bool overtimeDissolution;
        /// <summary>
        /// 胡牌类型描述
        /// </summary>
        public string huDescription = "";
        /// <summary>
        /// 是否是断线重连回来的
        /// </summary>
        public bool isReconnect;
        /// <summary>
        /// 总分
        /// </summary>
        public long totalScore;
        /// <summary>
        /// 分数细节，如：“胡 +100”、 “杠 +100”
        /// </summary>
        public List<string> scoreDetails;
        /// <summary>
        /// 庄家、点炮等小图标路径
        /// </summary>
        public List<string> iconImages;
        /// <summary>
        /// 副子配置数组
        /// </summary>
        //public MJMeld.Config[] meldConfigs;
        /// <summary>
        /// 手牌数据数组
        /// </summary>
        public List<int> handCardValues;
    }
}
