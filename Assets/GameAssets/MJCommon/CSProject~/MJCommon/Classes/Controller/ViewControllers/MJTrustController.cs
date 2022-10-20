// @Author: tanjinhua
// @Date: 2021/5/4  11:48


using Common;
using System.Collections.Generic;
using Unity.Widget;

namespace MJCommon
{
    public class MJTrustController : GameTrustController
    {
        private MJGameData _gameData;
        private MJTableController _tableController;
        private MJActionButtonController _actionButtonController;

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            _gameData = stage.gameData as MJGameData;
            _tableController = stage.GetController<MJTableController>();
            _actionButtonController = stage.GetController<MJActionButtonController>();
        }


        /// <summary>
        /// 倒计时事件，每秒执行一次
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="elapse"></param>
        public virtual void OnTimerTick(int timeout, int elapse)
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            if (playerSelf.isTrust)
            {
                _tableController.StopTimer();

                ExecuteTrustOperation();

                return;
            }

            if (timeout == elapse && !_gameData.isFriendRoom && !_gameData.trustForbidden)
            {
                _tableController.StopTimer();

                ExecuteTrustOperation();

                stage.Send("SendTrustRequest", true);
            }
        }


        /// <summary>
        /// 默认托管操作
        /// </summary>
        public virtual void ExecuteTrustOperation()
        {
            // 可以胡则自动胡牌
            if (HuActionable())
            {
                MJActionData data = _gameData.GetActionData(ActionShowType.Hu);

                stage.Send("SendAction", data.actionType, data.operateCard);

                _actionButtonController.ClearButtons();

                return;
            }

            // 退出听牌选择出牌状态
            _actionButtonController.ExitReadyHandSelectCardState();

            // 可以出牌则自动出牌
            if (OutActionable())
            {
                int cardValue = FindDiscardableCardValue();

                stage.Send("SendOutCard", cardValue, true, false);

                return;
            }

            // 有动作事件则发送放弃
            if (_gameData.HasAction() && (_gameData.playerSelf as MJGamePlayer).isTrust)
            {
                stage.Send("SendAction", ActionType.FangQi, 0);
            }
        }


        /// <summary>
        /// 是否可以胡牌
        /// </summary>
        protected virtual bool HuActionable()
        {
            return _gameData.HasAction(ActionShowType.Hu);
        }


        /// <summary>
        /// 是否可以出牌
        /// </summary>
        /// <returns></returns>
        protected virtual bool OutActionable()
        {
            if (_gameData.verifyingOutCardValue != Card.Invalid)
            {
                return false;
            }

            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            int handCardCount = playerSelf.handCardValues.Count;

            return handCardCount % 3 == 2;
        }


        /// <summary>
        /// 托管状态下找到可以自动打出的牌值
        /// </summary>
        /// <returns></returns>
        public virtual int FindDiscardableCardValue()
        {
            int dingqueCardValue = FindDingqueCardValue();

            if (dingqueCardValue != Card.Invalid)
            {
                return dingqueCardValue;
            }

            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            List<int> handCardValues = playerSelf.handCardValues;

            int lastIndex = handCardValues.Count - 1;

            int cardValue = handCardValues[lastIndex];

            if (_gameData.jokerDiscardable)
            {
                return cardValue;
            }

            if (_gameData.jokerCardValues.Contains(cardValue))
            {
                return handCardValues[lastIndex - 2];
            }

            return cardValue;
        }


        /// <summary>
        /// 反向查找定缺牌
        /// </summary>
        /// <returns></returns>
        public virtual int FindDingqueCardValue()
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            if (playerSelf.dingqueColorValue == Card.ColorNone)
            {
                return Card.Invalid;
            }

            List<int> handCardValues = playerSelf.handCardValues;

            for (int i = handCardValues.Count - 1; i >= 0; i--)
            {
                int cardValue = handCardValues[i];

                int colorValue = Card.GetCardColorValue(cardValue);

                if (colorValue == playerSelf.dingqueColorValue)
                {
                    return cardValue;
                }
            }

            return Card.Invalid;
        }

        /// <summary>
        /// 创建托管层，添加到场景并设置zorder
        /// </summary>
        /// <returns></returns>
        protected override TrustPanel OnCreateTrustPanel()
        {
            var panel = new MJTrustPanel();

            panel.AddTo(WDirector.GetRootLayer(), MJZorder.TrustPanel);

            return panel;
        }
    }
}
