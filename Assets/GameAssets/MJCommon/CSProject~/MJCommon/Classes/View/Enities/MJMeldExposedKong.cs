// MJMeldExposedKong.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/08/04

namespace MJCommon
{
    public class MJMeldExposedKong : MJMeld
    {
        public MJMeldExposedKong(int[] cards) : base(cards)
        {
            if(cards == null || cards.Length != 4)
            {
                WLDebug.LogWarning("MJMeldExposedKong param error");
            }
            gameObject.name = "MJMeldExposedKong";
        }
    }
}
