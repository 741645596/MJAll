// @Author: tanjinhua
// @Date: 2021/12/23  11:28


using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace MJCommon
{
    public abstract class MJEndRecord : BaseRecord
    {
        /// <summary>
        /// 结束类型
        /// </summary>
        protected abstract ushort endType { get; }
        /// <summary>
        /// 点炮玩家服务器座位号
        /// </summary>
        protected abstract int dianpaoChairId { get; }
        /// <summary>
        /// 胡牌玩家服务器座位号列表
        /// </summary>
        protected abstract List<ushort> huChairIds { get; }
        /// <summary>
        /// 胡牌类型列表
        /// </summary>
        protected abstract List<long> huTypes { get; }


        // references
        private MJGameData _gameData;
        private MJHandController _handController;
        private MJTableController _tableController;
        private MJAvatarController _avatarController;
        private MJSettleController _settleController;
        private MJDeskCardController _deskCardController;
        private MJActionButtonController _actionButtonController;
        private MJActionEffectController _actionEffectController;

        // states
        private bool _deskCardRemoved;
        private bool _handCardAdded;


        public override void OnInitialize()
        {
            _gameData = stage.GetGameData<MJGameData>();
            _handController = stage.GetController<MJHandController>();
            _tableController = stage.GetController<MJTableController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _deskCardController = stage.GetController<MJDeskCardController>();
            _actionButtonController = stage.GetController<MJActionButtonController>();
            _actionEffectController = stage.GetController<MJActionEffectController>();
            _settleController = stage.GetController<MJSettleController>();
        }


        #region Implementation
        public override float Execute()
        {
            StopCoroutine();

            StartCoroutine(IEPlayGameEnd());

            return 1f;
        }

        public override void Undo()
        {
            StopCoroutine();

            Clear();

            HandleBlankEnd(false, true);

            HandleDeskCard(false, true);

            HandleHandCard(false, true);

            HandleActionEffect(false, true);

            HandleHuEffect(false, true);

            HandleSettleView(false, true);
        }

        public override void Abort()
        {
            StopCoroutine();

            Clear();

            HandleBlankEnd(true, false);

            HandleDeskCard(true, false);

            HandleHandCard(true, false);

            HandleActionEffect(true, false);

            HandleHuEffect(true, false);

            HandleSettleView(true, false);
        }
        #endregion



        protected virtual IEnumerator IEPlayGameEnd()
        {
            Clear();

            if (HandleBlankEnd(false, false))
            {
                yield return new WaitForSeconds(1f);
            }

            HandleDeskCard(false, false);

            HandleHandCard(false, false);

            HandleActionEffect(false, false);

            yield return new WaitForSeconds(1.2f);

            HandleHuEffect(false, false);

            //yield return new WaitForSeconds(2f); // 暂无胡牌特效，屏蔽延时

            HandleSettleView(false, false);
        }


        /// <summary>
        /// 清除不必要UI
        /// </summary>
        protected void Clear()
        {
            _tableController.StopTurn();
            _avatarController.HideAllLightEffect();
            _actionButtonController.ClearButtons();
        }


        /// <summary>
        /// 荒庄动画处理
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected bool HandleBlankEnd(bool isAbort, bool isUndo)
        {
            if (endType != EndType.HuangZhuang)
            {
                return false;
            }

            // TODO:

            return true;
        }


        /// <summary>
        /// 点炮玩家出牌处理
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected void HandleDeskCard(bool isAbort, bool isUndo)
        {
            if (endType != EndType.DianPao)
            {
                return;
            }

            if (dianpaoChairId == Chair.Invalid)
            {
                return;
            }

            var viewChairId = stage.ToViewChairId(dianpaoChairId);
            var cardSet = _deskCardController.GetDeskCardSet(viewChairId);
            var player = _gameData.GetPlayerByChairId<MJGamePlayer>(dianpaoChairId);

            if (isUndo && _deskCardRemoved)
            {
                cardSet.Append(player.outCardValues[player.outCardValues.Count - 1]);
                cardSet.Refresh();
                _deskCardRemoved = false;
                return;
            }

            if (!_deskCardRemoved)
            {
                _deskCardController.Pop(viewChairId);
                _deskCardRemoved = true;
            }
        }


        /// <summary>
        /// 胡牌玩家手牌处理
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected void HandleHandCard(bool isAbort, bool isUndo)
        {
            if (endType != EndType.DianPao)
            {
                return;
            }

            if (dianpaoChairId == Chair.Invalid)
            {
                return;
            }

            if (isUndo && _handCardAdded)
            {
                for (int i = 0; i < huChairIds.Count; i++)
                {
                    var chairId = huChairIds[i];
                    var viewChairId = stage.ToViewChairId(chairId);
                    _handController.GetHandSet(viewChairId).RemoveLast();
                }

                _handCardAdded = false;

                return;
            }

            if (!_handCardAdded)
            {
                var dianpaoCardValue = _gameData.GetPlayerByChairId<MJGamePlayer>(dianpaoChairId).outCardValues.Peek(true);
                for (int i = 0; i < huChairIds.Count; i++)
                {
                    var chairId = huChairIds[i];
                    var viewChairId = stage.ToViewChairId(chairId);
                    var handSet = _handController.GetHandSet(viewChairId);
                    handSet.Append(dianpaoCardValue);
                    handSet.Refresh();
                }

                _handCardAdded = true;
            }
        }


        /// <summary>
        /// 动作特效处理
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected void HandleActionEffect(bool isAbort, bool isUndo)
        {
            if (endType != EndType.ZiMo && endType != EndType.DianPao)
            {
                return;
            }

            if (endType == EndType.DianPao && dianpaoChairId == Chair.Invalid)
            {
                return;
            }

            if (isUndo || isAbort)
            {
                _actionEffectController.Clear();
                return;
            }

            if (dianpaoChairId != Chair.Invalid)
            {
                _actionEffectController.PlayActionEffect(dianpaoChairId, ActionShowType.Dianpao, null);
            }

            var showType = endType == EndType.ZiMo ? ActionShowType.Zimo : ActionShowType.Hu;

            for (int i = 0; i < huChairIds.Count; i++)
            {
                _actionEffectController.PlayHuActionEffect(huChairIds[i], showType, huTypes[i]);
            }
        }


        /// <summary>
        /// 胡牌特效处理
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected void HandleHuEffect(bool isAbort, bool isUndo)
        {
            if (endType != EndType.ZiMo && endType != EndType.DianPao)
            {
                return;
            }

            if (endType == EndType.DianPao && dianpaoChairId == Chair.Invalid)
            {
                return;
            }

            // TODO
        }


        /// <summary>
        /// 结算界面处理
        /// </summary>
        /// <param name="isAbort"></param>
        /// <param name="isUndo"></param>
        protected void HandleSettleView(bool isAbort, bool isUndo)
        {
            if (isUndo)
            {
                _settleController.RemoveSettleView();
                return;
            }

            if (isAbort && !_settleController.SettleViewExist())
            {
                _settleController.DisplaySettleView(OnCreateSettleView());
                return;
            }

            _settleController.DisplaySettleView(OnCreateSettleView());
        }


        protected abstract SettleViewBase OnCreateSettleView();


        /// <summary>
        /// 获取有效胡牌玩家座位号列表
        /// </summary>
        /// <param name="huFlags"></param>
        /// <returns></returns>
        protected List<ushort> GetValidHuChairIds(List<bool> huFlags)
        {
            List<ushort> result = new List<ushort>();

            for (ushort chairId = 0; chairId < huFlags.Count; chairId++)
            {
                if (huFlags[chairId])
                {
                    result.Add(chairId);
                }
            }

            return result;
        }


        /// <summary>
        /// 获取有效胡牌类型列表
        /// </summary>
        /// <param name="huFlags"></param>
        /// <param name="allHuTypes"></param>
        /// <returns></returns>
        protected List<long> GetValidHuTypes(List<bool> huFlags, List<long> allHuTypes)
        {
            List<long> result = new List<long>();

            for (ushort chairId = 0; chairId < huFlags.Count; chairId++)
            {
                if (huFlags[chairId])
                {
                    result.Add(allHuTypes[chairId]);
                }
            }

            return result;
        }
    }
}
