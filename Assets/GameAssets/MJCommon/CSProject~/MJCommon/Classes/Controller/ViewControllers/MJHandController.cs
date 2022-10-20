// @Author: tanjinhua
// @Date: 2021/4/8  14:23


using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WLHall.Game;

namespace MJCommon
{
    public class MJHandController : BaseGameController
    {
        /// <summary>
        /// 变暗颜色
        /// </summary>
        protected virtual Color darkenColor => new Color(0.7f, 0.7f, 0.7f, 1);

        private MJGameData _gameData;
        private MJHandSetRoot _handRoot;
        private MJHandSet _ownHand;
        private MJSpace _space;

        private MJDingqueHandler _dingqueHandler;
        private MJDeskCardController _deskCardController;
        private MJCardTintController _cardTintController;
        private MJAvatarController _avatarController;
        private MJTableController _tableController;
        private MJActionButtonController _actionButtonController;
        private MJAudioController _audioController;
        private MJInputController _inputController;
        private MJHintsController _hintsController;
        private MJHuInfoHandler _huInfoHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _dingqueHandler = stage.GetController<MJDingqueHandler>();
            _deskCardController = stage.GetController<MJDeskCardController>();
            _cardTintController = stage.GetController<MJCardTintController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _tableController = stage.GetController<MJTableController>();
            _actionButtonController = stage.GetController<MJActionButtonController>();
            _audioController = stage.GetController<MJAudioController>();
            _inputController = stage.GetController<MJInputController>();
            _hintsController = stage.GetController<MJHintsController>();
            _huInfoHandler = stage.GetController<MJHuInfoHandler>();

            _space = stage.GetController<MJSpaceController>().GetSpace();

            _handRoot = _space.handRoot;

            _ownHand = _handRoot.GetMJHandSet(MJOrientation.Down);

            InitializeHands();
        }

        /// <summary>
        /// 获取自家手牌
        /// </summary>
        /// <returns></returns>
        public MJHandSet GetOwnHand()
        {
            return _ownHand;
        }

        /// <summary>
        /// 播放插牌动画
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="removeIndex"></param>
        public Sequence PlayInsert(int viewChairId)
        {
            var handSet = GetHandSet(viewChairId);

            return MJAnimationHelper.PlayInsertCardAnimation(handSet);
        }

        /// <summary>
        /// 播放摸牌动画
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValue"></param>
        public Sequence PlayDraw(int viewChairId, int cardValue)
        {
            var handSet = GetHandSet(viewChairId);

            handSet.Append(cardValue);

            handSet.Refresh();

            return MJAnimationHelper.PlaySendCardAnimation(handSet);
        }

        #region Management
        /// <summary>
        /// 重新加载，并刷新布局
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValues"></param>
        /// <param name="refresh"></param>
        /// <param name="updateAlignment"></param>
        public void Reload(int viewChairId, List<int> cardValues)
        {
            var hand = GetHandSet(viewChairId);

            hand.Reload(cardValues);

            hand.Refresh();
        }


        /// <summary>
        /// 清除所有手牌
        /// </summary>
        public void Clear()
        {
            var hands = _handRoot.GetMJHandSets();

            foreach(var pair in hands)
            {
                pair.Value.Clear();
            }
        }


        /// <summary>
        /// 排序手牌
        /// </summary>
        /// <param name="viewChairId"></param>
        public void Sort(int viewChairId)
        {
            GetHandSet(viewChairId).Sort();
        }


        /// <summary>
        /// 删除牌
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="card"></param>
        public int RemoveCard(int viewChairId, MJCard card)
        {
            var hand = GetHandSet(viewChairId);

            int index = hand.IndexOf(card);

            if (index == -1)
            {
                WLDebug.LogWarning("MJHandController.RemoveCard: 目标手牌不包含要删除的牌对象");
                return -1;
            }

            RemoveCardByIndex(viewChairId, index);

            return index;
        }


