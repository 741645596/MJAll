// @Author: tanjinhua
// @Date: 2021/5/25  10:18


using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// 朋友场游戏信息
    /// </summary>
    public class FriendGameInfo
    {
        /// <summary>
        /// 是否开启GPS
        /// </summary>
        public bool isOpenGPS;
        /// <summary>
        /// 是否开启语音聊天
        /// </summary>
        public bool isOpenVoiceChat;
        /// <summary>
        /// 是否开启实时语音
        /// </summary>
        public bool isOpenInstantVoiceChat;
        /// <summary>
        /// 是否开启超时解散
        /// </summary>
        public bool isOpenOverTimeDissolution;
        /// <summary>
        /// 是否开启离线事件累计
        /// </summary>
        public bool accumulatingOfflineTime;
        /// <summary>
        /// 自定义游戏名称
        /// </summary>
        public string gameName;
        /// <summary>
        /// 游戏总局数
        /// </summary>
        public int totalGameCount;
        /// <summary>
        /// 游戏当前局数
        /// </summary>
        public int currentGameCount;
        /// <summary>
        /// 规则列表
        /// </summary>
        public List<string> rules;
        /// <summary>
        /// 封顶/番数/倍数等标题
        /// </summary>
        public string otherInfoTitle;
        /// <summary>
        /// 封顶/番数/倍数等值字符串
        /// </summary>
        public string otherInfo;
        /// <summary>
        /// 自定义局数标题，如 “圈数：”
        /// </summary>
        public string customGameCountTitle;
        /// <summary>
        /// 自定义局数值字符串，如 “8圈”
        /// </summary>
        public string customGameCount;

        /// <summary>
        /// 是否使用自定义局数
        /// </summary>
        public bool useCustomGameCount =>
            !string.IsNullOrEmpty(customGameCountTitle) &&
            !string.IsNullOrEmpty(customGameCount);

        /// <summary>
        /// 是否有扩展信息
        /// </summary>
        public bool hasMultiplyInfo =>
            !string.IsNullOrEmpty(otherInfoTitle) &&
            !string.IsNullOrEmpty(otherInfo);
    }
}
