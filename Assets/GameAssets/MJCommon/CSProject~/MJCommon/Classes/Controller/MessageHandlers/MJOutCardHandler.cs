// @Author: tanjinhua
// @Date: 2021/5/3  19:08


using System;
using System.Collections.Generic;
using UnityEngine;
using WLHall.Game;

namespace MJCommon
{

    /// <summary>
    /// 出牌广播处理逻辑
    /// </summary>
    public class MJOutCardHandler : BaseGameController
    {
        private MJGameData _gameData;

        private MJHandController _handController;
        private MJDeskCardController _deskCardController;
        private MJAvatarController _avatarController;
        private MJTableController _tableController;
        private MJTrustController _trustController;
        private MJActionButtonController _actionButtonController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _handController = stage.GetController<MJHandController>();
            _deskCardController = stage.GetController<MJDeskCardController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _tableController = stage.GetController<MJTableController>();
            _trustController = stage.GetController<MJTrustController>();
            _actionButtonController = stage.GetController<MJActionButtonController>();
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="chairId"></param>
        /// <param name="cardValue"></param>
        /// <param name="isServerOut"></param>
        public virtual void UpdateData(ushort msgId, int chairId, int cardValue, bool isServerOut)
        {
            Debug.Assert(cardValue != Card.Invalid, "MJOutCardHandler.UpdateData: 牌值无效");

            MJGamePlayer player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;

            _gameData.verifyingMsgId = msgId;

            player.AddOutCardValue(cardValue);

            if (player.HandCardValueExists(cardValue))
            {
                player.RemoveHandCardValue(cardValue);
            }
            else
            {
                player.RemoveHandCardValue(Card.Rear);
            }

            if (player == _gameData.playerSelf)
            {
                player.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable);
            }
        }


        /// <summary>
        /// 使用动画队列播放出牌动画
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="cardNumber"></param>
        public virtual void EnqueueOutCard(int chairId, int cardValue)
        {
            int selfChairId = _gameData.playerSelf.chairId;
            if (chairId == selfChairId && _gameData.verifyingOutCardValue != Card.Invalid)
            {
                return;
            }

            int viewChairId = stage.ToViewChairId(chairId);
            var player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;
            stage.animationQueue.Enqueue(0.3f, () =>
            {
                _actionButtonController.ClearButtons();

                MJCard card = FindOutCard(viewChairId, cardValue, player.isReadyHand);
                if (card == null)
                {
                    WLDebug.LogWarning($"MJOutCardHandler.EnqueueOutCard: 找不到牌值为[{cardValue}]的牌节点");
                    return;
                }

                // 出牌动画
                _deskCardController.PlayDiscard(viewChairId, cardValue, card);

                // 移除手牌
                var handSet = _handController.GetHandSet(viewChairId);
                var isLastCard = handSet.IndexOf(card) == handSet.count - 1;
                _handController.RemoveCard(viewChairId, card);

                if (!isLastCard)
                {
                    // 理牌动画
                    _handController.PlayInsert(viewChairId);
                }

                var nextViewChairId = stage.NextViewChairId(viewChairId);

                _avatarController.SwitchLightEffect(nextViewChairId);

                _tableController.PlayTurn(nextViewChairId, _gameData.trustTimeout, null);
            });
        }


        /// <summary>
        /// 验证出牌
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="cardValue"></param>
        public virtual void EnqueueVerifyOutCard(int chairId, int cardValue)
        {
            if (chairId == _gameData.playerSelf.chairId)
            {
                int value = _gameData.verifyingOutCardValue;

                if (value != Card.Invalid && value != cardValue)
                {
                    EnqueueReloadHandCard(chairId);

                    EnqueueReloadDeskCard(chairId);
                }
            }

            _gameData.verifyingOutCardValue = Card.Invalid;
        }


        /// <summary>
        /// 更新手牌控件
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="updateAlignment">是否更新对齐，默认false。在执行动作并且有删除手牌时才传true</param>
        public virtual void EnqueueReloadHandCard(int chairId)
        {
            MJGamePlayer player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;

            List<int> handCardValues = player.handCardValues;

            int viewChairId = stage.ToViewChairId(chairId);

            stage.animationQueue.Enqueue(0, () => _handController.Reload(viewChairId, handCardValues));
        }


        /// <summary>
        /// 更新出牌控件
        /// </summary>
        /// <param name="chairId"></param>
        public virtual void EnqueueReloadDeskCard(int chairId)
        {
            MJGamePlayer player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;

            List<int> outCardValues = player.outCardValues;

            int viewChairId = stage.ToViewChairId(chairId);

            stage.animationQueue.Enqueue(0, () => _deskCardController.Reload(viewChairId, outCardValues));
        }


        /// <summary>
        /// 其他家出牌触发自家动作事件的倒计时
        /// </summary>
        public virtual void EnqueueSelfActionEventTimer(int timeout = -1)
        {
            timeout = timeout == -1 ? _gameData.trustTimeout : timeout;

            int viewChairId = Chair.Down;

            stage.animationQueue.Enqueue(0, () =>
            {
                _avatarController.SwitchLightEffect(viewChairId);
                Action<int> action = elapse => _trustController.OnTimerTick(timeout, elapse);
                _tableController.PlayTurn(viewChairId, timeout, action);
            });
        }


        /// <summary>
        /// 找到要打出的牌节点
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValue"></param>
        /// <param name="isReadyHand"></param>
        /// <returns></returns>
        protected virtual MJCard FindOutCard(int viewChairId, int cardValue, bool isReadyHand)
        {
            if (viewChairId == Chair.Down)
            {
                return _handController.FindCardByValue(viewChairId, cardValue, true);
            }

            MJHandSet hand = _handController.GetHandSet(viewChairId);

            int randomIndex = isReadyHand ? hand.count - 1 : UnityEngine.Random.Range(0, hand.count);

            return hand.GetCard(randomIndex);
        }
    }
}
