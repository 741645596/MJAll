// MJMeldPong.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/07/17

using System.Collections.Generic;
using UnityEngine;

namespace MJCommon
{
    /// <summary>
    /// 碰副子
    /// </summary>
    public class MJMeldPong : MJMeld
    {
        /// <summary>
        /// 麻将牌集合
        /// </summary>
        private readonly List<MJCard> cards = new List<MJCard>();
        /// <summary>
        /// 提供者和自己服务器座位号的差值
        /// 0: 不横排
        /// 1: 第三张横排
        /// 2: 第二张横排
        /// 3: 第一张横排
        /// </summary>
        private int providerIndex = 0;

        /// <summary>
        /// 构造方法，创建碰副子
        /// </summary>
        /// <param name="cardValues"></param>
        public MJMeldPong(int[] cardValues, int provider = 0) : base(cardValues)
        {
            providerIndex = provider;
            gameObject.name = "MJMeldPong";
            Refresh();
        }

        /// <summary>
        /// 补杠
        /// </summary>
        public void CollectedKong()
        {
            if (cards.Count != 3)
            {
                return;
            }

            var value = cards[0].cardValue;
            var card = new MJCard(value);
            card.SetParent(this);
            cards.Add(card);
            Refresh();
        }

        /// <summary>
        /// 获取碰牌牌值
        /// </summary>
        /// <returns></returns>
        public int GetCardValue()
        {
            return cards[0].cardValue;
        }

        /// <summary>
        /// 获取麻将牌尺寸
        /// </summary>
        /// <returns></returns>
        public override Vector3 GetSize()
        {
            var x = MJDefine.MJCardSizeX + MJDefine.MJCardSizeX + MJDefine.MJCardSizeZ;
            var y = MJDefine.MJCardSizeY;
            var z = MJDefine.MJCardSizeX + MJDefine.MJCardSizeX;

            if (providerIndex == 0)
            {
                Debug.Log($"count={cards.Count}");
                x = cards.Count * MJDefine.MJCardSizeX;
                z = MJDefine.MJCardSizeZ;
            }
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected void Refresh()
        {
            float x = 0;
            var index = 3 - providerIndex;
            for (int i = 0; i < 3; i++)
            {
                var card = cards[i];
                card.TowardUp();

                var transform = card.gameObject.transform;

                if(i == index)
                {
                    x += (MJDefine.MJCardSizeZ - MJDefine.MJCardSizeX) * 0.5f;
                    float z = -(MJDefine.MJCardSizeZ - MJDefine.MJCardSizeX) * 0.5f;
                    transform.localPosition = new Vector3(x, 0, z);
                    card.SetOrientation(MJOrientation.Right);
                    x += (MJDefine.MJCardSizeZ - MJDefine.MJCardSizeX) * 0.5f;
                }
                else
                {
                    transform.localPosition = new Vector3(x, 0, 0);
                }

                x += MJDefine.MJCardSizeX;
            }

            // 补杠
            if(cards.Count == 4)
            {
                if (providerIndex != 0)
                {
                    x = index * MJDefine.MJCardSizeX + (MJDefine.MJCardSizeZ - MJDefine.MJCardSizeX) * 0.5f;
                    float z = -(MJDefine.MJCardSizeZ - MJDefine.MJCardSizeX) * 0.5f;
                    z += MJDefine.MJCardSizeX;

                    var card = cards[3];
                    var transform = card.gameObject.transform;
                    transform.localPosition = new Vector3(x, 0, z);
                    card.TowardUp();
                    card.SetOrientation(MJOrientation.Right);
                }
                else
                {
                    var card = cards[3];
                    var transform = card.gameObject.transform;
                    transform.localPosition = new Vector3(x, 0, 0);
                    card.TowardUp();
                }
            }
        }

        /// <summary>
        /// 重写实例化并刷新位置
        /// </summary>
        /// <param name="cardValues"></param>
        protected override void Instantiate(int[] cardValues)
        {
            for (int i = 0; i < cardValues.Length; i++)
            {
                var value = cardValues[i];
                var card = new MJCard(value);
                cards.Add(card);
                card.SetParent(this);
            }

            Refresh();
        }

    }
}