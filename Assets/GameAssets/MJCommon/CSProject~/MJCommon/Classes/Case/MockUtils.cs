// @Author: tanjinhua
// @Date: 2021/9/3  18:37

using WLHall;
using UnityEngine;
using System.Collections.Generic;
using WLHall.Game;

namespace MJCommon
{
    public static class MockUtils
    {
        private static int[] _values = new int[]
        {
            // 万
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,

            // 条
            0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
            0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
            0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
            0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,

            // 筒
            0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29,
            0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29,
            0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29,
            0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29,

            // 风牌（东南西北）
            0x31, 0x32, 0x33, 0x34,
            0x31, 0x32, 0x33, 0x34,
            0x31, 0x32, 0x33, 0x34,
            0x31, 0x32, 0x33, 0x34,

            // 箭牌（中发白）
            0x35, 0x36, 0x37,
            0x35, 0x36, 0x37,
            0x35, 0x36, 0x37,
            0x35, 0x36, 0x37,

            // 花牌（春夏秋冬梅兰竹菊）
            //0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48,
        };

        public static List<int> Shuffle()
        {
            List<int> pool = new List<int>(_values);

            List<int> shuffled = new List<int>();

            int count = pool.Count;

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, pool.Count);

                int value = pool[index];

                shuffled.Add(value);

                pool.RemoveAt(index);
            }

            return shuffled;
        }

        public static MJActionData MockActionData()
        {
            return new MJActionData
            {
                actionType = Random.Range(0, 255),
                showType = Random.Range(2, 4),
                cardValues = MockRandomCardValues(Random.Range(3, 5)).ToArray()
            };
        }

        public static List<MJActionData> MockActionDatas(int count)
        {
            var result = new List<MJActionData>();

            for (int i = 0; i < count; i++)
            {
                result.Add(MockActionData());
            }

            return result;
        }

        public static int MockRandomCardValue()
        {
            int index = Random.Range(0, _values.Length);

            return _values[index];
        }

        public static List<int> MockRandomCardValues(int count)
        {
            var values = new List<int>();

            for (int i = 0; i < count; i++)
            {
                values.Add(MockRandomCardValue());
            }

            return values;
        }

        public static MJHintsData MockHintsData()
        {
            MJHintsData data = new MJHintsData();

            if (Random.Range(0f, 1f) > 0.7f)
            {
                return data;
            }

            data.type = (MJHintsData.Type)Random.Range(0, 3);

            int count = Random.Range(1, 20);

            data.cardValues = new List<int>();
            data.multiplies = new List<int>();
            data.descriptions = new List<string>();

            string[] descriptions = new string[]
            {
                "平胡", "夹胡", "大对子", "屁胡", "抢杠", "清龙", "大三元"
            };

            for (int i = 0; i < count; i++)
            {
                data.cardValues.Add(MockRandomCardValue());
                data.multiplies.Add(Random.Range(0, 255));
                data.descriptions.Add(descriptions[Random.Range(0, descriptions.Length)]);
            }

            return data;
        }

        public static FuziData MockPongFuzi(int cardValue, ushort provideChairId)
        {
            return new FuziData
            {
                weaveKind1 = ActionType.Peng,
                provideUser = provideChairId,
                tiHuiCard = Card.Invalid,
                operateCard = cardValue,
                cardValues = new int[] {cardValue, cardValue, cardValue, Card.Invalid},
                cardCounts = new int[] {1, 1, 1, 0},
                yaoJiCard = 0
            };
        }

        public static FuziData MockExposedKongFuzi(int cardValue, ushort provideChairId)
        {
            return new FuziData
            {
                weaveKind1 = ActionType.GangMing,
                provideUser = provideChairId,
                tiHuiCard = Card.Invalid,
                operateCard = cardValue,
                cardValues = new int[] { cardValue, cardValue, cardValue, cardValue },
                cardCounts = new int[] { 1, 1, 1, 1 },
                yaoJiCard = 0
            };
        }

        public static FuziData MockConcealedKongFuzi(int cardValue, ushort provideChairId)
        {
            return new FuziData
            {
                weaveKind1 = ActionType.GangAn,
                provideUser = provideChairId,
                tiHuiCard = Card.Invalid,
                operateCard = cardValue,
                cardValues = new int[] { Card.Rear, Card.Rear, Card.Rear, cardValue },
                cardCounts = new int[] { 1, 1, 1, 1 },
                yaoJiCard = 0
            };
        }

        public static FuziData MockFourWindKongFuzi()
        {
            return new FuziData
            {
                weaveKind1 = ActionType.Gang4Feng,
                provideUser = Chair.Invalid,
                operateCard = Card.Bei,
                cardValues = new int[] {Card.Dong, Card.Nan, Card.Xi, Card.Bei},
                cardCounts = new int[] {1, 1, 1, 1},
                yaoJiCard = 0
            };
        }


        public static List<FuziData> MockRandomFuziDatas(int count, int playerCount = 4)
        {
            var result = new List<FuziData>();
            for (int i = 0; i < count; i++)
            {
                var random = Random.Range(0f, 1f);
                var value = MockRandomCardValue();
                if (random < 0.25f)
                {
                    result.Add(MockPongFuzi(value, (ushort)Random.Range(0, playerCount)));
                }
                else if (random < 0.5f)
                {
                    result.Add(MockExposedKongFuzi(value, (ushort)Random.Range(0, playerCount)));
                }
                else if (random < 0.75f)
                {
                    result.Add(MockConcealedKongFuzi(value, (ushort)Random.Range(0, playerCount)));
                }
                else
                {
                    result.Add(MockFourWindKongFuzi());
                }
            }
            return result;
        }


        public static T MockPlayer<T>(ushort chairId) where T : BaseGamePlayer, new()
        {
            var player = new T();

            player.userInfo = new UserInfo
            {
                id = chairId,
                nickName = chairId.ToString(),
                avatarPath = ""
            };

            player.userGameInfo = new UserGameInfo
            {
                wChairID = chairId
            };

            return player;
        }
    }
}
