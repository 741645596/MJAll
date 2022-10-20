// @Author: tanjinhua
// @Date: 2021/9/8  11:02

using Common;
using MJCommon;
using System.Collections.Generic;

namespace NS_XLHZ
{
    public class XLHZWinHandler : BaseController
    {

        private ClientXLHZ _client;
        private XLHZGameData _gameData;

        public override void OnSceneLoaded()
        {
            _client = client as ClientXLHZ;

            _gameData = client.gameData as XLHZGameData;
        }


        /// <summary>
        /// 更新玩家数据
        /// </summary>
        /// <param name="huTypes"></param>
        /// <param name="cardValue"></param>
        public void UpdatePlayerData(List<long> huTypes, int cardValue, int operType, ushort providerChairId, List<long> scores)
        {
            UpdateWinCardValue(huTypes, cardValue);

            UpdateProviderDatas(operType, huTypes, providerChairId, cardValue);

            UpdateHandCardValues(operType, huTypes, cardValue);

            UpdateUnselectableCardValues(huTypes);

            UpdatePlayerScore(scores);
        }


        /// <summary>
        /// 更新一炮多响信息
        /// </summary>
        /// <param name="operType"></param>
        /// <param name="huTypes"></param>
        /// <param name="scores"></param>
        /// <param name="cardValue"></param>
        public void UpdateMultiWinInfo(int operType, List<long> huTypes, List<long> scores, int cardValue)
        {
            if (operType == XLHZOperType.ZiMo)
            {
                return;
            }

            int count = 0;
            for (int i = 0; i < 4; i++)
            {
                if (huTypes[i] != XLHZHuType.None && scores[i] > 0)
                {
                    count++;
                }
            }
            if (count > 0)
            {
                _gameData.multiWinInfos.Add(new XLHZGameData.MultiWinInfo
                {
                    cardValue = cardValue,
                    winPlayerCount = count
                });
            }
        }


        /// <summary>
        /// 更新流水数据
        /// </summary>
        public void UpdateWaterflow()
        {
            // TODO:
        }


        /// <summary>
        /// 播放胡牌特效
        /// </summary>
        /// <param name="operType"></param>
        /// <param name="providerChairId"></param>
        /// <param name="huTypes"></param>
        /// <param name="scores"></param>
        public void EnqueueWinEffect(int operType, ushort providerChairId, List<long> huTypes, List<long> scores)
        {
            if (operType != XLHZOperType.ZiMo && operType != XLHZOperType.DianPao)
            {
                return;
            }

            ushort endType = operType == XLHZOperType.ZiMo ? EndType.ZiMo : EndType.DianPao;

            List<ushort> huChairIds = new List<ushort>();
            List<long> validHuTypes = new List<long>();
            for (ushort chairId = 0; chairId < 4; chairId++)
            {
                var huType = huTypes[chairId];
                if (huType != XLHZHuType.None)
                {
                    huChairIds.Add(chairId);
                    validHuTypes.Add(huType);
                }
            }

            var dianpaoChairId = providerChairId;
            if (huChairIds.Count == 1 && huChairIds[0] == providerChairId)
            {
                dianpaoChairId = Chair.Invalid;
            }

            _client.gameOverHandler.EnqueueActionEffects(endType, dianpaoChairId, huChairIds, huTypes);

            //// 先用临时配置播放胡牌特效
            //List<MJHuEffectConfig> configs = new List<MJHuEffectConfig>();
            //for (int i = 0; i < huChairIds.Count; i++)
            //{
            //    configs.Add(new MJHuEffectConfig
            //    {
            //        textAssetName = "xlhz/main/hu_text",
            //        textImageKeys = new List<string>
            //        {
            //            "style_0/ping.png",
            //            "style_0/hu.png"
            //        }
            //    });
            //}

            _client.gameOverHandler.EnqueueHuEffect(endType, dianpaoChairId, huChairIds, scores);
        }


        /// <summary>
        /// 刷新头像分数
        /// </summary>
        public void EnqueueRefreshAvatarScore()
        {
            _client.animationQueue.Enqueue(0, () =>
            {
                _client.TraversePlayer(p =>
                {
                    _client.avatarController.UpdateScore(_client.ToViewChairId(p.chairId));
                });
            });
        }


        /// <summary>
        /// 播放添加胡牌动画
        /// </summary>
        /// <param name="huTypes"></param>
        /// <param name="cardValue"></param>
        public void EnqueueWinCardAnimation(List<long> huTypes, int cardValue)
        {
            _client.animationQueue.Enqueue(1.7f, () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    if (huTypes[i] != XLHZHuType.None)
                    {
                        int viewChairId = _client.ToViewChairId(i);
                        _client.winCardController.PlayWinCardAnimation(viewChairId, cardValue);
                    }
                }
            });

