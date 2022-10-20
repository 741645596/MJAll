// @Author: tanjinhua
// @Date: 2021/4/7  10:57


using System.Collections.Generic;
using WLCore;
using WLHall.Game;

namespace Common
{
    public abstract partial class GameMessage : BaseGameMessage
    {

        protected override void RegisterRecvInterface()
        {
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_GameChat_Broadcast, RecvChatText);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_BASE_SCORE, RecvBaseScore);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_LianYing_Info, RecvLianYingInfo);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_BiaoQing_Broadcast, RecvBiaoQingBroadcast);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Friend_CurrentPlayCount, RecvFriendCurrentPlayCount);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_FRIEND_TEAM_Dissolution_Broadcast, RecvFriendDissolutionBroadcast);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_FRIEND_Dissolution_Result, RecvFriendDissolutionResult);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_FRIEND_LiXian, RecvFriendLiXian);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_FRIEND_LiXian_Current, RecvFriendLiXianCurrent);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_AutoPlay_Broadcast, RecvAutoPlayBroadcast);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Ready_Broadcast, RecvReadyBroadcast);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_GPS, RecvGPS);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Record, RecvVoiceChat);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_ChangeDesk, RecvChangeDeskNotic);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_KickPlayerCause, RecvKickCause);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_FriendDissolutionLeftTime, RecvFriendDissolutionLeftTime);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_HonorScoreInfo, RecvHonorScoreInfo);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Ranking_Info, RecvRankingMatchInfo);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Ranking_Honor_Update, RecvRankingMatchHonorUpdate);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Game_Again, RecvOneMoreGameInvite);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_FriendCfg_ZYQS, RecvQifuConfig);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Friend_ZYQS, RecvQifu);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_CoinMarket_Task_Info, RecvCoinWinTaskInfo);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_CoinMarket_Task_Update, RecvCoinWinTaskUpdate);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_CoinMarket_Task_Receive, RecvCoinWinTaskReward);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_CoinMarket_Task_Close, RecvCoinWinTaskClose);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_OverTime_Auto_Dissolution, RecvOverTimeAutoDissolution);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Cancel_OverTime_Auto_Dissolution, RecvCancelOverTimeAutoDissolution);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_OverTime_Auto_Dissolution_Chair, RecvOverTimeDissolutionChair);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_OverTime_Auto_Dissolution_IS_Open, RecvOverTimeDissolutionIsOpen);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_InviteRuleInfo, RecvInviteRuleInfo);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Offline_CountDown, RecvPlayerOfflineCountdown);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2c_All_Offline_CountDown, RecvPlayersOfflineCountdown);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_Override_Money_Chairs, RecvPlayersScoreLimitFlag);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_CommonPriceList_Reply, RecvCommonPriceList);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_UseMagicProp_Reply, RecvUseMagicPropBroadcast);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_UseShuffle_Reply, RecvUseShuffleReply);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_NewGameRule, RecvNewGameRule);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_UserDuanWeiScore_Broadcast, RecvUserDuanweiScoreInfo);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_PiggyBankInfo_Notice, RecvPiggyBankInfo);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_PiggyBankClose_Notice, RecvPiggyBankCloseNotice);
            RegisterMessageProcessor(GameMsgDefine.MsgID_C2S_PiggyBankExchangeSuccessed, RecvPiggyBankExchangeSuccessed);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_OverScoreFlags_Broadcast, RecvPlayersOverScoreFlag);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_PiggyBank2Info_Notice, RecvPiggyBankInfo2);
            RegisterMessageProcessor(GameMsgDefine.MsgID_S2C_PiggyBank2Close_Notice, RecvPiggyBankCloseNotice2);
            RegisterMessageProcessor(GameMsgDefine.MsgID_C2S_PiggyBank2ExchangeSuccessed, RecvPiggyBankExchangeSuccessed2);
        }



        #region Recv Interfaces
        /// <summary>
        /// 收到短语聊天消息
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void RecvChatText(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvChatText</color>");
        }


        protected virtual void RecvBaseScore(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvBaseScore</color>");
        }


        protected virtual void RecvLianYingInfo(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvLianYingInfo</color>");
        }


        protected virtual void RecvBiaoQingBroadcast(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvBiaoQingBroadcast</color>");
        }


        protected virtual void RecvFriendCurrentPlayCount(MsgHeader msg)
        {
            int count = msg.ReadInt32();

            stage.GetGameData<GameData>().friendGameInfo.currentGameCount = count;
        }


        protected virtual void RecvFriendDissolutionBroadcast(MsgHeader msg)
        {
            msg.ReadByte();

            ushort chairId = msg.ReadUint16();

            stage.GetController<FriendDissolutionHandler>().RecvDissolutionBroadcast(chairId);
        }


        protected virtual void RecvFriendDissolutionResult(MsgHeader msg)
        {
            int result = msg.ReadByte();

            ushort chairId = msg.ReadUint16();

            stage.GetController<FriendDissolutionHandler>().RecvDissolutionResult(result == 1, chairId);
        }


        protected virtual void RecvFriendLiXian(MsgHeader msg)
        {
            ushort chairId = msg.ReadUint16();
            int state = msg.ReadByte();
            int time = 0;
            if (msg.len > msg.position)
            {
                time = msg.ReadInt32();
            }

            GamePlayer player = stage.gameData.GetPlayerByChairId(chairId) as GamePlayer;
            player.offlineState = state;
            player.currentOfflineTime = time;

            if (state == OffineState.Offline || state == OffineState.None)
            {
                bool isLeft = state == OffineState.Offline;
                string stateDes = isLeft ? "离开" : "回到游戏";
                string nickname = player.GetNickNameUtf16();
                string toast = $"[{nickname}] {stateDes}";
                // TODO: ToastMessageManager.PushMessage(toast);

                if (isLeft)
                {
                    // TODO:  RecvGps.playerLeave( data.wChairID )
                }
            }

            int viewChairId = stage.ToViewChairId(chairId);
            stage.animationQueue.Enqueue(0, () =>
            {
                var avatar = stage.GetController<GameAvatarController>().GetAvatar(viewChairId);
                if (state != OffineState.Offline && state != OffineState.Calling)
                {
                    avatar.HideOffline();
                }
                else
                {
                    avatar.ShowOfflineTime(time);
                }
            });
        }


        protected virtual void RecvFriendLiXianCurrent(MsgHeader msg)
        {
            int count = 4;

            HandleFriendOfflineUpdate(msg, count);
        }

        protected virtual void RecvAutoPlayBroadcast(MsgHeader msg)
        {
            bool isTrust = msg.ReadByte() == 1;
            int chairId = msg.ReadUint16();

            var gameData = stage.GetGameData<GameData>();

            if (gameData.trustForbidden && chairId == gameData.playerSelf.chairId)
            {
                return;
            }

            GamePlayer player = gameData.GetPlayerByChairId(chairId) as GamePlayer;

            if (isTrust)
            {
                stage.GetController<GameTrustController>().EnterTrust(false, player);
            }
            else
            {
                stage.GetController<GameTrustController>().ExitTrust(false, player);
            }
        }


        protected virtual void RecvReadyBroadcast(MsgHeader msg)
        {
            int chairId = msg.ReadByte();

            var gameData = stage.GetGameData<GameData>();

            // 如果是自己准备，并且有玩家离开则换桌
            if (!gameData.isFriendRoom && gameData.playerSelf.chairId == chairId && gameData.currentPlayerCount < gameData.maxPlayerCount)
            {
                stage.ChangeDesk();

                return;
            }

            var player = gameData.GetPlayerByChairId(chairId);

            player.SetReady(true);

            int viewChairId = stage.ToViewChairId(chairId);

            var inReadyState = stage.GetController<FriendGameStartHandler>().IsInReadyState();

            if (gameData.isFriendRoom && player == gameData.playerSelf && inReadyState)
            {
                stage.GetController<FriendReadyController>().ShowReady();
            }
            else
            {
                var avatar = stage.GetController<GameAvatarController>().GetAvatar(viewChairId);
                avatar?.ShowReady();
            }

            // TODO: GPSView
        }


        protected virtual void RecvGPS(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvGPS</color>");
        }


        protected virtual void RecvVoiceChat(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvVoiceChat</color>");
        }


        protected virtual void RecvChangeDeskNotic(MsgHeader msg)
        {
            if (stage.gameData.playerSelf.IsReady())
            {
                stage.ChangeDesk();
            }
        }


        protected virtual void RecvKickCause(MsgHeader msg)
        {
            string cause = msg.ReadStringW();

            (stage as GameStage).ExitWithDialog(cause);
        }


        protected virtual void RecvFriendDissolutionLeftTime(MsgHeader msg)
        {
            int leftTime = msg.ReadInt32();

            stage.GetController<FriendDissolutionHandler>().EnqueueDissolutionTimeUpdate(leftTime);
        }


        protected virtual void RecvHonorScoreInfo(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvHonorScoreInfo</color>");
        }


        protected virtual void RecvRankingMatchInfo(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvRankingMatchInfo</color>");
        }


        protected virtual void RecvRankingMatchHonorUpdate(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvRankingMatchHonorUpdate</color>");
        }


        protected virtual void RecvOneMoreGameInvite(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvOneMoreGameInvite</color>");
        }


        protected virtual void RecvQifuConfig(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvQifuConfig</color>");
        }


        protected virtual void RecvQifu(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvQifu</color>");
        }


        protected virtual void RecvCoinWinTaskInfo(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvCoinWinTaskInfo</color>");
        }


        protected virtual void RecvCoinWinTaskUpdate(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvCoinWinTaskUpdate</color>");
        }


        protected virtual void RecvCoinWinTaskReward(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvCoinWinTaskReward</color>");
        }


        protected virtual void RecvCoinWinTaskClose(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvCoinWinTaskClose</color>");
        }


        protected virtual void RecvOverTimeAutoDissolution(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvOverTimeAutoDissolution</color>");
        }


        protected virtual void RecvCancelOverTimeAutoDissolution(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvCancelOverTimeAutoDissolution</color>");
        }


        protected virtual void RecvOverTimeDissolutionChair(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvOverTimeDissolutionChair</color>");
        }


        protected virtual void RecvOverTimeDissolutionIsOpen(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvOverTimeDissolutionIsOpen</color>");
        }


        protected virtual void RecvInviteRuleInfo(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvInviteRuleInfo</color>");
        }


        protected virtual void RecvPlayerOfflineCountdown(MsgHeader msg)
        {
            ushort chairId = msg.ReadUint16();
            int countdown = msg.ReadByte();
            stage.animationQueue.Enqueue(0, () =>
            {
                int viewChairId = stage.ToViewChairId(chairId);
                var avatar = stage.GetController<GameAvatarController>().GetAvatar(viewChairId);
                avatar.ShowOfflineCountdown(countdown);
            });
        }


        protected virtual void RecvPlayersOfflineCountdown(MsgHeader msg)
        {
            int[] countdowns = new int[stage.gameData.maxPlayerCount];

            for (int i = 0; i < countdowns.Length; i++)
            {
                countdowns[i] = msg.ReadByte();
            }

            stage.animationQueue.Enqueue(0, () =>
            {
                for (int chairId = 0; chairId < countdowns.Length; chairId++)
                {
                    int viewChairId = stage.ToViewChairId(chairId);
                    var avatar = stage.GetController<GameAvatarController>().GetAvatar(viewChairId);
                    avatar.ShowOfflineCountdown(countdowns[chairId]);
                }
            });
        }


        protected virtual void RecvPlayersScoreLimitFlag(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvPlayersScoreLimitFlag</color>");
        }


        protected virtual void RecvCommonPriceList(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvCommonPriceList</color>");
        }


        protected virtual void RecvUseMagicPropBroadcast(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvUseMagicPropBroadcast</color>");
        }


        protected virtual void RecvUseShuffleReply(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvUseShuffleReply</color>");
        }


        protected virtual void RecvNewGameRule(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvNewGameRule</color>");
        }


        protected virtual void RecvUserDuanweiScoreInfo(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvUserDuanweiScoreInfo</color>");
        }


        protected virtual void RecvPiggyBankInfo(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvPiggyBankInfo</color>");
        }


        protected virtual void RecvPiggyBankCloseNotice(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvPiggyBankCloseNotice</color>");
        }


        protected virtual void RecvPiggyBankExchangeSuccessed(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvPiggyBankExchangeSuccessed</color>");
        }


        protected virtual void RecvPlayersOverScoreFlag(MsgHeader msg)
        {
            var gameData = stage.GetGameData<GameData>();
            int playerCount = gameData.maxPlayerCount;
            List<int> flags = new List<int>();
            for (int chairId = 0; chairId < playerCount; chairId++)
            {
                flags.Add(msg.ReadByte());
            }
            gameData.playerSettleScoreFlags = flags;
        }


        protected virtual void RecvPiggyBankInfo2(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvPiggyBankInfo2</color>");
        }


        protected virtual void RecvPiggyBankCloseNotice2(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvPiggyBankCloseNotice2</color>");
        }


        protected virtual void RecvPiggyBankExchangeSuccessed2(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvPiggyBankExchangeSuccessed2</color>");
        }
        #endregion

        protected void HandleFriendOfflineUpdate(MsgHeader msg, int count)
        {
            int[] states = new int[count];
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = msg.ReadByte();
            }

            int[] times = new int[count];
            if (msg.len > msg.position)
            {
                for (int i = 0; i < times.Length; i++)
                {
                    times[i] = msg.ReadInt32();
                }
            }

            stage.gameData.TraversePlayer(p =>
            {
                GamePlayer player = p as GamePlayer;

                player.offlineState = states[player.chairId];

                player.currentOfflineTime = times[player.chairId];

                if (player.offlineState == OffineState.Offline)
                {
                    // TODO: RecvGps.playerLeave(i);
                }
            });
        }
    }
}
