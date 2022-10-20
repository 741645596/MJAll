// @Author: tanjinhua
// @Date: 2021/10/21  13:38

using MJCommon;

namespace NS_XLHZ
{
    public class XLHZSendCardHandler : MJSendCradHandler
    {

        private ClientXLHZ _client;
        private XLHZGameData _gameData;

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            _client = client as ClientXLHZ;

            _gameData = client.gameData as XLHZGameData;
        }

        protected override bool CanAutoOutCard(int chairId)
        {
            MJGamePlayer playerSelf = _client.playerSelf as MJGamePlayer;

            bool isSelf = chairId == playerSelf.chairId;

            bool allowAuto = playerSelf.isReadyHand || _client.hintsController.allowAuto;

            bool noActions = !_gameData.HasAction();

            bool discardable = playerSelf.handCardCount % 3 == 2;

            return isSelf && noActions && discardable && (allowAuto || playerSelf.winCardCount > 0);
        }

        protected override bool CanAutoHu()
        {
            var playerSelf = _client.playerSelf as XLHZGamePlayer;

            return playerSelf.winCardCount > 0 && _gameData.HasAction(ActionShowType.Hu) && _gameData.currentActionDatas.Count == 1;
        }
    }
}
