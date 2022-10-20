// @Author: tanjinhua
// @Date: 2021/5/8  14:00


using System.Collections.Generic;
using Common;
using WLCore;
using WLHall.Game;

namespace MJCommon
{
    public class MJReconnectHandler : BaseGameController
    {
        private MJGameData _gameData;

        private MJTableController _tableController;
        private MJTrustController _trustController;
        private MJHintsController _hintsController;
        private MJAvatarController _avatarController;
        private MJHandController _handController;
        private MJMeldController _meldController;
        private MJDeskCardController _deskCardController;
        private MJWinCardController _winCardController;
        private MJWallController _wallController;
        private MJSendCardHandler _sendCardHandler;
        private MJOutCardHandler _outCardHandler;
        private MJActionResultHandler _actionResultHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _tableController = stage.GetController<MJTableController>();
            _trustController = stage.GetController<MJTrustController>();
            _hintsController = stage.GetController<MJHintsController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _handController = stage.GetController<MJHandController>();
            _meldController = stage.GetController<MJMeldController>();
            _deskCardController = stage.GetController<MJDeskCardController>();
            _winCardController = stage.GetController<MJWinCardController>();
            _sendCardHandler = stage.GetController<MJSendCardHandler>();
            _wallController = stage.GetController<MJWallController>();
            _outCardHandler = stage.GetController<MJOutCardHandler>();
            _actionResultHandler = stage.GetController<MJActionResultHandler>();
        }


        /// <summary>
        /// 重置游戏数据
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="bankerChairId"></param>
        /// <param name="gameState"></param>
        /// <param name="sendedCardCount"></param>
        /// <param name="sendedKongCardCount"></param>
        public virtual void ResetGameData(ushort msgId, ushort bankerChairId, int gameState,
            int sendedCardCount, int sendedKongCardCount, bool jokerDiscardable, List<int> jokerCardValues = null)
        {
            _gameData.Initialize();

            _gameData.verifyingMsgId = msgId;

            _gameData.bankerChairId = bankerChairId;

            _gameData.gameState = gameState;

            _gameData.sendedCardCount = sendedCardCount;

            _gameData.sendedKongCardCount = sendedKongCardCount;

            _gameData.jokerDiscardable = jokerDiscardable;

            if (jokerCardValues != null)
            {
                _gameData.jokerCardValues = jokerCardValues;
            }
        }


