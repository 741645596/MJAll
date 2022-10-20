// @Author: tanjinhua
// @Date: 2021/4/9  14:58


using System;
using Unity.Widget;
using UnityEngine;
using UnityEngine.EventSystems;
using WLCore.Entity;
using WLHall.Game;

namespace MJCommon
{
    public class MJInputController : BaseGameController
    {
        public Func<MJHandSet, MJCard, bool> onSelect; //选中事件
        public Action<MJHandSet, MJCard> onDiscard; // 出牌事件
        public Action onClickBlank; // 点击空白区域事件

        private MJGameData _gameData;
        private MJHandSet _target;
        private MJBlankPlane _blankPlane;
        private TouchInput3D _touchInput3D;
        private PointerEventData _currentEventData;
        private MJCard _lifted;
        private MJCard _selected;
        private MJCard _beginSelected;
        private MJCard _draging;
        private int _dragingIndex;

        private MJSpaceController _spaceController;
        private MJExchangeHandler _exchangeHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _spaceController = stage.GetController<MJSpaceController>();
            _exchangeHandler = stage.GetController<MJExchangeHandler>();

            var _touch3DLayer = new MJTouch3DLayer();

            _touch3DLayer.AddTo(WDirector.GetRootLayer(), MJZorder.HandCardControl);

            _touchInput3D = _touch3DLayer.touchInput3D;
        }

        /// <summary>
        /// 绑定需要操作的手牌节点
        /// </summary>
        /// <param name="target"></param>
        public void BindTarget(MJHandSet target)
        {
            if (SafeGet(_target) != null)
            {
                return;
            }

            _target = target;

            _blankPlane = new MJBlankPlane();

            _blankPlane.Initialize(_target.renderCamera, _touchInput3D);

            _blankPlane.onClick = HandleClickBlank;

            _blankPlane.SetParent(_spaceController.GetSpace());

            _target.Traverse((c, i) => Register(c));

            _target.onAppend += OnTargetAppendCard;

            _target.onRemove += OnTargetRemoveCard;
        }

        protected virtual MJCard OnCreateDragingCard(int cardValue)
        {
            return new MJCard(cardValue);
        }


        #region Input business
        protected virtual void OnBegin(MJHandSet target, MJCard card, TouchData3D touchData)
        {
            if (!IsFirstHit(touchData))
            {
                return;
            }

            if (Select(target, card))
            {
                _beginSelected = card;
            }
        }


        protected virtual void OnEnter(MJHandSet target, MJCard card, TouchData3D touchData)
        {
            if (_gameData.gameState == GameState.Exchange)
            {
                return;
            }

            if (!IsFirstHit(touchData))
            {
                return;
            }

            if (Select(target, card))
            {
                if (SafeGet(_beginSelected) != card)
                {
                    _beginSelected = null;
                }

                SafeGet(_draging)?.Destroy();
                _draging = null;
            }
        }


        protected virtual void OnMove(MJHandSet target, MJCard card, TouchData3D touchData)
        {
            if (_gameData.gameState == GameState.Exchange)
            {
                return;
            }

            if (SafeGet(_beginSelected) != card || touchData.hitIndex != -1)
            {
                return;
            }

            Vector3 pos = target.renderCamera.ScreenToWorldPoint(touchData.eventData.position);

            pos.z = card.gameObject.transform.position.z;

            Vector3 euler = card.body.transform.eulerAngles;

            if (SafeGet(_draging) == null)
            {
                _draging = OnCreateDragingCard(card.cardValue);
                _draging.gameObject.transform.localScale *= 1.2f;
                _draging.gameObject.SetLayer(LayerMask.NameToLayer(MJDefine.MJHandMask));
            }

            _draging.gameObject.transform.position = pos;
            _draging.body.transform.eulerAngles = euler;
            _dragingIndex = target.IndexOf(card);

            _selected = null;
        }


        protected virtual void OnEnd(MJHandSet target, MJCard card, TouchData3D touchData)
        {
            if (SafeGet(_selected) == null && SafeGet(_lifted) != null)
            {
                _selected = _lifted;
            }

            _beginSelected = null;

            if (SafeGet(_draging) != null)
            {
                _draging.Destroy();
                _draging = null;

                Discard(target, _dragingIndex);
            }
        }


        protected virtual void OnClick(MJHandSet target, MJCard card, TouchData3D touchData)
        {
            if (SafeGet(_selected) == card)
            {
                Discard(target, card);
            }
        }


        protected virtual void OnClickBlank(MJHandSet target)
        {
            if (_gameData.gameState == GameState.Exchange)
            {
                target.UnselectAll();

                _exchangeHandler.ClearSelfSelectedCards();

                return;
            }

            Deselect(target);

            _selected = null;

            onClickBlank?.Invoke();
        }

        protected virtual bool Select(MJHandSet target, MJCard card)
        {
            if (_gameData.gameState == GameState.Exchange)
            {
                if (_exchangeHandler.AddSelfSelectedCard(card))
                {
                    target.SelectCard(card.gameObject);
                }
                return false;
            }

            if (onSelect == null || !onSelect.Invoke(target, card))
            {
                return false;
            }

            Deselect(target);

            if (SafeGet(_selected) != card)
            {
                _selected = null;
            }

            target.SelectCard(card.gameObject);

            _lifted = card;

            return true;
        }

        protected virtual void Deselect(MJHandSet target)
        {
            if (SafeGet(_lifted) == null)
            {
                return;
            }

            int index = target.IndexOf(_lifted);

            if (index != -1)
            {
                target.UnselectCard(target.GetCard(index).gameObject);

                _lifted = null;
            }
        }

