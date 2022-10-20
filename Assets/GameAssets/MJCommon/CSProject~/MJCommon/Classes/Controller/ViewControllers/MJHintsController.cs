// @Author: tanjinhua
// @Date: 2021/4/20  11:17


using System.Collections.Generic;
using Unity.Widget;
using WLHall.Game;
using Common;

namespace MJCommon
{

    /// <summary>
    /// 胡牌提示、听牌提示UI控制器
    /// </summary>
    public class MJHintsController : BaseGameController
    {
        private MJGameData _gameData;
        private bool _isReadyHandHint;
        private bool _allowAuto;
        private MJHintsPanel _panel;

        private MJFunctionPanelController _functionPanelController;
        private MJTrustController _trustController;


        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _functionPanelController = stage.GetController<MJFunctionPanelController>();
            _trustController = stage.GetController<MJTrustController>();
        }

        /// <summary>
        /// 是否允许自动出牌
        /// </summary>
        /// <returns></returns>
        public bool IsAllowAuto()
        {
            return _allowAuto;
        }

        /// <summary>
        /// 获取胡牌提示界面
        /// </summary>
        /// <returns></returns>
        public MJHintsPanel GetPanel()
        {
            return _panel;
        }

        /// <summary>
        /// 显示提示弹窗
        /// </summary>
        /// <param name="data"></param>
        public virtual void ShowHints(MJHintsData data)
        {
            if (_panel == null || _panel.gameObject == null)
            {
                _panel = OnCreateHintsPanel();

                _panel.onClickAuto = OnClickAutoToggle;
            }

            if (data.cardValues == null || data.cardValues.Count == 0)
            {
                _panel.ShowAnyHints();

                return;
            }

            MJHintCell.Config[] configs = OnGetConfigs(data, out int totalCount);

            _panel.ShowHints(totalCount, configs);

            _panel.SetAutoToggleIsOn(false);

            _panel.SetAutoToggleVisible(false);
        }

        /// <summary>
        /// 创建胡牌提示弹框，添加到场景
        /// </summary>
        /// <returns></returns>
        protected virtual MJHintsPanel OnCreateHintsPanel()
        {
            var panel = new MJHintsPanel();

            panel.AddTo(WDirector.GetRootLayer());

            return panel;
        }

        /// <summary>
        /// 显示听牌提示按钮
        /// </summary>
        public void ShowHintButton()
        {
            var panel = _functionPanelController.GetPanel();
            panel.SetHintButtonVisible(true);
            panel.onClickHintBtn = OnClickHintsButton;
        }


        /// <summary>
        /// 隐藏胡牌提示弹窗
        /// </summary>
        /// <param name="force">是否强制隐藏(如果非强制，点击听牌提示按钮显示的弹窗不会被隐藏)</param>
        public virtual void HideHints(bool force = false)
        {
            if (_panel == null || _panel.gameObject == null)
            {
                _panel = null;

                return;
            }

            if (!force && _isReadyHandHint)
            {
                return;
            }

            _panel.RemoveFromParent();

            _panel = null;

            _isReadyHandHint = false;
        }


        /// <summary>
        /// 强制隐藏胡牌提示弹窗、听牌提示按钮及自动出牌开关
        /// </summary>
        public virtual void HideAll()
        {
            HideHints(true);

            _functionPanelController.GetPanel().SetHintButtonVisible(false);
        }


        /// <summary>
        /// 取消自动出牌
        /// </summary>
        public virtual void CancelAuto()
        {
            _allowAuto = false;

            _panel?.SetAutoToggleIsOn(false);
        }


        #region Business Logic
        protected virtual MJHintCell.Config[] OnGetConfigs(MJHintsData data, out int totalCount)
        {

            MJHintCell.Config[] configs;

            switch (data.type)
            {
                case MJHintsData.Type.Normal:
                    configs = OnGetNormalConfigs(data, out totalCount);
                    break;

                case MJHintsData.Type.Multiple:
                    configs = OnGetMultiplyConfigs(data, out totalCount);
                    break;

                case MJHintsData.Type.Descriptive:
                    configs = OnGetDescriptiveConfigs(data, out totalCount);
                    break;

                default:
                    configs = new MJHintCell.Config[0];
                    totalCount = 0;
                    break;
            }

            return configs;
        }


