// @Author: tanjinhua
// @Date: 2021/4/8  14:22


using System;
using System.Collections.Generic;
using Common;
using Unity.Widget;
using WLHall.Game;

namespace MJCommon
{
    public class MJTableController : BaseGameController
    {

        protected MJTable _table;
        private MJGameData _gameData;
        private SequenceAnimation _matchingAnim;
        private MJSpaceController _spaceController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _spaceController = stage.GetController<MJSpaceController>();

            _table = _spaceController.GetSpace().table;

            InitializeTable();
        }

        protected virtual void InitializeTable()
        {
            if (!_gameData.isFriendRoom && !stage.isReplay)
            {
                PlayMatchingAnimation();
            }
        }


        /// <summary>
        /// 设置剩余牌数
        /// </summary>
        /// <param name="count"></param>
        public void SetRemainCardCount(int count)
        {
            string text = count < 10 ? $"0{count}" : count.ToString();

            bool useRed = count <= _gameData.remainCardCountWarnLimit;

            //if (m_client.isFriendRoom)
            //{
            //    m_table.SetFriendRemainCardCount(text, useRed);
            //}
            //else
            //{
            //    m_table.SetRemainCardCount(text, useRed);
            //}
        }


        /// <summary>
        /// 设置朋友常局数信息
        /// </summary>
        /// <param name="text"></param>
        public void SetFriendGameCount(string text)
        {
            //m_table.SetFriendGameCount(text);
        }


        /// <summary>
        /// 显示朋友场游戏名称
        /// </summary>
        /// <param name="gameName"></param>
        public void SetFriendGameName(string gameName)
        {
            //m_table.SetFriendGameName(gameName);
        }


        /// <summary>
        /// 播放防作弊排队动画
        /// </summary>
        public void PlayMatchingAnimation()
        {
            if (_matchingAnim != null && _matchingAnim.gameObject != null)
            {
                return;
            }

            _matchingAnim = new SequenceAnimation("MJCommon/MJ/mj_ui_atlas", new List<string>
            {
                "matching/zheng.png",
                "matching/zai.png",
                "matching/pi.png",
                "matching/pei.png",
                "matching/pai.png",
                "matching/you.png",
                "matching/dot.png",
                "matching/dot.png",
                "matching/dot.png",
            });

            _matchingAnim.AddTo(WDirector.GetRootLayer());
            _matchingAnim.SetAnchor(cc.p(0.5f, 0.5f));
            _matchingAnim.Layout(layout.center, cc.p(0, -160));
            _matchingAnim.PlayJump();
            _matchingAnim.gameObject.name = "matching_anim";
        }


        /// <summary>
        /// 移除防作弊排队动画
        /// </summary>
        public void RemoveMatchingAnimation()
        {
            if (_matchingAnim != null && _matchingAnim.gameObject != null)
            {
                _matchingAnim.RemoveFromParent();
            }

            _matchingAnim = null;
        }


        /// <summary>
        /// 设置开始(即自家方位)
        /// </summary>
        /// <param name="wind"></param>
        public void SetStartingWind(Wind wind)
        {
            _table.startWind = wind;
        }


        /// <summary>
        /// 播放骰子动画
        /// </summary>
        /// <param name="data"></param>
        public void PlayDice()
        {
            DiceUsage diceUsage = _gameData.diceUsage;

            if (diceUsage == DiceUsage.Hidden)
            {
                return;
            }

            int dice1 = _gameData.diceValues[0];

            int dice2 = _gameData.diceValues[1];

            if (diceUsage == DiceUsage.One)
            {
                _table.PlayDice(dice1);
            }
            else
            {
                _table.PlayDice(dice1, dice2);
            }
        }


        /// <summary>
        /// 播放指向动画，同时开启计时器
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="time"></param>
        /// <param name="onTick"></param>
        public void PlayTurn(int viewChairId, int time, Action<int> onTick = null)
        {
            _table.PlayPointing(viewChairId);

            _table.StartTimer(time, onTick);
        }


        /// <summary>
        /// 停止倒计时，停止方位指示动画
        /// </summary>
        public void StopTurn()
        {
            _table.StopTimer();

            _table.StopPointing();
        }


        /// <summary>
        /// 播放方位指示动画
        /// </summary>
        /// <param name="viewChairId"></param>
        public void PlayPointing(int viewChairId)
        {
            _table.PlayPointing(viewChairId);
        }


        /// <summary>
        /// 停止方位指示动画
        /// </summary>
        public void StopPointing()
        {
            _table.StopPointing();
        }


        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time"></param>
        /// <param name="onTick"></param>
        public void StartTimer(int time, Action<int> onTick = null)
        {
            _table.StartTimer(time, onTick);
        }


        /// <summary>
        /// 停止倒计时
        /// </summary>
        public virtual void StopTimer()
        {
            _table.StopTimer();
        }


        /// <summary>
        /// 暂停倒计时
        /// </summary>
        public virtual void PauseTimer()
        {
            _table.PauseTimer();
        }


        /// <summary>
        /// 恢复倒计时
        /// </summary>
        public virtual void ResumeTimer()
        {
            _table.ResumeTimer();
        }


        /// <summary>
        /// 重置。停止倒计时、停止方位指示动画、隐藏骰子，重置方位方向
        /// </summary>
        public void Reset()
        {
            StopTurn();

            //_table.oneDiceNode.gameObject.SetActive(false);

            //_table.twoDiceNode.gameObject.SetActive(false);

            //_table.HideRemainCardCount();

            SetStartingWind(Wind.South);
        }


        #region Game Events
        public override void OnContinue()
        {
            base.OnContinue();

            Reset();
        }

        public override void OnChangeDesk()
        {
            base.OnChangeDesk();

            Reset();

            if (!_gameData.isFriendRoom)
            {
                PlayMatchingAnimation();
            }
        }

        public override void OnGameStart()
        {
            base.OnGameStart();

            RemoveMatchingAnimation();
        }

        public override void OnGameOver()
        {
            base.OnGameOver();

            StopTurn();
        }
        #endregion
    }
}
