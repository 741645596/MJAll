// @Author: tanjinhua
// @Date: 2021/6/1  11:57


using System;
using Unity.Widget;

namespace Common
{
    public abstract class AvatarBase : WNode
    {
        public Action<uint> onClick;

        /// <summary>
        /// 用户ID
        /// </summary>
        public uint userId { get; protected set; }

        /// <summary>
        /// 视图座位号
        /// </summary>
        public int viewChairId;

        /// <summary>
        /// 设置用户数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gender"></param>
        /// <param name="avatarUrl"></param>
        public abstract void SetUserInfo(uint userId, int gender, string avatarUrl);

        /// <summary>
        /// 设置分数
        /// </summary>
        /// <param name="text"></param>
        public abstract void SetScore(string text);

        /// <summary>
        /// 隐藏分数
        /// </summary>
        public abstract void HideScore();

        /// <summary>
        /// 显示托管状态
        /// </summary>
        public abstract void ShowTrust();

        /// <summary>
        /// 隐藏托管状态
        /// </summary>
        public abstract void HideTrust();

        /// <summary>
        /// 显示破产图标
        /// </summary>
        /// <param name="animated"></param>
        public abstract void ShowBankruptcy(bool animated = true);

        /// <summary>
        /// 隐藏破产图标
        /// </summary>
        public abstract void HideBankruptcy();

        /// <summary>
        /// 显示准备图标
        /// </summary>
        public abstract void ShowReady();

        /// <summary>
        /// 隐藏准备图标
        /// </summary>
        public abstract void HideReady();

        /// <summary>
        /// 显示GPS警告灯
        /// </summary>
        /// <param name="type"></param>
        public abstract void ShowGpsWarning(GPSWarning.Type type);

        /// <summary>
        /// 隐藏GPS警告灯
        /// </summary>
        public abstract void HideGpsWarning();

        /// <summary>
        /// 显示离线状态
        /// </summary>
        public abstract void ShowOffline();

        /// <summary>
        /// 显示累计离线时间
        /// </summary>
        /// <param name="time"></param>
        public abstract void ShowOfflineTime(int time);

        /// <summary>
        /// 显示离线倒计时
        /// </summary>
        /// <param name="countdown"></param>
        public abstract void ShowOfflineCountdown(int countdown);

        /// <summary>
        /// 隐藏离线状态
        /// </summary>
        public abstract void HideOffline();

        /// <summary>
        /// 重置状态
        /// </summary>
        public virtual void Reset()
        {
            HideTrust();

            HideBankruptcy();

            HideReady();

            HideGpsWarning();

            HideOffline();
        }
    }
}
