// MJMeldConcealedKong.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/07/17

using UnityEngine;

namespace MJCommon
{
    /// <summary>
    /// 暗杠副子
    /// </summary>
    public class MJMeldConcealedKong : MJMeld
    {
        /// <summary>
        /// 麻将牌的数组
        /// </summary>
        public MJCard[] cards;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cards">牌值数组</param>
        public MJMeldConcealedKong(int[] cards) : base(cards)
        {
            if (cards == null || cards.Length != 4)
            {
                WLDebug.LogWarning("MJMeldConcealedKong param error");
            }
            gameObject.name = "MJMeldConcealedKong";
        }

        protected override GameObject CreateGameObject()
        {
            return null;
        }

        protected override void Instantiate(int[] cards)
        {
            this.cards = new MJCard[cards.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                var value = cards[i];
                var card = new MJCard(value);
                this.cards[i] = card;
            }

            Style1();
        }

        public void Style1()
        {
            float x = 0;
            this.unitWidth = cards.Length;

            for (int i = 0; i < cards.Length; i++)
            {
                var card = this.cards[i];
                card.TowardDown();
                var transform = card.gameObject.transform;
                transform.SetParent(gameObject.transform);
                transform.localPosition = new Vector3(x, 0, 0);
                x += MJDefine.MJCardSizeX;
            }
        }

        public void Style2()
        {
            float x = 0;
            this.unitWidth = 3;

            for (int i = 0; i < 3; i++)
            {
                var card = cards[i];
                card.TowardDown();
                var transform = card.gameObject.transform;
                transform.SetParent(gameObject.transform);
                transform.localPosition = new Vector3(x, 0, 0);
                x += MJDefine.MJCardSizeX;
            }

            var card4 = cards[3];
            var t = card4.gameObject.transform;
            t.SetParent(gameObject.transform);
            t.localPosition = new Vector3(MJDefine.MJCardSizeX, MJDefine.MJCardSizeY, 0);
            card4.TowardUp();
        }

        public void Style3()
        {
            float x = 0;
            this.unitWidth = 4;

            for (int i = 0; i < 4; i++)
            {
                var card = cards[i];
                if (i == 0)
                {
                    card.TowardUp();
                }
                else
                {
                    card.TowardDown();
                }
                var transform = card.gameObject.transform;
                transform.SetParent(gameObject.transform);
                transform.localPosition = new Vector3(x, 0, 0);
                x += MJDefine.MJCardSizeX;
            }
        }

        public void Style4()
        {
            float x = 0;
            this.unitWidth = 3;
            for (int i = 0; i < 3; i++)
            {
                var card = cards[i];
                card.TowardDown();
                var transform = card.gameObject.transform;
                transform.SetParent(gameObject.transform);
                transform.localPosition = new Vector3(x, 0, 0);
                x += MJDefine.MJCardSizeX;
            }

            var card4 = cards[3];
            var t = card4.gameObject.transform;
            t.SetParent(gameObject.transform);
            t.localPosition = new Vector3(MJDefine.MJCardSizeX, MJDefine.MJCardSizeY, 0);
            card4.TowardDown();
        }
    }
}