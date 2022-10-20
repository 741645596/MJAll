// @Author: tanjinhua
// @Date: 2021/8/18  18:18


using WLHall;
using MJCommon;
using System.Collections;
using System.Collections.Generic;
using WLCore;
using Common;

namespace NS_XLHZ
{
    public partial class ClientXLHZ
    {
        protected override void RegisterRecvInterface()
        {
            base.RegisterRecvInterface();

            // 消息ID冲突重新注册
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_GAME_CHAT_NOTIFY, RecvChatText);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_CHANGE_DESK, RecvChangeDeskNotic);
            RegisterMessageProcessor(XLHZMessages.MsgID_S2C_Offline_CountDown, RecvPlayerOfflineCountdown);
            RegisterMessageProcessor(XLHZMessages.MsgID_S2c_All_Offline_CountDown, RecvPlayersOfflineCountdown);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_OUTCARD_BROADCAST, RecvOutCardBroadcast);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_SENDCARD_BROADCAST, RecvSendCardBroadcast);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_AUTOPLAY_BROADCAST, RecvAutoPlayBroadcast);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_ACTION_BROADCAST, RecvActionResultBroadcast);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_FRIENDROOM_DISSOLUTION, RecvFriendDissolutionBroadcast);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_FRIENDROOM_DISSOLUTION_RESULT, RecvFriendDissolutionResult);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_CeJu, RecvGPS);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_Record, RecvVoiceChat);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_FRIENDROOM_LIXIAN, RecvFriendLiXian);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_FRIENDROOM_LIXIAN_CURRENT, RecvFriendLiXianCurrent);
            RegisterMessageProcessor(XLHZMessages.MsgID_S2C_KickPlayerCause, RecvKickCause);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_BIAOQING, RecvBiaoQingBroadcast);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_TEST_SIGNAL, RecvTestSignal);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_READY_REQUEST_BROADCAST, RecvReadyBroadcast);
            RegisterMessageProcessor(XLHZMessages.MsgID_S2C_Override_Money_Chairs, RecvPlayersScoreLimitFlag);
            RegisterMessageProcessor(XLHZMessages.MSG_S2S_HUINFO_BROADCAST, RecvHuInfoMultiple);
            RegisterMessageProcessor(XLHZMessages.MsgID_S2C_TingInfo, RecvTingInfoMultiple);


            // 自定义消息处理注册
            RegisterMessageProcessor(XLHZMessages.MsgID_S2C_DeskInf_Normal, RecvDeskInfoNormallRoom);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_GAMESTART_BROADCAST, RecvGameStart);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_CURRENT_GAMESTATE, RecvResetGame);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_GAMEEND_BROADCAST, RecvGameEnd);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_CHANGECARD_BROADCAST, RecvChangeCardSelected);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_CHANGETYPE_BROADCAST, RecvChangeCardResult);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_DINGQUE_BROADCAST, RecvDingQueResult);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_HU_INFO, RecvWinInfo);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_GANG_INFO, RecvGangInfo);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_LIUJU_SCORE, RecvLiujuScore);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_HAVE_PLAYER_NO_MONEY, RecvPlayerBankcrupcy);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_GIVE_UP, RecvGiveUp);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_TOPUP_SUCCEED, RecvTopUpSuccessed);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_CONTINUE_GAME, RecvContinueGame);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_RECONNECT_GIVEUP, RecvReconnectGiveUp);
            RegisterMessageProcessor(XLHZMessages.MsgID_S2C_LiuShuiInfo, RecvWaterflowInfo);
            RegisterMessageProcessor(XLHZMessages.MsgID_S2C_YiPaoDuoXiangCard_Info, RecvMultiWinInfo);
            RegisterMessageProcessor(XLHZMessages.MsgID_S2C_YYY_Money, RecvYYYMomey);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_DWSGameEndScore, RecvDuanweiScoreInfo);

            RegisterMessageProcessor(XLHZMessages.MSG_S2C_FRIENDROOM_DESKINFO, RecvFriendDeskInfo);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_FRIENDROOM_CURRENT_GAMESTATE, RecvFriendReconnect);
            RegisterMessageProcessor(XLHZMessages.Msg_S2C_FriendStartScore, RecvFriendStartScore);
            RegisterMessageProcessor(XLHZMessages.MSG_S2C_FRIENDROOM_END, RecvFriendEnd);
        }

        private void RecvDeskInfoNormallRoom(MsgHeader msg)
        {
            var msgFormat = new ArrayList
            {
                new ArrayList {"BYTE", "fangShu"},
                new ArrayList {"int", "fengDing"},
                new ArrayList {"BYTE", "ziMoType"},
                // new ArrayList {"BYTE", "dianGang"},
                new ArrayList {"BYTE", "byHongZhongCount"}, //新增红中数
                new ArrayList {"BYTE", "huanPai"},
                new ArrayList {"BYTE", "huJiaoZhuanYi"},
                new ArrayList {"BYTE", "tianDiHu"},
                new ArrayList {"BYTE", "yaoJiu"},
                new ArrayList {"BYTE", "menQing"},
                new ArrayList {"BYTE", "showLiushui"},
            };

            var data = MsgParser.Parse(msg, msgFormat);

            var xlhzGameData = gameData as XLHZGameData;
            xlhzGameData.fangShu = data.GetByte("fangShu");
            xlhzGameData.changeCard = data.GetByte("huanPai") == 1;
            xlhzGameData.huJiaoZhuanYi = data.GetByte("huJiaoZhuanYi") == 1;
            xlhzGameData.showLiuShui = data.GetByte("showLiushui") == 1;
            xlhzGameData.hongZhongNum = data.GetByte("byHongZhongCount");
            xlhzGameData.fengDing = data.GetInt("fengDing");

        }

        private void RecvGameStart(MsgHeader msg)
        {
            var msgFormat = new ArrayList
            {
                new ArrayList {"WORD","wMsgID" },
                new ArrayList {"WORD","wBankerID"},
                new ArrayList {"BYTE","byDice1"},
                new ArrayList {"BYTE","byDice2"},
                new ArrayList {"Card","byHand",14},
                new ArrayList {"BYTE","bActionCount"},
            };

            var data = MsgParser.Parse(msg, msgFormat);

            // 通用逻辑：初始化游戏数据
            gameStartHandler.InitGameData(data.GetWord("wMsgID"), data.GetWord("wBankerID"), true, new List<int>{ Card.Zhong } );

            // 游戏逻辑：设置游戏自定义数据
            XLHZGameData xlGameData = gameData as XLHZGameData;
            xlGameData.diceValues = new List<int> { data.GetByte("byDice1"), data.GetByte("byDice2") }; // 设置骰子值
            if (!isFriendRoom && roomInfo.cmd.ContainsKey("tab"))
            {
                var tab = roomInfo.cmd["tab"];
                xlGameData.hongZhongNum = tab == "shz" ? 4 : 6; // 设置红中数
            }

            // 通用逻辑：初始化玩家数据
            gameStartHandler.InitPlayers();

            // 通用逻辑：初始化手牌数据
            gameStartHandler.InitHandCardValues(data.GetList<int>("byHand"), data.GetWord("wBankerID"));

            // 通用逻辑：增加朋友场局数
            gameStartHandler.IncreaseFriendGameCount();

            // 通用逻辑：播放对局开始、骰子、发牌等动画
            gameStartHandler.EnqueueGameStart();


            if (xlGameData.changeCard) // 换三张
            {
                exchangeHandler.EnqueueExchangeStart();
            }
            else if (xlGameData.dingque) // 定缺
            {
                dingqueHandler.EnqueueDingqueStart();
            }
            else
            {
                xlGameData.gameState = GameState.Play;

                // 通用逻辑：显示动作按钮
                actionButtonController.EnqueueShowButtons(msg, data.GetByte("bActionCount"));

                // 通用逻辑：开始倒计时
                gameStartHandler.EnqueueBankerTimer();

                // 设置自家手牌可交互
                handController.ownHand.interactable = true;

            }

            // 通用逻辑：触发OnGameStart事件，固定放在最后
            OnGameStart();
        }


        private void RecvResetGame(MsgHeader msg)
        {
            var msgFormat = new ArrayList
            {
                new ArrayList{ "WORD","msgID" },
                new ArrayList{ "long","roomBase" },
                new ArrayList{ "int","gameState" },
                new ArrayList{ "int","sendGangCardCount" },
                new ArrayList{ "int","sendCardCount" },
                new ArrayList{ "WORD","bankerChair" },
                new ArrayList{ "BYTE","dice1" },
                new ArrayList{ "BYTE","dice2" },
                new ArrayList{ "WORD","outChair" },
                // new ArrayList{ "WORD","lastChair" },
                new ArrayList{ "BYTE","eventType" },
                new ArrayList{ "WORD","eventChairId" },
                new ArrayList{ "Card","eventCard" },
                // new ArrayList{ "BYTE","curOutCard" },
                new ArrayList{ "BYTE","isHandCardValue" },
                new ArrayList{ "BYTE","handCardCounts", 4 },
                new ArrayList{ "BYTE","outCardCount", 4 },
                new ArrayList{ "BYTE","fuziCount", 4 },
                // new ArrayList{ "FuZi","fuziData" },
                // new ArrayList{ "BYTE","sendGangCard" },
                new ArrayList{ "TCHAR","nickName", 4, 36 },
                new ArrayList{ "BYTE","isAutoPlay", 4 },
                new ArrayList{ "BYTE","dingQueColor", 4 },
                new ArrayList{ "BYTE","isChangeCard", 4 },
                new ArrayList{ "BYTE","huPaiCount", 4 },
                new ArrayList{ "LONGLONG","userMoney", 4 },
                new ArrayList{ "BYTE","bActionCount" },
                new ArrayList{ "BYTE", "byHuInfoCount" }
            };

            var data = MsgParser.Parse(msg, msgFormat);

            // 通用逻辑：重置游戏数据
            reconnectHandler.ResetGameData(
                data.GetWord("msgID"),
                data.GetWord("bankerChair"),
                data.GetInt("gameState"),
                data.GetInt("sendCardCount"),
                data.GetInt("sendGangCardCount"),
                true,
                new List<int>{ Card.Zhong });

            // 通用逻辑: 重置玩家托管、听牌标志
            reconnectHandler.ResetPlayerFlags(data.GetList<int>("isAutoPlay"));

            // 通用逻辑：重置玩家手牌数据
            reconnectHandler.ResetHandCardValues(msg, data.GetList<int>("handCardCounts"), (uint)data.GetByte("eventType"));

            // 通用逻辑：重置玩家出牌数据
            reconnectHandler.ResetOutCardValues(msg, data.GetList<int>("outCardCount"));

            // 通用逻辑：重置玩家副子数据
            reconnectHandler.ResetFuziDatas(msg, data.GetList<int>("fuziCount"));

            // 游戏业务：重置游戏独有数据
            ResetSpecificDatas(msg, data);

            // 通用逻辑：重置玩家胡牌数据
            reconnectHandler.ResetWinCardValues(msg, data.GetList<int>("huPaiCount"));

            // 通用逻辑：重置游戏场景
            reconnectHandler.ResetGame((uint)data.GetByte("eventType"), data.GetWord("eventChairId"));

            // 游戏业务：重置特有场景内容
            ResetSpecificGameScene(msg, data);

            // 通用逻辑：触发OnGameReconnect事件，固定放在最后
            OnGameReconnect();
        }

        private void ResetSpecificDatas(MsgHeader msg, MsgData data)
        {
            XLHZGameData xlhzGameData = gameData as XLHZGameData;
            xlhzGameData.diceValues = new List<int> { 1, 1 }; // 设置骰子值。服务器下发骰子值为（0，0）有问题，请检查
            if (!isFriendRoom && roomInfo.cmd.ContainsKey("tab"))
            {
                var tab = roomInfo.cmd["tab"];
                xlhzGameData.hongZhongNum = tab == "shz" ? 4 : 6; // 设置红中数
            }

            if (data.GetList<int>("isChangeCard")[playerSelf.chairId] != 0)
            {
                List<int> changeCardValues = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    changeCardValues.Add(msg.ReadByte());
                }
                // TODO: 保存换牌数据
            }

            // 更新玩家豆豆/定缺数据
            List<long> beans = data.GetList<long>("userMoney");
            List<int> dingqueValues = data.GetList<int>("dingQueColor");
            TraversePlayer(p =>
            {
                XLHZGamePlayer player = p as XLHZGamePlayer;
                player.SetMoney(beans[player.chairId]);
                player.dingqueColorValue = dingqueValues[player.chairId];
                reconnectHandler.SortSelfHandCardValues((uint)data.GetByte("eventType"));
            });


            var winCardReadCounts = data.GetList<int>("huPaiCount");
            // 更新不可点击牌数据
            if (winCardReadCounts[playerSelf.chairId] > 0)
            {
                var pSelf = playerSelf as XLHZGamePlayer;
                var handCardValues = pSelf.handCardValues;
                pSelf.unselectableCardValues = pSelf.handCardCount % 3 == 2 ? handCardValues.Slice(0, pSelf.handCardCount - 1) : handCardValues;
            }
        }

        private void ResetSpecificGameScene(MsgHeader msg, MsgData data)
        {
            var gameState = data.GetInt("gameState");
            var dingqueValues = data.GetList<int>("dingQueColor");
            if (gameState == GameState.Exchange)
            {
                List<int> changeCardFlags = data.GetList<int>("isChangeCard");

                exchangeHandler.EnqueueResetExchangeState(changeCardFlags);
            }
            else if (gameState == GameState.Dingque)
            {
                dingqueHandler.EnqueueResetDingqueState(dingqueValues);
            }
            else if (gameState == GameState.Play)
            {
                if ((gameData as XLHZGameData).dingque)
                {
                    animationQueue.Enqueue(0, () => dingqueHandler.ShowDingqueIcon(dingqueValues));
                }

                // 通用逻辑：显示动作按钮
                actionButtonController.EnqueueShowButtons(msg, data.GetByte("bActionCount"));

                // 通用逻辑：开始倒计时
                reconnectHandler.StartReconnectTimer((uint)data.GetByte("eventType"), data.GetWord("eventChairId"));
            }
        }

        private void RecvGameEnd(MsgHeader msg)
        {
            var msgFormat = new ArrayList
            {
                new ArrayList { "longlong", "llMoney", 4 },
                new ArrayList { "longlong", "llHuScores", 4 },
                new ArrayList { "WORD", "wEndType" },
                new ArrayList { "Card", "byHandCard", 4, 14 },
                new ArrayList { "FuZi", "tagPlayerFuzi", 4, 4 },
                new ArrayList { "BYTE", "byDelaySendData" },
                new ArrayList { "BYTE", "byOverType" },
                new ArrayList { "BYTE", "byReconnect" }
            };

            var data = MsgParser.Parse(msg, msgFormat);
            var xlhzGameData = gameData as XLHZGameData;
            xlhzGameData.gameState = GameState.End;

            // 通用逻辑：更新数据
            gameOverHandler.UpdateData(data.GetByte("byReconnect") == 1, data.GetCrossList<int>("byHandCard"), data.GetCrossList<FuziData>("tagPlayerFuzi"));

            // 通用逻辑：更新朋友场分数
            gameOverHandler.UpdateFriendScore(data.GetList<long>("llMoney"));

            // 通用逻辑：清理动作按钮、托管等
            gameOverHandler.EnqueueGameOver();

            // 通用逻辑：播放亮牌动画
            gameOverHandler.EnqueueShowHand();

            // 通用逻辑: 显示结束界面
            settleController.EnqueueSettleView(new MJSettleView());

            // 通用逻辑：触发OnGameOver事件，固定放在最后
            OnGameOver();
        }

        private void RecvChangeCardSelected(MsgHeader msg)
        {
            int chairId = msg.ReadUint16();

            exchangeHandler.EnqueueExchangeSelected(chairId);
        }

        private void RecvChangeCardResult(MsgHeader msg)
        {
            var msgFormat = new ArrayList
            {
                new ArrayList { "WORD","wMsgID" },
                new ArrayList { "BYTE","byChangeType" },
                new ArrayList { "BYTE","byCards", 3 },
                new ArrayList { "BYTE","bActionCount" },
            };
            var data = MsgParser.Parse(msg, msgFormat);

            var xlGameData = gameData as XLHZGameData;
            xlGameData.verifyingMsgId = data.GetWord("wMsgID");

            exchangeHandler.EnqueueExchangeResult((ExchangeCardType)data.GetInt("byChangeType"), data.GetList<int>("byCards"));

            if (xlGameData.dingque)
            {
                dingqueHandler.EnqueueDingqueStart();
            }
            else
            {
                xlGameData.gameState = GameState.Play;

                // 通用逻辑：显示动作按钮
                actionButtonController.EnqueueShowButtons(msg, data.GetByte("bActionCount"));

                // 通用逻辑：开始倒计时
                gameStartHandler.EnqueueBankerTimer();

                // 设置自家手牌可交互
                handController.ownHand.interactable = true;
            }
        }


        private void RecvWinInfo(MsgHeader msg)
        {
            var msgFormat = new ArrayList
            {
                new ArrayList {"BYTE","byActionType"},
                new ArrayList {"WORD","wProvideChairID"},
                new ArrayList {"Card","byCard"},
                new ArrayList {"LONGLONG","dwHuType",4},
                new ArrayList {"int","iBeiShu",4},
                new ArrayList {"LONGLONG","llHuScore",4},
                new ArrayList {"BYTE","byGenShu",4},
                new ArrayList {"BYTE","byAnGangShu",4}, //新增暗杠数
                new ArrayList {"BYTE","byMingGangShu",4},//新增明杠数
                new ArrayList {"LONGLONG","llMoney",4},
                new ArrayList {"BYTE",   "upTop", 4}
            };

            var data = MsgParser.Parse(msg, msgFormat);

            var huTypes = data.GetList<long>("dwHuType");
            var cardValue = data.GetByte("byCard");
            var operType = data.GetByte("byActionType");
            var providerChairId = data.GetWord("wProvideChairID");
            var huScores = data.GetList<long>("llHuScore");
            var scores = data.GetList<long>("llMoney");

            // 更新玩家数据
            winHandler.UpdatePlayerData(huTypes, cardValue, operType, providerChairId, scores);

            // 更新一炮多响信息
            winHandler.UpdateMultiWinInfo(operType, huTypes, huScores, cardValue);

            // 更新流水数据
            //huHandler.UpdateWaterflow();//TODO

            winHandler.EnqueueReloadHandCards(operType, huTypes);

            winHandler.EnqueueReloadProviderMeldDeskCard(operType, huTypes, providerChairId);

            winHandler.EnqueueWinCardAnimation(huTypes, cardValue);

            winHandler.EnqueueWinEffect(operType, providerChairId, huTypes, huScores);

            winHandler.EnqueueRefreshHandCards();

            winHandler.EnqueueRefreshAvatarScore();
        }

        private void RecvGangInfo(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvGangInfo</color>");
        }

        private void RecvLiujuScore(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvLiujuScore</color>");
        }

        private void RecvPlayerBankcrupcy(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvPlayerBankcrupcy</color>");
        }

        private void RecvGiveUp(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvGiveUp</color>");
        }

        private void RecvTopUpSuccessed(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvTopUpSuccessed</color>");
        }

        private void RecvContinueGame(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvContinueGame</color>");
        }

        private void RecvReconnectGiveUp(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvReconnectGiveUp</color>");
        }

        private void RecvWaterflowInfo(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvWaterflowInfo</color>");
        }

        private void RecvFriendDeskInfo(MsgHeader msg)
        {
            var msgFormat = new ArrayList
            {
                new ArrayList {"int", "gameCount"},
                new ArrayList {"BYTE", "byGPS"},
                new ArrayList {"BYTE", "byYuYin"},
                new ArrayList {"BYTE", "byIsDaiKai"},
                new ArrayList {"BYTE", "cbOptVoice"},
                new ArrayList {"BYTE", "cbOfflineTime"},   //断线累计时间
                new ArrayList {"BYTE", "fangShu"},
                new ArrayList {"int", "fengDing"},
                new ArrayList {"BYTE", "ziMoType"},
                new ArrayList {"BYTE", "byHongZhongCount"}, //新增红中数
                new ArrayList {"BYTE", "huanPai"},
                new ArrayList {"BYTE", "huJiaoZhuanYi"},
                new ArrayList {"BYTE", "tianDiHu"},
                new ArrayList {"BYTE", "yaoJiu"},
                new ArrayList {"BYTE", "menQing"},
                new ArrayList {"BYTE", "showLiushui"},
            };

            var data = MsgParser.Parse(msg, msgFormat);

            // 游戏逻辑：保存特有数据
            var xlhzGameData = gameData as XLHZGameData;
            xlhzGameData.fangShu = data.GetByte("fangShu");
            xlhzGameData.changeCard = data.GetByte("huanPai") == 1;
            xlhzGameData.huJiaoZhuanYi = data.GetByte("huJiaoZhuanYi") == 1;
            xlhzGameData.showLiuShui = data.GetByte("showLiushui") == 1;
            xlhzGameData.hongZhongNum = data.GetByte("byHongZhongCount");
            xlhzGameData.fengDing = data.GetInt("fengDing");

            FriendGameInfo friendGameInfo = GetFriendGameInfo(data);

            // 通用逻辑：更新朋友场游戏信息
            friendGameStartHandler.UpdateFriendGameInfo(friendGameInfo);

            // 通用逻辑：触发OnFriendGameStart，固定放在最后
            OnFriendGameStart();
        }

        private FriendGameInfo GetFriendGameInfo(MsgData data, int currentGameCount = 0)
        {
            return new FriendGameInfo
            {
                isOpenGPS = data.GetByte("byGPS") == 1,
                isOpenVoiceChat = data.GetByte("byYuYin") == 1,
                isOpenInstantVoiceChat = data.GetByte("cbOptVoice") == 1,
                accumulatingOfflineTime = data.GetByte("cbOfflineTime") == 1,
                gameName = roomManager.gameInfo.name + $"-{maxPlayerCount}人",
                totalGameCount = data.GetInt("gameCount"),
                currentGameCount = currentGameCount,
                rules = GetRules(data)
            };
        }

        private List<string> GetRules(MsgData data)
        {
            var rules = new List<string>();

            if (data.GetByte("fengDing") == 0) rules.Add("不封顶");
            if (data.GetInt("fengDing") > 0) rules.Add("封顶:" + data.GetInt("fengDing") + "番");
            if (data.GetByte("fangShu") == 3) rules.Add("三房");
            if (data.GetByte("fangShu") == 2) rules.Add("两房");
            if (data.GetByte("ziMoType") == 0) rules.Add("自摸不加");
            if (data.GetByte("ziMoType") != 0) rules.Add("自摸加番");
            // if (data.GetByte("dianGang") == 0) info.Add("点杠花(点炮)");
            // if (data.GetByte("dianGang") != 0) info.Add("点杠花(自摸)");

            if (data.GetByte("byHongZhongCount") == 4) rules.Add("4红中");
            if (data.GetByte("byHongZhongCount") == 6) rules.Add("6红中");

            if (data.GetByte("huanPai") == 1) rules.Add("换三张");
            if (data.GetByte("huJiaoZhuanYi") == 1) rules.Add("呼叫转移");
            if (data.GetByte("tianDiHu") == 1) rules.Add("天地胡");
            if (data.GetByte("yaoJiu") == 1) rules.Add("幺九将对");
            if (data.GetByte("menQing") == 1) rules.Add("门清中张");
            if (data.GetByte("showLiushui") == 1) rules.Add("显示流水");

            return rules;
        }

        private void RecvFriendReconnect(MsgHeader msg)
        {
            var msgFormat = new ArrayList
            {
                new ArrayList {"int", "nPlayGameCount"},
                new ArrayList {"WORD", "wSQJSId"},
                new ArrayList {"BYTE", "byJSJGuo", 4},
                new ArrayList {"LONGLONG", "llScores", 4},

                new ArrayList {"int", "gameCount"},
                new ArrayList {"BYTE", "byGPS"},
                new ArrayList {"BYTE", "byYuYin"},
                new ArrayList {"BYTE", "byIsDaiKai"},
                new ArrayList {"BYTE", "cbOptVoice"},
                new ArrayList {"BYTE", "cbOfflineTime"},   //断线累计时间

                new ArrayList {"BYTE", "fangShu"},
                new ArrayList {"int", "fengDing"},
                new ArrayList {"BYTE", "ziMoType"},
                new ArrayList {"BYTE", "byHongZhongCount"}, //新增红中数
                new ArrayList {"BYTE", "huanPai"},
                new ArrayList {"BYTE", "huJiaoZhuanYi"},
                new ArrayList {"BYTE", "tianDiHu"},
                new ArrayList {"BYTE", "yaoJiu"},
                new ArrayList {"BYTE", "menQing"},
                new ArrayList {"BYTE", "showLiushui"},
            };

            var data = MsgParser.Parse(msg, msgFormat);

            // 游戏逻辑：保存特有数据
            var xlhzGameData = gameData as XLHZGameData;
            xlhzGameData.fangShu = data.GetByte("fangShu");
            xlhzGameData.changeCard = data.GetByte("huanPai") == 1;
            xlhzGameData.huJiaoZhuanYi = data.GetByte("huJiaoZhuanYi") == 1;
            xlhzGameData.showLiuShui = data.GetByte("showLiushui") == 1;
            xlhzGameData.hongZhongNum = data.GetByte("byHongZhongCount");
            xlhzGameData.fengDing = data.GetInt("fengDing");

            var friendGameInfo = GetFriendGameInfo(data, data.GetInt("nPlayGameCount"));

            // 通用逻辑：更新朋友场游戏信息
            friendGameStartHandler.UpdateFriendGameInfo(friendGameInfo);

            // 通用逻辑：更新玩家分数
            friendReconnectHandler.RefreshPlayerScore(data.GetList<long>("llScores"));

            // 通用逻辑：恢复解散对话框
            friendReconnectHandler.RefreshDissolutionDialog(data.GetWord("wSQJSId"), data.GetList<int>("byJSJGuo"));

            // 通用逻辑：触发朋友场断线重连事件，固定放在最后。
            OnFriendGameReconnect();
        }

        private void RecvFriendStartScore(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvFriendStartScore</color>");
        }

        private void RecvFriendEnd(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvFriendEnd</color>");

            OnFriendGameOver();
        }

        private void RecvMultiWinInfo(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvMultiWinInfo</color>");
        }

        private void RecvYYYMomey(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvYYYMomey</color>");
        }

        private void RecvDuanweiScoreInfo(MsgHeader msg)
        {
            WLDebug.Log("<color=yellow>RecvDuanweiScoreInfo</color>");
        }


        protected override void RecvHuInfoMultiple(MsgHeader msg)
        {
            int readCount = msg.ReadByte();

            MsgHuInfo info = new MsgHuInfo()
            {
                type = MJHintsData.Type.Multiple,
                hintsDataMap = new Dictionary<int, MJHintsData>()
            };

            for (int i = 0; i < readCount; i++)
            {
                int cardCount = msg.ReadByte() - 1;
                int cardValue = msg.ReadByte();
                var hintsData = new MJHintsData()
                {
                    type = MJHintsData.Type.Multiple,
                    cardValues = new List<int>(),
                    multiplies = new List<int>()
                };
                for (int j = 0; j < cardCount; j++)
                {
                    hintsData.cardValues.Add(msg.ReadByte());
                    hintsData.multiplies.Add(msg.ReadInt32());
                }
                info.hintsDataMap.Add(cardValue, hintsData);
            }

            huInfoHandler.UpdateData(info);

            huInfoHandler.EnqueueShowHintMark();
        }

        protected override void RecvTingInfoMultiple(MsgHeader msg)
        {
            int readCount = msg.ReadByte();

            if (readCount == 0)
            {
                tingInfoHandler.Clear();

                return;
            }

            var hintsData = new MJHintsData()
            {
                type = MJHintsData.Type.Multiple,
                cardValues = new List<int>(),
                multiplies = new List<int>()
            };

            for (int i = 0; i < readCount; i++)
            {
                hintsData.cardValues.Add(msg.ReadByte());
                hintsData.multiplies.Add(msg.ReadInt32());
            }

            tingInfoHandler.UpdateData(hintsData);

            tingInfoHandler.EnqueueShowHintButton();
        }
    }
}
