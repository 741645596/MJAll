// @Author: tanjinhua
// @Date: 2021/5/2  19:46


using System.Collections.Generic;
using DG.Tweening;
using Unity.Widget;
using UnityEngine;
using WLHall.Game;

namespace MJCommon
{
    /// <summary>
    /// 游戏开始消息逻辑
    /// </summary>
    public class MJGameStartHandler : BaseGameController
    {
        private struct DealInfo
        {
            public int viewChairId;
            public List<MJCard> cards;
            public float delay;
        }

        private MJGameData _gameData;
        private WNode _gameStartAnim;

        private MJAvatarController _avatarController;
        private MJHandController _handController;
        private MJWallController _wallController;
        private MJTableController _tableController;
        private MJEnvController _envController;
        private MJSpaceController _spaceController;
        private MJAudioController _audioController;
        private MJSendCardHandler _sendCardHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _avatarController = stage.GetController<MJAvatarController>();
            _handController = stage.GetController<MJHandController>();
            _wallController = stage.GetController<MJWallController>();
            _tableController = stage.GetController<MJTableController>();
            _envController = stage.GetController<MJEnvController>();
            _audioController = stage.GetController<MJAudioController>();
            _sendCardHandler = stage.GetController<MJSendCardHandler>();
            _spaceController = stage.GetController<MJSpaceController>();
        }


        /// <summary>
        /// 初始化GameData
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="bankerChairId"></param>
        /// <param name="jokerCardValues">赖子牌值列表</param>
        public virtual void InitGameData(ushort msgId, int bankerChairId, bool jokerDiscardable, List<int> jokerCardValues = null)
        {
            _gameData.Initialize();

            _gameData.verifyingMsgId = msgId;

            _gameData.bankerChairId = bankerChairId;

            _gameData.jokerDiscardable = jokerDiscardable;

            if (jokerCardValues != null)
            {
                _gameData.jokerCardValues = jokerCardValues;
            }

            _gameData.TraversePlayer<MJGamePlayer>(p => p.Initialize());
        }


        /// <summary>
        /// 初始化玩家手牌数据
        /// </summary>
        /// <param name="handCardValues"></param>
        /// <param name="bankerChairId"></param>
        public virtual void InitHandCardValues(List<int> handCardValues, int bankerChairId)
        {
            handCardValues = new List<int>(handCardValues);

            handCardValues.RemoveByCondition(v => v == Card.Invalid);

            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            int selfChairId = playerSelf.chairId;

            bool selfIsBanker = selfChairId == bankerChairId;

            // 服务器bug，自家不是庄家，需要删除最后一张牌
            if (!selfIsBanker && handCardValues.Count == _gameData.maxHandCardCount)
            {
                handCardValues.RemoveAt(handCardValues.Count - 1);
            }

            playerSelf.AddHandCardValues(handCardValues);

            playerSelf.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable);

            int playerCount = _gameData.maxPlayerCount;

