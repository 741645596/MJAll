// @Author: tanjinhua
// @Date: 2021/5/4  10:10


using System;
using System.Collections.Generic;
using UnityEngine;
using WLCore;
using WLHall.Game;

namespace MJCommon
{
    /// <summary>
    /// 发牌广播消息逻辑
    /// </summary>
    public class MJSendCardHandler : BaseGameController
    {
        private MJGameData _gameData;

        private MJHandController _handController;
        private MJWallController _wallController;
        private MJWinCardController _winCardController;
        private MJTableController _tableController;
        private MJTrustController _trustController;
        private MJAvatarController _avatarController;
        private MJHintsController _hintsController;
        private MJActionButtonController _actionButtonController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _handController = stage.GetController<MJHandController>();
            _wallController = stage.GetController<MJWallController>();
            _winCardController = stage.GetController<MJWinCardController>();
            _tableController = stage.GetController<MJTableController>();
            _trustController = stage.GetController<MJTrustController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _hintsController = stage.GetController<MJHintsController>();
            _actionButtonController = stage.GetController<MJActionButtonController>();
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="chairId"></param>
        /// <param name="cardValue"></param>
        /// <param name="isKongCard">是否为杠后补发牌</param>
        public virtual void UpdateData(ushort msgId, int chairId, int cardValue, bool isKongCard, MsgHeader msg, int actionCount)
        {
            _gameData.verifyingMsgId = msgId;

            if (isKongCard)
            {
                _gameData.sendedKongCardCount++;
            }
            else
            {
                _gameData.sendedCardCount++;
            }

            MJGamePlayer player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;

            player.AddHandCardValue(cardValue);

            UpdateUnselectableCardValues();

            if (player == _gameData.playerSelf)
            {
                UpdateActionData(msg, actionCount);
            }
        }


        /// <summary>
        /// 更新自家听牌状态下的不可点击牌
        /// </summary>
        public virtual void UpdateUnselectableCardValues()
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            if (!playerSelf.isReadyHand)
            {
                return;
            }

            List<int> handValues = playerSelf.handCardValues;

            bool includeLastCard = playerSelf.handCardCount % 3 != 2;

            if (!includeLastCard)
            {
                handValues.RemoveAt(handValues.Count - 1);
            }

            playerSelf.unselectableCardValues = handValues;
        }


        /// <summary>
        /// 发牌动画
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="cardValue"></param>
        /// <param name="isKong"></param>
        public virtual void EnqueueSendCard(int chairId, int cardValue, bool isKong)
        {
            // 播放发牌动画、更新牌墙、剩余牌数
            int viewChairId = stage.ToViewChairId(chairId);
            int remainCardCount = _gameData.remainCardCount;
            bool isZimo = _gameData.HasAction(ActionShowType.Hu);
            if (isZimo && viewChairId == Chair.Down)
            {
                stage.animationQueue.Enqueue(1.5f, () =>
                {
                    var card = MJCardPool.Get<MJCard>(cardValue);
                    card.HideShadow();
                    card.body.gameObject.transform.localEulerAngles = new Vector3(180, 0, 0);

                    var trs = card.gameObject.transform;
                    trs.position = _winCardController.GetNextPosition(viewChairId);
                    trs.RunTweenGraph("MJCommon/MJ/mj_tween", "card_mo.asset", (t) => MJCardPool.Recycle(card));
                });
            }
            stage.animationQueue.Enqueue(0, () =>
            {
                _handController.PlayDraw(viewChairId, cardValue);

                _wallController.Take(isKong);

                _tableController.SetRemainCardCount(remainCardCount);
            });
        }


        /// <summary>
        /// 尝试自动出牌、自动执行动作事件等操作
        /// </summary>
        /// <param name="cardValue"></param>
        public virtual void EnqueueAutoOperations(int chairId, int cardValue)
        {
            if (CanAutoOutCard(chairId))
            {
                EnqueueAutoSendOutCard();
            }
            else if (CanAutoGang())
            {
                EnqueueAutoSendAction();
            }
            else if (CanAutoHu())
            {
                EnqueueAutoSendAction();
            }
            else if (Card.GetCardColorValue(cardValue) != Card.ColorFlower)
            {
                EnqueueSendCardTimer(chairId);
            }
        }


        /// <summary>
        /// 开始发牌倒计时
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="cardValue"></param>
        /// <param name="timeout"></param>
        public virtual void EnqueueSendCardTimer(int chairId, int timeout = -1)
        {
            timeout = timeout == -1 ? _gameData.trustTimeout : timeout;

            int viewChairId = stage.ToViewChairId(chairId);

            stage.animationQueue.Enqueue(0, () =>
            {
                _avatarController.SwitchLightEffect(viewChairId);

                Action<int> action = elapse =>
                {
                    if (viewChairId != Chair.Down)
                    {
                        return;
                    }

                    _trustController.OnTimerTick(timeout, elapse);
                };
                _tableController.PlayTurn(viewChairId, timeout, action);
            });
        }


        /// <summary>
        /// 发牌后是否能够自动出牌
        /// </summary>
        /// <param name="chairId"></param>
        /// <returns></returns>
        protected virtual bool CanAutoOutCard(int chairId)
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            bool isSelf = chairId == playerSelf.chairId;

            bool allowAuto = playerSelf.isReadyHand || _hintsController.IsAllowAuto();

            bool noActions = !_gameData.HasAction();

            bool discardable = playerSelf.handCardCount % 3 == 2;

            return isSelf && noActions && discardable && allowAuto;
        }


        /// <summary>
        /// 发牌后是否能够自动杠, 默认false
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanAutoGang()
        {
            return false;
        }


        /// <summary>
        /// 发牌后是否能够自动胡, 默认false
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanAutoHu()
        {
            return false;
        }


        /// <summary>
        /// 自动发送出牌消息给服务器
        /// </summary>
        private void EnqueueAutoSendOutCard()
        {
            int outValue = _trustController.FindDiscardableCardValue();

            stage.animationQueue.Enqueue(0, () => stage.Send("SendOutCard", outValue, true, false));
        }


        /// <summary>
        /// 自动发送执行动作给服务器
        /// </summary>
        private void EnqueueAutoSendAction()
        {
            List<MJActionData> actionDatas = _gameData.currentActionDatas;

            stage.animationQueue.Enqueue(0, () =>
            {
                stage.Send("SendAction", actionDatas[0].actionType, actionDatas[0].operateCard);

                _actionButtonController.ClearButtons();
            });
        }

        private void UpdateActionData(MsgHeader msg, int actionCount)
        {
            if (actionCount == 0)
            {
                return;
            }


            List<MJActionData> actionDatas = new List<MJActionData>();

            for (int i = 0; i < actionCount; i++)
            {
                actionDatas.Add(MJActionData.From(msg));
            }

            // 保存当前动作数据
            _gameData.currentActionDatas = new List<MJActionData>(actionDatas);
        }
    }
}
