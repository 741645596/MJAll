// @ GameInfoKit.cs
// @ Email : 415546748@qq.com
// @ Author： chenfaxin
// @ Date : 2021/3/30 星期二 16:51:19
// @ Description : 


using System.Collections.Generic;
using WLCore;

namespace WLHall
{
    public static class GameInfoKit
    {

        private static Dictionary<uint, GameInfo> _gameInfos = new Dictionary<uint, GameInfo>();

        public static void DeserializeGameInfo(MsgHeader msg)
        {
            _gameInfos.Clear();
            uint count = msg.ReadUint32();
            for (int i = 0; i < count; i++)
            {
                GameInfo gameInfo = GameInfo.From(msg);
                if (gameInfo.disable == 1)
                {
                    continue;
                }
                if (!_gameInfos.ContainsKey(gameInfo.id))
                {
                    _gameInfos.Add(gameInfo.id, gameInfo);
                }
            }
        }

        public static GameInfo GetGameByGameID(uint gameId)
        {
            return _gameInfos[gameId];
        }

        public static GameInfo GetGameByShortName(string shortName)
        {
            uint gameId = GetGameIDByShortName(shortName);

            if (gameId == uint.MaxValue)
            {
                WLDebug.LogWarning($"GameInfoKit.GetGameByShortName: 找不到短名为[{shortName}]的游戏id");

                return default;
            }

            return GetGameByGameID(gameId);
        }

        public static uint GetGameIDByShortName(string shortName)
        {
            foreach (var pair in _gameInfos)
            {
                if (pair.Value.shortName == shortName)
                {
                    return pair.Key;
                }
            }

            return uint.MaxValue;
        }
    }
}
