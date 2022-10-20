// @Author: tanjinhua
// @Date: 2021/4/28  13:47


using Common;
using System.Collections.Generic;

namespace MJCommon
{

    /// <summary>
    /// 麻将玩家数据
    /// </summary>
    public class MJGamePlayer : GamePlayer
    {
        
        /// <summary>
        /// 是否已经听牌
        /// </summary>
        public bool isReadyHand = false;


        /// <summary>
        /// 玩家定缺花色
        /// </summary>
        public int dingqueColorValue = Card.ColorNone;


        /// <summary>
        /// 玩家手牌牌值数组(克隆)
        /// </summary>
        public List<int> handCardValues => _handCardValues.Clone();


        /// <summary>
        /// 手牌牌值数量
        /// </summary>
        public int handCardCount => _handCardValues.Count;


        /// <summary>
        /// 玩家(通常是自家用)不可选择牌值数组，当不为空时会依次屏蔽牌值为此列表中的值的手牌选中事件(克隆)
        /// </summary>
        public List<int> unselectableCardValues
        {
            get => _unselectableCardValues.Clone();

            set
            {
                if (value == null)
                {
                    _unselectableCardValues.Clear();
                }
                else
                {
                    _unselectableCardValues = value;
                }
            }
        }


        /// <summary>
        /// 玩家出牌牌值数组(克隆)
        /// </summary>
        public List<int> outCardValues => _outCardValues.Clone();


        /// <summary>
        /// 玩家副子数据数组(克隆)
        /// </summary>
        public List<FuziData> fuziDatas => _fuziDatas.Clone();


        /// <summary>
        /// 玩家副子数量
        /// </summary>
        public int fuziCount => _fuziDatas.Count;


        /// <summary>
        /// 玩家花牌(胡牌)牌值数组(克隆)
        /// </summary>
        public List<int> winCardValues => _winCardValues.Clone();

        /// <summary>
        /// 花牌/胡牌数量
        /// </summary>
        public int winCardCount => _winCardValues.Count;


        #region Protected members
        protected List<int> _handCardValues;
        protected List<int> _outCardValues;
        protected List<FuziData> _fuziDatas;
        protected List<int> _winCardValues;
        protected List<int> _unselectableCardValues;
        #endregion



        public MJGamePlayer()
        {
            _handCardValues = new List<int>();
            _outCardValues = new List<int>();
            _fuziDatas = new List<FuziData>();
            _winCardValues = new List<int>();
            _unselectableCardValues = new List<int>();
        }


        /// <summary>
        /// 初始化，重置动态数据。注：此方法会在每局游戏开始时调用，朋友场需要每局累加的数据不要在这里初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            isReadyHand = false;
            dingqueColorValue = Card.ColorNone;
            _handCardValues.Clear();
            _outCardValues.Clear();
            _fuziDatas.Clear();
            _winCardValues.Clear();
            _unselectableCardValues.Clear();
        }


        #region HandCardValues Functions
        /// <summary>
        /// 添加一张手牌
        /// </summary>
        /// <param name="cardValue"></param>
        public void AddHandCardValue(int cardValue)
        {
            _handCardValues.Add(cardValue);
        }


        /// <summary>
        /// 添加多张手牌牌
        /// </summary>
        /// <param name="cardValues"></param>
        public void AddHandCardValues(List<int> cardValues)
        {
            _handCardValues.AddRange(cardValues);
        }


