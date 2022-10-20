// @Author: tanjinhua
// @Date: 2021/4/8  14:24


using WLHall.Game;

namespace MJCommon
{
    public class MJWallController : BaseGameController
    {
        private MJGameData _gameData;
        private MJWallRoot _root;
        private MJSpaceController _spaceController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _spaceController = stage.GetController<MJSpaceController>();

            _root = _spaceController.GetSpace().wallRoot;
        }

        /// <summary>
        /// 准备牌墙
        /// </summary>
        /// <param name="visible"></param>
        public void InitializeWalls(bool visible = true)
        {
            var config = _spaceController.GetSpaceConfig().wallConfig;
            config.stackCountPerSide = _gameData.wallStacks;
            var bankerChairId = _gameData.bankerChairId;
            var dice1 = _gameData.diceValues[0];
            var dice2 = _gameData.diceValues[1];
            _root.InitializeWalls(config, bankerChairId, dice1, dice2, visible);
        }

        /// <summary>
        /// 清除所有牌墙
        /// </summary>
        public void Clear()
        {
            var walls = _root.GetWallArray();

            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].Clear();
            }
        }


        /// <summary>
        /// 移除一张牌
        /// </summary>
        /// <param name="reverse"></param>
        public void Take(bool reverse = false)
        {
            _root.Take(reverse);
        }


        /// <summary>
        /// 移除多个牌
        /// </summary>
        /// <param name="count"></param>
        public void TakeMultiple(int count, bool reverse = false)
        {
            _root.Take(count, reverse);
        }


        /// <summary>
        /// 根据指定牌墩索引移除牌
        /// </summary>
        /// <param name="stackIndex"></param>
        public void TakeAtStack(int stackIndex, int takeCount, bool reverse = false)
        {
            _root.Take(stackIndex, takeCount, reverse);
        }


        /// <summary>
        /// 根据指定牌墩和牌索引替换一张牌
        /// </summary>
        /// <param name="stackIndex"></param>
        /// <param name="cardIndex"></param>
        /// <param name="cardValue"></param>
        /// <param name="reverse"></param>
        public void Replace(int stackIndex, int cardIndex, int cardValue, bool reverse = false)
        {
            _root.Replace(stackIndex, cardIndex, cardValue, reverse);
        }


        /// <summary>
        /// 恢复一张牌
        /// </summary>
        /// <param name="reverse"></param>
        public void Recover(bool reverse = false)
        {
            _root.Recover(reverse);
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
        #endregion
    }
}
