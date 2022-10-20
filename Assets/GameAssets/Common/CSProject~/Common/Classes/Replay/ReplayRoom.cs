// @Author: tanjinhua
// @Date: 2021/12/14  19:07


using WLCore;
using WLHall;
using WLHall.Game;

namespace Common
{
    public class ReplayRoom : BaseRoomManager
    {
        public ReplayRoom(RecordInfo recordInfo)
        {
            var roomInfos = RoomInfoKit.GetFriendRoomInfosByGameId(uint.Parse(recordInfo.game_id));
            var playerCount = recordInfo.players.Length;
            for (int i = 0; i < roomInfos.Count; i++)
            {
                var roomInfo = roomInfos[i];
                if (roomInfo.playersPerDesk == playerCount)
                {
                    _roomInfo = roomInfo;
                }
            }

            if (_roomInfo == null)
            {
                _roomInfo = new RoomInfo
                {
                    type = Constants.ROOM_TYPE_FRIEND_TEAM,
                    playersPerDesk = (ushort)recordInfo.players.Length,
                };
            }

            _gameInfo = GameInfoKit.GetGameByGameID(uint.Parse(recordInfo.game_id));

            var selfPlayerInfo = GetSelfPlayer(recordInfo.players, out ushort chairId);
            _playerSelf = new BaseGamePlayer
            {
                userInfo = new UserInfo
                {
                    id = selfPlayerInfo.user_id,
                    nickName = selfPlayerInfo.nickname,
                    avatarPath = selfPlayerInfo.avatar,
                    sex = selfPlayerInfo.sex,
                },

                userGameInfo = new UserGameInfo
                {
                    wChairID = chairId,
                    llScore = selfPlayerInfo.score
                }
            };
        }

        private RecordInfoPlayer GetSelfPlayer(RecordInfoPlayer[] players, out ushort chairId)
        {
            var selfUserId = UserInfoKit.GetSelfID();
            for (ushort i = 0; i < players.Length; i++)
            {
                var p = players[i];
                if (p.user_id == selfUserId)
                {
                    chairId = i;
                    return p;
                }
            }
            chairId = 0;
            return players[0];
        }

        protected override bool NeedsWaiting(MsgHeader msg) => false;
    }
}
