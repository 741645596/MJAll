// @Author: tanjinhua
// @Date: 2021/4/13  13:48


using System.Collections.Generic;
using Unity.Widget;
using UnityEngine;
using WLCore;
using WLHall.Game;

namespace MJCommon
{

    public class MJActionButtonController : BaseGameController
    {
        private MJGameData _gameData;
        private MJActionButtonPanel _panel;                 // 动作按钮层
        private MJReplayActionButtonPanel _replayPanel;     // 回放按钮层
        private bool _inReadyHandSelectCardState;

        private MJAvatarController _avatarController;
        private MJHintsController _hintsController;
        private MJHandController _handController;
        private MJTableController _tableController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _avatarController = stage.GetController<MJAvatarController>();
            _hintsController = stage.GetController<MJHintsController>();
            _handController = stage.GetController<MJHandController>();
            _tableController = stage.GetController<MJTableController>();

            _panel = OnCreateActionButtonPanel();

            _panel.onSelectAction = OnSelectAction;

            if (stage.isReplay)
            {
                _replayPanel = OnCreateReplayActionButtonPanel();

                for (int viewChairId = 0; viewChairId < 4; viewChairId++)
                {
                    if (viewChairId == 1 || viewChairId == 3)
                    {
                        var offset = new Vector2(120 * (viewChairId - 2), 0);
                        var avatarPos = _avatarController.GetAvatarPosition(viewChairId);
                        var container = _replayPanel.GetContainer(viewChairId);
                        container.SetPositionInZero(avatarPos + offset);
                    }
                }
            }
        }

        /// <summary>
        /// 是否在听牌选择出牌状态
        /// </summary>
        /// <returns></returns>
        public bool IsInReadyHandSelectCardState()
        {
            return _inReadyHandSelectCardState;
        }

        /// <summary>
        /// 创建动作按钮层，添加到场景并设置zorder
        /// </summary>
        /// <returns></returns>
        protected virtual MJActionButtonPanel OnCreateActionButtonPanel()
        {
            var panel = new MJActionButtonPanel();

            panel.AddTo(WDirector.GetRootLayer(), MJZorder.ActionButtonPanel);

            return panel;
        }

        /// <summary>
        /// 创建回放动作按钮层
        /// </summary>
        /// <returns></returns>
        protected virtual MJReplayActionButtonPanel OnCreateReplayActionButtonPanel()
        {
            var panel = new MJReplayActionButtonPanel();

            panel.AddTo(WDirector.GetRootLayer(), MJZorder.ActionButtonPanel);

            return panel;
        }

        /// <summary>
        /// 尝试显示动作事件按钮
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="actionCount"></param>
        public virtual void EnqueueShowButtons(MsgHeader msg, int actionCount)
        {
            var actionDatas = ParseActionDatas(msg, actionCount);

            if (actionDatas.Count == 0)
            {
                return;
            }

            // 保存当前动作数据
            _gameData.currentActionDatas = new List<MJActionData>(actionDatas);

            stage.animationQueue.Enqueue(0, () => ShowButtons(actionDatas));
        }


        /// <summary>
        /// 同上，但是不使用动画队列
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="actionCount"></param>
        public void DirectShowButtons(MsgHeader msg, int actionCount)
        {
            var actionDatas = ParseActionDatas(msg, actionCount);

            if (actionDatas.Count == 0)
            {
                return;
            }

            // 保存当前动作数据
            _gameData.currentActionDatas = new List<MJActionData>(actionDatas);

            ShowButtons(actionDatas);
        }


        /// <summary>
        /// 解析动作数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="actionCount"></param>
        /// <returns></returns>
        public List<MJActionData> ParseActionDatas(MsgHeader msg, int actionCount)
        {
            var result = new List<MJActionData>();

            if (actionCount == 0)
            {
                return result;
            }

            for (int i = 0; i < actionCount; i++)
            {
                result.Add(MJActionData.From(msg));
            }

            return result;
        }


        /// <summary>
        /// 显示动作按钮
        /// </summary>
        /// <param name="actionDatas"></param>
        /// <param name="insertPass"></param>
        /// <param name="saveData"></param>
        public virtual void ShowButtons(List<MJActionData> actionDatas, bool insertPass = true)
        {
            if (actionDatas == null || actionDatas.Count == 0)
            {
                return;
            }

            // 添加过按钮数据
            if (insertPass)
            {
                actionDatas.Add(new MJActionData
                {
                    showType = ActionShowType.Guo
                });
            }

            // 排序，按showType从小到大
            if (actionDatas.Count > 1)
            {
                actionDatas.Sort((a, b) => a.showType - b.showType);
            }

            // 根据showType分类
            Dictionary<int, List<MJActionData>> classified = Classify(actionDatas);

            Dictionary<KeyValuePair<string, string>, List<MJActionData>> configDataDic = GetConfigDataDic(classified);

            _panel.Show(configDataDic);

            _hintsController.HideHints(true);
        }

        /// <summary>
        /// 显示回放动作按钮
        /// </summary>
        public virtual void ShowReplayButtons(List<MJActionEventRecord.Data> datas)
        {
            ClearButtons();

            datas.ForEach(data =>
            {
                int viewChairId = stage.ToViewChairId(data.chairId);
                var showTypes = data.showTypes;
                var selectIdx = showTypes.IndexOf(data.selectShowType);
                var configs = new List<KeyValuePair<string, string>>();
                showTypes.ForEach(showType => configs.Add(OnGetActionButtonConfig(showType)));
                _replayPanel.Show(viewChairId, configs, selectIdx);
            });
        }