            _client.animationQueue.Enqueue(0, () => _client.envController.PlayShake());
        }


        /// <summary>
        /// 刷新点炮玩家副子出牌节点
        /// </summary>
        /// <param name="operType"></param>
        /// <param name="huTypes"></param>
        /// <param name="providerChairId"></param>
        public void EnqueueReloadProviderMeldDeskCard(int operType, List<long> huTypes, ushort providerChairId)
        {
            if (operType != XLHZOperType.DianPao)
            {
                return;
            }

            XLHZGamePlayer provider = _client.GetPlayerByChairId(providerChairId) as XLHZGamePlayer;

            // 如果是抢杠胡
            if (huTypes.GetIndexByCondition(t => (t & XLHZHuType.QiangGangHu) != 0) != -1)
            {
                _client.actionResultHandler.EnqueueReloadMeldSet(providerChairId);
            }
            else
            {
                int viewChairId = _client.ToViewChairId(providerChairId);
                _client.animationQueue.Enqueue(0, () => _client.deskCardController.Pop(viewChairId));
            }
        }


        /// <summary>
        /// 刷新手牌节点
        /// </summary>
        /// <param name="operType"></param>
        /// <param name="huTypes"></param>
        public void EnqueueReloadHandCards(int operType, List<long> huTypes)
        {
            if (operType != XLHZOperType.ZiMo)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                if (huTypes[i] == XLHZHuType.None)
                {
                    continue;
                }

                _client.outCardHandler.EnqueueReloadHandCard(i);
            }
        }


        /// <summary>
        /// 盖牌
        /// </summary>
        public void EnqueueRefreshHandCards()
        {
            _client.animationQueue.Enqueue(0, () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    _client.handController.GetHandSet(i)?.Refresh();
                }
            });
        }


        public void UpdateWinCardValue(List<long> huTypes, int cardValue)
        {
            for (int i = 0; i < 4; i++)
            {
                if (huTypes[i] != XLHZHuType.None)
                {
                    var player = _client.GetPlayerByChairId(i) as XLHZGamePlayer;
                    player.AddWinCardValue(cardValue);
                }
            }
        }

        private void UpdateProviderDatas(int operType, List<long> huTypes, ushort providerChairId, int cardValue)
        {
            // 如果不是点炮则返回
            if (operType != XLHZOperType.DianPao)
            {
                return;
            }

            XLHZGamePlayer provider = _client.GetPlayerByChairId(providerChairId) as XLHZGamePlayer;

            // 如果是抢杠胡
            if (huTypes.GetIndexByCondition(t => (t & XLHZHuType.QiangGangHu) != 0) != -1)
            {
                var fuziDatas = provider.fuziDatas;
                int index = fuziDatas.GetIndexByCondition(d => d.operateCard == cardValue);
                if (index == -1)
                {
                    return;
                }

                var fuziData = fuziDatas[index];
                fuziData.cardValues[3] = Card.Invalid;
                fuziData.cardCounts[3] = 0;
                fuziData.weaveKind1 = XLHZActionType.Peng;
                provider.ReplaceFuziData(index, fuziData);
            }
            else
            {
                provider.RemoveLastOutCardValue();
            }
        }

        private void UpdateHandCardValues(int operType, List<long> huTypes, int cardValue)
        {
            if (operType != XLHZOperType.ZiMo)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                if (huTypes[i] == XLHZHuType.None)
                {
                    continue;
                }

                XLHZGamePlayer player = _client.GetPlayerByChairId(i) as XLHZGamePlayer;

                if (player.HandCardValueExists(cardValue))
                {
                    player.RemoveHandCardValue(cardValue, true);
                }
                else
                {
                    player.RemoveHandCardValueByIndex(player.handCardCount - 1);
                }
            }
        }

        private void UpdateUnselectableCardValues(List<long> huTypes)
        {
            XLHZGamePlayer playerSelf = _client.playerSelf as XLHZGamePlayer;

            if (huTypes[playerSelf.chairId] != XLHZHuType.None && playerSelf.winCardCount > 0)
            {
                playerSelf.unselectableCardValues = playerSelf.handCardValues;
            }
        }

        private void UpdatePlayerScore(List<long> scores)
        {
            if (_client.isFriendRoom && _gameData.showLiuShui)
            {
                _client.TraversePlayer<MJGamePlayer>(p => p.friendGameScore = scores[p.chairId]);
            }

            _client.TraversePlayer(p => p.SetMoney(scores[p.chairId]));
        }

    }
}
