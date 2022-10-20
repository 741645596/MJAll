// @Author: tanjinhua
// @Date: 2021/5/12  10:13


using Common;
using Unity.Widget;

namespace MJCommon
{
    public class MJSettleController : GameSettleController
    {
        private MJAudioController _audioController;

        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            _audioController = stage.GetController<MJAudioController>();
        }


        public override void DisplaySettleView(SettleViewBase settleView)
        {
            base.DisplaySettleView(settleView);

            if (stage.isReplay)
            {
                GetSettleView<MJSettleView>().HideButtons();
            }
        }

        /// <summary>
        /// 结算界面事件
        /// </summary>
        /// <param name="eventType"></param>
        protected override void OnSettleViewEvent(SettleEvent eventType)
        {
            base.OnSettleViewEvent(eventType);

            if (eventType == SettleEvent.PlayScoreAnim)
            {
                _audioController.PlayScoring();
            }
            else if (eventType != SettleEvent.Timeup)
            {
                _audioController.PlayButtonClick();
            }
        }

        /// <summary>
        /// 创建单独战绩统计按钮
        /// </summary>
        /// <returns></returns>
        protected override SettleViewBase OnCreateStatisticsButton()
        {
            //return new MJFriendStatisticsButton();
            return null;
        }

        protected override void OnAppendSettleView(SettleViewBase settleView)
        {
            settleView.AddTo(WDirector.GetRootLayer(), MJZorder.SettleView);
        }
    }
}