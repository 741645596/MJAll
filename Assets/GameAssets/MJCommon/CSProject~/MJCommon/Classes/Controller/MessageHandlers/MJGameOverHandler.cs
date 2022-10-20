// @Author: tanjinhua
// @Date: 2021/5/10  10:40


using System.Collections.Generic;
using UnityEngine;
using WLHall.Game;

namespace MJCommon
{
    public class MJGameOverHandler : BaseGameController
    {
        private MJGameData _gameData;
        private MJBlankEndAnimation _blankEndAnimation;

        private MJHandController _handController;
        private MJTrustController _trustController;
        private MJTableController _tableController;
        private MJAvatarController _avatarController;
        private MJActionButtonController _actionButtonController;
        private MJActionEffectController _actionEffectController;
        private MJDeskCardController _deskCardController;
        private MJSpaceController _spaceController;
        private MJOutCardHandler _outCardHandler;
        private MJTingInfoHandler _tingInfoHandler;
        private MJFriendGameOverHandler _friendGameOverHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _handController = stage.GetController<MJHandController>();
            _trustController = stage.GetController<MJTrustController>();
            _tableController = stage.GetController<MJTableController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _actionButtonController = stage.GetController<MJActionButtonController>();
            _actionEffectController = stage.GetController<MJActionEffectController>();
            _deskCardController = stage.GetController<MJDeskCardController>();
            _spaceController = stage.GetController<MJSpaceController>();
            _outCardHandler = stage.GetController<MJOutCardHandler>();
            _tingInfoHandler = stage.GetController<MJTingInfoHandler>();
            _friendGameOverHandler = stage.GetController<MJFriendGameOverHandler>();
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="isReconnect"></param>
        /// <param name="playerHandValues"></param>
        /// <param name="playerFuziDatas"></param>
        public virtual void UpdateData(bool isReconnect, List<List<int>> playerHandValues, List<List<FuziData>> playerFuziDatas)
        {
            _gameData.isReconnectGameEnd = isReconnect;

            _gameData.autoUpdatePlayerInfo = false;

            // 更新退出对话框开关标记
            UpdateExitDialogSwitch();

            // 更新玩家数据
            UpdatePlayerData(playerHandValues, playerFuziDatas);
        }


        /// <summary>
        /// 更新完家手牌、副子数据
        /// </summary>
        /// <param name="playerHandValues"></param>
        /// <param name="playerFuziDatas"></param>
        protected virtual void UpdatePlayerData(List<List<int>> playerHandValues, List<List<FuziData>> playerFuziDatas)
        {
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                // 更新玩家手牌数据
                List<int> handValues = playerHandValues[player.chairId];
                handValues.RemoveByCondition(v => v == Card.Invalid);
                player.ReloadHandCardValues(handValues);
                bool includeLast = handValues.Count % 3 != 2;
                player.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable, includeLast);

                // 更新玩家副子数据
                player.ClearFuziDatas();
                List<FuziData> fuziDatas = playerFuziDatas[player.chairId];
                for (int i = 0; i < fuziDatas.Count; i++)
                {
                    FuziData fuziData = fuziDatas[i];

                    if (fuziData.isValid)
                    {
                        player.AddFuziData(fuziData);
                    }
                }

