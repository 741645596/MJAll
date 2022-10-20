// @Author: tanjinhua
// @Date: 2021/5/10  13:45


using System;
using System.Collections.Generic;
using WLHall.Game;
using Config = Common.FriendRecordStatistics.Config;
using PlayerInfo = Common.FriendRecordStatistics.PlayerInfo;

namespace Common
{
    public abstract class FriendGameOverHandler : BaseGameController
    {

        private bool _isFriendGameOver = false;
        private long[] _scores;
        private List<Dictionary<string, string>> _detailScores;
        private FriendRecordStatistics _recordStatistics;
        private GameData _gameData;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as GameData;
        }

        /// <summary>
        /// 朋友场是否已结束
        /// </summary>
        /// <returns></returns>
        public bool IsFriendGameOver()
        {
            return _isFriendGameOver;
        }

        /// <summary>
        /// 保存战绩数据
        /// </summary>
        /// <param name="scores">玩家分数</param>
        /// <param name="detailScores">分数细节</param>
        public void SetRecordData(long[] scores, List<Dictionary<string, string>> detailScores)
        {
            _scores = scores;

            _detailScores = detailScores;
        }


        /// <summary>
        /// 显示战绩统计界面
        /// </summary>
        public virtual void ShowRecordStatistics()
        {
            if (_recordStatistics != null && _recordStatistics.gameObject != null)
            {
                return;
            }

            Config config = GetRecordStatisticsConfig();

            FriendRecordStatistics statistics = OnCreateRecordStatistics();

            statistics.SetConfig(config);

            AddRecordStatisticsButtons(statistics);

            _recordStatistics = statistics;
        }


        /// <summary>
        /// 添加战绩统计界面按钮
        /// </summary>
        /// <param name="statistics"></param>
        protected virtual void AddRecordStatisticsButtons(FriendRecordStatistics statistics)
        {
            statistics.AddButton(FriendRecordButton.Type.Exit);

            statistics.onClickButton = OnClickStatisticsButton;
        }


        /// <summary>
        /// 战绩统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventType"></param>
        protected virtual void OnClickStatisticsButton(FriendRecordButton sender, FriendRecordButton.Type eventType)
        {
            switch (eventType)
            {
                case FriendRecordButton.Type.Exit:
                    stage.Exit();
                    break;
            }
        }


        /// <summary>
        /// 创建战绩统计界面, 子类实现添加到场景并设置zorder
        /// </summary>
        /// <returns></returns>
        protected abstract FriendRecordStatistics OnCreateRecordStatistics();


        /// <summary>
        /// 获取战绩统计界面参数配置
        /// </summary>
        /// <returns></returns>
        protected virtual Config GetRecordStatisticsConfig()
        {
            return new Config
            {
                gameInfoText = GetRecordGameInfoText(),
                rules = _gameData.friendGameInfo.rules.ToArray(),
                playerInfos = GetRecordPlayerInfos()
            };
        }


        /// <summary>
        /// 获取战绩统计游戏信息字符串
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRecordGameInfoText()
        {
            string clubInfoText = "";
            if (_gameData.isQinYouQuan)
            {
                bool isJingJiChang = false; // TODO: RecvInviteRuleInfo
                clubInfoText = isJingJiChang ? "竞技场ID" : "亲友圈ID:" + _gameData.clubId;

                string clubName = ""; // TODO:RecvInviteRuleInfo
                if (!string.IsNullOrEmpty(clubName))
                {
                    clubInfoText += $"({clubName})";
                }
            }
            if (!string.IsNullOrEmpty(clubInfoText))
            {
                clubInfoText += "  ";
            }
            string gameName = _gameData.friendGameInfo.gameName + "  ";
            string roomKey = $"房号:{_gameData.roomKey}  ";
            string gameCountText = $"共{_gameData.friendGameInfo.currentGameCount}局  ";

            string dateText = DateTime.Now.ToString("g");

            return $"{clubInfoText}{gameName}{roomKey}{gameCountText}{dateText}";
        }


        /// <summary>
        /// 获取战绩统计玩家信息列表
        /// </summary>
        /// <returns></returns>
        protected virtual List<PlayerInfo> GetRecordPlayerInfos()
        {
            List<PlayerInfo> playerInfos = new List<PlayerInfo>();

            bool allZero = _scores.CountByCondition(s => s == 0) == _scores.Length;

            int rank = 1;

            bool isJixiang = false; // TODO: 吉祥平台判断

            _gameData.TraversePlayer(p =>
            {
                GamePlayer player = p as GamePlayer;

                long score = _scores[player.chairId];

                if (!allZero && playerInfos.Count > 0 && score < playerInfos[playerInfos.Count - 1].score)
                {
                    rank++;
                }

                PlayerInfo info = new PlayerInfo
                {
                    nickname = player.GetNickNameUtf16(),
                    avatarUrl = player.GetAvatarPath(),
                    userId = player.userId,
                    gender = player.gender,
                    isRoomOwner = _gameData.roomOwnerId == player.userId,
                    isSelfPlayer = _gameData.playerSelf == player,
                    applyDissolution = isJixiang && player.chairId == _gameData.applyDissolutionChairId,
                    overtimeDissolution = player.chairId == _gameData.overtimeDissolutionChairId,
                    score = _scores[player.chairId],
                    rank = allZero ? 0 : rank,
                    details = _detailScores[player.chairId]
                };

                playerInfos.Add(info);
            });

            playerInfos.Sort((a, b) =>
            {
                if (a.score == b.score)
                {
                    int viewChairIdA = stage.ToViewChairId(_gameData.GetPlayerByUserId(a.userId).chairId);
                    int viewChairIdB = stage.ToViewChairId(_gameData.GetPlayerByUserId(b.userId).chairId);

                    return viewChairIdA - viewChairIdB;
                }

                return (int)(b.score - a.score);
            });

            return playerInfos;
        }


        public override void OnFriendGameOver()
        {
            base.OnFriendGameOver();

            _isFriendGameOver = true;
        }
    }
}
