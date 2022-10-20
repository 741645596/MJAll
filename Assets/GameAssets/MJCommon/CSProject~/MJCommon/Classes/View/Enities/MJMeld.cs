// MJMeld.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/07/15

using System.Collections.Generic;
using UnityEngine;

namespace MJCommon
{
    /// <summary>
    /// 麻将副子，每张麻将牌为该对象的子节点。
    /// </summary>
    public class MJMeld : MJCardSet
    {

        #region Definitions
        /// <summary>
        /// 显示类型，用来决定布局方式
        /// </summary>
        public enum ShowType
        {
            /// <summary>
            /// 杠副子3叠1方式布局
            /// </summary>
            Stacking,

            /// <summary>
            /// 普通副子平铺方式布局
            /// </summary>
            Tiling
        }


        /// <summary>
        /// 副子节点数据配置
        /// </summary>
        public struct Args
        {
            /// <summary>
            /// 副子类型
            /// </summary>
            public uint type;
            /// <summary>
            /// 副子提供者的视图座位号(以副子所有者视角为准)
            /// </summary>
            public int providerViewChairId;
            /// <summary>
            /// 牌值数组
            /// </summary>
            public List<int> cardValues;
            /// <summary>
            /// 牌数量数组，与牌值数组一一对应
            /// </summary>
            public List<int> multiplies;
            /// <summary>
            /// 显示类型
            /// </summary>
            public ShowType showType;
        }
        #endregion


        public ShowType showType { get; private set; }

        public bool isInvert;



        public MJMeld()
        {
            gameObject.name = "MJMeld";
        }

        public MJMeld(Args data) : this()
        {
            Initialize(data);
        }


        public MJMeld(int[] cards) : this()
        {
            showType = ShowType.Tiling;

            Reload(cards);

            Refresh();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(Args config)
        {
            showType = config.showType;

            Reload(config.cardValues.ToArray());

            Refresh();
        }

        /// <summary>
        /// 设置盖牌的材质
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="cardKey"></param>
        /// <param name="backKey"></param>
        public void SetCoverCardMaterial(string asset, string cardKey, string backKey)
        {
            Traverse((c, i) =>
            {
                if (c.cardValue == Card.Rear)
                {
                    c.SetMaterial(asset, cardKey, backKey);
                }
            });
        }

        public override Vector3 GetSize()
        {
            if (showType == ShowType.Tiling)
            {
                return new Vector3(MJDefine.MJCardSizeX * count, MJDefine.MJCardSizeY, MJDefine.MJCardSizeZ);
            }

            return new Vector3(MJDefine.MJCardSizeX * (count - 1), MJDefine.MJCardSizeY * 2, MJDefine.MJCardSizeZ);
        }

        public override void Refresh()
        {
            for (int i = 0; i < count; i++)
            {
                float dx = MJDefine.MJCardSizeX, dy = MJDefine.MJCardSizeY;
                float x = dx * 0.5f + i * dx;
                float y = dy * 0.5f;
                float z = 0;

                MJCard card = GetCard(i);

                if (showType == ShowType.Stacking && i == count - 1)
                {
                    x = dx * 1.5f;
                    y = dy * 1.5f;

                    card.HideShadow();
                }

                card.gameObject.transform.localPosition = new Vector3(x, y, z);
                // 是否反转对家的牌
                if(isInvert)
                {
                    card.gameObject.transform.localEulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    card.gameObject.transform.localEulerAngles = Vector3.zero;
                }
            }
        }
    }
}