        protected virtual void Discard(MJHandSet target, MJCard card)
        {
            if (_gameData.gameState == GameState.Exchange)
            {
                return;
            }

            onDiscard?.Invoke(target, card);
        }

        protected void Discard(MJHandSet target, int index)
        {
            MJCard card = target.GetCard(index);

            Discard(target, card);
        }
        #endregion


        #region Widget Event
        private void OnTargetAppendCard(MJCardSet target, MJCard card)
        {
            if (target == _target)
            {
                Register(card);
            }
        }

        private void OnTargetRemoveCard(MJCardSet target, MJCard card)
        {
            _touchInput3D.RemoveListener(card.body.gameObject);
        }

        //protected virtual void OnPlaceCard(MJCardSet target, int index, MJCard card)
        //{
        //    MJHandSet hand = target as MJHandSet;

        //    if (SafeGet(_lifted) == card)
        //    {
        //        Deselect(hand);
        //    }

        //    if (SafeGet(_selected) == card)
        //    {
        //        _selected = null;
        //    }
        //}
        #endregion


        #region Dispatch events
        private void Register(MJCard card)
        {
            if (SafeGet(_target) == null)
            {
                WLDebug.LogWarning("MJInputController.Register: 目标手牌为空");
                return;
            }

            card.body.AddComponent<BoxCollider>();

            var renderCamera = _target.renderCamera;

            _touchInput3D.AddListener(card.body, renderCamera, TouchEvent3D.Begin, HandleBegin);

            _touchInput3D.AddListener(card.body, renderCamera, TouchEvent3D.Move, HandleMove);

            _touchInput3D.AddListener(card.body, renderCamera, TouchEvent3D.End, HandleEnd);

            _touchInput3D.AddListener(card.body, renderCamera, TouchEvent3D.Enter, HandleEnter);

            _touchInput3D.AddListener(card.body, renderCamera, TouchEvent3D.Click, HandleClick);
        }

        private void HandleBegin(TouchData3D touchData)
        {
            if (SafeGet(_target) == null)
            {
                WLDebug.LogWarning("MJInputController.HandleBegin: 目标手牌为空");
                return;
            }

            if (!_target.interactable)
            {
                return;
            }

            if (_currentEventData != null)
            {
                return;
            }

            _currentEventData = touchData.eventData;

            var cards = _target.GetCards();

            int index = cards.GetIndexByCondition(card => card.body == touchData.receiver);

            if (index != -1)
            {
                OnBegin(_target, _target.GetCard(index), touchData);
            }
        }

        private void HandleMove(TouchData3D touchData)
        {
            if (SafeGet(_target) == null)
            {
                WLDebug.LogWarning("MJInputController.HandleMove: 目标手牌为空");
                return;
            }

            if (!_target.interactable)
            {
                return;
            }

            var cards = _target.GetCards();

            int index = cards.GetIndexByCondition(card => card.body == touchData.receiver);

            if (IsCurrentTouch(touchData) && index != -1)
            {
                OnMove(_target, _target.GetCard(index), touchData);
            }
        }

        private void HandleEnd(TouchData3D touchData)
        {
            if (SafeGet(_target) == null)
            {
                WLDebug.LogWarning("MJInputController.HandleEnd: 目标手牌为空");
                return;
            }

            if (!_target.interactable)
            {
                return;
            }

            if (IsCurrentTouch(touchData))
            {
                var cards = _target.GetCards();
                int index = cards.GetIndexByCondition(card => card.body == touchData.receiver);
                if (index != -1)
                {
                    OnEnd(_target, _target.GetCard(index), touchData);
                }

                _currentEventData = null;
            }
        }

        private void HandleEnter(TouchData3D touchData)
        {
            if (SafeGet(_target) == null)
            {
                WLDebug.LogWarning("MJInputController.HandleEnter: 目标手牌为空");
                return;
            }

            if (!_target.interactable)
            {
                return;
            }
            var cards = _target.GetCards();
            int index = cards.GetIndexByCondition(card => card.body == touchData.receiver);
            if (IsCurrentTouch(touchData) && index != -1)
            {
                OnEnter(_target, _target.GetCard(index), touchData);
            }
        }

        private void HandleClick(TouchData3D touchData)
        {
            if (SafeGet(_target) == null)
            {
                WLDebug.LogWarning("MJInputController.HandleClick: 目标手牌为空");
                return;
            }

            if (!_target.interactable)
            {
                return;
            }
            var cards = _target.GetCards();
            int index = cards.GetIndexByCondition(card => card.body == touchData.receiver);
            if (IsCurrentTouch(touchData) && index != -1)
            {
                OnClick(_target, _target.GetCard(index), touchData);
            }
        }

        private void HandleClickBlank()
        {
            if (SafeGet(_target) == null)
            {
                WLDebug.LogWarning("MJInputController.HandleClickBlank: 目标手牌为空");
                return;
            }

            if (!_target.interactable)
            {
                return;
            }

            OnClickBlank(_target);
        }
        #endregion



        private bool IsCurrentTouch(TouchData3D touchData)
        {
            if (_currentEventData == null)
            {
                return false;
            }

            return _currentEventData.pointerId == touchData.eventData.pointerId;
        }

        private bool IsFirstHit(TouchData3D data)
        {
            return data.hitIndex == 0;
        }

        private BaseEntity SafeGet(BaseEntity c)
        {
            if (c == null || c.gameObject == null)
            {
                return null;
            }
            return c;
        }
    }
}
