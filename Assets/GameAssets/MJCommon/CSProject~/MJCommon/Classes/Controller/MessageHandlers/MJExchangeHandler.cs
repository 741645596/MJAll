// @Author: tanjinhua
// @Date: 2021/10/20  10:38

using System;
using System.Collections.Generic;
using Unity.Widget;
using UnityEngine;
using WLHall.Game;

namespace MJCommon
{
    /// <summary>
    /// 换三张业务
    /// </summary>
    public class MJExchangeHandler : BaseGameController
    {
        private MJGameData _gameData;
        private MJExchangePanel _panel;
        private MJExchangeAnimation _animNode;
        private Dictionary<int, List<MJCard>> _selectedCardDic;

        private MJTableController _tableController;
        private MJHandController _handController;
        private MJSpaceController _spaceController;
        private MJAvatarController _avatarController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;
            _selectedCardDic = new Dictionary<int, List<MJCard>>();

            _tableController = stage.GetController<MJTableController>();
            _handController = stage.GetController<MJHandController>();
            _spaceController = stage.GetController<MJSpaceController>();
            _avatarController = stage.GetController<MJAvatarController>();
        }


        /// <summary>
        /// 开始换牌，gameStart调用
        /// </summary>
        public void EnqueueExchangeStart()
        {
            _gameData.gameState = GameState.Exchange;

            stage.animationQueue.Enqueue(0, () =>
            {
                _gameData.trustForbidden = true;

                ShowExchangePanel();

                SelectRecommandCards();

                StartExchangeTimer();
            });
        }

        /// <summary>
        /// 收到已选择消息逻辑
        /// </summary>
        /// <param name="chairId"></param>
        public void EnqueueExchangeSelected(int chairId)
        {
            // 暂不处理
        }

        /// <summary>
        /// 收到换牌结果消息处理
        /// </summary>
        /// <param name="type"></param>
        /// <param name="exchangedCardValues"></param>
        public void EnqueueExchangeResult(ExchangeCardType type, List<int> exchangedCardValues)
        {
            var playerSelf = _gameData.playerSelf as MJGamePlayer;
            playerSelf.AddHandCardValues(exchangedCardValues);
            playerSelf.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable);

            // 禁止托管
            _gameData.trustForbidden = false;

            _selectedCardDic.Clear();

            _tableController.StopTimer();

            // 隐藏选牌中UI
            _panel.HideInfoSelecting();

            // 播放其他家选择动画
            stage.animationQueue.Enqueue(1.3f, () => PlaySelected());

            // 播放换牌动画
            stage.animationQueue.Enqueue(2.2f, () => PlayExchange(type));

