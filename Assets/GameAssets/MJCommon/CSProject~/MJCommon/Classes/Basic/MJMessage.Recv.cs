// @Author: tanjinhua
// @Date: 2021/4/7  11:07


using System.Collections.Generic;
using Common;
using WLCore;

namespace MJCommon
{
    public partial class MJMessage : GameMessage
    {

        protected override void RegisterRecvInterface()
        {
            base.RegisterRecvInterface();

            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_Test_Signal, RecvTestSignal);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_Signal_Intensity, RecvSignalIntensity);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_SendCard_Broadcast, RecvSendCardBroadcast);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_OutCard_Broadcast, RecvOutCardBroadcast);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_ActionResult_Broadcast, RecvActionResultBroadcast);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_Friend_Task, RecvFriendTask);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_Friend_TaskOver, RecvFriendTaskOver);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_ActionResult2_Broadcast, RecvScrambleActionResultBroadcast);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_SendCard_FenZang, RecvSendCardFromFenZang);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_HuInfo_Broadcast, RecvHuInfoNormal);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_TingInfo_Broadcast, RecvTingInfoNormal);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_HuInfo2_Broadcast, RecvHuInfoMultiple);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_HuInfo3_Broadcast, RecvHuInfoDescriptive);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_TingInfo2_Broadcast, RecvTingInfoMultiple);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_TingInfo3_Broadcast, RecvTingInfoDescriptive);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_Update_Cup, RecvUpdateCup);
            RegisterMessageProcessor(MJMsgDefine.MsgID_S2C_Not_Enough_Cup, RecvNotEnoughCup);
        }


        #region Recv Interfaces
        protected virtual void RecvTestSignal(MsgHeader msg)
        {
            ushort serverMsgId = msg.ReadUint16();

            SendTestSignal(serverMsgId);
        }


        protected virtual void RecvSignalIntensity(MsgHeader msg)
        {
            int playerCount = stage.gameData.maxPlayerCount;
            int[] signals = new int[playerCount];
            for (int chairId = 0; chairId < playerCount; chairId++)
            {
                signals[chairId] = msg.ReadByte();
            }

            // TODO: 显示信号弱UI
        }


        protected virtual void RecvSendCardBroadcast(MsgHeader msg)
        {
            MsgSendCard data = MsgSendCard.From(msg);

            var actionButtonCtl = stage.GetController<MJActionButtonController>();
            var sendCardHandler = stage.GetController<MJSendCardHandler>();

            // 清除动作按钮及数据
            actionButtonCtl.Clear();

            // 更新数据
            sendCardHandler.UpdateData(data.msgId, data.chairId, data.cardValue, data.isKongCard, msg, data.actionCount);

            // 出牌动画
            sendCardHandler.EnqueueSendCard(data.chairId, data.cardValue, data.isKongCard);

            // 显示动作按钮
            var gameData = stage.GetGameData<MJGameData>();
            var actionDatas = gameData.currentActionDatas;
            stage.animationQueue.Enqueue(0, () => actionButtonCtl.ShowButtons(actionDatas));

            stage.animationQueue.Delay(0.3f);

            // 尝试自动出牌
            sendCardHandler.EnqueueAutoOperations(data.chairId, data.cardValue);
        }


        protected virtual void RecvSendCardFromFenZang(MsgHeader msg)
        {
            MsgSendCard data = MsgSendCard.From(msg);

            var actionButtonCtl = stage.GetController<MJActionButtonController>();
            var sendCardHandler = stage.GetController<MJSendCardHandler>();

            // 清除动作按钮及数据
            actionButtonCtl.Clear();

            // 更新数据
            sendCardHandler.UpdateData(data.msgId, data.chairId, data.cardValue, data.isKongCard, msg, data.actionCount);

            // 出牌动画
            sendCardHandler.EnqueueSendCard(data.chairId, data.cardValue, data.isKongCard);

            // 显示动作按钮
            var gameData = stage.GetGameData<MJGameData>();
            var actionDatas = gameData.currentActionDatas;
            stage.animationQueue.Enqueue(0, () => actionButtonCtl.ShowButtons(actionDatas, true));
        }


        protected virtual void RecvOutCardBroadcast(MsgHeader msg)
        {
            MsgOutCard data = MsgOutCard.From(msg);

            var outCardHandler = stage.GetController<MJOutCardHandler>();
            var actionButtonCtl = stage.GetController<MJActionButtonController>();

            // 更新数据
            outCardHandler.UpdateData(data.msgId, data.chairId, data.cardValue, data.serverOut);

            if (data.chairId == stage.gameData.playerSelf.chairId)
            {
                // 退出听牌选择出牌状态
                actionButtonCtl.ExitReadyHandSelectCardState();
            }

            // 出牌动画
            outCardHandler.EnqueueOutCard(data.chairId, data.cardValue);

            // 验证出牌
            outCardHandler.EnqueueVerifyOutCard(data.chairId, data.cardValue);

            if (data.actionCount > 0)
            {
                // 显示动作按钮
                actionButtonCtl.EnqueueShowButtons(msg, data.actionCount);

                // 开始动作事件倒计时
                outCardHandler.EnqueueSelfActionEventTimer();
            }
        }


        protected virtual void RecvActionResultBroadcast(MsgHeader msg)
        {
            MsgActionResult data = MsgActionResult.From(msg);

            var actionResultHandler = stage.GetController<MJActionResultHandler>();
            var huInfoHandler = stage.GetController<MJHuInfoHandler>();
            var tingInfoHandler = stage.GetController<MJTingInfoHandler>();
            var actionButtonCtl = stage.GetController<MJActionButtonController>();

            // 更新数据
            actionResultHandler.UpdateData(data.msgId, data.chairId, data.showType, data.fuziIndex, data.fuziData, data.toRemoveCardValues);

            // 清除胡牌提示数据及相关UI
            huInfoHandler.Clear();

            // 如果是自家动作，则清除听牌提示UI及数据
            if (data.chairId == stage.gameData.playerSelf.chairId && data.showType != ActionShowType.Buhua)
            {
                tingInfoHandler.Clear();
            }

            actionButtonCtl.ClearButtons();

            actionResultHandler.EnqueueActionResult(msg, data);
        }


        protected virtual void RecvScrambleActionResultBroadcast(MsgHeader msg)
        {
            MsgScrambleActionResult data = MsgScrambleActionResult.From(msg);

            var actionResultHandler = stage.GetController<MJActionResultHandler>();
            var huInfoHandler = stage.GetController<MJHuInfoHandler>();
            var tingInfoHandler = stage.GetController<MJTingInfoHandler>();
            var actionButtonCtl = stage.GetController<MJActionButtonController>();

            // 更新被抢玩家副子数据
            var player = stage.gameData.GetPlayerByChairId(data.recoverChairId) as MJGamePlayer;
            actionResultHandler.UpdateFuziData(player, data.recoverFuziIndex, data.recoverFuziData);

            // 更新动作玩家数据
            actionResultHandler.UpdateData(data.msgId, data.chairId, data.showType, data.fuziIndex, data.fuziData, data.toRemoveCardValues);

            // 清除胡牌提示数据及相关UI
            huInfoHandler.Clear();

            // 如果是自家动作，则清除听牌提示UI及数据
            if (data.chairId == stage.gameData.playerSelf.chairId && data.showType != ActionShowType.Buhua)
            {
                tingInfoHandler.Clear();
            }

            actionButtonCtl.ClearButtons();

            // 刷新被抢玩家副子合集节点
            if (data.showType != ActionShowType.Ting && data.recoverFuziIndex != MJDefine.InvaildFuziIndex)
            {
                actionResultHandler.EnqueueReloadMeldSet(data.recoverChairId);
            }

            actionResultHandler.EnqueueActionResult(msg, data);
        }


        protected virtual void RecvHuInfoNormal(MsgHeader msg)
        {
            HandleHuInfo(msg, MJHintsData.Type.Normal);
        }


        protected virtual void RecvHuInfoMultiple(MsgHeader msg)
        {
            HandleHuInfo(msg, MJHintsData.Type.Multiple);
        }


        protected virtual void RecvHuInfoDescriptive(MsgHeader msg)
        {
            HandleHuInfo(msg, MJHintsData.Type.Descriptive);
        }


        protected virtual void RecvTingInfoNormal(MsgHeader msg)
        {
            HandleTingInfo(msg, MJHintsData.Type.Normal);
        }


        protected virtual void RecvTingInfoMultiple(MsgHeader msg)
        {
            HandleTingInfo(msg, MJHintsData.Type.Multiple);
        }


        protected virtual void RecvTingInfoDescriptive(MsgHeader msg)
        {
            HandleTingInfo(msg, MJHintsData.Type.Descriptive);
        }


        protected virtual void RecvUpdateCup(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvUpdateCup</color>");
        }


        protected virtual void RecvNotEnoughCup(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvNotEnoughCup</color>");
        }


        protected virtual void RecvFriendTask(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvFriendTask</color>");
        }


        protected virtual void RecvFriendTaskOver(MsgHeader msg)
        {
            WLDebug.Info("<color=green>RecvFriendTaskOver</color>");
        }

        protected virtual void RecvDingQueSelected(MsgHeader msg)
        {
            ushort chairId = msg.ReadUint16();

            stage.GetController<MJDingqueHandler>().EnqueueDingqueSelected(chairId);
        }

        protected virtual void RecvDingQueResult(MsgHeader msg)
        {
            List<int> dingqueColorValues = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                dingqueColorValues.Add(msg.ReadByte());
            }
            int actionCount = msg.ReadByte();

            stage.GetController<MJDingqueHandler>().EnqueueDingqueResult(dingqueColorValues);

            stage.GetController<MJActionButtonController>().EnqueueShowButtons(msg, actionCount);
        }
        #endregion


        #region Inherit
        protected override void RecvFriendCurrentPlayCount(MsgHeader msg)
        {
            base.RecvFriendCurrentPlayCount(msg);

            var gameData = stage.GetGameData<MJGameData>();

            FriendGameInfo info = gameData.friendGameInfo;

            string text = info.useCustomGameCount ?
                info.customGameCountTitle + info.customGameCount :
                $"{info.currentGameCount}/{info.totalGameCount}局";

            stage.GetController<MJTableController>().SetFriendGameCount(text);
        }
        #endregion


        private void HandleHuInfo(MsgHeader msg, MJHintsData.Type type)
        {
            MsgHuInfo data = MsgHuInfo.From(msg, type);

            var huInfoHandler = stage.GetController<MJHuInfoHandler>();

            // 更新数据
            huInfoHandler.UpdateData(data);

            // 手牌显示胡牌提示标志
            huInfoHandler.EnqueueShowHintMark();
        }


        private void HandleTingInfo(MsgHeader msg, MJHintsData.Type type)
        {
            int count = msg.ReadByte();

            var tingInfoHandler = stage.GetController<MJTingInfoHandler>();

            if (count == 0xFF)
            {
                // 清除数据及UI
                tingInfoHandler.Clear();

                return;
            }

            MJHintsData data = MJHintsData.From(msg, type, count);

            // 更新数据
            tingInfoHandler.UpdateData(data);

            // 显示听牌提示按钮
            tingInfoHandler.EnqueueShowHintButton();
        }
    }
}
