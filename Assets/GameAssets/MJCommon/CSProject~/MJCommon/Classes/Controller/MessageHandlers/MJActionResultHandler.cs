// @Author: tanjinhua
// @Date: 2021/5/5  10:55


using System;
using System.Collections.Generic;
using UnityEngine;
using WLCore;
using WLHall.Game;

namespace MJCommon
{
    /// <summary>
    /// 动作结果广播消息逻辑
    /// </summary>
    public class MJActionResultHandler : BaseGameController
    {
        private MJGameData _gameData;

        private MJActionButtonController _actionButtonController;
        private MJActionEffectController _actionEffectController;
        private MJHandController _handController;
        private MJMeldController _meldController;
        private MJDeskCardController _deskCardController;
        private MJWinCardController _winCardController;
        private MJAvatarController _avatarController;
        private MJTrustController _trustController;
        private MJTableController _tableController;
        private MJSendCardHandler _sendCardHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _actionButtonController = stage.GetController<MJActionButtonController>();
            _actionEffectController = stage.GetController<MJActionEffectController>();
            _handController = stage.GetController<MJHandController>();
            _meldController = stage.GetController<MJMeldController>();
            _deskCardController = stage.GetController<MJDeskCardController>();
            _winCardController = stage.GetController<MJWinCardController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _trustController = stage.GetController<MJTrustController>();
            _tableController = stage.GetController<MJTableController>();
            _sendCardHandler = stage.GetController<MJSendCardHandler>();
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="chairId"></param>
        /// <param name="fuziIndex"></param>
        /// <param name="fuziData"></param>
        /// <param name="toRemoveCardValues"></param>
        public virtual void UpdateData(ushort msgId, int chairId, int showType, int fuziIndex, FuziData fuziData, List<int> toRemoveCardValues)
        {
            _gameData.verifyingMsgId = msgId;

            // 更新副子提供者的出牌数据
            if (NeedsRemoveLastOutCard(chairId, fuziIndex, fuziData))
            {
                MJGamePlayer player = _gameData.GetPlayerByChairId(fuziData.provideUser) as MJGamePlayer;
                player.RemoveLastOutCardValue();
            }

            MJGamePlayer actionPlayer = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;

            // 如果是听牌动作，更新玩家听牌状态
            UpdateReadyHandState(actionPlayer, showType);

            // 如果是补花动作，更新玩家花牌数据
            UpdateFlowerValues(actionPlayer, showType, toRemoveCardValues);

            // 更新动作玩家的副子数据
            UpdateFuziData(actionPlayer, fuziIndex, fuziData);

            // 更新动作玩家的手牌数据
            actionPlayer.RemoveHandCardValues(toRemoveCardValues);

            // 自家手牌数据排序
            if (chairId == _gameData.playerSelf.chairId)
            {
                actionPlayer.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable, true);
            }
        }

        /// <summary>
        /// 更新动作玩家听牌状态
        /// </summary>
        /// <param name="actionPlayer"></param>
        /// <param name="showType"></param>
        public virtual void UpdateReadyHandState(MJGamePlayer actionPlayer, int showType)
        {
            if (showType != ActionShowType.Ting)
            {
                return;
            }
            actionPlayer.isReadyHand = true;
            _sendCardHandler.UpdateUnselectableCardValues();
        }

        /// <summary>
        /// 更新动作玩家花牌数据
        /// </summary>
        /// <param name="actionPlayer"></param>
        /// <param name="showType"></param>
        /// <param name="toRemoveCardValues"></param>
        public virtual void UpdateFlowerValues(MJGamePlayer actionPlayer, int showType, List<int> toRemoveCardValues)
        {
            if (showType != ActionShowType.Buhua)
            {
                return;
            }
            string errorMsg = "MJActionResultHandler.UpdateDatas: 补花操作toRemoveCardValues必须有一个值";
            Debug.Assert(toRemoveCardValues.Count > 0 && toRemoveCardValues[0] != Card.Invalid, errorMsg);
            actionPlayer.AddWinCardValue(toRemoveCardValues[0]);
        }