        /// <summary>
        /// 重置玩家托管、听牌标记
        /// </summary>
        /// <param name="trustFlags"></param>
        /// <param name="readyHandFlags"></param>
        public virtual void ResetPlayerFlags(List<int> trustFlags, List<int> readyHandFlags = null)
        {
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                player.isTrust = trustFlags[player.chairId] == 1;

                if (readyHandFlags != null && readyHandFlags.Count == _gameData.maxPlayerCount)
                {
                    player.isReadyHand = readyHandFlags[player.chairId] == 1;
                }
            });
        }


        /// <summary>
        /// 重置玩家手牌数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="readCounts"></param>
        public virtual void ResetHandCardValues(MsgHeader msg, List<int> readCounts, uint lastEventType)
        {
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                player.ClearHandCardValues();
                bool isSelf = player == _gameData.playerSelf;
                int readCount = readCounts[player.chairId];
                if (isSelf)
                {
                    for (int i = 0; i < readCount; i++)
                    {
                        int cardValue = msg.ReadByte();

                        if (cardValue != Card.Invalid)
                        {
                            player.AddHandCardValue(cardValue);
                        }
                    }
                }
                else
                {
                    player.AddHandCardValues(MJUtils.FakeHandCardValues(readCount));
                }
            });

            // 自己手牌数据排序
            SortSelfHandCardValues(lastEventType);
        }


        /// <summary>
        /// 重置玩家出牌数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="readCounts"></param>
        public virtual void ResetOutCardValues(MsgHeader msg, List<int> readCounts)
        {
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                player.ClearOutCardValues();

                int readCount = readCounts[player.chairId];

                for (int i = 0; i < readCount; i++)
                {
                    int cardValue = msg.ReadByte();

                    if (cardValue != Card.Invalid)
                    {
                        player.AddOutCardValue(cardValue);
                    }
                }
            });
        }


        /// <summary>
        /// 重置玩家副子数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="readCounts"></param>
        public virtual void ResetFuziDatas(MsgHeader msg, List<int> readCounts)
        {
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                player.ClearFuziDatas();

                int readCount = readCounts[player.chairId];

                for (int i = 0; i < readCount; i++)
                {
                    player.AddFuziData(FuziData.From(msg));
                }
            });
        }


        /// <summary>
        /// 重置胡牌数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="readCounts"></param>
        public virtual void ResetWinCardValues(MsgHeader msg, List<int> readCounts)
        {
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                player.ClearWinCardValues();

                int readCount = readCounts[player.chairId];

                for (int i = 0; i < readCount; i++)
                {
                    player.AddWinCardValue(msg.ReadByte());
                }
            });
        }


        /// <summary>
        /// 重置游戏
        /// </summary>
        /// <param name="lastEventType"></param>
        /// <param name="lastEventChairId"></param>
        public virtual void ResetGame(uint lastEventType, ushort lastEventChairId)
        {
            // 清空消息队列
            stage.animationQueue.Clear();

            // 移除防作弊动画
            _tableController.RemoveMatchingAnimation();

            // 取消托管状态
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;
            playerSelf.isTrust = false;
            _trustController.ExitTrust(true, playerSelf);

            // 取消自动出牌勾选
            _hintsController.CancelAuto();

            // 重置场景
            ResetGameScene(lastEventType, lastEventChairId);
        }


        /// <summary>
        /// 开启断线重连倒计时
        /// </summary>
        /// <param name="lastEventType"></param>
        /// <param name="lastEventChairId"></param>
        public virtual void StartReconnectTimer(uint lastEventType, ushort lastEventChairId)
        {
            int viewChairId = stage.ToViewChairId(lastEventChairId);

            if  (lastEventType == 0) // 上个事件如果是发牌
            {
                _sendCardHandler.EnqueueSendCardTimer(lastEventChairId, _gameData.trustTimeout);
            }
            else if (lastEventType == 1) // 上个事件如果是出牌
            {
                if (_gameData.HasAction())
                {
                    _outCardHandler.EnqueueSelfActionEventTimer(_gameData.trustTimeout);
                }
                else
                {
                    var nextViewChairId = stage.NextViewChairId(viewChairId);
                    _avatarController.SwitchLightEffect(nextViewChairId);
                    _tableController.PlayTurn(nextViewChairId, _gameData.trustTimeout, null);
                }
            }
            else if (lastEventType == 2) // 上个事件如果是动作
            {
                _actionResultHandler.EnqueueActionResultTimer(lastEventChairId);
            }
        }


        /// <summary>
        /// 重置游戏场景
        /// </summary>
        /// <param name="lastEventType"></param>
        /// <param name="lastEventChairId"></param>
        protected virtual void ResetGameScene(uint lastEventType, ushort lastEventChairId)
        {
            // 刷新手牌、出牌、副子、花牌、头像等
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                int viewChairId = stage.ToViewChairId(player.chairId);

                var meldDatas = _meldController.ToMeldDatas(player.chairId, player.fuziDatas);
                _meldController.Reload(viewChairId, meldDatas);

                _handController.Reload(viewChairId, player.handCardValues);

                _deskCardController.Reload(viewChairId, player.outCardValues);

                _winCardController.Reload(viewChairId, player.winCardValues);

                MJAvatar avatar = _avatarController.GetAvatar<MJAvatar>(viewChairId);

                ResetPlayerAvatar(player, avatar, viewChairId);
            });

            // 设置自家手牌可交互
            _handController.GetOwnHand().interactable = true;

            // 刷新牌墙
            _wallController.InitializeWalls(true);
            _wallController.TakeMultiple(_gameData.sendedCardCount);
            _wallController.TakeMultiple(_gameData.sendedKongCardCount, true);

            // 刷新出牌指示器
            if (lastEventType == 1) // 如果上个事件是出牌
            {
                _deskCardController.ShowPointer(stage.ToViewChairId(lastEventChairId));
            }
            else
            {
                _deskCardController.HidePointer();
            }

            _tableController.SetRemainCardCount(_gameData.remainCardCount);


            // TODO: 刷新GPS显示
        }


        /// <summary>
        /// 重置玩家头像
        /// </summary>
        /// <param name="player"></param>
        /// <param name="avatar"></param>
        protected virtual void ResetPlayerAvatar(MJGamePlayer player, MJAvatar avatar, int viewChairId)
        {
            avatar.Reset();

            if (player.chairId == _gameData.bankerChairId)
            {
                _avatarController.ShowBankerIcon(viewChairId);
            }

            if (player.isReadyHand)
            {
                _avatarController.ShowReadyHandIcon(viewChairId);
            }

            if (player.isTrust)
            {
                avatar.ShowTrust();
            }

            if (_gameData.isFriendRoom)
            {
                bool isLeft = player.offlineState == OffineState.Offline || player.offlineState == OffineState.Calling;

                if (isLeft)
                {
                    avatar.ShowOfflineTime(player.currentOfflineTime);
                }
                else
                {
                    avatar.HideOffline();
                }
            }
        }


        /// <summary>
        /// 自家手牌数据排序
        /// </summary>
        /// <param name="lastEventType"></param>
        public virtual void SortSelfHandCardValues(uint lastEventType)
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            if (lastEventType == 2) // 上一个事件如果是动作事件
            {
                playerSelf.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable);

                return;
            }

            bool includeLastCard = playerSelf.handCardCount % 3 != 2;

            playerSelf.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable, includeLastCard);
        }
    }
}
