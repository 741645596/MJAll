// MJWinCardSet.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/9/8

using UnityEngine;

namespace MJCommon
{
    public class MJWinCardSet : MJCardSet
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public MJWinCardSet()
        {
            gameObject.name = "MJWinCardSet";
        }

        /// <summary>
        /// 添加一张牌
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        public override MJCard Append(int cardValue)
        {
            MJCard card = MJCardPool.Get<MJCard>(cardValue);

            _cards.Add(card);

            card.gameObject.transform.SetParent(gameObject.transform, false);

            Refresh();
            return card;
        }

        public override void Refresh()
        {
            for (int i = 0; i < count; i++)
            {
                MJCard card = GetCard(i);
                card.gameObject.transform.localPosition = ComputePosition(i, out bool needsShowShadow);
                card.gameObject.transform.localEulerAngles = Vector3.zero;
                if(needsShowShadow)
                {
                    card.ShowShadow();
                }
            }
        }

        public Vector3 ComputePosition(int index, out bool needsShowShadow)
        {
            if (orientation == MJOrientation.Down || orientation == MJOrientation.Up)
            {
                var height = Mathf.FloorToInt(index / (float)4);
                var col = index % 4;

                float x = MJDefine.MJCardSizeX * 0.5f + col * MJDefine.MJCardSizeX;
                float y = MJDefine.MJCardSizeY * 0.5f + height * MJDefine.MJCardSizeY;
                float z = -MJDefine.MJCardSizeZ * 0.5f + height * 0.0011f * (orientation == MJOrientation.Down ? -1 : 1);

                needsShowShadow = height == 0;

                return new Vector3(x, y, z);
            }
            else if (orientation == MJOrientation.Right)
            {
                var height = Mathf.FloorToInt(index / 6f);
                var col = index % 3;
                var row = Mathf.FloorToInt((index % 6) / 3f);

                float x = MJDefine.MJCardSizeX * 0.5f + col * MJDefine.MJCardSizeX - height * 0.0011f;
                float y = MJDefine.MJCardSizeY * 0.5f + height * MJDefine.MJCardSizeY;
                float z = -MJDefine.MJCardSizeZ * 0.5f - row * MJDefine.MJCardSizeZ;

                needsShowShadow = height == 0f;

                return new Vector3(x, y, z);
            }
            else
            {
                var height = Mathf.FloorToInt(index / 6f);
                var col = index % 2;
                var row = Mathf.FloorToInt((index % 6) / 2f);

                float x = MJDefine.MJCardSizeX * 0.5f + col * MJDefine.MJCardSizeX + height * 0.0011f;
                float y = MJDefine.MJCardSizeY * 0.5f + height * MJDefine.MJCardSizeY;
                float z = -MJDefine.MJCardSizeZ * 0.5f - row * MJDefine.MJCardSizeZ;

                needsShowShadow = height == 0;

                return new Vector3(x, y, z);
            }
        }
    }
}