        protected virtual MJHintCell.Config[] OnGetNormalConfigs(MJHintsData data, out int totalCount)
        {
            MJHintCell.Config[] configs = new MJHintCell.Config[data.cardValues.Count];

            totalCount = 0;

            for (int i = 0; i < configs.Length; i++)
            {
                int cardValue = data.cardValues[i];

                int count = OnFindCardCount(cardValue);

                totalCount += count;

                configs[i] = new MJHintCell.Config
                {
                    cardValue = cardValue,
                    firstInfo = $"{count}张"
                };
            }

            return configs;
        }


        protected virtual MJHintCell.Config[] OnGetMultiplyConfigs(MJHintsData data, out int totalCount)
        {
            MJHintCell.Config[] configs = new MJHintCell.Config[data.cardValues.Count];

            totalCount = 0;

            for (int i = 0; i < configs.Length; i++)
            {
                int cardValue = data.cardValues[i];

                int count = OnFindCardCount(cardValue);

                totalCount += count;

                int multiply = data.multiplies[i];

                configs[i] = new MJHintCell.Config
                {
                    cardValue = cardValue,
                    firstInfo = $"{multiply}番",
                    secondInfo = $"{count}张"
                };
            }

            return configs;
        }


        protected virtual MJHintCell.Config[] OnGetDescriptiveConfigs(MJHintsData data, out int totalCount)
        {
            MJHintCell.Config[] configs = new MJHintCell.Config[data.cardValues.Count];

            totalCount = 0;

            for (int i = 0; i < configs.Length; i++)
            {
                int cardValue = data.cardValues[i];

                int count = OnFindCardCount(cardValue);

                totalCount += count;

                string description = data.descriptions[i];

                configs[i] = new MJHintCell.Config
                {
                    cardValue = cardValue,
                    firstInfo = description,
                    secondInfo = $"{count}张"
                };
            }

            return configs;
        }


        /// <summary>
        /// 查找桌面可见剩余牌数量
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        protected virtual int OnFindCardCount(int cardValue)
        {
            return 4 - FindSelfHandCardCount(cardValue) - FindOutCardCount(cardValue) - FindFuziCardCount(cardValue);
        }


        /// <summary>
        /// 点击自动打牌开关逻辑
        /// </summary>
        /// <param name="isOn"></param>
        protected virtual void OnClickAutoToggle(bool isOn)
        {
            _allowAuto = isOn;

            if (isOn)
            {
                _trustController.ExecuteTrustOperation();
            }
        }


        /// <summary>
        /// 点击听牌提示按钮逻辑
        /// </summary>
        protected virtual void OnClickHintsButton()
        {
            if (_gameData.tingInfo == null)
            {
                return;
            }

            ShowHints(_gameData.tingInfo);

            _isReadyHandHint = true;

            _panel.SetAutoToggleVisible(true);

            _panel.SetAutoToggleIsOn(_allowAuto);
        }


        private int FindSelfHandCardCount(int cardValue)
        {
            List<int> values = (_gameData.playerSelf as MJGamePlayer).handCardValues;

            return values.Count(cardValue);
        }


        private int FindOutCardCount(int cardValue)
        {
            int playerCount = _gameData.maxPlayerCount;

            int result = 0;

            for (int i = 0; i < playerCount; i++)
            {
                List<int> values = (_gameData.GetPlayerByChairId(i) as MJGamePlayer).outCardValues;

                result += values.Count(cardValue);
            }

            return result;
        }


        private int FindFuziCardCount(int cardValue)
        {
            int playerCount = _gameData.maxPlayerCount;

            int result = 0;

            for (int i = 0; i < playerCount; i++)
            {
                List<FuziData> fuzis = (_gameData.GetPlayerByChairId(i) as MJGamePlayer).fuziDatas;

                for (int j = 0; j < fuzis.Count; j++)
                {
                    FuziData fuzi = fuzis[j];

                    if ((fuzi.isConcealedKong || fuzi.isExposedKong) && fuzi.cardValues.Contains(cardValue))
                    {
                        result += 4;
                    }
                    else
                    {
                        for (int k = 0; k < fuzi.cardValues.Length; k++)
                        {
                            if (fuzi.cardValues[k] == cardValue)
                            {
                                result += fuzi.cardCounts[k] > 0 ? fuzi.cardCounts[k] : 1;
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion


        public override void OnContinue()
        {
            base.OnContinue();

            HideAll();

            CancelAuto();
        }


        public override void OnChangeDesk()
        {
            base.OnChangeDesk();

            HideAll();

            CancelAuto();
        }
    }
}
