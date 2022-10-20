// MJMeldChow.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/08/04

namespace MJCommon
{
    public class MJMeldChow : MJMeld
    {
        public MJMeldChow(int[] cards) : base(cards)
        {
            if(cards == null || cards.Length != 3)
            {
                WLDebug.LogWarning("MJMeldChow param error.");
            }

            gameObject.name = "MJMeldChow";
        }
    }
}
