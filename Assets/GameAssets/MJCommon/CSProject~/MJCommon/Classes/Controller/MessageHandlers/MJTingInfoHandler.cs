// @Author: tanjinhua
// @Date: 2021/5/7  11:33


using WLHall.Game;

namespace MJCommon
{
    public class MJTingInfoHandler : BaseGameController
    {
        private MJGameData _gameData;

        private MJHintsController _hintsController;


        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _hintsController = stage.GetController<MJHintsController>();
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="data"></param>
        public virtual void UpdateData(MJHintsData data)
        {
            _gameData.tingInfo = data;
        }


        /// <summary>
        /// 显示听牌提示按钮
        /// </summary>
        public virtual void EnqueueShowHintButton()
        {
            stage.animationQueue.Enqueue(0, () =>
            {
                _hintsController.ShowHintButton();

                _hintsController.GetPanel()?.SetAutoToggleVisible(true);
            });
        }


        /// <summary>
        /// 清除听牌提示数据及UI，并取消自动出牌
        /// </summary>
        public virtual void Clear()
        {
            _gameData.tingInfo = null;

            _hintsController.HideAll();

            _hintsController.CancelAuto();
        }


        #region Game Events
        public override void OnGameOver()
        {
            base.OnGameOver();

            Clear();
        }
        #endregion
    }
}
