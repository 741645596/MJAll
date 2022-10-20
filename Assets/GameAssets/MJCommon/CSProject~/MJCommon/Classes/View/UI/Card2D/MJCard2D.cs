// @Author: tanjinhua
// @Date: 2021/9/6  10:15

using Unity.Widget;

namespace MJCommon
{
    public abstract class MJCard2D : WNode
    {

        public int cardValue;

        public MJCard2D(int value)
        {
            cardValue = value;

            OnInit();
        }


        protected abstract void OnInit();
    }
}
