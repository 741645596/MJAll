// @Author: tanjinhua
// @Date: 2021/8/27  19:27


using UnityEngine;
using System.Collections.Generic;
using WLCore.Entity;
using System;

namespace MJCommon
{
    /// <summary>
    /// 麻将牌的集合
    /// </summary>
    public abstract class MJCardSet : BaseEntity
    {
        /// <summary>
        /// 添加牌事件
        /// </summary>
        public Action<MJCardSet, MJCard> onAppend;

        /// <summary>
        /// 移除牌事件
        /// </summary>
        public Action<MJCardSet, MJCard> onRemove;

        /// <summary>
        /// 牌数量
        /// </summary>
        public int count => _cards.Count;

        /// <summary>
        /// 牌集所在的客户端方位
        /// </summary>
        protected MJOrientation orientation = MJOrientation.Down;

        /// <summary>
        /// 层掩码
        /// </summary>
        public int layer
        {
            get => _layer;
            set
            {
                _layer = value;
                gameObject.SetLayer(_layer);
            }
        }

        protected List<MJCard> _cards;
        private int _layer;


        public MJCardSet()
        {
            _cards = new List<MJCard>();

            _layer = gameObject.layer;
        }


        /// <summary>
        /// 遍历
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="reverse">是否反向</param>
        public void Traverse(Action<MJCard, int> handler, bool reverse = false)
        {
            if (reverse)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    handler.Invoke(GetCard(i), i);
                }

                return;
            }

            for (int i = 0; i < count; i++)
            {
                handler.Invoke(GetCard(i), i);
            }
        }


        /// <summary>
        /// 获取牌索引
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int IndexOf(MJCard card)
        {
            return _cards.IndexOf(card);
        }


        /// <summary>
        /// 根据索引获取牌对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MJCard GetCard(int index)
        {
            return _cards[index];
        }

        /// <summary>
        /// 获取所有牌对象
        /// </summary>
        /// <returns></returns>
        public List<MJCard> GetCards()
        {
            return _cards;
        }

        /// <summary>
        /// 获取最后一张牌
        /// </summary>
        /// <returns></returns>
        public MJCard Peek()
        {
            if (count == 0)
            {
                return null;
            }

            return GetCard(count - 1);
        }


        /// <summary>
        /// 使用牌值在末尾添加一个牌对象
        /// </summary>
        /// <param name="cardValue"></param>
        public virtual MJCard Append(int cardValue)
        {
            MJCard card = MJCardPool.Get<MJCard>(cardValue);

            return Append(card);
        }

        /// <summary>
        /// 添加一张牌
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public MJCard Append(MJCard card)
        {
            _cards.Add(card);

            card.gameObject.transform.SetParent(gameObject.transform, false);

            card.ShowShadow();

            onAppend?.Invoke(this, card);

            return card;
        }


        /// <summary>
        /// 使用牌值插入牌对象
        /// </summary>
        /// <param name="index"></param>
        /// <param name="cardValue"></param>
        public virtual MJCard Insert(int index, int cardValue)
        {
            MJCard card = MJCardPool.Get<MJCard>(cardValue);

            return Insert(index, card);
        }

        /// <summary>
        /// 插入一张牌
        /// </summary>
        /// <param name="index"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        public MJCard Insert(int index, MJCard card)
        {
            _cards.Insert(index, card);

            card.gameObject.transform.SetParent(gameObject.transform, false);

            card.ShowShadow();

            onAppend?.Invoke(this, card);

            return card;
        }


        /// <summary>
        /// 移除一个牌节点
        /// </summary>
        /// <param name="card"></param>
        public virtual MJCard Remove(MJCard card)
        {
            if (!_cards.Contains(card))
            {
                return null;
            }

            _cards.Remove(card);

            MJCardPool.Recycle(card);

            onRemove?.Invoke(this, card);

            return card;
        }


        /// <summary>
        /// 根据索引移除牌对象
        /// </summary>
        /// <param name="index"></param>
        public MJCard Remove(int index)
        {
            MJCard card = GetCard(index);

            return Remove(card);
        }


        /// <summary>
        /// 根据指定范围移除牌节点(包含结束索引节点)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Remove(int from, int to)
        {
            List<MJCard> clone = new List<MJCard>(_cards);

            for (int i = from; i <= to; i++)
            {
                MJCard card = clone[i];

                Remove(card);
            }
        }


        /// <summary>
        /// 移除最有一个
        /// </summary>
        public MJCard RemoveLast()
        {
            if (count == 0)
            {
                return null;
            }

            return Remove(count - 1);
        }


        /// <summary>
        /// 设置牌所在的方位并旋转方向
        /// </summary>
        /// <param name="orientation">方位</param>
        public virtual void SetOrientation(MJOrientation orientation)
        {
            this.orientation = orientation;
            var q = Quaternion.Euler(0, (int)orientation * -90, 0);
            gameObject.transform.localRotation = q;
        }

        /// <summary>
        /// 获取牌集所在客户端方位
        /// </summary>
        /// <returns></returns>
        public MJOrientation GetOrientation()
        {
            return orientation;
        }

        /// <summary>
        /// 获取尺寸
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 GetSize() => Vector3.zero;


        /// <summary>
        /// 刷新布局
        /// </summary>
        public abstract void Refresh();


        /// <summary>
        /// 重新加载整个合集
        /// </summary>
        /// <param name="cardValues"></param>
        public void Reload(int[] cardValues)
        {
            Clear();

            for (int i = 0; i < cardValues.Length; i++)
            {
                Append(cardValues[i]);
            }
        }

        /// <summary>
        /// 同上
        /// </summary>
        /// <param name="cardValues"></param>
        public void Reload(List<int> cardValues)
        {
            Reload(cardValues.ToArray());
        }

        /// <summary>
        /// 清除合集
        /// </summary>
        public void Clear()
        {
            for (int i = count - 1; i >= 0; i--)
            {
                Remove(i);
            }
        }


        protected override GameObject CreateGameObject()
        {
            return new GameObject("MJCardSet");
        }
    }
}