        /// <summary>
        /// 清除动作按钮
        /// </summary>
        public virtual void ClearButtons()
        {
            _panel.Clear();

            _replayPanel?.Clear();
        }


        /// <summary>
        /// 清除按钮及数据
        /// </summary>
        public virtual void Clear()
        {
            ClearButtons();

            _gameData.currentActionDatas = null;
        }



        /// <summary>
        /// 进入听牌选择出牌状态
        /// </summary>
        /// <param name="selectedData"></param>
        public void EnterReadyHandSelectCardState(MJActionData selectedData)
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            // 设置不可选择牌
            UpdateUnselectableCardValues(selectedData.cardValues);

            // 不可选择牌变暗
            _handController.DarkenByValues(Chair.Down, playerSelf.unselectableCardValues);

            // 显示取消按钮
            List<MJActionData> actionDatas = new List<MJActionData>
            {
                new MJActionData { showType = ActionShowType.Calcel }
            };

            ShowButtons(actionDatas, false);

            _inReadyHandSelectCardState = true;
        }


        /// <summary>
        /// 退出听牌选择出牌状态
        /// </summary>
        public void ExitReadyHandSelectCardState()
        {
            if (!_inReadyHandSelectCardState)
            {
                return;
            }

            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            playerSelf.unselectableCardValues = null;

            _handController.ResetTint(Chair.Down);

            _inReadyHandSelectCardState = false;
        }


        /// <summary>
        /// 处理点击按钮事件
        /// </summary>
        /// <param name="selectedData"></param>
        protected virtual void OnSelectAction(MJActionData selectedData)
        {
            _panel.Clear();

            if (selectedData.showType == ActionShowType.Guo)
            {
                _tableController.StopTimer();

                stage.Send("SendAction", ActionType.FangQi, 0);

                _inReadyHandSelectCardState = false;

                return;
            }

            if (selectedData.showType == ActionShowType.Calcel)
            {
                // 恢复动作事件按钮
                ShowButtons(_gameData.currentActionDatas);

                // 退出听牌选择出牌状态
                ExitReadyHandSelectCardState();

                return;
            }

            if (selectedData.showType == ActionShowType.Ting)
            {
                // 前听
                if (selectedData.cardValues.Length == 0)
                {
                    stage.Send("SendAction", selectedData.actionType, selectedData.operateCard);

                    return;
                }

                // 后听，且可出牌只有一张，直接发送
                if (selectedData.cardValues.Length == 1)
                {
                    stage.Send("SendAction", selectedData.actionType, selectedData.cardValues[0]);

                    return;
                }

                // 进入听牌选择出牌状态
                EnterReadyHandSelectCardState(selectedData);

                return;
            }

            _tableController.StopTimer();

            stage.Send("SendAction", selectedData.actionType, selectedData.operateCard);
        }


        /// <summary>
        /// 把showType相同的动作归为一类
        /// </summary>
        /// <param name="actionDatas"></param>
        /// <returns></returns>
        protected Dictionary<int, List<MJActionData>> Classify(List<MJActionData> actionDatas)
        {
            Dictionary<int, List<MJActionData>> result = new Dictionary<int, List<MJActionData>>();

            foreach (MJActionData actionData in actionDatas)
            {
                List<MJActionData> classified;
                if (result.ContainsKey(actionData.showType))
                {
                    classified = result[actionData.showType];
                }
                else
                {
                    classified = new List<MJActionData>();
                }

                classified.Add(actionData);

                result[actionData.showType] = classified;
            }

            return result;
        }


        /// <summary>
        /// 获取按钮配置与数据配对字典
        /// </summary>
        /// <param name="classified"></param>
        /// <returns></returns>
        protected Dictionary<KeyValuePair<string, string>, List<MJActionData>> GetConfigDataDic(Dictionary<int, List<MJActionData>> classified)
        {
            Dictionary<KeyValuePair<string, string>, List<MJActionData>> result = new Dictionary<KeyValuePair<string, string>, List<MJActionData>>();

            foreach (var pair in classified)
            {
                KeyValuePair<string, string> config = OnGetActionButtonConfig(pair.Key);

                result[config] = pair.Value;
            }

            return result;
        }


        /// <summary>
        /// 获取按钮配置
        /// </summary>
        /// <param name="showType"></param>
        protected virtual KeyValuePair<string, string> OnGetActionButtonConfig(int showType)
        {
            if (showType == 255 && stage.isReplay)
            {
                showType = ActionShowType.Guo;
            }
            return MJActionButton.GetDefaultConfig(showType);           
        }


        private void UpdateUnselectableCardValues(int[] excludeValues)
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            List<int> handValues = playerSelf.handCardValues;

            List<int> unselectable = new List<int>();

            for (int i = 0; i < handValues.Count; i++)
            {
                int value = handValues[i];
                if (!excludeValues.Contains(value))
                {
                    unselectable.Add(value);
                }
            }

            playerSelf.unselectableCardValues = unselectable;
        }


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

        public override void OnGameOver()
        {
            base.OnGameOver();

            Clear();
        }
        #endregion
    }
}
