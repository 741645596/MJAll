// @Author: tanjinhua
// @Date: 2021/4/8  14:27


using System;
using System.Collections.Generic;
using WLHall.Game;
using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public class MJWinCardController : BaseGameController
    {
        private MJWinCardRoot _root;

        public override void OnSceneLoaded()
        {
            _root = stage.GetController<MJSpaceController>().GetSpace().winCardRoot;
        }


        /// <summary>
        /// 播放添加花牌动画
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValue"></param>
        public void PlayWinCardAnimation(int viewChairId, int cardValue, Action onFinished = null)
        {
            var set = GetWinCardSet(viewChairId);
            var card = set.Append(cardValue);
            var shadowShown = card.IsShadowShown();
            card.SetActive(false);
            card.HideShadow();
            card.gameObject.transform.RunTweenGraph("MJCommon/MJ/mj_tween", "card_hu.asset", (t) =>
            {
                var effect = WNode3D.Create("MJCommon/MJ/mj_zm_effe_tongyong", "hu_l_01_01.prefab");

                effect.gameObject.transform.SetParent(card.gameObject.transform, false);

                if (shadowShown)
                {
                    card.ShowShadow();
                }
            });
        }

        /// <summary>
        /// 添加一张牌
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValue"></param>
        public void Push(int viewChairId, int cardValue)
        {
            var set = GetWinCardSet(viewChairId);
            set.Append(cardValue);
        }

        /// <summary>
        /// 移除一张花牌
        /// </summary>
        /// <param name="viewChairId"></param>
        public void Pop(int viewChairId)
        {
            GetWinCardSet(viewChairId).RemoveLast();
        }


        /// <summary>
        /// 获取花牌合集节点
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public MJWinCardSet GetWinCardSet(int viewChairId)
        {
            return _root.GetMJWinSet((MJOrientation)viewChairId);
        }


        /// <summary>
        /// 重新加载，并刷新布局
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValues"></param>
        public void Reload(int viewChairId, List<int> cardValues)
        {
            var set = GetWinCardSet(viewChairId);

            set.Reload(cardValues);

            set.Refresh();
        }


        /// <summary>
        /// 清除所有花牌
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                var set = GetWinCardSet(i);
                set?.Clear();
            }
        }


        /// <summary>
        /// 获取下一个牌的位置
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="worldSpace">是否返回世界坐标，默认true</param>
        /// <returns></returns>
        public Vector3 GetNextPosition(int viewChairId, bool worldSpace = true)
        {
            var set = GetWinCardSet(viewChairId);

            var localPos = set.ComputePosition(set.count, out bool showShadow);

            if (worldSpace)
            {
                return set.gameObject.transform.localToWorldMatrix.MultiplyPoint(localPos);
            }

            return localPos;
        }


        /// <summary>
        /// 根据传入牌值设置牌颜色
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="color"></param>
        /// <param name="cardValue"></param>
        public void TintByValue(int viewChairId, Color32 color, int cardValue)
        {
            var set = GetWinCardSet(viewChairId);

            set?.Traverse((c, i) =>
            {
                if (c.cardValue == cardValue)
                {
                    c.SetColor(color);
                }
            });
        }


        /// <summary>
        /// 根据传入牌值，设置所有出牌颜色
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
            var set = GetWinCardSet(viewChairId);

            set?.Traverse((c, i) => c.SetColor(Color.white));
        }


        /// <summary>
        /// 重置所有出牌颜色
        /// </summary>
        public void ResetTint()
        {
            for (int i = 0; i < 4; i++)
            {
                ResetTint(i);
            }
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
