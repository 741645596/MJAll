// @Author: tanjinhua
// @Date: 2021/5/25  15:14


using System;
using UnityEngine;
using WLFishingHall;
using WLHall.Game;

namespace Common
{
    public abstract class GameAvatarController : BaseGameController
    {

        private IdentityController _identityController;

        public override void OnSceneLoaded()
        {
            _identityController = stage.GetController<IdentityController>();
        }

        /// <summary>
        /// 获取头像节点
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public abstract AvatarBase GetAvatar(int viewChairId);

        /// <summary>
        /// 获取头像节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public T GetAvatar<T>(int viewChairId) where T : AvatarBase => GetAvatar(viewChairId) as T;

        /// <summary>
        /// 添加头像
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="gender"></param>
        /// <param name="avatarUrl"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public abstract AvatarBase Append(int viewChairId, int gender, string avatarUrl, uint userId);

        /// <summary>
        /// 移除头像
        /// </summary>
        /// <param name="viewChairId"></param>
        public abstract void Remove(int viewChairId);

        /// <summary>
        /// 清除所有头像
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// 获取头像屏幕坐标
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public abstract Vector2 GetAvatarPosition(int viewChairId);

        /// <summary>
        /// 点击头像事件
        /// </summary>
        /// <param name="userId"></param>
        protected virtual void OnClickAvatar(uint userId)
        {
            _identityController?.ShowIdentity(userId);

            // TODO: 完善段位赛个人信息显示逻辑
        }

        /// <summary>
        /// 遍历头像节点
        /// </summary>
        /// <param name="handler"></param>
        public abstract void Traverse(Action<AvatarBase, int> handler);

        /// <summary>
        /// 遍历头像节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Traverse<T>(Action<T, int> handler) where T : AvatarBase
        {
            Traverse((avatar, viewChairId) => handler.Invoke(avatar as T, viewChairId));
        }

        /// <summary>
        /// 更新分数/金币
        /// </summary>
        /// <param name="viewChairId"></param>
        public virtual void UpdateScore(int viewChairId)
        {
            int chairId = stage.ToServerChairId(viewChairId);

            GamePlayer player = stage.gameData.GetPlayerByChairId(chairId) as GamePlayer;

            long score = stage.gameData.isFriendRoom ? player.friendGameScore : player.GetMoney();

            string text = MathKit.HongFuQianRequest(score);

            GetAvatar(viewChairId)?.SetScore(text);
        }


        #region Game Events
        public override void OnChangeDesk()
        {
            base.OnChangeDesk();

            Clear();
        }

        public override void OnContinue()
        {
            base.OnContinue();

            Traverse((avatar, i) => avatar?.Reset());
        }

        public override void OnGameStart()
        {
            base.OnGameStart();

            Traverse((avatar, i) => avatar?.HideReady());
        }

        public override void OnFriendGameOver()
        {
            base.OnFriendGameOver();

            Traverse((avatar, i) => avatar?.HideReady());
        }
        #endregion
    }
}
