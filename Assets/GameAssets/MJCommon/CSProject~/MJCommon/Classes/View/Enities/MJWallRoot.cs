// MJWallRoot.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/5

using System.Collections.Generic;
using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{
    /// <summary>
    /// 麻将牌墙根节点
    /// </summary>
    public class MJWallRoot : BaseEntity
    {
        public MJWallConfig config;

        private MJWall[] walls;
        private List<MJStack> allStack;
        private int currentStackIndex;
        private int currentStackIndexReversed;

        public MJWallRoot(MJWallConfig cfg)
        {
            gameObject.name = "MJWallRoot";

            if (cfg == null || cfg.stackCountPerSide == null)
            {
                WLDebug.LogWarning("MJWallRoot config error");
                return;
            }

            config = cfg;

            walls = new MJWall[cfg.stackCountPerSide.Count];

            var stackCount = 0;
            for (int i = 0; i < cfg.stackCountPerSide.Count; i++)
            {
                MJOrientation orientation = (MJOrientation)i;
                walls[i] = new MJWall();
                walls[i].SetOrientation(orientation);

                var size = walls[i].GetSize();
                var distance = (float)config.centreDistance;
                var position = CalcWallPostion(orientation, size, distance);

                walls[i].gameObject.transform.localPosition = position;
                walls[i].SetParent(this);

                stackCount += config.stackCountPerSide[i];
            }
        }

        public void InitializeWalls(MJWallConfig wallConfig, int bankerViewChairId, int dice1 = 1, int dice2 = 1, bool visible = true)
        {
            config = wallConfig;

            List<int> orderedStackCounts = ToViewOrder(bankerViewChairId, wallConfig.stackCountPerSide);

            for (int i = 0; i < orderedStackCounts.Count; i++)
            {
                int stackCount = orderedStackCounts[i];

                if (stackCount == 0)
                {
                    continue;
                }

                MJWall wall = walls[i];

                wall.Initialize(stackCount, wallConfig.countPerStack, wallConfig.takeCardClockwise);

                wall.gameObject.SetActive(visible);
            }

            SetupStackList(wallConfig, bankerViewChairId, dice1, dice2);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public MJWallConfig GetConfig()
        {
            return config;
        }

        /// <summary>
        /// 获取全部牌墙
        /// </summary>
        /// <returns></returns>
        public MJWall[] GetWallArray()
        {
            return walls;
        }
        /// <summary>
        /// 获取全部牌墩
        /// </summary>
        /// <returns></returns>
        public List<MJStack> GetAllStack()
        {
            return allStack;
        }

        /// <summary>
        /// 取牌
        /// </summary>
        /// <param name="reverse"></param>
        public void Take(bool reverse = false)
        {
            if (reverse)
            {
                RemoveReversely();
                return;
            }

            Remove();
        }

        /// <summary>
        /// 取多张牌
        /// </summary>
        /// <param name="count"></param>
        /// <param name="reverse"></param>
        public void Take(int count, bool reverse = false)
        {
            for (int i = 0; i < count; i++)
            {
                Take(reverse);
            }
        }

        /// <summary>
        /// 根据指定牌墩索引取牌
        /// </summary>
        /// <param name="stackIndex"></param>
        /// <param name="takeCount"></param>
        /// <param name="reverse"></param>
        public void Take(int stackIndex, int takeCount, bool reverse = false)
        {
            int index = reverse ? allStack.Count - 1 - stackIndex : stackIndex;

            MJStack stack = allStack[index];

            if (stack.empty)
            {
                return;
            }

            int count = stack.count >= takeCount ? takeCount : stack.count;

            stack.Remove(0, count - 1);
        }

        /// <summary>
        /// 恢复一张牌
        /// </summary>
        /// <param name="reverse"></param>
        public void Recover(bool reverse = false)
        {
            if (reverse)
            {
                RecoverReversely();
            }
            else
            {
                Recover();
            }
        }

        /// <summary>
        /// 根据指定牌墩和牌索引替换一张牌
        /// </summary>
        /// <param name="stackIndex"></param>
        /// <param name="cardIndex"></param>
        /// <param name="cardValue"></param>
        /// <param name="reverse"></param>
        public void Replace(int stackIndex, int cardIndex, int cardValue, bool reverse = false)
        {
            int index = reverse ? allStack.Count - 1 - stackIndex : stackIndex;
            MJStack stack = allStack[index];
            if (stack.empty)
            {
                return;
            }

            MJCard toReplace = stack.GetCard(cardIndex);
            Vector3 pos = toReplace.gameObject.transform.localPosition;
            Vector3 euler = toReplace.gameObject.transform.localEulerAngles;
            stack.Remove(cardIndex);
            MJCard card = stack.Insert(cardIndex, cardValue);
            card.gameObject.transform.localPosition = pos;
            card.gameObject.transform.localEulerAngles = euler;
        }

        /// <summary>
        /// 批量取牌墩
        /// </summary>
        /// <param name="stackCount">牌墩的数量</param>
        public void TakeStack(int stackCount)
        {
            int count = stackCount * config.countPerStack;

            Take(count);
        }

        /// <summary>
        /// 计算牌墙的位置，每个方向居中
        /// </summary>
        /// <param name="orientation">方位</param>
        /// <param name="size">牌墙尺寸</param>
        /// <param name="centreDistance">距离中心点的垂直距离</param>
        /// <returns></returns>
        public Vector3 CalcWallPostion(MJOrientation orientation, Vector3 size, float centreDistance)
        {
            switch (orientation)
            {
                case MJOrientation.Down:
                    {
                        float z = -centreDistance;
                        return new Vector3(0, 0, z);
                    }
                case MJOrientation.Right:
                    {
                        float x = centreDistance;
                        return new Vector3(x, 0, 0);
                    }
                case MJOrientation.Up:
                    {
                        float z = centreDistance;
                        return new Vector3(0, 0, z);
                    }
                case MJOrientation.Left:
                    {
                        float x = -centreDistance;
                        return new Vector3(x, 0, 0);
                    }
                default:
                    return Vector3.zero;
            }
        }



        #region calculations
        private void SetupStackList(MJWallConfig wallConfig, int bankerViewChairId, int dice1, int dice2)
        {
            allStack = new List<MJStack>();

            int diceTotal = wallConfig.diceUsage != DiceUsage.One ? dice1 + dice2 : dice1;
            int minDice = wallConfig.diceUsage != DiceUsage.One ? Mathf.Min(dice1, dice2) : dice1;
            bool diceClockwise = wallConfig.diceClockwise;
            bool takeCardClockwise = wallConfig.takeCardClockwise;

            // 获取有效的开始座位号，即当庄家位置没有牌墙时，按指定方向往下找
            int startingChair = GetStartingChair(bankerViewChairId, diceClockwise);

            // 根据开始座位号重新排序牌墙
            List<MJWall> orderedWalls = GetOrderedWalls(startingChair);

            // 根据骰子值和指定方向获取开始牌墙的索引
            int startingWallIndex = GetStartingWallIndex(diceTotal, diceClockwise, orderedWalls.Count);

            MJWall startingWall = orderedWalls[startingWallIndex];

            Debug.Assert(startingWall.count > 6, "牌墙墩数必须大于6");

            // 以骰子值中较小的值+1作为开始的牌墩索引，添加牌墙的牌墩到列表
            for (int i = minDice; i < startingWall.count; i++)
            {
                allStack.Add(startingWall.Get(i));
            }

            // 按指定顺序添加其他牌墙的牌墩到列表
            int wallCount = orderedWalls.Count;
            for (int j = 1; j < wallCount; j++)
            {
                int index = takeCardClockwise ? (startingWallIndex - j + wallCount) % wallCount : (j + startingWallIndex) % wallCount;
                MJWall set = orderedWalls[index];
                set.Traverse((stack, idx) => allStack.Add(stack));
            }

            // 添加开始牌墙剩下的牌墩到列表
            for (int k = 0; k < minDice; k++)
            {
                allStack.Add(startingWall.Get(k));
            }

            // 初始化正向取牌的牌墩索引
            currentStackIndex = 0;

            // 初始化反向取牌的牌墩索引
            currentStackIndexReversed = allStack.Count - 1;
        }

        private int GetStartingChair(int bankerViewChairId, bool diceClockwise)
        {
            int result = bankerViewChairId;

            int count = walls.Length;

            for (int i = 0; i < count; i++)
            {
                int index = diceClockwise ? (count + bankerViewChairId - i) % count : (bankerViewChairId + i) % count;

                if (!walls[index].empty)
                {
                    result = index;

                    break;
                }
            }

            return result;
        }

        private List<MJWall> GetOrderedWalls(int startingChairId)
        {
            List<MJWall> ordered = new List<MJWall>();

            int from = startingChairId, to = startingChairId + walls.Length;

            for (int i = from; i < to; i++)
            {
                int index = i % walls.Length;

                MJWall wall = walls[index];

                if (!wall.empty)
                {
                    ordered.Add(wall);
                }
            }

            return ordered;
        }

        private int GetStartingWallIndex(int diceTotal, bool diceClockwise, int wallSetCount)
        {
            int index = (diceTotal - 1) % wallSetCount;  // diceTotal - 1 表示从1开始数

            if (!diceClockwise)
            {
                return index;
            }

            return (wallSetCount - index) % wallSetCount;
        }

        // 正方向移除一张牌
        private void Remove()
        {
            if (currentStackIndex >= allStack.Count)
            {
                return;
            }

            MJStack stack = allStack[currentStackIndex];

            if (stack.empty)
            {
                currentStackIndex++;

                Remove();

                return;
            }

            stack.Remove(0);
        }

        // 反向移除一张牌
        private void RemoveReversely()
        {
            if (currentStackIndexReversed < 0)
            {
                return;
            }

            MJStack stack = allStack[currentStackIndexReversed];

            if (stack.empty)
            {
                currentStackIndexReversed--;

                RemoveReversely();

                return;
            }

            stack.Remove(0);
        }

        private void Recover()
        {
            if (currentStackIndex < 0)
            {
                return;
            }

            var stack = allStack[currentStackIndex];

            if (stack.count == config.countPerStack)
            {
                currentStackIndex--;

                Recover();

                return;
            }

            stack.Append(Card.Invalid);

            stack.Refresh();
        }

        private void RecoverReversely()
        {
            if (currentStackIndexReversed >= allStack.Count)
            {
                return;
            }

            var stack = allStack[currentStackIndexReversed];

            if (stack.count == config.countPerStack)
            {
                currentStackIndexReversed++;

                RecoverReversely();

                return;
            }

            stack.Append(Card.Invalid);

            stack.Refresh();
        }

        /// <summary>
        /// 转换为以自己视角为准的顺序。默认认为传入的stackCounts数组是按照服务器座位号排序的，即第0个为庄家位置的牌墙墩数
        /// </summary>
        /// <param name="bankerViewChairId"></param>
        /// <param name="stackCounts"></param>
        /// <returns></returns>
        private List<int> ToViewOrder(int bankerViewChairId, List<int> stackCounts)
        {
            int length = stackCounts.Count;

            List<int> result = new List<int>();

            for (int i = 0; i < length; i++)
            {
                int index = (length - bankerViewChairId + i) % length;

                result.Add(stackCounts[index]);
            }

            return result;
        }
        #endregion
    }
}