        /// <summary>
        /// 更新动作玩家副子数据
        /// </summary>
        /// <param name="fuziIndex"></param>
        /// <param name="fuziData"></param>
        public virtual void UpdateFuziData(MJGamePlayer actionPlayer, int fuziIndex, FuziData fuziData)
        {
            if (fuziIndex == MJDefine.InvaildFuziIndex)
            {
                return;
            }

            if (fuziIndex >= actionPlayer.fuziCount)
            {
                actionPlayer.AddFuziData(fuziData);
            }
            else
            {
                actionPlayer.ReplaceFuziData(fuziIndex, fuziData);
            }
        }


        /// <summary>
        /// 播放动作结果一系列动画
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        public virtual void EnqueueActionResult(MsgHeader msg, MsgActionResult data)
        {
            EnqueueActionEffect(data.chairId, data.showType, data.fuziData);

            EnqueueUpdateActionPlayerHand(data.chairId, data.showType, data.toRemoveCardValues);

            EnqueueUpdateActionPlayerFlower(data.chairId, data.showType, data.toRemoveCardValues);

            EnqueueRemoveProviderDeskCard(data.chairId, data.fuziIndex, data.fuziData);

            EnqueueUpdateActionPlayerMeld(data.chairId, data.showType, data.fuziIndex, data.fuziData);

            EnqueueShowTingIcon(data.chairId, data.showType);

            EnqueueRefreshActionPlayerHand(data.chairId, data.showType, data.toRemoveCardValues);

            _actionButtonController.EnqueueShowButtons(msg, data.actionCount);

            EnqueueActionResultTimer(data.chairId);
        }


        /// <summary>
        /// 播放动作特效
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="showType"></param>
        /// <param name="toRemoveCardValues"></param>
        public virtual void EnqueueActionEffect(int chairId, int showType, FuziData fuziData)
        {
            stage.animationQueue.Enqueue(0, () => _actionEffectController.PlayActionEffect(chairId, showType, fuziData));
        }


        /// <summary>
        /// 更新动作玩家手牌节点
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="showType"></param>
        /// <param name="toRemoveCardValues"></param>
        public virtual void EnqueueUpdateActionPlayerHand(int chairId, int showType, List<int> toRemoveCardValues)
        {
            if (showType == ActionShowType.Ting)
            {
                return;
            }

            if (toRemoveCardValues.Count == 0)
            {
                return;
            }

            int viewChairId = stage.ToViewChairId(chairId);
            var handSet = _handController.GetHandSet(viewChairId);

            stage.animationQueue.Enqueue(0.5f, () => MJAnimationHelper.PlaySelectHandCardAnimation(handSet, toRemoveCardValues, null));
            stage.animationQueue.Enqueue(0, () => handSet.Remove(toRemoveCardValues));
        }


        /// <summary>
        /// 更新动作玩家副子
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="showType"></param>
        /// <param name="fuziIndex"></param>
        /// <param name="fuziData"></param>
        public virtual void EnqueueUpdateActionPlayerMeld(int chairId, int showType, int fuziIndex, FuziData fuziData)
        {
            if (showType == ActionShowType.Ting)
            {
                return;
            }

            if (fuziIndex == MJDefine.InvaildFuziIndex)
            {
                return;
            }

            //MJMeld.Args args = stage.Call<MJMeld.Args>("MJMeldController.ToMeldData", chairId, fuziData);

            var args = stage.GetController<MJMeldController>().ToMeldData(chairId, fuziData);

            int viewChairId = stage.ToViewChairId(chairId);

            var meldStack = _meldController.GetMeldStack(viewChairId);
            if (fuziIndex >= meldStack.count)
            {
                stage.animationQueue.Enqueue(0.1f, () => _meldController.PlayAppend(viewChairId, args));
            }
            else
            {
                stage.animationQueue.Enqueue(0f, () => _meldController.PlayReplace(viewChairId, fuziIndex, args));
            }
        }


