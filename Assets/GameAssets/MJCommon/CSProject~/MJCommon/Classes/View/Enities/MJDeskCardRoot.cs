// MJDeskCardRoot.cs
// Author: shihongyang shihongyang0218@gmail.com
// Data: 2021/8/5

using System.Collections.Generic;
using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{
    public class MJDeskCardRoot : BaseEntity
    {
        /// <summary>
        /// 桌牌的集合，key：客户端方位，value：MJDeskCardSet
        /// </summary>
        protected Dictionary<MJOrientation, MJDeskCardSet> deskCards;

        /// <summary>
        /// 桌面上打出的麻将牌顺序的历史记录
        /// </summary>
        protected Stack<MJOrientation> localHistory;

        public MJDeskCardRoot(MJDeskCardConfig config)
        {
            if(config == null && config.position == null)
            {
                WLDebug.LogWarning("MJDeskCardRoot config error");
                return;
            }
            gameObject = new GameObject("MJDeskCardRoot");

            deskCards = new Dictionary<MJOrientation, MJDeskCardSet>();
            localHistory = new Stack<MJOrientation>();

            var count = config.position.Length;
            for (int i = 0; i < count; i++)
            {
                var orientation = (MJOrientation)i;
                var pos = config.position[i].ToVector3();

                var desk_card_set = new MJDeskCardSet();
                desk_card_set.SetOrientation(orientation);
                desk_card_set.UpdateConfig(config);
                desk_card_set.gameObject.transform.localPosition = pos;
                var scale = (float)config.scale[i];
                desk_card_set.gameObject.transform.localScale = new Vector3(scale, 1, scale);
                desk_card_set.SetParent(this);

                deskCards.Add(orientation, desk_card_set);
            }
        }

        /// <summary>
        /// 添加一张桌面牌
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="value"></param>
        public void PushCard(MJOrientation orientation, int value)
        {
            if (deskCards.ContainsKey(orientation) == false)
            {
                Debug.LogWarning($"MJDeskCards PushCard Error orientation={orientation} value={value}");
                return;
            }

            var sideCards = deskCards[orientation];
            sideCards.Append(value);
            sideCards.Refresh();

            localHistory.Push(orientation);
        }

        /// <summary>
        /// 撤销上一张出牌
        /// </summary>
        public void PopCard()
        {
            if (localHistory.Count == 0)
            {
                Debug.LogWarning("MJDeskCards PopCard Error localHistory.Count == 0");
                return;
            }
            var orientation = localHistory.Pop();

            if (deskCards.ContainsKey(orientation) == false)
            {
                Debug.LogWarning($"MJDeskCards PopCard Error orientation={orientation}");
                return;
            }
            var sideCards = deskCards[orientation];
            sideCards.RemoveLast();
        }


        /// <summary>
        /// 设置上一次出牌的玩家位置
        /// </summary>
        /// <param name="orientation"></param>
        public void SetLastHistory(MJOrientation orientation)
        {
            if (localHistory.Count == 0)
            {
                localHistory.Push(orientation);
                return;
            }
            else
            {
                // 替换最后一个
                localHistory.Pop();
                localHistory.Push(orientation);
            }
        }

        /// <summary>
        /// 获取牌组
        /// </summary>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public MJDeskCardSet GetDeskCardSet(MJOrientation orientation)
        {
            return deskCards[orientation];
        }

        public Dictionary<MJOrientation, MJDeskCardSet> GetDeskCardSets()
        {
            return deskCards;
        }

        protected override GameObject CreateGameObject()
        {
            return null;
        }
    }
}
