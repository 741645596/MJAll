// @Author: tanjinhua
// @Date: 2021/4/8  14:24


using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WLHall.Game;

namespace MJCommon
{
    public class MJDeskCardController : BaseGameController
    {
        private MJGameData _gameData;
        private MJDiscardPointer _pointer;
        private MJCard _currentPointing;
        private MJDeskCardRoot _root;

        private MJHandController _handController;
        private MJHintsController _hintsController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _handController = stage.GetController<MJHandController>();
            _hintsController = stage.GetController<MJHintsController>();

            _pointer = OnCreatePointer();

            _pointer.gameObject.SetActive(false);

            _root = stage.GetController<MJSpaceController>().GetSpace().deskRoot;

            InitializeDiscards();
        }


        protected virtual MJDiscardPointer OnCreatePointer()
        {
            return new MJDiscardPointer();
        }

        /// <summary>
        /// 添加牌
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValue"></param>
        public MJCard Push(int viewChairId, int cardValue)
        {
            var set = GetDeskCardSet(viewChairId);

            var card = set.Append(cardValue);

            set.Refresh();

            ShowPointer(viewChairId);

            return card;
        }

        /// <summary>
        /// 播放出牌动画
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValue"></param>
        /// <param name="startPos"></param>
        /// <param name="startEuler"></param>
        public Sequence PlayDiscard(int viewChairId, int cardValue, Vector3 startPos, Vector3 startEuler)
        {
            var card = Push(viewChairId, cardValue);
            var trs = card.gameObject.transform;
            Vector3 end = trs.localPosition;
            trs.position = startPos;
            trs.eulerAngles = startEuler;

            var set = GetDeskCardSet(viewChairId);
            set.CalculatePlacementArgs(set.count - 1, out int layer, out int row, out int rowCount, out int indexInRow);

            if (layer > 0 || indexInRow == 0) // 不是最下面一层或者每行的第一个牌播放直接飞到位置上的动画
            {
                return MJAnimationHelper.PlayDirectOutCardAnimation(card, end);
            }
            else
            {
                return MJAnimationHelper.PlayOutCardAnimation(card, end);
            }
        }

        /// <summary>
        /// 根据传入的手牌节点播放出牌动画
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValue"></param>
        /// <param name="handCard"></param>
        public Sequence PlayDiscard(int viewChairId, int cardValue, MJCard handCard)
        {
            var trs = handCard.gameObject.transform;
            var startPos = trs.position + new Vector3(0, MJDefine.MJCardSizeZ * 2, 0);
            var starEuler = trs.eulerAngles;
            return PlayDiscard(viewChairId, cardValue, startPos, starEuler);
        }

        /// <summary>
        /// 移除末尾一张牌
        /// </summary>
        /// <param name="viewChairId"></param>
        public void Pop(int viewChairId)
        {
            MJCard card = GetDeskCardSet(viewChairId).RemoveLast();

            if (card != null && _currentPointing != null && _currentPointing == card)
            {
                _pointer.gameObject.SetActive(false);

                _currentPointing = null;
            }
        }


        /// <summary>
        /// 获取出牌集合对象
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public MJDeskCardSet GetDeskCardSet(int viewChairId)
        {
            return _root.GetDeskCardSet((MJOrientation)viewChairId);
        }


        /// <summary>
        /// 重新加载，并刷新布局
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValues"></param>
        public void Reload(int viewChairId, List<int> cardValues)
        {
            var discard = GetDeskCardSet(viewChairId);

            discard?.Reload(cardValues);

            discard?.Refresh();
        }

        /// <summary>
        /// 清除指定出牌组
        /// </summary>
        /// <param name="viewChairId"></param>
        public void Clear(int viewChairId)
        {
            var discard = GetDeskCardSet(viewChairId);

            if (discard.IndexOf(_currentPointing) != -1)
            {
                HidePointer();
            }

            discard?.Clear();
        }

        /// <summary>
        /// 清除所有出牌
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                Clear(i);
            }
        }


        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="discard"></param>
        /// <returns></returns>
        public int IndexOf(MJDeskCardSet discard)
        {
            var deskCards = _root.GetDeskCardSets();

            foreach(var pair in deskCards)
            {
                if (pair.Value == discard)
                {
                    return (int)pair.Key;
                }
            }

            return -1;
        }


        /// <summary>
        /// 根据传入牌值设置牌颜色
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="color"></param>
        /// <param name="cardValue"></param>
        public void TintByValue(int viewChairId, Color32 color, int cardValue)
        {
            var discard = GetDeskCardSet(viewChairId);

            discard?.Traverse((c, i) =>
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
            var discard = GetDeskCardSet(viewChairId);

            discard?.Traverse((c, i) => c.SetColor(Color.white));
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


        /// <summary>
        /// 显示出牌指示器
        /// </summary>
        /// <param name="viewChairId"></param>
        public void ShowPointer(int viewChairId)
        {
            var discard = GetDeskCardSet(viewChairId);

            var card = discard.Peek();

            if (card != null)
            {
                ShowPointer(card);
            }
        }


        /// <summary>
        /// 隐藏出牌指示器
        /// </summary>
        public void HidePointer()
        {
            _pointer.gameObject.SetActive(false);
        }


        #region Widget Events
        /// <summary>
        /// 添加出牌事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="card"></param>
        protected virtual void OnAppend(MJCardSet sender, MJCard card)
        {
            if (_gameData.jokerCardValues.Contains(card.cardValue))
            {
                card.ShowJokerMark();
            }

            card.SetColor(Color.white);

            // 清除胡牌提示相关UI
            _handController.ClearHintMark(Chair.Down);

            _hintsController?.HideHints(false);

            if (IndexOf(sender as MJDeskCardSet) == Chair.Down)
            {
                _gameData.huInfo = null;
            }
        }
        #endregion


        private void ShowPointer(MJCard card)
        {
            _pointer.gameObject.SetActive(true);

            _pointer.gameObject.transform.position = card.gameObject.transform.position;

            _pointer.gameObject.transform.eulerAngles = Vector3.zero;

            _currentPointing = card;
        }

        private void InitializeDiscards()
        {
            var discards = _root.GetDeskCardSets();
            foreach (var pair in discards)
            {
                var discard = pair.Value;

                discard.onAppend = OnAppend;
            }
        }

        public override void OnContinue()
        {
            base.OnContinue();

            Clear();
        }


        public override void OnChangeDesk()
        {
            base.OnChangeDesk();

            Clear();
        }
    }
}