        /// <summary>
        /// 刷新动作玩家手牌
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="showType"></param>
        /// <param name="toRemoveCardValues"></param>
        public virtual void EnqueueRefreshActionPlayerHand(int chairId, int showType, List<int> toRemoveCardValues)
        {
            if (showType == ActionShowType.Ting)
            {
                return;
            }

            if (toRemoveCardValues.Count == 0)
            {
                return;
            }

            stage.animationQueue.Enqueue(0.3f, () =>
            {
                int viewChairId = stage.ToViewChairId(chairId);
                var handSet = _handController.GetHandSet(viewChairId);
                MJAnimationHelper.PlayRefreshHandsetAnimation(handSet);
            });
        }


        /// <summary>
        /// 删除提供者出牌节点
        /// </summary>
        /// <param name="providerChairId"></param>
        public virtual void EnqueueRemoveProviderDeskCard(int chairId, int fuziIndex, FuziData fuziData)
        {
            if (!NeedsRemoveLastOutCard(chairId, fuziIndex, fuziData))
            {
                return;
            }

            int viewChairId = stage.ToViewChairId(fuziData.provideUser);
            stage.animationQueue.Enqueue(0, () => _deskCardController.Pop(viewChairId));
        }


        /// <summary>
        /// 更新花牌
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="showType"></param>
        /// <param name="toRemoveCardValues"></param>
        public virtual void EnqueueUpdateActionPlayerFlower(int chairId, int showType, List<int> toRemoveCardValues)
        {
            if (showType != ActionShowType.Buhua)
            {
                return;
            }

            int viewChairId = stage.ToViewChairId(chairId);

            _gameData.isActionDone = false;
            stage.animationQueue.Enqueue(0, () =>
            {
                _winCardController.Push(viewChairId, toRemoveCardValues[0]);
                _gameData.isActionDone = true;
            });
        }


        /// <summary>
        /// 显示听牌小图标
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="showType"></param>
        public virtual void EnqueueShowTingIcon(int chairId, int showType)
        {
            if (showType != ActionShowType.Ting)
            {
                return;
            }

            int viewChairId = stage.ToViewChairId(chairId);

            stage.animationQueue.Enqueue(0, () => _avatarController.ShowBankerIcon(viewChairId));
        }


        /// <summary>
        /// 更新玩家副子合集控件
        /// </summary>
        /// <param name="chairId"></param>
        public virtual void EnqueueReloadMeldSet(int chairId)
        {
            MJGamePlayer player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;

            var datas = _meldController.ToMeldDatas(chairId, player.fuziDatas);

            int viewChairId = stage.ToViewChairId(chairId);

            stage.animationQueue.Enqueue(0, () => _meldController.Reload(viewChairId, datas));
        }


        /// <summary>
        /// 开始动作结果倒计时
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="timeout"></param>
        public virtual void EnqueueActionResultTimer(int chairId, int timeout = -1)
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
        /// 是否需要删除上一张出牌
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="fuziIndex"></param>
        /// <param name="fuziData"></param>
        /// <returns></returns>
        protected virtual bool NeedsRemoveLastOutCard(int chairId, int fuziIndex, FuziData fuziData)
        {
            if (fuziIndex == MJDefine.InvaildFuziIndex)
            {
                return false;
            }

            if (fuziData.operateCard == Card.Invalid)
            {
                return false;
            }

            if (fuziData.provideUser == MJDefine.InvaildFuziIndex)
            {
                return false;
            }

            if (chairId == fuziData.provideUser)
            {
                return false;
            }

            if (fuziData.weaveKind1 == ActionType.GangPuBuGang)
            {
                return false;
            }

            return true;
        }
    }
}
