// @Author: tanjinhua
// @Date: 2021/12/20  18:18


using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using WLCore;

namespace MJCommon
{
    public class MJActionResultRecord : BaseRecord
    {
        // datas
        protected int _actionChairId;
        protected int _actionViewChairId;
        protected int _actionShowType;
        protected List<int> _toRemoveCardValues;
        protected int _fuziIndex;
        protected FuziData _fuziData;
        protected FuziData _savedFuziData; // 用来做恢复

        // references
        protected MJGameData _gameData;
        protected MJGamePlayer _actionPlayer;
        protected MJGamePlayer _providerPlayer;
        protected MJHandController _handController;
        protected MJMeldController _meldController;
        protected MJAvatarController _avatarController;
        protected MJWinCardController _winCardController;
        protected MJDeskCardController _deskCardController;
        protected MJActionButtonController _actionButtonController;
        protected MJActionEffectController _actionEffectController;

        // states
        private Sequence _handAnimation;
        private Sequence _meldAnimation;
        private Sequence _refreshHandAnimation;
        private bool _handCardRemoved;
        private bool _winCardAdded;
        private bool _providerDeskCardRemoved;

        public override void OnInitialize()
        {
            _gameData = stage.gameData as MJGameData;
            _handController = stage.GetController<MJHandController>();
            _meldController = stage.GetController<MJMeldController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _winCardController = stage.GetController<MJWinCardController>();
            _deskCardController = stage.GetController<MJDeskCardController>();
            _actionButtonController = stage.GetController<MJActionButtonController>();
            _actionEffectController = stage.GetController<MJActionEffectController>();
        }

        #region Implementation
        public override void Read(MsgHeader msg)
        {
            _actionChairId = msg.ReadByte();
            _actionShowType = msg.ReadByte();
            _toRemoveCardValues = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                var v = msg.ReadByte();
                if (v != Card.Invalid)
                {
                    _toRemoveCardValues.Add(v);
                }
            }
            _fuziIndex = msg.ReadByte();
            _fuziData = FuziData.From(msg);

            _actionViewChairId = stage.ToViewChairId(_actionChairId);
            _actionPlayer = _gameData.GetPlayerByChairId(_actionChairId) as MJGamePlayer;
            _providerPlayer = _gameData.GetPlayerByChairId(_fuziData.provideUser) as MJGamePlayer;
        }

        public override float Execute()
        {
            // 更新数据
            UpdateFlowerValue(false);
            UpdateFuziData(false);
            UpdateProviderOutValue(false);
            UpdateReadyHandState(false);
            UpdateHandValue(false);

            // 更新UI
            StopCoroutine();
            StartCoroutine(IEPlayActionResult());

            return 1f;
        }

        public override void Undo()
        {
            // 恢复数据
            UpdateFlowerValue(true);
            UpdateFuziData(true);
            UpdateProviderOutValue(true);
            UpdateReadyHandState(true);
            UpdateHandValue(true);

            // 恢复UI
            StopCoroutine();
            _actionEffectController.Clear();
            _actionButtonController.ClearButtons();
            UpdateHandSet(false, true);
            UpdateFlower(false, true);
            UpdateMeldStack(false, true);
            UpdateProviderDeskCard(false, true);
            ShowReadyHandIcon(false);
            RefreshActionPlayerHand(false, true);
        }

        public override void Abort()
        {
            StopCoroutine();
            _actionEffectController.Clear();
            _actionButtonController.ClearButtons();
            UpdateHandSet(true, false);
            UpdateFlower(true, false);
            UpdateMeldStack(true, false);
            UpdateProviderDeskCard(true, false);
            ShowReadyHandIcon(true);
            RefreshActionPlayerHand(true, false);
        }
        #endregion


