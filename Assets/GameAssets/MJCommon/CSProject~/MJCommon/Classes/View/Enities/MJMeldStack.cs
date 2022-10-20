// MJMeldStack.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/07/17

using System;
using System.Collections.Generic;
using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{
    /// <summary>
    /// 副子的堆栈
    /// </summary>
    public class MJMeldStack : BaseEntity
    {
        /// <summary>
        /// 添加副子事件
        /// </summary>
        public Action<MJMeldStack, MJMeld> onAppend;

        /// <summary>
        /// 移除副子事件
        /// </summary>
        public Action<MJMeldStack, MJMeld> onRemove;

        /// <summary>
        /// 替换副子事件
        /// </summary>
        public Action<MJMeldStack, MJMeld> onReplace;


        public int count => melds.Count;

        /// <summary>
        /// 方位
        /// </summary>
        protected MJOrientation orientation;
        /// <summary>
        /// 堆栈集合
        /// </summary>
        protected List<MJMeld> melds;

        /// <summary>
        /// 副子间隔
        /// </summary>
        protected float interval;
        /// <summary>
        /// 是否反转对家的手牌
        /// </summary>
        protected bool isInvert;

        /// <summary>
        /// 副子排列方向
        /// </summary>
        protected MeldStackDirection direction;

        /// <summary>
        /// 构造方法
        /// </summary>
        public MJMeldStack()
        {
            melds = new List<MJMeld>();
        }

        public void UpdateConfig(MJMeldConfig config)
        {
            interval = (float)config.interval;
            direction = config.direction;
            isInvert = config.invert;
            Refresh();
        }

        /// <summary>
        /// 添加副子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public MJMeld Append(MJMeld.Args data)
        {
            var meld = new MJMeld(data);

            return Append(meld);
        }


        public MJMeld Append(MJMeld meld)
        {
            melds.Add(meld);
            if(orientation == MJOrientation.Up)
            {
                meld.isInvert = isInvert;
            }

            meld.gameObject.transform.SetParent(gameObject.transform, false);

            if (orientation != MJOrientation.Down)
            {
                var assetName = "MJCommon/MJ/mj_cards";
                var cardKey = $"materials/mj_card_wall_{(int)orientation}.mat";
                var backKey = $"materials/mj_back_wall_{(int)orientation}.mat";
                meld.SetCoverCardMaterial(assetName, cardKey, backKey);
            }

            Refresh();

            onAppend?.Invoke(this, meld);

            return meld;
        }

        /// <summary>
        /// 插入副子
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public MJMeld Insert(int index, MJMeld.Args data)
        {
            var meld = new MJMeld(data);

            melds.Insert(index, meld);

            meld.gameObject.transform.SetParent(gameObject.transform, false);

            if (orientation != MJOrientation.Down)
            {
                var assetName = "MJCommon/MJ/mj_cards";
                var cardKey = $"materials/mj_card_wall_{(int)orientation}.mat";
                var backKey = $"materials/mj_back_wall_{(int)orientation}.mat";
                meld.SetCoverCardMaterial(assetName, cardKey, backKey);
            }

            Refresh();

            onAppend?.Invoke(this, meld);

            return meld;
        }

        /// <summary>
        /// 移除副子
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            var meld = GetMeld(index);

            melds.Remove(meld);

            meld.Traverse((card, i) => MJCardPool.Recycle(card));

            meld.Destroy();

            Refresh();

            onRemove?.Invoke(this, meld);
        }

        /// <summary>
        /// 移除最后一个副子
        /// </summary>
        public void RemoveLast()
        {
            Remove(count - 1);
        }

        /// <summary>
        /// 获取副子
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MJMeld GetMeld(int index)
        {
            return melds[index];
        }

        /// <summary>
        /// 替换副子
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public MJMeld Replace(int index, MJMeld.Args data)
        {
            var toReplace = GetMeld(index);

            toReplace.Traverse((card, i) => MJCardPool.Recycle(card));

            toReplace.Destroy();

            var meld = new MJMeld(data);

            melds[index] = meld;

            meld.gameObject.transform.SetParent(gameObject.transform, false);

            if (orientation != MJOrientation.Down)
            {
                var assetName = "MJCommon/MJ/mj_cards";
                var cardKey = $"materials/mj_card_wall_{(int)orientation}.mat";
                var backKey = $"materials/mj_back_wall_{(int)orientation}.mat";
                meld.SetCoverCardMaterial(assetName, cardKey, backKey);
            }

            Refresh();

            onReplace?.Invoke(this, meld);

            return meld;
        }

        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="meld"></param>
        /// <returns></returns>
        public int IndexOf(MJMeld meld)
        {
            return melds.IndexOf(meld);
        }

        /// <summary>
        /// 重新加载所有副子
        /// </summary>
        /// <param name="datas"></param>
        public void Reload(List<MJMeld.Args> datas)
        {
            Clear();

            datas.ForEach(data => Append(data));

            Refresh();
        }

        /// <summary>
        /// 清除所有副子
        /// </summary>
        public void Clear()
        {
            for (int i = count - 1; i >= 0; i--)
            {
                Remove(i);
            }
        }


        /// <summary>
        /// 遍历
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="reverse"></param>
        public void Traverse(Action<MJMeld, int> handler, bool reverse = false)
        {
            if (reverse)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    handler.Invoke(GetMeld(i), i);
                }

                return;
            }

            for (int i = 0; i < count; i++)
            {
                handler.Invoke(GetMeld(i), i);
            }
        }

        /// <summary>
        /// 设置牌墙所在的方位并旋转方向
        /// </summary>
        /// <param name="orientation">方位</param>
        public virtual void SetOrientation(MJOrientation orientation)
        {
            this.orientation = orientation;
            var q = Quaternion.Euler(0, (int)orientation * -90, 0);
            gameObject.transform.rotation = q;
        }

        /// <summary>
        /// 刷新位置
        /// </summary>
        public void Refresh()
        {
            if (count == 0)
            {
                return;
            }

            float x = direction == MeldStackDirection.LeftToRight ? 0 : -melds[0].GetSize().x - interval;

            for (int i = 0; i < count; i++)
            {
                var meld = melds[i];
                var size = meld.GetSize();

                meld.gameObject.transform.localPosition = new Vector3(x, 0, 0);
                meld.gameObject.transform.localEulerAngles = Vector3.zero;

                if (direction == MeldStackDirection.LeftToRight)
                {
                    x += size.x + interval;
                }

                meld.Refresh();
            }
        }

        protected override GameObject CreateGameObject()
        {
            return new GameObject("MJMeldStack"); ;
        }
    }
}