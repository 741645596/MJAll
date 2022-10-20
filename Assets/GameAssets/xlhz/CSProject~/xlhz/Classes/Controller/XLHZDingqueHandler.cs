// @Author: tanjinhua
// @Date: 2021/9/10  13:44

using System.Collections.Generic;
using MJCommon;

namespace NS_XLHZ
{
    public class XLHZDingqueHandler : MJDingqueHandler
    {
        private ClientXLHZ _client;

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            _client = client as ClientXLHZ;
        }

        public override void UpdateSelfHandByDingque()
        {
            XLHZGamePlayer playerSelf = _client.playerSelf as XLHZGamePlayer;

            _client.handController.ResetTint(Chair.Down);

            int dingqueValue = playerSelf.dingqueColorValue;
            if (dingqueValue == Card.ColorNone)
            {
                return;
            }

            List<int> values = _client.handController.GetValues(Chair.Down);

            List<int> dingqueCardValues = values.Fetch(v => Card.GetCardColorValue(v) == dingqueValue);

            if (dingqueCardValues.Count == 0)
            {
                return;
            }

            _client.handController.DarkenByValues(Chair.Down, dingqueCardValues);
        }
    }
}
