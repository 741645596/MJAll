// @Author: tanjinhua
// @Date: 2021/4/8  14:25


using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WLHall.Game;

namespace MJCommon
{
    public class MJMeldController : BaseGameController
    {
        private MJMeldRoot _root;

        public override void OnSceneLoaded()
        {
            _root = stage.GetController<MJSpaceController>().GetSpace().meldRoot;
        }


        /// <summary>
        /// 获取副子合集节点
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public MJMeldStack GetMeldStack(int viewChairId)
        {
            return _root.GetMJMeldStack((MJOrientation)viewChairId);
        }


        /// <summary>
        /// 播放添加副子动画
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="args"></param>
        public Sequence PlayAppend(int viewChairId, MJMeld.Args args)
        {
            var meldStack = GetMeldStack(viewChairId);

            meldStack.Append(args);

            return MJAnimationHelper.PlayAddMeldAnimation(meldStack);
        }


        /// <summary>
        /// 播放替换副子动画
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="index"></param>
        /// <param name="args"></param>
        public Sequence PlayReplace(int viewChairId, int index, MJMeld.Args args)
        {
            var meld = GetMeldStack(viewChairId).Replace(index, args);

            if (args.showType == MJMeld.ShowType.Stacking)
            {
                meld.GetCard(meld.count - 1).SetActive(false);
                return MJAnimationHelper.PlayGangAnimation(meld);
            }

            return null;
        }


        /// <summary>
        /// 在末尾移除一个副子
        /// </summary>
        /// <param name="viewChairId"></param>
        public void Pop(int viewChairId)
        {
            GetMeldStack(viewChairId).RemoveLast();
        }


        /// <summary>
        /// 重新加载副子，并刷新布局
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="datas"></param>
        public void Reload(int viewChairId, List<MJMeld.Args> datas)
        {
            var meldSet = GetMeldStack(viewChairId);

            meldSet.Reload(datas);

            meldSet.Refresh();
        }


        /// <summary>
        /// 清除所有副子节点
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                var meldSet = _root.GetMJMeldStack((MJOrientation)i);

                meldSet?.Clear();
            }
        }


        /// <summary>
        /// 根据传入牌值，设置牌颜色
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="color"></param>
        /// <param name="cardValue"></param>
        public void TintByValue(int viewChairId, Color32 color, int cardValue)
        {
            var set = GetMeldStack(viewChairId);

            set?.Traverse((m, i) => m.Traverse((c, j) =>
            {
                if (c.cardValue == cardValue)
                {
                    c.SetColor(color);
                }
            }));
        }


        /// <summary>
        /// 根据传入牌值，设置所有牌颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="cardValue"></param>
        public void TintByValue(Color32 color, int cardValue)
        {
            for (int i = 0; i < 4; i++)
            {
                TintByValue(i, color, cardValue);
            }
        }


        /// <summary>
        /// 重置牌颜色
        /// </summary>
        /// <param name="viewChairId"></param>
        public void ResetTint(int viewChairId)
        {
            var set = GetMeldStack(viewChairId);

            set?.Traverse((m, i) => m.Traverse((c, j) => c.SetColor(Color.white)));
        }


        /// <summary>
        /// 重置所有牌颜色
        /// </summary>
        public void ResetTint()
        {
            for (int i = 0; i < 4; i++)
            {
                ResetTint(i);
            }
        }


        /// <summary>
        /// 原始副子数据转换成副子节点配置
        /// </summary>
        /// <param name="actionViewChairId"></param>
        /// <param name="fuziData"></param>
        /// <returns></returns>
        public virtual MJMeld.Args ToMeldData(int chairId, FuziData fuziData)
        {
            MJMeld.Args data = new MJMeld.Args
            {
                type = fuziData.weaveKind1,
                cardValues = new List<int>(),
                multiplies = new List<int>(),
                showType = GetMeldShowType(fuziData)
            };

            data.providerViewChairId = GetMeldProviderViewChairId(chairId, fuziData.provideUser);

            for (int i = 0; i < fuziData.cardValues.Length; i++)
            {
                int cardValue = fuziData.cardValues[i];
                int multiply = fuziData.cardCounts[i];
                if (cardValue != Card.Invalid)
                {
                    data.cardValues.Add(cardValue);
                    data.multiplies.Add(multiply);
                }
            }

            if (fuziData.yaoJiCard == 0)
            {
                return data;
            }

            if (fuziData.weaveKind1 == 0x00000200) // 如果是幺杠
            {
                for (int i = 0; i < data.cardValues.Count; i++)
                {
                    if (data.cardValues[i] == Card.Tiao1)
                    {
                        data.multiplies[i] += fuziData.yaoJiCard;
                    }
                }
            }
            else
            {
                data.cardValues.Add(Card.Tiao1);
                data.multiplies.Add(fuziData.yaoJiCard);
            }

            return data;
        }

        /// <summary>
        /// 获取副子提供者视图座位号(以副子所有者视角为准)
        /// </summary>
        /// <param name="actionChairId"></param>
        /// <param name="providerChairId"></param>
        /// <returns></returns>
        protected int GetMeldProviderViewChairId(int actionChairId, int providerChairId)
        {
            if (actionChairId == providerChairId)
            {
                return Chair.Invalid;
            }

            return stage.ToViewChairIdRelative(providerChairId, actionChairId);
        }

        /// <summary>
        /// 获取副子显示类型
        /// </summary>
        /// <param name="fuziData"></param>
        /// <returns></returns>
        public MJMeld.ShowType GetMeldShowType(FuziData fuziData)
        {
            return fuziData.isConcealedKong || fuziData.isExposedKong ? MJMeld.ShowType.Stacking : MJMeld.ShowType.Tiling;
        }


        /// <summary>
        /// 原始副子数据转换成副子节点配置
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="fuziDatas"></param>
        /// <returns></returns>
        public virtual List<MJMeld.Args> ToMeldDatas(int chairId, List<FuziData> fuziDatas)
        {
            List<MJMeld.Args> datas = new List<MJMeld.Args>();

            for (int i = 0; i < fuziDatas.Count; i++)
            {
                datas.Add(ToMeldData(chairId, fuziDatas[i]));
            }

            return datas;
        }


        public override void OnChangeDesk()
        {
            base.OnChangeDesk();

            Clear();
        }


        public override void OnContinue()
        {
            base.OnContinue();

            Clear();
        }
    }
}