        /// <summary>
        /// 移除一张牌
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public void RemoveCardByIndex(int viewChairId, int index)
        {
            var hand = GetHandSet(viewChairId);

            hand.Remove(index);
        }

        /// <summary>
        /// 根据牌值列表删除牌节点
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValues"></param>
        public void RemoveCardsByValues(int viewChairId, List<int> cardValues)
        {
            var cards = FindCardsByValues(viewChairId, cardValues);

            cards.ForEach(c => RemoveCard(viewChairId, c));
        }

        /// <summary>
        /// 根据牌值查找牌对象，返回第一个找到的
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValue"></param>
        /// <param name="reverseSearch">是否反向查找</param>
        public MJCard FindCardByValue(int viewChairId, int cardValue, bool reverseSearch = false)
        {
            var hand = GetHandSet(viewChairId);

            if (reverseSearch)
            {
                for (int i = hand.count - 1; i >= 0; i--)
                {
                    MJCard card = hand.GetCard(i);

                    if (card.cardValue == cardValue)
                    {
                        return card;
                    }
                }
            }

            for (int i = 0; i < hand.count; i++)
            {
                MJCard card = hand.GetCard(i);

                if (card.cardValue == cardValue)
                {
                    return card;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据牌值列表获取牌对象列表，一个牌值只找一张牌
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public List<MJCard> FindCardsByValues(MJHandSet hand, List<int> cardValues)
        {
            List<MJCard> result = new List<MJCard>();

            List<MJCard> clone = new List<MJCard>(hand.GetCards());

            for (int i = 0; i < cardValues.Count; i++)
            {
                int value = cardValues[i];

                MJCard card = clone.Find(c => c.cardValue == value);

                if (card != null)
                {
                    result.Add(card);

                    clone.Remove(card);
                }
            }

            return result;
        }

        /// <summary>
        /// 同上
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValues"></param>
        /// <returns></returns>
        public List<MJCard> FindCardsByValues(int viewChairId, List<int> cardValues)
        {
            var hand = GetHandSet(viewChairId);

            return FindCardsByValues(hand, cardValues);
        }

        /// <summary>
        /// 获取牌值包含在cardValues中的所有牌节点
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValues"></param>
        /// <returns></returns>
        public List<MJCard> FindAllCardsByValues(int viewChairId, List<int> cardValues)
        {
            var hand = GetHandSet(viewChairId);

            List<MJCard> result = new List<MJCard>();

            for (int i = 0; i < hand.count; i++)
            {
                if (cardValues.Contains(hand.GetCard(i).cardValue))
                {
                    result.Add(hand.GetCard(i));
                }
            }

            return result;
        }

        /// <summary>
        /// 获取手牌对象
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public MJHandSet GetHandSet(int viewChairId)
        {
            return _handRoot.GetMJHandSet((MJOrientation)viewChairId);
        }

        /// <summary>
        /// 获取手牌对象索引
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public int IndexOf(MJHandSet hand)
        {
            var handSets = _handRoot.GetMJHandSets();

            foreach (var pair in handSets)
            {
                if (pair.Value == hand)
                {
                    return (int)pair.Key;
                }
            }

            return -1;
        }

        /// <summary>
        /// 根据牌控件获取牌值列表
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public List<int> GetValues(int viewChairId)
        {
            var result = new List<int>();
            var handSet = GetHandSet(viewChairId);
            var cards = handSet.GetCards();
            cards.ForEach(c => result.Add(c.cardValue));
            return result;
        }
        #endregion


        #region Functionalities
        /// <summary>
        /// 打乱顺序
        /// </summary>
        /// <param name="viewChairId"></param>
        public void ShuffleCards(int viewChairId)
        {
            var handSet = GetHandSet(viewChairId);
            var cards = handSet.GetCards();
            var clone = cards.Clone();
            cards.Clear();
            while(clone.Count > 0)
            {
                int idx = UnityEngine.Random.Range(0, clone.Count);
                cards.Add(clone[idx]);
                clone.RemoveAt(idx);
            }
            handSet.Refresh();
        }

        /// <summary>
        /// 隐藏所有牌节点
        /// </summary>
        public void HideAllCards()
        {
            for (int i = 0; i < 4; i++)
            {
                var handSet = GetHandSet(i);
                handSet.Traverse((c, idx) => c.gameObject.SetActive(false));
            }
        }

        /// <summary>
        /// 根据传入牌值设置手牌颜色，每个牌值只找一张牌
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="color"></param>
        /// <param name="cardValues"></param>
        public void TintByValues(int viewChairId, Color32 color, List<int> cardValues)
        {
            List<MJCard> cards = FindCardsByValues(viewChairId, cardValues);

            cards.ForEach(c => c.SetColor(color));
        }

        /// <summary>
        /// 根据传入牌值设置手牌颜色
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="color"></param>
        /// <param name="cardValues"></param>
        public void TintAllByValues(int viewChairId, Color32 color, List<int> cardValues)
        {
            List<MJCard> cards = FindAllCardsByValues(viewChairId, cardValues);

            cards.ForEach(c => c.SetColor(color));
        }

        /// <summary>
        /// 根基传入牌值设置手牌颜色
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="color"></param>
        /// <param name="cardValue"></param>
        public void TintByValue(int viewChairId, Color32 color, int cardValue)
        {
            TintAllByValues(viewChairId, color, new List<int> { cardValue });
        }

        /// <summary>
        /// 设置所有牌色调
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="color"></param>
        public void Tint(int viewChairId, Color32 color)
        {
            var hand = GetHandSet(viewChairId);

            hand.Traverse((c, i) => c.SetColor(color));
        }


        /// <summary>
        /// 重置颜色
        /// </summary>
        /// <param name="viewChairId"></param>
        public void ResetTint(int viewChairId)
        {
            var hand = GetHandSet(viewChairId);

            hand.Traverse((c, i) => c.SetColor(Color.white));
        }

        /// <summary>
        /// 根据传入牌值设置手牌变暗，每个牌值只找一张牌
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValues"></param>
        public void DarkenByValues(int viewChairId, List<int> cardValues)
        {
            TintByValues(viewChairId, darkenColor, cardValues);
        }

        /// <summary>
        /// 根据传入牌值设置手牌变暗
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValues"></param>
        public void DarkenAllByValues(int viewChairId, List<int> cardValues)
        {
            TintAllByValues(viewChairId, darkenColor, cardValues);
        }

        /// <summary>
        /// 根据传入牌值设置手牌变暗
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="cardValue"></param>
        public void DarkenByValue(int viewChairId, int cardValue)
        {
            DarkenAllByValues(viewChairId, new List<int> { cardValue });
        }

        /// <summary>
        /// 设置所有牌变暗
        /// </summary>
        /// <param name="viewChairId"></param>
        public void Darken(int viewChairId)
        {
            Tint(viewChairId, darkenColor);
        }

        /// <summary>
        /// 清除手牌胡牌提示标记
        /// </summary>
        /// <param name="viewChairId"></param>
        public void ClearHintMark(int viewChairId)
        {
            var hand = GetHandSet(viewChairId);

            hand.Traverse((c, i) => c.HideHintMark());
        }
        #endregion


        #region Widget Events
        /// <summary>
        /// 添加牌事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="card"></param>
        protected virtual void OnAppend(MJCardSet sender, MJCard card)
        {
            var hand = sender as MJHandSet;

            int viewChairId = IndexOf(hand);

            if (viewChairId != Chair.Down && !stage.isReplay)
            {
                return;
            }

            if (_gameData.jokerCardValues.Contains(card.cardValue))
            {
                card.ShowJokerMark();
            }

            int chairId = stage.ToServerChairId(viewChairId);
            var player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;
            if (player.dingqueColorValue == Card.GetCardColorValue(card.cardValue))
            {
                card.ShowLackMark();
            }

            _dingqueHandler?.UpdateSelfHandByDingque();
        }

        /// <summary>
        /// 移除牌事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="card"></param>
        protected virtual void OnRemove(MJCardSet sender, MJCard card)
        {
            if (IndexOf(sender as MJHandSet) != Chair.Down)
            {
                return;
            }

            _dingqueHandler?.UpdateSelfHandByDingque();
        }
        #endregion


        #region Widget Delegates
        /// <summary>
        /// 获取牌状态委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        protected virtual MJHandSet.State OnGetCardState(MJHandSet sender, int index)
        {
            int viewChairId = IndexOf(sender);

            if (viewChairId != Chair.Down)
            {
                if (stage.isReplay)
                {
                    return MJHandSet.State.Lying;
                }

                int chairId = stage.ToServerChairId(viewChairId);
                MJGamePlayer player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;
                if (player.isReadyHand && (sender.count % 3 != 2 || index != sender.count - 1))
                {
                    return MJHandSet.State.Covering;
                }
            }

            return MJHandSet.State.Standing;
        }

        /// <summary>
        /// 排序委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        protected virtual void OnSortCards(MJHandSet sender, List<MJCard> cards)
        {
            List<int> jokerCardValues = _gameData.jokerCardValues;

            bool jokerDiscardable = _gameData.jokerDiscardable;

            int viewChairId = IndexOf(sender);

            int chairId = stage.ToServerChairId(viewChairId);

            MJGamePlayer player = _gameData.GetPlayerByChairId(chairId) as MJGamePlayer;

            int dingqueColorValue = player.dingqueColorValue;

            bool includeLastCard = sender.count % 3 != 2;

            MJCard lastCard = null;

            if (!includeLastCard)
            {
                int lastIndex = cards.Count - 1;

                lastCard = cards[lastIndex];

                cards.RemoveAt(lastIndex);
            }

            // 从小到大排序
            cards.Sort((a, b) => a.cardValue - b.cardValue);

            // 赖子牌子放前面
            if (jokerCardValues.Count > 0)
            {
                cards.Classify(c => jokerCardValues.Contains(c.cardValue));
            }

            // 定缺牌放到最后
            if (dingqueColorValue != Card.ColorNone)
            {
                cards.Classify(c =>
                {
                    int colorValue = Card.GetCardColorValue(c.cardValue);

                    if (colorValue != dingqueColorValue)
                    {
                        return true;
                    }

                    // 是定缺牌的情况下如果是赖子并且赖子不可打出则放前面
                    if (jokerCardValues.Contains(c.cardValue) && !jokerDiscardable)
                    {
                        return true;
                    }

                    return false;
                });
            }

            if (lastCard != null)
            {
                cards.Add(lastCard);
            }
        }
        #endregion


        #region Input Delegates
        /// <summary>
        /// 自家手牌拖拽或双击手牌的出牌委托
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        protected virtual void OnDiscard(MJHandSet hand, MJCard card)
        {
            if (!Discardable(hand, card))
            {
                return;
            }

            int cardValue = card.cardValue;

            // 发送出牌消息
            SendRequest(cardValue);

            // 播放出牌动画
            _deskCardController.PlayDiscard(Chair.Down, cardValue, card);

            var isLastCard = _ownHand.IndexOf(card) == _ownHand.count - 1;

            RemoveCard(Chair.Down, card);

            if (!isLastCard)
            {
                PlayInsert(Chair.Down);
            }

            _cardTintController?.Reset();

            var nextViewChairId = stage.NextViewChairId(Chair.Down);

            _avatarController.SwitchLightEffect(nextViewChairId);

            _tableController.PlayTurn(nextViewChairId, _gameData.trustTimeout, null);

            _actionButtonController.Clear();
        }

        /// <summary>
        /// 自家手牌触摸选牌事件，返回false表示不可选中
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        protected virtual bool OnSelect(MJHandSet hand, MJCard card)
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            List<int> unselectableValues = playerSelf.unselectableCardValues;

            if (unselectableValues.Count > 0)
            {
                List<MJCard> unselectableCards = FindCardsByValues(hand, unselectableValues);

                if (unselectableCards.Contains(card))
                {
                    return false;
                }
            }

            // 显示胡牌提示弹窗
            _huInfoHandler?.ShowHintsPanel(card.cardValue);

            _cardTintController?.Reset();

            _cardTintController?.Highlight(card.cardValue);

            _audioController.PlaySelectCard();

            return true;
        }

        /// <summary>
        /// 点击空白区域
        /// </summary>
        protected virtual void OnClickBlank()
        {
            // 隐藏胡牌提示UI
            _hintsController?.HideHints(true);

            _cardTintController?.Reset();
        }
        #endregion


        #region Business Logic
        /// <summary>
        /// 手牌是否可出牌
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        protected virtual bool Discardable(MJHandSet hand, MJCard card)
        {
            if (!_gameData.jokerDiscardable && _gameData.jokerCardValues.Contains(card.cardValue))
            {
                return false;
            }

            var discardable = _dingqueHandler == null ? false : _dingqueHandler.DiscardableByDingque(card.cardValue);
            if (!discardable)
            {
                return false;
            }

            if (_gameData.verifyingOutCardValue != Card.Invalid)
            {
                return false;
            }

            return hand.count % 3 == 2;
        }

        /// <summary>
        /// 发送出牌或动作请求逻辑
        /// </summary>
        /// <param name="cardValue"></param>
        protected virtual void SendRequest(int cardValue)
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            if (!playerSelf.handCardValues.Contains(cardValue))
            {
                WLDebug.LogWarning($"MJHandController.SendRequest: 错误提示：自家手牌数据中不包含牌值[{cardValue}]");

                return;
            }

            var inReadyHandSelectCardState = _actionButtonController.IsInReadyHandSelectCardState();
            if (!inReadyHandSelectCardState)
            {
                stage.Send("SendOutCard", cardValue, false, true);

                return;
            }

            stage.Send("SendAction", GetReadyHandActionType(), cardValue);

            _actionButtonController.ExitReadyHandSelectCardState();

            _gameData.verifyingOutCardValue = cardValue;
        }

        /// <summary>
        /// 获取听牌动作类型值
        /// </summary>
        /// <returns></returns>
        protected virtual int GetReadyHandActionType()
        {
            return _gameData.GetActionData(ActionShowType.Ting).actionType;
        }
        #endregion


        #region Initialization
        private void InitializeHands()
        {
            int maxCount = _gameData.maxHandCardCount;

            var handSets = _handRoot.GetMJHandSets();
            foreach(var pair in handSets)
            {
                var hand = pair.Value;

                Camera worldCamera = hand == _ownHand ? _space.handSetCamera.camera : CameraUtil.GetMainCamera();

                hand.Initialize(maxCount, worldCamera);

                hand.onAppend = OnAppend;

                hand.onRemove = OnRemove;

                hand.onSortCards = OnSortCards;

                hand.onGetCardState = OnGetCardState;

            }

            InitializeOwnHand();
        }

        private void InitializeOwnHand()
        {
            if (!stage.isReplay)
            {
                _inputController.onSelect = OnSelect;
                _inputController.onDiscard = OnDiscard;
                _inputController.onClickBlank = OnClickBlank;
                _inputController.BindTarget(_ownHand);
            }

            float width = _ownHand.maxWidth;
            Vector3 pos = _ownHand.gameObject.transform.position;
            pos.x += _ownHand.GetHandCardAnchor() == HandCardAnchor.Left ? width * 0.5f : -width * 0.5f;
            _space.handSetCamera.SetupCamera(pos, width, _gameData.maxHandCardCount == 17 ? 0.2f : 0.165f);
        }
        #endregion


        #region Game Events
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
        #endregion
    }
}