                // 更新玩家准备状态
                player.SetReady(false);
            });
        }


        /// <summary>
        /// 获取有效胡牌玩家座位号列表
        /// </summary>
        /// <param name="huFlags"></param>
        /// <returns></returns>
        public List<ushort> GetValidHuChairIds(List<bool> huFlags)
        {
            List<ushort> result = new List<ushort>();

            for (ushort chairId = 0; chairId < huFlags.Count; chairId++)
            {
                if (huFlags[chairId])
                {
                    result.Add(chairId);
                }
            }

            return result;
        }


        /// <summary>
        /// 获取有效胡牌类型列表
        /// </summary>
        /// <param name="huFlags"></param>
        /// <param name="allHuTypes"></param>
        /// <returns></returns>
        public List<long> GetValidHuTypes(List<bool> huFlags, List<long> allHuTypes)
        {
            List<long> result = new List<long>();

            for (ushort chairId = 0; chairId < huFlags.Count; chairId++)
            {
                if (huFlags[chairId])
                {
                    result.Add(allHuTypes[chairId]);
                }
            }

            return result;
        }


        /// <summary>
        /// 更新玩家朋友场分数
        /// </summary>
        /// <param name="score"></param>
        public virtual void UpdateFriendScore(List<long> friendScores)
        {
            if (!_gameData.isFriendRoom)
            {
                return;
            }

            if (_gameData.isReconnectGameEnd)
            {
                return;
            }

            _gameData.TraversePlayer<MJGamePlayer>(player => player.friendGameScore += friendScores[player.chairId]);
        }


        /// <summary>
        /// 对局结束，清理场景
        /// </summary>
        public virtual void EnqueueGameOver()
        {
            // 设置自家手牌不可交互
            _handController.GetOwnHand().interactable = false;

            stage.animationQueue.Enqueue(0, () =>
            {
                // 清除所有玩家托管UI
                _trustController.ExitTrust(false);

                // 停止倒计时
                _tableController.StopTurn();

                // 隐藏头像光效
                _avatarController.HideAllLightEffect();

                // 清除动作按钮及数据
                _actionButtonController.Clear();

                // 清除胡牌提示、听牌按钮、自动出牌开关
                _tingInfoHandler.Clear();
            });
        }


        /// <summary>
        /// 播放荒庄(解散动画)
        /// </summary>
        /// <param name="endType"></param>
        /// <param name="duration"></param>
        public virtual void EnqueueBlankEnd(ushort endType, float duration)
        {
            if (endType != EndType.HuangZhuang)
            {
                return;
            }

            stage.animationQueue.Enqueue(duration, () =>
            {
                // TODO:
            });
        }


        /// <summary>
        /// 尝试删除点炮玩家最后一张出牌
        /// </summary>
        /// <param name="endType"></param>
        /// <param name="dianpaoChairId"></param>
        public virtual void EnqueueUpdateDeskCard(ushort endType, ushort dianpaoChairId)
        {
            if (endType != EndType.DianPao)
            {
                return;
            }

            if (dianpaoChairId == 0xFF)
            {
                return;
            }

            MJGamePlayer player = _gameData.GetPlayerByChairId(dianpaoChairId) as MJGamePlayer;

            player.RemoveLastOutCardValue();

            int viewChairId = stage.ToViewChairId(dianpaoChairId);

            stage.animationQueue.Enqueue(0, () => _deskCardController.Pop(viewChairId));
        }


        /// <summary>
        /// 播放玩家亮牌动画
        /// </summary>
        public virtual void EnqueueShowHand()
        {
            _gameData.TraversePlayer(p =>
            {
                if (p != _gameData.playerSelf)
                {
                    _outCardHandler.EnqueueReloadHandCard(p.chairId);
                }
            });


            stage.animationQueue.Enqueue(1f, () =>
            {
                var space = _spaceController.GetSpace();
                foreach(var pair in space.handRoot.GetMJHandSets())
                {
                    if (pair.Key != MJOrientation.Down)
                    {
                        pair.Value.Refresh(MJHandSet.State.Standing);

                        MJAnimationHelper.PlayShowHandCardAnimation(pair.Value);
                    }
                }
            });
        }

        /// <summary>
        /// 播放胡动作特效
        /// </summary>
        /// <param name="endType"></param>
        /// <param name="dianpaoChairId"></param>
        /// <param name="huChairIds"></param>
        /// <param name="huTypes"></param>
        public virtual void EnqueueActionEffects(ushort endType, ushort dianpaoChairId, List<ushort> huChairIds, List<long> huTypes)
        {
            if (endType != EndType.ZiMo && endType != EndType.DianPao)
            {
                return;
            }

            if (endType == EndType.DianPao && dianpaoChairId == Chair.Invalid)
            {
                return;
            }

            if (dianpaoChairId != Chair.Invalid)
            {
                EnqueueDianpaoActionEffect(dianpaoChairId);
            }

            int showType = endType == EndType.ZiMo ? ActionShowType.Zimo : ActionShowType.Hu;

            EnqueueHuActionEffect(showType, huChairIds, huTypes);
        }

        /// <summary>
        /// 播放胡特效
        /// </summary>
        /// <param name="endType"></param>
        /// <param name="dianpaoChairId"></param>
        /// <param name="huChairIds"></param>
        /// <param name="configs"></param>
        /// <param name="scores"></param>
        public virtual void EnqueueHuEffect(ushort endType, ushort dianpaoChairId, List<ushort> huChairIds, List<long> scores = null)
        {
            if (endType != EndType.ZiMo && endType != EndType.DianPao)
            {
                return;
            }

            if (endType == EndType.DianPao && dianpaoChairId == Chair.Invalid)
            {
                return;
            }

            stage.animationQueue.Enqueue(0f, () =>
            {
                PlayInstantScoreAnim(scores);

                //for (int i = 0; i < huChairIds.Count; i++)
                //{
                //    var chairId = huChairIds[i];
                //    var viewChairId = _client.ToViewChairId(chairId);
                //    var cfg = configs[i];
                //    if (viewChairId == Chair.Down)
                //    {
                //        PlaySelfHuEffect(cfg);
                //    }
                //    else
                //    {
                //        PlayOtherHuEffect(viewChairId, cfg);
                //    }

                //    if (cfg.hasCustomAudio)
                //    {
                //        int gender = _client.GetPlayerByChairId(chairId).gender;
                //        var key = gender == 1 ? cfg.audioKeyMale : cfg.audioKeyFemale;
                //        _audio.PlayEffect(cfg.audioAssetName, key);
                //    }

                //}
            });
        }


        /// <summary>
        /// 播放分数动画
        /// </summary>
        /// <param name="scores"></param>
        public virtual void PlayInstantScoreAnim(List<long> scores)
        {
            if (scores == null)
            {
                return;
            }

            for (int i = 0; i < scores.Count; i++)
            {
                long score = scores[i];
                if (score == 0)
                {
                    continue;
                }

                int viewChairId = stage.ToViewChairId(i);
                var node = new MJInstantScore(score);
                var actionEffectPanel = _actionEffectController.GetPanel();
                node.AddTo(actionEffectPanel);
                Vector2 ancohr = viewChairId % 3 == 0 ? new Vector2(0, 0.5f) : new Vector2(1, 0.5f);
                node.rectTransform.anchorMin = ancohr;
                node.rectTransform.anchorMax = ancohr;
                node.rectTransform.pivot = ancohr;

                var offset = viewChairId % 3 == 0 ? new Vector2(100, 0) : new Vector2(-100, 0);
                var avatarPos = _avatarController.GetAvatarPosition(viewChairId);
                node.rectTransform.SetPositionInZero(avatarPos + offset);

                node.Animate();
            }
        }


        /// <summary>
        /// 播放点炮动作特效
        /// </summary>
        /// <param name="dianpaoChairId"></param>
        protected virtual void EnqueueDianpaoActionEffect(int dianpaoChairId)
        {
            // TODO:
        }


        /// <summary>
        /// 播放自摸或胡动作特效
        /// </summary>
        /// <param name="showType"></param>
        /// <param name="huChairIds"></param>
        /// <param name="huTypes"></param>
        protected virtual void EnqueueHuActionEffect(int showType, List<ushort> huChairIds, List<long> huTypes)
        {
            stage.animationQueue.Enqueue(1.2f, () =>
            {
                for (int i = 0; i < huChairIds.Count; i++)
                {
                    _actionEffectController.PlayHuActionEffect(huChairIds[i], showType, huTypes[i]);
                }
            });
        }


        /// <summary>
        /// 更新点击退出时是否需要弹出对话框的开关标志
        /// </summary>
        protected virtual void UpdateExitDialogSwitch()
        {
            bool isFriendGameOver = _friendGameOverHandler.IsFriendGameOver();

            _gameData.showDialogOnExit = isFriendGameOver;
        }


        public override void OnChangeDesk()
        {
            base.OnChangeDesk();

            if (_blankEndAnimation != null && _blankEndAnimation.gameObject != null)
            {
                _blankEndAnimation.RemoveFromParent();
            }

            _blankEndAnimation = null;
        }


        public override void OnContinue()
        {
            base.OnContinue();

            if (_blankEndAnimation != null && _blankEndAnimation.gameObject != null)
            {
                _blankEndAnimation.RemoveFromParent();
            }

            _blankEndAnimation = null;
        }
    }
}
