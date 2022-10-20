// @Author: tanjinhua
// @Date: 2021/9/8  20:53


using MJCommon;

namespace NS_XLHZ
{
    public class XLHZHandController : MJHandController
    {
        private ClientXLHZ _client;
        private XLHZGameData _gameData;

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            _client = client as ClientXLHZ;

            _gameData = client.gameData as XLHZGameData;
        }

        protected override MJHandSet.State OnGetCardState(MJHandSet sender, int index)
        {
            int viewChairId = IndexOf(sender);

            if (viewChairId == Chair.Down)
            {
                return MJHandSet.State.Standing;
            }

            int chairId = _client.ToServerChairId(viewChairId);
            XLHZGamePlayer player = _client.GetPlayerByChairId(chairId) as XLHZGamePlayer;
            if (player.winCardCount > 0 && (sender.count % 3 != 2 || index != sender.count - 1))
            {
                return MJHandSet.State.Covering;
            }

            return base.OnGetCardState(sender, index);
        }

        protected override bool Discardable(MJHandSet hand, MJCard card)
        {
            if (_gameData.verifyingOutCardValue != Card.Invalid)
            {
                return false;
            }

            return hand.count % 3 == 2;
        }
    }
}
