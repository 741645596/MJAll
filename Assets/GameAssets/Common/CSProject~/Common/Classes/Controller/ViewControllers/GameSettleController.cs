// @Author: tanjinhua
// @Date: 2021/6/8  17:14

using WLHall.Game;

namespace Common
{
    public abstract class GameSettleController : BaseGameController
    {
        private GameData _gameData;
        private SettleViewBase _settleView;
        private SettleViewBase _statisticsButton;
        private FriendGameOverHandler _friendGameOverHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as GameData;
            _friendGameOverHandler = stage.GetController<FriendGameOverHandler>();
        }

        /// <summary>
        /// 获取结算界面
        /// </summary>
        /// <returns></returns>
        public SettleViewBase GetSettleView()
        {
            if (_settleView != null && _settleView.gameObject != null)
            {
                return _settleView;
            }
            return null;
        }

        /// <summary>
        /// 同上，支持泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSettleView<T>() where T : SettleViewBase
        {
            return GetSettleView() as T;
        }

        /// <summary>
        /// 获取单独的战绩统计按钮
        /// </summary>
        /// <returns></returns>
        public SettleViewBase GetStatisticsButton()
        {
            if (_statisticsButton != null && _statisticsButton.gameObject != null)
            {
                return _statisticsButton;
            }
            return null;
        }

        /// <summary>
        /// 是否存在结算界面
        /// </summary>
        /// <returns></returns>
        public bool SettleViewExist()
        {
            return GetSettleView() != null;
        }

        /// <summary>
        /// 显示结算界面
        /// </summary>
        /// <param name="settleView"></param>
        public virtual void DisplaySettleView(SettleViewBase settleView)
        {
            OnAppendSettleView(settleView);

            settleView.onEvent = OnSettleViewEvent;

            _settleView = settleView;

            _gameData.autoUpdatePlayerInfo = true;

            _gameData.TraversePlayer(player => stage.OnUpdatePlayerInfo(player));

            // TODO: 显示免输再得、洗牌、背包、。。。
        }

        /// <summary>
        /// 同上，使用动画队列
        /// </summary>
        /// <param name="settleView"></param>
        public virtual void EnqueueSettleView(SettleViewBase settleView)
        {
            RemoveSettleView();

            stage.animationQueue.Enqueue(0, () => DisplaySettleView(settleView));
        }

        /// <summary>
        /// 移除结算界面
        /// </summary>
        public virtual void RemoveSettleView()
        {
            GetSettleView()?.RemoveFromParent();

            _settleView = null;

            GetStatisticsButton()?.RemoveFromParent();

            _statisticsButton = null;
        }

        /// <summary>
        /// 结算界面事件处理
        /// </summary>
        /// <param name="eventType"></param>
        protected virtual void OnSettleViewEvent(SettleEvent eventType)
        {
            switch (eventType)
            {
                case SettleEvent.Continue:
                    stage.Continue();
                    break;

                case SettleEvent.ChangeDesk:
                    stage.ChangeDesk();
                    break;

                case SettleEvent.Leave:
                    stage.ExitWithDialog();
                    break;

                case SettleEvent.Statistics:
                    _friendGameOverHandler.ShowRecordStatistics();
                    RemoveSettleView();
                    break;

                case SettleEvent.Timeup:
                    var isFriendGameOver = _friendGameOverHandler.IsFriendGameOver();
                    if (isFriendGameOver)
                    {
                        _friendGameOverHandler.ShowRecordStatistics();
                        RemoveSettleView();
                    }
                    break;
                    // TODO: 其他结算事件处理

            }
        }

        /// <summary>
        /// 创建独立战绩统计按钮(准备后解散游戏时显示)
        /// </summary>
        /// <returns></returns>
        protected abstract SettleViewBase OnCreateStatisticsButton();

        /// <summary>
        /// 添加结算到场景，子类实现添加在场景并设置zorder
        /// </summary>
        protected abstract void OnAppendSettleView(SettleViewBase settleView);


        #region Game Events
        public override void OnContinue()
        {
            base.OnContinue();

            RemoveSettleView();
        }

        public override void OnChangeDesk()
        {
            base.OnChangeDesk();

            RemoveSettleView();
        }

        public override void OnFriendGameOver()
        {
            base.OnFriendGameOver();

            EnqueueShowFriendGameOver();
        }

        private void EnqueueShowFriendGameOver()
        {
            stage.animationQueue.Enqueue(0, () =>
            {
                if (SettleViewExist())
                {
                    _settleView.ShowFriendGameOverState();
                }
                else
                {
                    _statisticsButton = OnCreateStatisticsButton();

                    _statisticsButton.onEvent = OnSettleViewEvent;

                    _statisticsButton.Countdown(15);
                }
            });
        }
        #endregion
    }
}