            // 播放插入3张牌动画
            stage.animationQueue.Enqueue(1f, () => PlayInsertCards(exchangedCardValues));

        }


        /// <summary>
        /// 重置换牌状态，断线重连用
        /// </summary>
        /// <param name="selectedFlags">玩家已经选择的标志，列表索引为务器座位号</param>
        public void EnqueueResetExchangeState(List<int> selectedFlags)
        {
            _gameData.gameState = GameState.Exchange;

            stage.animationQueue.Enqueue(0, () =>
            {
                ShowExchangePanel();

                _gameData.TraversePlayer<MJGamePlayer>(player =>
                {
                    var viewChairId = stage.ToViewChairId(player.chairId);
                    bool selected = selectedFlags[player.chairId] == 1;
                    if (selected)
                    {
                        ShowExchangeCardOnDesk(viewChairId);

                        _panel.HideInfoSelecting(viewChairId);

                        if (viewChairId == Chair.Down)
                        {
                            _handController.GetOwnHand().interactable = false;

                            _panel.SetConfirmButtonActive(false);
                        }
                    }
                    else
                    {
                        if (viewChairId == Chair.Down)
                        {
                            SelectRecommandCards();
                        }
                        else
                        {
                            _panel.ShowInfoSelecting(viewChairId, GetInfoPos(viewChairId));
                        }
                    }
                });
            });
        }


        /// <summary>
        /// 在桌面上显示以选中的牌
        /// </summary>
        /// <param name="viewChairId"></param>
        public void ShowExchangeCardOnDesk(int viewChairId)
        {
            if (_animNode == null || _animNode.gameObject == null)
            {
                _animNode = OnCreateExchangeAnimation();
                var space = _spaceController.GetSpace();
                _animNode.transform.SetParent(space.gameObject.transform, false);
            }

            _animNode.ShowCard(viewChairId);
        }


        /// <summary>
        /// 移除桌面显示的牌
        /// </summary>
        public void RemoveExchangeAnimation()
        {
            if (_animNode != null && _animNode.gameObject != null)
            {
                _animNode.RemoveFromParent();
            }

            _animNode = null;
        }


        /// <summary>
        /// 显示换牌UI
        /// </summary>
        public void ShowExchangePanel()
        {
            if (_panel == null || _panel.gameObject == null)
            {
                _panel = OnCreateExchangeCardPanel();

                _panel.onClickConfirm = OnConfirm;
            }

            _gameData.TraversePlayer(p =>
            {
                var viewChairId = stage.ToViewChairId(p.chairId);
                if (viewChairId == Chair.Down)
                {
                    return;
                }
                _panel.ShowInfoSelecting(viewChairId, GetInfoPos(viewChairId));
            });
        }


        /// <summary>
        /// 点击确定按钮事件
        /// </summary>
        protected virtual void OnConfirm()
        {
            var cards = GetSelectedCards(Chair.Down);
            if (cards.Count < 3)
            {
                WLDebug.LogWarning("选择牌数不足，请继续选择");
                // TODO: 吐司提示
                return;
            }

            var values = GetSelfSelectedCardValues();

            (_gameData.playerSelf as MJGamePlayer).RemoveHandCardValues(values);

            _panel?.SetConfirmButtonActive(false);

            var ownHand = _handController.GetOwnHand();
            cards.ForEach(c => ownHand.Remove(c));
            ownHand.interactable = false;
            ownHand.Refresh();
            _tableController.StopTimer();

            ShowExchangeCardOnDesk(Chair.Down);

            stage.Send("SendChangeCards", values);
        }


        /// <summary>
        /// 移除换牌UI
        /// </summary>
        public void RemoveExchangePanel()
        {
            if (_panel != null && _panel.gameObject != null)
            {
                _panel.RemoveFromParent();
            }

            _panel = null;
        }


        /// <summary>
        /// 播放已选择动画
        /// </summary>
        public void PlaySelected()
        {
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                if (player == _gameData.playerSelf)
                {
                    return;
                }

                var viewChairId = stage.ToViewChairId(player.chairId);
                var handSet = _handController.GetHandSet(viewChairId);

                var selectedCards = new List<MJCard>();
                MJAnimationHelper.PlaySelectRandomHandCardAnimation(handSet, 3, selectedCards, () =>
                {
                    ShowExchangeCardOnDesk(viewChairId);
                    selectedCards.ForEach(c => handSet.Remove(c));
                    handSet.Refresh();
                });
            });
        }


        /// <summary>
        /// 播放换牌动画
        /// </summary>
        /// <param name="type"></param>
        public void PlayExchange(ExchangeCardType type)
        {
            _panel.ShowInfoChanging(type);

            _animNode.Play(type, () =>
            {
                RemoveExchangeAnimation();

                RemoveExchangePanel();
            });
        }


        /// <summary>
        /// 播放插入牌动画
        /// </summary>
        /// <param name="cardvalues"></param>
        public void PlayInsertCards(List<int> cardvalues)
        {
            _gameData.TraversePlayer<MJGamePlayer>(p =>
            {
                int viewChairId = stage.ToViewChairId(p.chairId);
                var handSet = _handController.GetHandSet(viewChairId);
                if (viewChairId == Chair.Down)
                {
                    MJAnimationHelper.PlayInsert3Cards(handSet, cardvalues);
                }
                else
                {
                    MJAnimationHelper.PlayInsert3Cards(handSet, new List<int>{ Card.Rear, Card.Rear, Card.Rear });
                }
            });
        }


        /// <summary>
        /// 开始换牌倒计时
        /// </summary>
        /// <param name="timeout"></param>
        public void StartExchangeTimer(int timeout = -1)
        {
            timeout = timeout == -1 ? _gameData.trustTimeout : timeout;

            Action<int> action = elapsed =>
            {
                if (_gameData.isFriendRoom)
                {
                    return;
                }

                if (elapsed == timeout)
                {
                    SelectRecommandCards();

                    OnConfirm();
                }
            };
            _tableController.StartTimer(timeout, action);
        }


        /// <summary>
        /// 清除自家所有已选中牌
        /// </summary>
        public void ClearSelfSelectedCards()
        {
            GetSelectedCards(Chair.Down).Clear();
        }


        /// <summary>
        /// 添加自家选择牌
        /// </summary>
        /// <param name="card"></param>
        public bool AddSelfSelectedCard(MJCard card)
        {
            var cards = GetSelectedCards(Chair.Down);

            if (cards.Count == 3)
            {
                return false;
            }

            if (cards.Contains(card))
            {
                return false;
            }

            if (card.cardValue == Card.Zhong)
            {
                return false;
            }

            cards.Add(card);

            return true;
        }


        /// <summary>
        /// 选择推荐牌
        /// </summary>
        public virtual void SelectRecommandCards()
        {
            var handSet = _handController.GetHandSet(Chair.Down);

            var values = (_gameData.playerSelf as MJGamePlayer).handCardValues;
            var characters = values.Fetch(c => Card.GetCardColorValue(c) == Card.ColorWan);
            var banboos = values.Fetch(c => Card.GetCardColorValue(c) == Card.ColorTiao);
            var dots = values.Fetch(c => Card.GetCardColorValue(c) == Card.ColorBing);

            var groups = new List<List<int>> { characters, banboos, dots };
            groups.Sort((a, b) => a.Count - b.Count);

            var selectedValues = new List<int>();
            while (selectedValues.Count < 3)
            {
                var group = groups[0];
                if (group.Count == 0)
                {
                    groups.RemoveAt(0);
                }
                else
                {
                    var cardValue = group[0];
                    selectedValues.Add(cardValue);
                    group.RemoveAt(0);
                }
            }

            var selectedCards = GetSelectedCards(Chair.Down);
            MJAnimationHelper.PlaySelectHandCardAnimation(handSet, selectedValues, selectedCards);
        }


        /// <summary>
        /// 获取自家当前选择的牌值数组
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelfSelectedCardValues()
        {
            var cards = GetSelectedCards(Chair.Down);
            var values = new List<int>();
            cards.ForEach(c => values.Add(c.cardValue));
            return values;
        }


        protected virtual Vector2 GetInfoPos(int viewChairId)
        {
            var offset = new List<Vector2>
            {
                new Vector2(200, 20),
                new Vector2(-200, 20),
                new Vector2(-200, 20),
                new Vector2(200, 20)
            }[viewChairId];

            return _avatarController.GetAvatarPosition(viewChairId) + offset;
        }


        protected virtual MJExchangePanel OnCreateExchangeCardPanel()
        {
            var panel = new MJExchangePanel();

            panel.AddTo(WDirector.GetRootLayer(), MJZorder.ExchangePanel);

            return panel;
        }


        protected virtual MJExchangeAnimation OnCreateExchangeAnimation()
        {
            return new MJExchangeAnimation();
        }


        private List<MJCard> GetSelectedCards(int viewChairId)
        {
            if (_selectedCardDic.ContainsKey(viewChairId))
            {
                return _selectedCardDic[viewChairId];
            }

            var cards = new List<MJCard>();
            _selectedCardDic[viewChairId] = cards;
            return cards;
        }
    }
}
