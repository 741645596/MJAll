// @Author: tanjinhua
// @Date: 2021/12/24  10:08

using UnityEngine;
using System.Collections.Generic;

namespace MJCommon
{
    public static class MJCardPool
    {
        private static Transform _container;
        private static Transform container
        {
            get
            {
                if (_container == null)
                {
                    _container = new GameObject("mj_card_pool").transform;
                    _container.gameObject.SetActive(false);
                    _container.hideFlags = HideFlags.HideInHierarchy;
                }
                return _container;
            }
        }

        private static Dictionary<string, Dictionary<int, List<MJCard>>> _pool;
        private static Dictionary<string, Dictionary<int, List<MJCard>>> pool
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new Dictionary<string, Dictionary<int, List<MJCard>>>();
                }
                return _pool;
            }
        }

        /// <summary>
        /// 获取牌
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        public static T Get<T>(int cardValue) where T : MJCard, new()
        {
            var key = typeof(T).Name;

            if (pool.ContainsKey(key))
            {
                var map = pool[key];

                if (map.ContainsKey(cardValue))
                {
                    var list = map[cardValue];

                    if (list.Count > 0)
                    {
                        var card = list[0];
                        if (!card.gameObject.activeSelf)
                        {
                            card.gameObject.SetActive(true);
                        }
                        list.RemoveAt(0);
                        return card as T;
                    }
                }
            }

            var newCard = new T();
            newCard.Initialize(cardValue);
            return newCard;
        }

        /// <summary>
        /// 回收牌
        /// </summary>
        /// <param name="card"></param>
        public static void Recycle(MJCard card)
        {
            card.gameObject.transform.SetParent(container);
            card.OnRecycle();

            var key = card.GetType().Name;

            Dictionary<int, List<MJCard>> map;
            if (pool.ContainsKey(key))
            {
                map = pool[key];
            }
            else
            {
                map = new Dictionary<int, List<MJCard>>();
            }

            List<MJCard> list;
            if (map.ContainsKey(card.cardValue))
            {
                list = map[card.cardValue];
            }
            else
            {
                list = new List<MJCard>();
            }

            list.Add(card);

            map[card.cardValue] = list;

            pool[key] = map;
        }

        /// <summary>
        /// 清理
        /// </summary>
        public static void Clear()
        {
            _pool?.Clear();
            _pool = null;

            if (_container != null)
            {
                Object.Destroy(_container.gameObject);
                _container = null;
            }
        }
    }
}