        /// <summary>
        /// 移除一张牌
        /// </summary>
        /// <param name="cardValue"></param>
        public void RemoveHandCardValue(int cardValue, bool reverse = false)
        {
            if (reverse)
            {
                for (int i = _handCardValues.Count - 1; i >= 0; i--)
                {
                    if (_handCardValues[i] == cardValue)
                    {
                        RemoveHandCardValueByIndex(i);

                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _handCardValues.Count; i++)
                {
                    if (_handCardValues[i] == cardValue)
                    {
                        RemoveHandCardValueByIndex(i);

                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 删除多个手牌
        /// </summary>
        /// <param name="cardValues"></param>
        public void RemoveHandCardValues(List<int> cardValues, bool reverse = false)
        {
            for (int i = 0; i < cardValues.Count; i++)
            {
                RemoveHandCardValue(cardValues[i], reverse);
            }
        }


        /// <summary>
        /// 根据index移除手牌数据
        /// </summary>
        /// <param name="index"></param>
        public void RemoveHandCardValueByIndex(int index)
        {
            _handCardValues.RemoveAt(index);
        }


        /// <summary>
        /// 删除最后一张手牌
        /// </summary>
        public void RemoveLastHandCardValue()
        {
            RemoveHandCardValueByIndex(_handCardValues.Count - 1);
        }


        /// <summary>
        /// 清除手牌数据
        /// </summary>
        public void ClearHandCardValues()
        {
            _handCardValues.Clear();
        }


        /// <summary>
        /// 重新加载手牌数据
        /// </summary>
        /// <param name="handCardValues"></param>
        public void ReloadHandCardValues(List<int> handCardValues)
        {
            ClearHandCardValues();

            AddHandCardValues(handCardValues);
        }


        /// <summary>
        /// 手牌数据是否存在某个牌值
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        public bool HandCardValueExists(int cardValue)
        {
            return _handCardValues.Contains(cardValue);
        }


        /// <summary>
        /// 手牌排序
        /// </summary>
        /// <param name="jokerCardValues">赖子牌列表</param>
        /// <param name="jokerDiscardable">赖子是否可打出</param>
        /// <param name="includeLastCard">是否包含最后一张牌</param>
        public virtual void SortHandCardValues(List<int> jokerCardValues, bool jokerDiscardable = false, bool includeLastCard = true)
        {
            int lastValue = Card.Invalid;
            if (!includeLastCard)
            {
                int lastIndex = _handCardValues.Count - 1;
                lastValue = _handCardValues[lastIndex];
                _handCardValues.RemoveAt(lastIndex);
            }

            // 从小到大排序
            _handCardValues.Sort((a, b) => a - b);


            // 赖子提到前面
            if (jokerCardValues.Count > 0)
            {
                _handCardValues.Classify(value => jokerCardValues.Contains(value));
            }

            // 定缺牌放到最后
            if (dingqueColorValue != Card.ColorNone)
            {
                _handCardValues.Classify(value =>
                {
                    int colorValue = Card.GetCardColorValue(value);

                    if (colorValue != dingqueColorValue)
                    {
                        return true;
                    }

                    // 是定缺牌的情况下如果是赖子并且赖子不可打出则放前面
                    if (jokerCardValues.Contains(value) && !jokerDiscardable)
                    {
                        return true;
                    }

                    return false;
                });
            }

            if (lastValue != Card.Invalid)
            {
                _handCardValues.Add(lastValue);
            }
        }
        #endregion


        #region OutCardValues Functions
        /// <summary>
        /// 添加一张出牌
        /// </summary>
        /// <param name="cardValue"></param>
        public void AddOutCardValue(int cardValue)
        {
            _outCardValues.Add(cardValue);
        }


        /// <summary>
        /// 添加多张出牌
        /// </summary>
        /// <param name="cardValues"></param>
        public void AddOutCardValues(List<int> cardValues)
        {
            _outCardValues.AddRange(cardValues);
        }


        /// <summary>
        /// 根据index移除一张出牌
        /// </summary>
        /// <param name="index"></param>
        public void RemoveOutCardValue(int index)
        {
            _outCardValues.RemoveAt(index);
        }


        /// <summary>
        /// 移除最后一张出牌
        /// </summary>
        public void RemoveLastOutCardValue()
        {
            RemoveOutCardValue(_outCardValues.Count - 1);
        }


        /// <summary>
        /// 清除出牌数据
        /// </summary>
        public void ClearOutCardValues()
        {
            _outCardValues.Clear();
        }


        /// <summary>
        /// 替换某个出牌牌值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="cardValue"></param>
        public void ReplaceOutCardValue(int index, int cardValue)
        {
            string errorMsg = "MJGamePlayer.ReplaceOutCardValue: 操作失败，索引越界！";

            if (_outCardValues.IndexValid(index, errorMsg))
            {
                _outCardValues[index] = cardValue;
            }
        }

        /// <summary>
        /// 重新加载出牌数据
        /// </summary>
        /// <param name="cardValues"></param>
        public void ReloadOutCardValues(List<int> cardValues)
        {
            ClearOutCardValues();

            AddOutCardValues(cardValues);
        }
        #endregion


        #region FuziData Functions
        /// <summary>
        /// 添加一个副子
        /// </summary>
        /// <param name="fuziData"></param>
        public void AddFuziData(FuziData fuziData)
        {
            _fuziDatas.Add(fuziData);
        }


        /// <summary>
        /// 添加多个副子
        /// </summary>
        /// <param name="fuziDatas"></param>
        public void AddFuziDatas(List<FuziData> fuziDatas)
        {
            _fuziDatas.AddRange(fuziDatas);
        }


        /// <summary>
        /// 根据index移除一个副子
        /// </summary>
        /// <param name="index"></param>
        public void RemoveFuziData(int index)
        {
            if (!_fuziDatas.IndexValid(index))
            {
                return;
            }
            _fuziDatas.RemoveAt(index);
        }


        /// <summary>
        /// 移除最后一个副子
        /// </summary>
        public void RemoveLastFuziData()
        {
            if (_fuziDatas.Count == 0)
            {
                return;
            }
            RemoveFuziData(_fuziDatas.Count - 1);
        }


        /// <summary>
        /// 替换副子数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="fuziData"></param>
        public void ReplaceFuziData(int index, FuziData fuziData)
        {
            _fuziDatas[index] = fuziData;
        }


        /// <summary>
        /// 清除副子数据
        /// </summary>
        public void ClearFuziDatas()
        {
            _fuziDatas.Clear();
        }

        /// <summary>
        /// 重新加载副子数据
        /// </summary>
        /// <param name="fuziDatas"></param>
        public void ReloadFuziDatas(List<FuziData> fuziDatas)
        {
            ClearFuziDatas();

            AddFuziDatas(fuziDatas);
        }
        #endregion


        #region WinCard Functions
        /// <summary>
        /// 添加一张花牌
        /// </summary>
        /// <param name="cardValue"></param>
        public void AddWinCardValue(int cardValue)
        {
            _winCardValues.Add(cardValue);
        }


        /// <summary>
        /// 添加多张花牌
        /// </summary>
        /// <param name="cardValues"></param>
        public void AddWinCardValues(List<int> cardValues)
        {
            _winCardValues.AddRange(cardValues);
        }


        /// <summary>
        /// 根据index移除一张花牌
        /// </summary>
        /// <param name="index"></param>
        public void RemoveWinCardValue(int index)
        {
            _winCardValues.RemoveAt(index);
        }


        /// <summary>
        /// 移除最后一张花牌
        /// </summary>
        public void RemoveLastWinCardValue()
        {
            RemoveWinCardValue(_winCardValues.Count - 1);
        }

        /// <summary>
        /// 移除所有胡牌数据
        /// </summary>
        public void ClearWinCardValues()
        {
            _winCardValues.Clear();
        }

        /// <summary>
        /// 重新加载胡牌数据
        /// </summary>
        /// <param name="cardValues"></param>
        public void ReloadWinCardValues(List<int> cardValues)
        {
            ClearWinCardValues();

            AddWinCardValues(cardValues);
        }
        #endregion
    }
}
