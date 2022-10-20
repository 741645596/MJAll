// @Author: tanjinhua
// @Date: 2021/9/9  13:49


using MJCommon;
using UnityEngine;

namespace NS_XLHZ
{
    public class XLHZOutCardHandler : MJOutCardHandler
    {
        private ClientXLHZ _client;
        private XLHZGameData _gameData;
        private MJTrustController _trustController;

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            _client = client as ClientXLHZ;

            _gameData = client.gameData as XLHZGameData;

            _trustController = _client.trustController as MJTrustController;
        }


        public override void EnqueueSelfActionEventTimer(int timeout = -1)
        {
            timeout = timeout == -1 ? _gameData.trustTimeout : timeout;

            int viewChairId = Chair.Down;

            var actionDatas = _gameData.currentActionDatas;

            _client.animationQueue.Enqueue(0, () => _client.tableController.PlayTurn(viewChairId, timeout, elapse =>
            {
                var playerSelf = _client.playerSelf as XLHZGamePlayer;
                if (playerSelf.winCardCount > 0 && _gameData.HasAction(ActionShowType.Hu) && _gameData.currentActionDatas.Count == 1)
                {
                    _client.SendAction(actionDatas[0].actionType, actionDatas[0].operateCard);
                    _client.actionButtonController.ClearButtons();
                }
                else
                {
                    _trustController.OnTimerTick(timeout, elapse);
                }
            }));
        }


        protected override MJCard FindOutCard(int viewChairId, int cardValue, bool isReadyHand)
        {
            if (viewChairId == Chair.Down)
            {
                return _client.handController.FindCardByValue(viewChairId, cardValue, true);
            }

            var chairId = _client.ToServerChairId(viewChairId);
            var player = _client.GetPlayerByChairId(chairId) as XLHZGamePlayer;
            var hand = _client.handController.GetHandSet(viewChairId);

            int randomIndex = (isReadyHand || player.winCardCount > 0)  ? hand.count - 1 : Random.Range(0, hand.count);

            return hand.GetCard(randomIndex);
        }
    }
}