            for (int chairId = 0; chairId < playerCount; chairId++)
            {
                if (chairId == selfChairId)
                {
                    continue;
                }

                MJGamePlayer player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;

                int sendCount = chairId == bankerChairId ? _gameData.maxHandCardCount : _gameData.maxHandCardCount - 1;

                player.AddHandCardValues(MJUtils.FakeHandCardValues(sendCount));
            }
        }


        /// <summary>
        /// 更加朋友场局数
        /// </summary>
        public virtual void IncreaseFriendGameCount()
        {
            if (!_gameData.isFriendRoom)
            {
                return;
            }

            _gameData.friendGameInfo.currentGameCount++;
        }


        /// <summary>
        /// 播放游戏开始一系列动画
        /// </summary>
        public virtual void EnqueueGameStart()
        {
            // 显示庄家
            int viewChairId = stage.ToViewChairId(_gameData.bankerChairId);
            _avatarController.ShowBankerIcon(viewChairId);

            // 初始化牌墙
            _wallController.InitializeWalls(false);

            // 设置自家手牌不可交互
            _handController.GetOwnHand().interactable = false;

            // 播放“对局开始”动画
            stage.animationQueue.Enqueue(1f, () =>
            {
                _envController.PlayZoomOut();

                PlayGameStartAnimation();
            });
        
            // 播放骰子、牌墙升降机动画
            stage.animationQueue.Enqueue(1.3f, () =>
            {
                if (_gameData.diceUsage != DiceUsage.Hidden)
                {
                    _tableController.PlayDice();
                }
                var space = _spaceController.GetSpace();
                MJAnimationHelper.PlayLiftAnimation(space);
            });

            // 发牌动画
            stage.animationQueue.Enqueue(1.5f, () =>
            {
                ReloadHandCards();

                PlayDealAnimation();
            });
        }


        public void PlayDealAnimation()
        {
            _handController.HideAllCards();
            _handController.ShuffleCards(Chair.Down);

            var dealInfos = PrepareDealInfos();
            for (var i = 0; i < dealInfos.Count; i++)
            {
                var info = dealInfos[i];
                var step = DOTween.Sequence();
                step.AppendInterval(info.delay);
                step.AppendCallback(() =>
                {
                    if (info.viewChairId == Chair.Down)
                    {
                        info.cards.ForEach(c =>
                        {
                            var deal = DOTween.Sequence();
                            deal.AppendCallback(() =>
                            {
                                c.body.gameObject.transform.localEulerAngles = new Vector3(-180, 0, 0);
                                c.gameObject.SetActive(true);
                            });
                            deal.Append(c.body.gameObject.transform.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(-90, 0, 0)), 0.3f));
                            deal.SetTarget(c.gameObject);
                            deal.SetLink(c.gameObject);
                            deal.SetAutoKill(true);
                            deal.Play();
                        });
                        _audioController.PlayDealCard();
                    }
                    else
                    {
                        info.cards.ForEach(c => c.gameObject.SetActive(true));
                    }
                    _wallController.TakeMultiple(info.cards.Count);

                    //TODO: 更新剩余牌数ui
                });
                if (i == dealInfos.Count - 1)
                {
                    step.AppendInterval(0.3f);
                    var ownHand = _handController.GetOwnHand();
                    step.AppendCallback(() => MJAnimationHelper.PlaySortHandCardAnimation(ownHand, () =>
                    {
                        ownHand.Sort(null, false);
                        ownHand.RefreshX();
                        ownHand.interactable = true;
                    }));
                }
                var handSet = _handController.GetHandSet(info.viewChairId);
                step.SetTarget(handSet.gameObject);
                step.SetLink(handSet.gameObject);
                step.SetAutoKill(true);
                step.Play();
            }
        }


        protected void ReloadHandCards()
        {
            _gameData.TraversePlayer(p =>
            {
                MJGamePlayer player = p as MJGamePlayer;

                int viewChairId = stage.ToViewChairId(player.chairId);

                _handController.Reload(viewChairId, player.handCardValues);
            });
        }


        /// <summary>
        /// 开始出牌庄家倒计时
        /// </summary>
        public virtual void EnqueueBankerTimer(int timeout = -1)
        {
            int bankerChairId = _gameData.bankerChairId;

            timeout = timeout == -1 ? _gameData.trustTimeout : timeout;

            _sendCardHandler.EnqueueSendCardTimer(bankerChairId, timeout);
        }


        /// <summary>
        /// 移除对局开始动画
        /// </summary>
        public void RemoveGameStartAnimation()
        {
            if (_gameStartAnim != null && _gameStartAnim.gameObject != null)
            {
                _gameStartAnim.RemoveFromParent();
            }

            _gameStartAnim = null;
        }


        /// <summary>
        /// 播放对局开始动画
        /// </summary>
        public void PlayGameStartAnimation()
        {
            RemoveGameStartAnimation();

            _gameStartAnim = OnCreateGameStartAnimation();

            _gameStartAnim.DelayInvoke(3f, () => RemoveGameStartAnimation());
        }


        /// <summary>
        /// 创建对局开始动画节点，并添加到场景、设置布局
        /// </summary>
        /// <returns></returns>
        protected virtual WNode OnCreateGameStartAnimation()
        {
            var anim = WNode.Create("MJCommon/MJ/mj_ui_effe_tishi", "duijukaishi_x_01_02.prefab");

            anim.AddTo(WDirector.GetRootLayer());

            anim.Layout(layout.center, new Vector2(0, 40));

            return anim;
        }


        private List<DealInfo> PrepareDealInfos()
        {
            int stepCount = 4;
            float stepDelay = 0.04f;

            var result = new List<DealInfo>();

            var currentIdx = 0;
            var currentDelay = 0f;
            var viewChairIds = new List<int>();
            var cardLists = GetOrderedCardLists(viewChairIds);
            while(cardLists.Count > 0)
            {
                currentIdx %= cardLists.Count;
                var list = cardLists[currentIdx];
                if (list.Count == 0)
                {
                    cardLists.RemoveAt(currentIdx);
                    continue;
                }

                var info = new DealInfo
                {
                    viewChairId = viewChairIds[currentIdx],
                    cards = new List<MJCard>(),
                    delay = currentDelay
                };
                int count = Mathf.Min(list.Count, stepCount);
                for (int i = 0; i < count; i++)
                {
                    info.cards.Add(list[i]);
                }
                list.RemoveAll(c => info.cards.Contains(c));
                result.Add(info);

                currentIdx++;
                currentDelay += stepDelay;
            }

            return result;
        }


        private List<List<MJCard>> GetOrderedCardLists(List<int> outViewChairIds)
        {
            var startViewChaidId = stage.ToViewChairId(_gameData.bankerChairId);

            var orderedHandSets = new List<List<MJCard>>();

            for (int i = startViewChaidId; i < startViewChaidId + 4; i++)
            {
                var viewChairId = i % 4;
                var handSet = _handController.GetHandSet(viewChairId);
                if (handSet.count == 0)
                {
                    continue;
                }
                var cards = handSet.GetCards().Clone();
                orderedHandSets.Add(cards);
                outViewChairIds.Add(viewChairId);
            }

            return orderedHandSets;
        }
    }
}