        #region Data update
        /// <summary>
        ///  更新动作玩家手牌数据
        /// </summary>
        /// <param name="isUndo"></param>
        protected virtual void UpdateHandValue(bool isUndo)
        {
            if (isUndo)
            {
                _actionPlayer.AddHandCardValues(_toRemoveCardValues);
            }
            else
            {
                _actionPlayer.RemoveHandCardValues(_toRemoveCardValues);
            }

            _actionPlayer.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable, true);
        }

        /// <summary>
        /// 更新动作玩家花牌数据
        /// </summary>
        /// <param name="isUndo"></param>
        protected virtual void UpdateFlowerValue(bool isUndo)
        {
            if (_actionShowType != ActionShowType.Buhua)
            {
                return;
            }

            if (isUndo)
            {
                _actionPlayer.RemoveLastWinCardValue();
            }
            else
            {
                _actionPlayer.AddWinCardValue(_toRemoveCardValues[0]);
            }
        }

        /// <summary>
        /// 更新动作玩家副子数据
        /// </summary>
        /// <param name="isUndo"></param>
        protected virtual void UpdateFuziData(bool isUndo)
        {
            if (_fuziIndex == MJDefine.InvaildFuziIndex)
            {
                return;
            }

            if (isUndo)
            {
                if (_savedFuziData == null)
                {
                    _actionPlayer.RemoveLastFuziData();
                }
                else
                {
                    _actionPlayer.ReplaceFuziData(_fuziIndex, _savedFuziData);
                    _savedFuziData = null;
                }
                return;
            }

            if (_fuziIndex >= _actionPlayer.fuziCount)
            {
                _actionPlayer.AddFuziData(_fuziData);
            }
            else
            {
                _savedFuziData = _actionPlayer.fuziDatas[_fuziIndex];
                _actionPlayer.ReplaceFuziData(_fuziIndex, _fuziData);
            }
        }

        /// <summary>
        /// 更新副子提供者出牌数据
        /// </summary>
        /// <param name="isUndo"></param>
        protected virtual void UpdateProviderOutValue(bool isUndo)
        {
            if (!NeedsRemoveOutCard())
            {
                return;
            }

            if (isUndo)
            {
                _providerPlayer.AddOutCardValue(_fuziData.operateCard);
            }
            else
            {
                _providerPlayer.RemoveLastOutCardValue();
            }
        }

        /// <summary>
        /// 更新动作玩家听牌状态
        /// </summary>
        /// <param name="isUndo"></param>
        protected virtual void UpdateReadyHandState(bool isUndo)
        {
            if (_actionShowType != ActionShowType.Ting)
            {
                return;
            }

            if (isUndo)
            {
                _actionPlayer.isReadyHand = false;
                _actionPlayer.unselectableCardValues = null;
                return;
            }

            _actionPlayer.isReadyHand = true;

            var handValues = _actionPlayer.handCardValues;
            var includeLastCard = handValues.Count % 3 != 2;
            if (!includeLastCard)
            {
                handValues.RemoveAt(handValues.Count - 1);
            }
            _actionPlayer.unselectableCardValues = handValues;
        }
        #endregion


        #region Scene update
        protected virtual IEnumerator IEPlayActionResult()
        {
            // 清除动作按钮
            _actionButtonController.ClearButtons();

            // 播放动作特效
            _actionEffectController.PlayActionEffect(_actionChairId, _actionShowType, _fuziData);

            // 播放手牌动画
            UpdateHandSet(false, false);

            yield return new WaitForSeconds(0.5f);

            // 播放花牌动画
            UpdateFlower(false, false);

            // 播放副子动画
            UpdateMeldStack(false, false);

            // 删除副子提供者出牌
            UpdateProviderDeskCard(false, false);

            yield return new WaitForSeconds(0.1f);

            // 显示听牌小图标
            ShowReadyHandIcon(true);

            // 播放理牌动画
            RefreshActionPlayerHand(false, false);
        }

        /// <summary>
        /// 更新手牌
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected virtual void UpdateHandSet(bool isAbort, bool isUndo)
        {
            if (_actionShowType == ActionShowType.Ting)
            {
                return;
            }

            if (_toRemoveCardValues.Count == 0)
            {
                return;
            }

            _handAnimation?.Kill();
            _handAnimation = null;

            var handSet = _handController.GetHandSet(_actionViewChairId);

            if (isUndo && _handCardRemoved)
            {
                _toRemoveCardValues.ForEach(v => handSet.Append(v));
                handSet.Refresh();
                _handCardRemoved = false;
                return;
            }

            if (isAbort && !_handCardRemoved)
            {
                handSet.Remove(_toRemoveCardValues);
                handSet.Refresh();
                _handCardRemoved = true;
                return;
            }

            _handAnimation = MJAnimationHelper.PlaySelectHandCardAnimation(handSet, _toRemoveCardValues, null, () =>
            {
                handSet.Remove(_toRemoveCardValues);
                _handCardRemoved = true;
            });
        }

        /// <summary>
        /// 更新花牌
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected virtual void UpdateFlower(bool isAbort, bool isUndo)
        {
            if (_actionShowType != ActionShowType.Buhua)
            {
                return;
            }

            if (isUndo && _winCardAdded)
            {
                _winCardController.Pop(_actionViewChairId);
                _winCardAdded = false;
                return;
            }

            if (isAbort && !_winCardAdded)
            {
                _winCardController.Push(_actionViewChairId, _toRemoveCardValues[0]);
                _winCardAdded = true;
                return;
            }

            _winCardController.Push(_actionViewChairId, _toRemoveCardValues[0]); // 暂时直接显示
            _winCardAdded = true;
        }

        /// <summary>
        /// 更新副子
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected virtual void UpdateMeldStack(bool isAbort, bool isUndo)
        {
            if (_actionShowType == ActionShowType.Ting)
            {
                return;
            }

            if (_fuziIndex == MJDefine.InvaildFuziIndex)
            {
                return;
            }

            var arg = _meldController.ToMeldData(_actionChairId, _fuziData);
            var meldStack = _meldController.GetMeldStack(_actionViewChairId);

            if (isUndo && _meldAnimation != null)
            {
                _meldAnimation.Kill();
                _meldAnimation = null;

                if (_fuziIndex == meldStack.count - 1)
                {
                    meldStack.RemoveLast();
                }
                else
                {
                    meldStack.Replace(_fuziIndex, _meldController.ToMeldData(_actionChairId, _actionPlayer.fuziDatas[_fuziIndex]));
                }
                meldStack.Refresh();

                return;
            } 

            if (isAbort && _meldAnimation == null)
            {
                if (_fuziIndex == meldStack.count)
                {
                    meldStack.Append(arg);
                }
                else
                {
                    meldStack.Replace(_fuziIndex, arg);
                }
                meldStack.Refresh();
                _meldAnimation = DOTween.Sequence();

                return;
            }

            if (_fuziIndex == meldStack.count)
            {
                _meldAnimation = _meldController.PlayAppend(_actionViewChairId, arg);
            }
            else
            {
                _meldAnimation = _meldController.PlayReplace(_actionViewChairId, _fuziIndex, arg);
            }
        }

        /// <summary>
        /// 更新提供者出牌
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected virtual void UpdateProviderDeskCard(bool isAbort, bool isUndo)
        {
            if (!NeedsRemoveOutCard())
            {
                return;
            }

            var viewChairId = stage.ToViewChairId(_fuziData.provideUser);
            if (isUndo && _providerDeskCardRemoved)
            {
                var deskCardSet = _deskCardController.GetDeskCardSet(viewChairId);
                deskCardSet.Append(_fuziData.operateCard);
                deskCardSet.Refresh();
                _providerDeskCardRemoved = false;
                return;
            }

            if (isAbort && !_providerDeskCardRemoved)
            {
                _deskCardController.Pop(viewChairId);
                _providerDeskCardRemoved = true;
                return;
            }

            _deskCardController.Pop(viewChairId);
            _providerDeskCardRemoved = true;
        }

        /// <summary>
        /// 更新听牌小图表状态
        /// </summary>
        /// <param name="visible"></param>
        protected virtual void ShowReadyHandIcon(bool visible)
        {
            if (_actionShowType != ActionShowType.Ting)
            {
                return;
            }

            if (visible)
            {
                _avatarController.ShowReadyHandIcon(_actionViewChairId);
            }
            else
            {
                _avatarController.RemoveReadyHandIcon(_actionViewChairId);
            }
        }

        /// <summary>
        /// 刷新手牌
        /// </summary>
        protected virtual void RefreshActionPlayerHand(bool isAbort, bool isUndo)
        {
            if (_actionShowType == ActionShowType.Ting)
            {
                return;
            }

            if (_toRemoveCardValues.Count == 0)
            {
                return;
            }

            var handSet = _handController.GetHandSet(_actionViewChairId);
            if (isUndo && _refreshHandAnimation != null)
            {
                _refreshHandAnimation.Kill();
                _refreshHandAnimation = null;
                handSet.Refresh();
                return;
            }

            if (isAbort && _refreshHandAnimation == null)
            {
                handSet.Refresh();
                _refreshHandAnimation = DOTween.Sequence();
                return;
            }

            _refreshHandAnimation = MJAnimationHelper.PlayRefreshHandsetAnimation(handSet);
        }
        #endregion


        /// <summary>
        /// 是否需要移除副子提供者出牌
        /// </summary>
        /// <returns></returns>
        protected virtual bool NeedsRemoveOutCard()
        {
            if (_fuziIndex == MJDefine.InvaildFuziIndex)
            {
                return false;
            }

            if (_fuziData.operateCard == Card.Invalid)
            {
                return false;
            }

            if (_actionChairId == _fuziData.provideUser)
            {
                return false;
            }

            return _fuziData.weaveKind1 != ActionType.GangPuBuGang;
        }
    }
}
