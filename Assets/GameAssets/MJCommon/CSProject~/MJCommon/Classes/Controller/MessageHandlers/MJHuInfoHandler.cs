// @Author: tanjinhua
// @Date: 2021/5/6  11:26


using System.Collections.Generic;
using WLHall.Game;

namespace MJCommon
{
    public class MJHuInfoHandler : BaseGameController
    {
        private MJGameData _gameData;

        private MJHandController _handController;
        private MJHintsController _hintsController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _handController = stage.GetController<MJHandController>();
            _hintsController = stage.GetController<MJHintsController>();
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="data"></param>
        public virtual void UpdateData(MsgHuInfo data)
        {
            _gameData.huInfo = data;
        }


        /// <summary>
        /// 手牌显示胡牌提示标志
        /// </summary>
        public virtual void EnqueueShowHintMark()
        {
            _handController.ClearHintMark(Chair.Down);

            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            if (playerSelf.handCardCount % 3 != 2)
            {
                return;
            }

            List<int> values = new List<int>(_gameData.huInfo.hintsDataMap.Keys);

            if (values.Count == 0)
            {
                return;
            }

            stage.animationQueue.Enqueue(0, () =>
            {
                var cards = _handController.FindAllCardsByValues(Chair.Down, values);

                cards.ForEach(c => c.ShowHintMark(OnGetHintMarkType(c.cardValue)));
            });
        }


        /// <summary>
        /// 获取胡牌提示类型
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        protected virtual MJCard.HintMarkType OnGetHintMarkType(int cardValue)
        {
            return MJCard.HintMarkType.Normal;
        }


        /// <summary>
        /// 显示胡牌提示弹窗
        /// </summary>
        /// <param name="cardValue"></param>
        public virtual void ShowHintsPanel(int cardValue)
        {
            if (_gameData.huInfo == null)
            {
                return;
            }

            if (!_gameData.huInfo.hintsDataMap.ContainsKey(cardValue))
            {
                return;
            }

            MJHintsData hitsData = _gameData.huInfo.hintsDataMap[cardValue];

            _hintsController.ShowHints(hitsData);
        }


        /// <summary>
        /// 清除胡牌提示数据以及UI
        /// </summary>
        public virtual void Clear()
        {
            _gameData.huInfo = null;

            _handController.ClearHintMark(Chair.Down);

            _hintsController.HideHints(false);
        }
    }
}
