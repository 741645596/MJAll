// MJHandSet.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/07/15

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MJCommon
{
    /// <summary>
    /// 手牌
    /// </summary>
    public class MJHandSet : MJCardSet
    {

        private const float Spacing = 0.0005f;

        /// <summary>
        /// 手牌状态类型
        /// </summary>
        public enum State
        {
            /// <summary>
            /// 站立
            /// </summary>
            Standing,
            /// <summary>
            /// 躺下
            /// </summary>
            Lying,
            /// <summary>
            /// 盖起
            /// </summary>
            Covering
        }

        #region Delegates
        /// <summary>
        /// 获取偏移开始索引委托
        /// </summary>
        public Func<MJHandSet, int> onGetBiasStartIndex;
        /// <summary>
        /// 获取手牌状态委托
        /// </summary>
        public Func<MJHandSet, int, State> onGetCardState;
        /// <summary>
        /// 计算布局位置委托
        /// </summary>
        public Func<MJHandSet, int, State, int, Vector3> onComputePosition;
        /// <summary>
        /// 计算布局旋转委托
        /// </summary>
        public Func<MJHandSet, State, Vector3> onComputeEuler;
        /// <summary>
        /// 排序委托
        /// </summary>
        public Action<MJHandSet, List<MJCard>> onSortCards;
        #endregion

        /// <summary>
        /// 渲染摄像机
        /// </summary>
        public Camera renderCamera;
        /// <summary>
        /// 是否锁手牌
        /// </summary>
        public bool interactable = false;
        /// <summary>
        /// 最大宽度
        /// </summary>
        public float maxWidth;

        /// <summary>
        /// 排序比较器
        /// </summary>
        protected IComparer<object> comparer = new MJCardDefaultComparer();
        /// <summary>
        /// 手牌和新牌的间距
        /// </summary>
        protected float biasAmount = 0.02f;
        /// <summary>
        /// 选中提起的高度
        /// </summary>
        protected float liftHeight = 0.02f;
        /// <summary>
        /// 手牌的锚点
        /// </summary>
        public HandCardAnchor anchor = HandCardAnchor.Right;

        /// <summary>
        /// 构造方法
        /// </summary>
        public MJHandSet()
        {
            gameObject.name = "MJHandSet";
        }

        public HandCardAnchor GetHandCardAnchor()
        {
            return anchor;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="maxCount">手牌最大数</param>
        /// <param name="camera"></param>
        public void Initialize(int maxCount, Camera camera)
        {
            renderCamera = camera;

            maxWidth = maxCount * MJDefine.MJCardSizeX + biasAmount + Spacing * (maxCount - 1);
        }

        /// <summary>
        /// 使用层名字设置
        /// </summary>
        /// <param name="layerName"></param>
        public void SetLayer(string layerName)
        {
            layer = LayerMask.NameToLayer(layerName);
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="config"></param>
        public void UpdateConfig(MJHandSetConfig config)
        {
            anchor = config.anchor;
            biasAmount = (float)config.interval;
            liftHeight = (float)config.liftHeight;
            Refresh();
        }

        public override MJCard Append(int cardValue)
        {
            MJCard card = orientation == MJOrientation.Down ?
                MJCardPool.Get<MJCard>(cardValue):
                MJCardPool.Get<MJHandCard>(cardValue);

            card.gameObject.SetLayer(layer);

            var assetName = "MJCommon/MJ/mj_cards";
            var card_mat_key = $"materials/mj_card_hand_{(int)orientation}.mat";
            var back_mat_key = $"materials/mj_back_hand_{(int)orientation}.mat";
            card.SetMaterial(assetName, card_mat_key, back_mat_key);

            Append(card);

            if (orientation == MJOrientation.Down)
            {
                card.HideShadow();
            }

            return card;
        }

        public override MJCard Insert(int index, int cardValue)
        {
            MJCard card;
            if (orientation == MJOrientation.Down)
            {
                card = MJCardPool.Get<MJCard>(cardValue);
                card.HideShadow();
            }
            else
            {
                 card = MJCardPool.Get<MJHandCard>(cardValue);
            }
            Insert(index, card);

            card.gameObject.SetLayer(layer);

            var assetName = "MJCommon/MJ/mj_cards";
            var card_mat_key = $"materials/mj_card_hand_{(int)orientation}.mat";
            var back_mat_key = $"materials/mj_back_hand_{(int)orientation}.mat";
            card.SetMaterial(assetName, card_mat_key, back_mat_key);

            return card;
        }

        /// <summary>
        /// 根据GameObject获取一张MJCard
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public MJCard FindCard(GameObject obj)
        {
            for (int i = 0; i < count; i++)
            {
                var card = _cards[i];
                if (card.gameObject == obj)
                {
                    return card;
                }
            }

            return null;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="comparer">比较器</param>
        public void Sort(IComparer<object> comparer = null, bool refresh = true)
        {
            if (onSortCards != null)
            {
                onSortCards(this, _cards);

                if (refresh)
                {
                    Refresh();
                }

                return;
            }

            if (comparer == null)
            {
                _cards.Sort(this.comparer);
            }
            else
            {
                this.comparer = comparer;
                _cards.Sort(comparer);
            }

            if (refresh)
            {
                Refresh();
            }
        }

        /// <summary>
        /// 选中手牌
        /// </summary>
        /// <param name="obj"></param>
        public void SelectCard(GameObject obj)
        {
            var card = FindCard(obj);
            if(card == null || card.gameObject == null)
            {
                return;
            }    
            var pos = card.gameObject.transform.localPosition;
            float y = MJDefine.MJCardSizeZ * 0.5f;
            card.gameObject.transform.localPosition = new Vector3(pos.x, y + liftHeight, pos.z);
        }

        /// <summary>
        /// 取消选中手牌
        /// </summary>
        /// <param name="obj"></param>
        public void UnselectCard(GameObject obj)
        {
            var card = FindCard(obj);
            var pos = card.gameObject.transform.localPosition;
            float y = MJDefine.MJCardSizeZ * 0.5f;
            card.gameObject.transform.localPosition = new Vector3(pos.x, y, pos.z);
        }

        /// <summary>
        /// 取消选中所有手牌
        /// </summary>
        public void UnselectAll()
        {
            for (var i = 0; i < count; i++)
            {
                var card = _cards[i];
                var pos = card.gameObject.transform.localPosition;
                float y = MJDefine.MJCardSizeZ * 0.5f;
                card.gameObject.transform.localPosition = new Vector3(pos.x, y, pos.z);
            }
        }

        public void Remove(List<int> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                var value = values[i];
                for (int j = 0; j < _cards.Count; j++)
                {
                    var card = _cards[j];
                    if (card.cardValue == value)
                    {
                        Remove(card);
                        break;
                    }
                }
            }
        }

        public List<MJCard> GetCardsByValues(List<int> values)
        {
            var result = new List<MJCard>();
            for (int i = 0; i < values.Count; i++)
            {
                var value = values[i];
                for (int j = 0; j < _cards.Count; j++)
                {
                    var card = _cards[j];
                    if (card.cardValue == value && result.Contains(card) == false)
                    {
                        result.Add(card);
                        break;
                    }
                }
            }

            return result;
        }

        public override Vector3 GetSize()
        {
            if (count == 0)
            {
                return Vector3.zero;
            }

            var x = count * MJDefine.MJCardSizeX + Spacing * (count - 1);
            var biasIndex = GetBiasStartIndex();
            if (biasIndex < count)
            {
                x += biasAmount;
            }
            var y = MJDefine.MJCardSizeY;
            var z = MJDefine.MJCardSizeZ;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 刷新x轴位置，保留手牌当前朝向和y，z坐标
        /// </summary>
        public override void Refresh()
        {
            Traverse((card, index) =>
            {
                var transform = card.gameObject.transform;
                var biasIndex = GetBiasStartIndex();
                var state = GetCardState(index);
                transform.localPosition = ComputePosition(index, state, biasIndex);
                SetupState(card, state);
            });
        }


        /// <summary>
        /// 传入指定状态刷新布局
        /// </summary>
        /// <param name="state"></param>
        public void Refresh(State state)
        {
            Traverse((card, index) =>
            {
                var transform = card.gameObject.transform;
                var biasIndex = GetBiasStartIndex();
                transform.localPosition = ComputePosition(index, state, biasIndex);
                SetupState(card, state);
            });
        }

        /// <summary>
        /// 只刷新X轴坐标
        /// </summary>
        public void RefreshX()
        {
            Traverse((card, index) =>
            {
                var transform = card.gameObject.transform;
                var biasIndex = GetBiasStartIndex();
                var pos = ComputePosition(index, State.Standing, biasIndex);
                var curPos = transform.localPosition;
                transform.localPosition = new Vector3(pos.x, curPos.y, curPos.z);
            });
        }


        public State GetCardState(int index)
        {
            if (onGetCardState != null)
            {
                return onGetCardState(this, index);
            }

            return State.Standing;
        }

        public int GetBiasStartIndex()
        {
            if (onGetBiasStartIndex != null)
            {
                return onGetBiasStartIndex(this);
            }

            return count % 3 == 2 ? count - 1 : count;
        }

        public Vector3 ComputePosition(int index, State state, int biasIndex)
        {
            if (onComputePosition != null)
            {
                return onComputePosition(this, index, state, biasIndex);
            }

            float dx = MJDefine.MJCardSizeX + Spacing; // 手牌增加固定间距

            float x = dx * 0.5f + index * dx + (index >= biasIndex ? biasAmount : 0);

            if (anchor == HandCardAnchor.Right)
            {
                x = -dx * 0.5f - (count - index - 1) * dx - (index < biasIndex ? biasAmount : 0);

                if (count % 3 != 2)
                {
                    x -= dx;
                }
            }

            float y = 0;
            if (state == State.Standing)
            {
                y = MJDefine.MJCardSizeZ * 0.5f;
            }
            if (state == State.Covering || state == State.Lying)
            {
                y = MJDefine.MJCardSizeX * 0.5f;
            }

            return new Vector3(x, y, 0);
        }


        private void SetupState(MJCard card, State state)
        {
            card.gameObject.transform.localEulerAngles = Vector3.zero;

            if (state == State.Standing)
            {
                card.TowardBack();
                if (orientation != MJOrientation.Down)
                {
                    var assetName = "MJCommon/MJ/mj_cards";
                    var card_mat_key = $"materials/mj_card_hand_{(int)orientation}.mat";
                    var back_mat_key = $"materials/mj_back_hand_{(int)orientation}.mat";
                    card.SetMaterial(assetName, card_mat_key, back_mat_key);
                }
            }
            if (state == State.Lying)
            {
                card.TowardUp();
            }
            if (state == State.Covering)
            {
                card.TowardDown();
                if (orientation != MJOrientation.Down)
                {
                    var assetName = "MJCommon/MJ/mj_cards";
                    var card_mat_key = $"materials/mj_card_wall_{(int)orientation}.mat";
                    var back_mat_key = $"materials/mj_back_wall_{(int)orientation}.mat";
                    card.SetMaterial(assetName, card_mat_key, back_mat_key);
                }
            }
        }
    }

    /// <summary>
    /// 默认的麻将牌排序比较器（MJCard）
    /// </summary>
    public class MJCardDefaultComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            var a = (MJCard)x;
            var b = (MJCard)y;
            return a.cardValue - b.cardValue;
        }
    }

    /// <summary>
    /// 默认的麻将牌排序比较器（牌值）
    /// </summary>
    public class MJCardValueDefaultComparer : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return a - b;
        }
    }
}